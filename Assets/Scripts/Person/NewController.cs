using UnityEngine;
using System.Collections;

public class NewController : MonoBehaviour {

	//Prefix for controller type
	public string prefix;

	//Prefix for producer of controller
	public string producer;

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
	private float autoRunSpeed = 1.5f;
	public float jumpPower = 10f;
	public float fallDownForce = 45f;
	public float horizontalJumpScalar = 3f;

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
		autoRunSpeed *= LevelManager.SPEED;

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
			if (moveHorizontal != 0 || moveVertical != 0) {
				Move ();
			} else {
				//TODO only if active
				//AutoRun ();
			}
			//Check if player should fall or land
			if (!IsGrounded ()) {
				FallDown ();
			} else {
				Landing ();
			}

			if (pressPush)
				Suicide ();


			//Check if player should throw relic
			if (pressThrow > 0.05f && this.gameObject.GetComponent<PlayerRelicHandler> ().HasRelic () == true) {

				//Reverse if Xbox


				if (producer.Equals("Xbox"))
				{
						throwBuffer += pressThrow;
				}
				else
				{
						throwBuffer += (pressThrow+1)/2;		
				}

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

		LimitBoundariesOnXAxis ();
	}


	/**
	 * Inverts the velocity on the X axis, 
	 * if player is trying to go on to sides of the level.
	 * This script should be changed, when we start using the PCG levels 
	 * (because of placement of boundaries)
	 */
	private void LimitBoundariesOnXAxis(){

		if (rb.transform.position.x < LevelManager.MIN_X) {
			Vector3 vel = rb.velocity;
			vel.x *= -1f;
			rb.velocity = vel;
		} else if (rb.transform.position.x > LevelManager.MAX_X) {
			Vector3 vel = rb.velocity;
			vel.x *= -1f;
			rb.velocity = vel;
		}
	
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
		if (GetSurfaceTag () == "Water") {
			force /= 5;
		}

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

//		float bottomline = GameObject.Find ("LeapControllerBlockHand").GetComponent<Transform> ().transform.position.z;
//		bottomline += 0;
//
//		Debug.Log ((rb.transform.position.z < bottomline) + " | " + rb.transform.position.z + " <  " + bottomline);
//		if (rb.transform.position.z < bottomline) {
//			Debug.Log ("Hello rb.transform.position.z = " +  rb.transform.position.z);
//			rb.AddForce (new Vector3 (0, 0, LevelManager.SPEED));
//		}
//		else 

		if (GetSurfaceTag () == "Water") {
			//rb.AddForce (new Vector3 (0, 0, 0.1f));
			rb.AddForce (new Vector3 (0, 0, autoRunSpeed / 2f * Time.deltaTime));
		} else {
			//rb.AddForce (new Vector3 (0, 0, 0.15f));
			rb.AddForce (new Vector3 (0, 0, autoRunSpeed * Time.deltaTime));
		}
	}


	/**
	 * 
	 * TODO If we use this function multiply by Time.deltaTime in the AddForce method!!!
	 */
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
		if (GetSurfaceTag () == "Water")
			jumpPower /= 3;

		//Add speed on X and Z axis if jumping
		float x = moveHorizontal * accelerationRate * horizontalJumpScalar;
		float z = moveVertical * accelerationRate * horizontalJumpScalar;

		rb.AddForce ( new Vector3(x, jumpPower, z) * Time.deltaTime );
	}


	/**
	 * Add additional downward force to falling
	 */
	private void FallDown(){
		rb.AddForce (new Vector3 (0, -fallDownForce, 0) * Time.deltaTime);
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
		//Debug.Log (force);

		//Get relic
		GameObject relic = this.transform.Find("Relic").gameObject;
	
		//Throw relic and remove as child
		if (relic) {
			player.GetComponentInChildren<RelicController> ().Throw(force);
			this.gameObject.GetComponent<PlayerRelicHandler>().ReleaseRelic ();
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
		moveVertical = Input.GetAxis ( prefix + "_Vertical");
		throwHorizontal = Input.GetAxis( prefix + "_Horizontal2" );
		throwVertical = Input.GetAxis (prefix + "_Vertical2");
		pressJump = (Input.GetAxis (prefix + "_Jump") == 1) ? true : false;
		pressPush = (Input.GetAxis (prefix + "_Push") == 1) ? true : false;
		pressExplode = (Input.GetAxis (prefix + "_Explode") == 1) ? true : false;
		pressThrow = Input.GetAxis (prefix + "_Throw" );
	}

	private void GetInputButtonValuesOLD(){
		producer = "";

		moveHorizontal = Input.GetAxis( prefix + "_" + producer + "Horizontal" );
		moveVertical = Input.GetAxis ( prefix + "_" + producer + "Vertical");
		throwHorizontal = Input.GetAxis( prefix + "_" + producer + "Horizontal2" );
		throwVertical = Input.GetAxis (prefix + "_" + producer + "Vertical2");
		pressJump = (Input.GetAxis (prefix + "_Jump") == 1) ? true : false;
		pressPush = (Input.GetAxis (prefix + "_Push") == 1) ? true : false;
		pressExplode = (Input.GetAxis (prefix + "_Explode") == 1) ? true : false;
		pressThrow = Input.GetAxis (prefix + "_" + producer + "Throw" );
	}

	/**
	 * Shoots a ray down and returns the tag as a string 
	 * if the ray hits anything. Else it returns null.
	 */
	private string GetSurfaceTag(){
		
		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast (rb.position, Vector3.down, out hit, 0.8f)) {
			return hit.collider.gameObject.tag;
		} else {
			return null;
		}
	}

	public Vector3 GetDirection(){
		return direction;
	}
		
	private void UpdateDirection (){
		
		//Use right stick
		Vector2 rightStickDirection = new Vector2 (throwHorizontal, throwVertical);
		//Use left stick
		Vector2 leftStickDirection = new Vector2 (moveHorizontal, moveVertical);

		//Check if right stick is used
		if (rightStickDirection.magnitude > 0.05f) {
			direction = new Vector3 (rightStickDirection.x, 0f, rightStickDirection.y).normalized;
		
		//Check if left stick is used
		} else if (leftStickDirection.magnitude > 0.05f) {
			
			direction = new Vector3 (leftStickDirection.x, 0f, leftStickDirection.y).normalized;
		
			//Else throw straight ahead
		} else {
			direction = new Vector3 (0f, 0f, 1f);
		} 
	}
		
	private bool IsGrounded(){
		bool isGrounded = Physics.Raycast (player.transform.position, -Vector3.up, 0.4f);//distToGround);
		return isGrounded;
	}
}