using UnityEngine;
using System.Collections;

public class NewController : MonoBehaviour {

	//Prefix for controller type
	public string prefix;

	//Buttons
	private float moveHorizontal;
	private float moveVertical;
	private float throwHorizontal;
	private float throwVertical;
	private bool pressPush;
	private bool pressJump;
	private float pressThrow;
	private bool pressExplode;

	private float throwBuffer;
	private float throwCounter;

	//Movement limits
	private float farWalkZone;
	private float nearWalkZone;
	private float walkZoneWidth;

	GameObject player;
	Rigidbody rb;

	private Vector3 direction;

	private bool isJumping = false;

	public float accelerationRate = 10;
	public float maxVelocity = 1;
	public float jumpPower = 10;

	private int explosionCounter = 300;

	int falldownCounter = 0;

	string playerState;

	// Use this for initialization
	void Start () {
		rb = this.gameObject.GetComponent<Rigidbody> ();
		player = this.gameObject;

		//So the number doesn't have to be that big in the editor
		accelerationRate *= 100;

		//Keeps the tempo, while changing mass;
		accelerationRate *= rb.mass;

		//Multiply with the game speed
		maxVelocity *= LevelManager.SPEED;
		accelerationRate *= LevelManager.SPEED;

		nearWalkZone = LevelManager.MOVE_MINZ+LevelManager.MOVE_ZONEWIDTH;
		farWalkZone = LevelManager.MOVE_MAXZ-LevelManager.MOVE_ZONEWIDTH;
		walkZoneWidth = LevelManager.MOVE_ZONEWIDTH;
	}

	
	// Update is called once per frame
	void FixedUpdate () {

		//Get state of player, eg. dead, active etc.
		playerState = this.gameObject.GetComponent<PlayerManager> ().GetState ();

		if (playerState == "active" || playerState == "invulnerable") {
			
			GetInputButtonValues ();
			UpdateDirection ();

			//Check if player is jumping
			if (pressJump && !isJumping) {
				isJumping = true;
				Jump ();
			}

			//Move on X and Z axis
			if (moveHorizontal != 0 || moveVertical != 0)
				Move ();
			else
				AutoRun ();
			
			//Check if player should fall or land
			if (!IsGrounded ()) {
				FallDown ();
			} else {
				Landing ();
			}

			if (pressPush)
				Suicide ();


			//Check if player should throw relic
			if (pressThrow > 0.05f && this.gameObject.GetComponent<CarriedObject> ().isCarrying () == true) {
				throwBuffer += (pressThrow+1)/2;
				throwCounter++;

				if (throwCounter >= 5)
				{
					Throw ();
					throwBuffer = 0; throwCounter = 0;
				}
			}
			else
			{
				if (throwCounter >= 1)
				{
					Throw ();
				}

				throwBuffer = 0; throwCounter = 0;
			}

			//If explosionCounter is above target, then make explosion
			if (pressExplode) {
				if (explosionCounter > 200) {

					Explode();
					//reset counter
					explosionCounter = 0;
				}
			}

			//Charge explosionCounter 
			explosionCounter++;
		}
			
		//LimitWalkingDistance();
		LimitWalkingDistanceSoft();
	}


	/**
	 * Add force on X and Z axis based on controller input.
	 */
	private void Move(){

		//Get force to rigidbody
		float hor = moveHorizontal * accelerationRate;
		float ver = moveVertical * accelerationRate;

		Vector3 force = new Vector3 (hor, 0, ver);

		//Check if player is moving through water
		if (GetTagOfSurface () == "Water")
			force /= 4;
		

		//If player is running towards camera, slow down velocity
		if (rb.velocity.z < 0) {
			if (force.z < 0) {
				force.z /= 2;
			}
		}
			
		rb.AddForce (force * Time.deltaTime);

		//Make 2D vector to check velocity on X and Y axis
		Vector2 ground_speed = new Vector2 (rb.velocity.x, rb.velocity.z);

		//If magnitude of x and z velocity is exceeded, then set it to max value.
		if (ground_speed.magnitude > GetMaxVelocity()) {

			//Set speed to the maxVelocity
			ground_speed = Vector3.Normalize (ground_speed) * GetMaxVelocity();

			//Convert to 3 dimension vector
			Vector3 updatedVel = new Vector3 (ground_speed.x, rb.velocity.y, ground_speed.y);
			rb.velocity = updatedVel;
		}
	}



	private void AutoRun(){
		rb.AddForce( new Vector3(0, 0, 0.15f) );
	}


	private void Explode(){

		//Get the center of minion
		Vector3 center = this.gameObject.transform.position;

		//Radius to check for collision
		float radius = 3;

		//Get objects from collision
		Collider[] hitColliders = Physics.OverlapSphere(center, radius);

		//Set variables
		int i = 0;
	
		while (i < hitColliders.Length) {

			if (hitColliders [i].gameObject.tag == "Environment") {

				Rigidbody rb = hitColliders [i].GetComponent<Rigidbody> ();

				if (rb != null) {

					Vector3 objectPosition = hitColliders [i].transform.position;
					Vector3 vectorFromCenterToObject = objectPosition - center;

					float distance = vectorFromCenterToObject.magnitude;

					rb.AddForce (vectorFromCenterToObject * (2500 - distance * 250));
					Debug.Log (i + "| " + hitColliders [i].name + " @" + objectPosition);

				}
			}
			i++;
		}
	}


	/**
	 * Add upwards force
	 */
	private void Jump(){

		float jumpPower = GetJumpPower();

		//Make smaller jump, if on water
		if (GetTagOfSurface () == "Water")
			jumpPower /= 3;

		//Add speed on X and Z axis if jumping
//		float hor = moveHorizontal * accelerationRate / 5;
//		float ver = moveVertical * accelerationRate / 5;

		//rb.AddForce ( new Vector3(hor, jumpPower, ver) );
		rb.AddForce ( new Vector3(0, jumpPower, 0) );
	}


	/**
	 * Add additional downward force to falling
	 */
	private void FallDown(){
		rb.AddForce (new Vector3 (0, -1f, 0));
		falldownCounter = 1;
	}


	/**
	 * When a player land, it sets jumping to false, when falldownCounter has decremented to zero
	 */
	private void Landing(){

		if (falldownCounter == 0)
			isJumping = false;
		else
			falldownCounter--;
	}


	/**
	 * Throws the relic and removes it as child
	 * TODO Check if gameObject is present
	 */ 
	private void Throw(){
		float force = Mathf.Min (1, throwBuffer / 5);
		Debug.Log (force);

		//Get relic
		GameObject relic = this.transform.Find("Relic").gameObject;
	
		//Throw relic and remove as child
		if (relic) {
			player.GetComponentInChildren<RelicController> ().Throw(force);
			this.gameObject.GetComponent<CarriedObject>().ReleaseCarriedObject ();
		}
	}


	public void Suicide(){
		this.gameObject.GetComponent<PlayerManager> ().Suicide ();
	}

	/**
	 * Sets a limit on how far a character can move on the Z-axis
	 */
	private void LimitWalkingDistance(){
		if (GetDistanceFromCamera () > 0) {
			Vector3 vel = rb.velocity;

			if (vel.z > 0) {

				int divisor = 20;

				//vel.z /= (GetDistanceFromCamera () / 20 + 0.95f);
				vel.z /= (GetDistanceFromCamera () / divisor + 1);
				rb.velocity = vel;
			}
		}
	}

	private void LimitWalkingDistanceSoft(){

		float dis = GetDistanceFromCamera ();
	
		//In farzone
		if (dis > farWalkZone) {
			
			Vector3 oldVel = rb.velocity;
			float counterForce = maxVelocity * (dis-farWalkZone)/walkZoneWidth;

			Vector3 counterVel = new Vector3 (0, 0, -counterForce);

			rb.velocity = oldVel + counterVel;
		}

		if (dis < nearWalkZone) {

			Vector3 oldVel = rb.velocity;
			float counterForce = maxVelocity * (dis-nearWalkZone)/walkZoneWidth;

			Vector3 counterVel = new Vector3 (0, 0, -counterForce);

			rb.velocity = oldVel + counterVel;
		}
	}
	
	 // Returns the distance on the Z-axis from the player to the LEAP
	private float GetDistanceFromCamera(){

		//Get Z-position of the LEAP
		float cameraZ = GameObject.Find ("LeapControllerBlockHand").GetComponent<CameraController> ().GetZPosition ();

		float z = rb.position.z - cameraZ;

		return z;
	}
		
	/**
	 * Return the jumppower, multiplied if push button is pressed
	 */
	private float GetJumpPower(){
//		if (pressPush)
//			return jumpPower * 1.3f;
//		else
//			return jumpPower;

		return jumpPower;
	}

	/**
	 * Return the MaxVelocity, multiplied if push button is pressed
	 */
	private float GetMaxVelocity(){
//		if (pressPush)
//			return maxVelocity * 1.3f;
//		else
//			return maxVelocity;
		return maxVelocity;
	}

	//Gets the input from the controller and maps it to variables
	private void GetInputButtonValues(){
		moveHorizontal = Input.GetAxis( prefix + "_Horizontal" );
		moveVertical = Input.GetAxis (prefix + "_Vertical");
		throwHorizontal = Input.GetAxis( prefix + "_Horizontal2" );
		throwVertical = Input.GetAxis (prefix + "_Vertical2");
		pressJump = (Input.GetAxis (prefix + "_Jump") == 1) ? true : false;
		pressPush = (Input.GetAxis (prefix + "_Push") == 1) ? true : false;
		pressExplode = (Input.GetAxis (prefix + "_Explode") == 1) ? true : false;
		pressThrow = Input.GetAxis (prefix + "_Throw" );
	}

	/**
	 * Shoots a ray down and returns the tag as a string 
	 * if the ray hits anything. Else it returns null.
	 */
	private string GetTagOfSurface(){
		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast (rb.position, Vector3.down, out hit, 1f)) {
			return hit.collider.gameObject.tag;
		} else {
			return null;
		}
	}

	public Vector3 GetDirection(){
		return direction;
	}
		
	private void UpdateDirection (){
		//Make 2D vector to check velocity on X and Y axis
		Vector2 throwDirection = new Vector2 (throwHorizontal, throwVertical);

		//If right analog is in use
		if (throwDirection.magnitude > 0.05f )
		{
			//Set to throwing angle (right stick), and normalize both
			direction = new Vector3 (throwDirection.x, 0f, throwDirection.y).normalized ;
		}
		//Otherwise, go by velocity (if above 1f)
		else if(rb.velocity.magnitude > 1f)
		{
			direction = rb.velocity.normalized;
		}
	}
		
	private bool IsGrounded(){
		bool isGrounded = Physics.Raycast (player.transform.position, -Vector3.up, 0.4f);//distToGround);
		return isGrounded;
	}
}