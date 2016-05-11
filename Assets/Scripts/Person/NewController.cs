using UnityEngine;
using System.Collections;

public class NewController : MonoBehaviour {

	//Prefix for controller type
	public string prefix;

	//Prefix for producer of controller
	public string producer;

	//Speed
	public float accelerationRate = 10;
	public float maxVelocity = 1;

	//Jump
	public float jumpPower = 0.75f;
	//public float horizontalJumpScalar = 1f;
	public float jumpXZscalar = 1.3f;

	//Dash
	public int numDashFrames = 5;
	public int dashPower = 700;
	public float dashCoolDownForSeconds = 1.5f;

	public float fallDownForce = 30f;


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

	private string lastSurfaceTag; //Keeps last tag that was not null
	private string surfaceTag; //Can be null

	//Movement limits
	private float farWalkZone;
	private float nearWalkZone;
	private float walkZoneWidth;

	GameObject player;
	Rigidbody rb;

	//This is the throwDirection, reads right stick if used else the left stick
	private Vector3 direction;

	private bool isJumping = false;
	private bool isDashing = false;
	private bool isThrowing = false;

	//Right after dash movement speed is lowered
	private bool dashPenalizesMovementSpeed = false;

	private int explosionCounter = 300;

	int falldownCounter = 0;

//	public float dashWaitTime = 60;
//	float dashTime;

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
		GetInputButtonValues ();

		//If player is inactive
		if (playerState == "inactive") {
			
			if (pressJump) {

				this.gameObject.GetComponent<PlayerManager> ().SetActive (true);
			}
		}
			
		//If player is active
		else if (playerState == "active" || playerState == "invulnerable") {
	
			UpdateDirection ();
			UpdateSurfaceTags ();

			//Check if player is jumping
			if (!isJumping && pressJump ) {
				
				isJumping = true;
				//Jump ();
				StartCoroutine (Jump());
			}

			//Move on X and Z axis
			if ( (moveHorizontal != 0 || moveVertical != 0) /* && IsGrounded () */ ) {
				Move ();
			}

			//Check if player should fall or land
//			if (!IsGrounded ()) {
//
//				//IF ELEVATION IS USED CHANGE ONE TO LEVELAREA NOISE
//				if (IsAboveHeight (1) == false) {
//					FallDown ();
//				}
//			} else {
//				Landing ();
//			}

			if (!IsGrounded ()) {
				//IF ELEVATION IS USED CHANGE ONE TO LEVELAREA NOISE
				if (IsAboveHeight (1) == false) {
					FallDown ();
				}
			}


			//If not dashing, button is pressed, and the left stick is used to give direction to the dash
			if (!isDashing && pressPush && (moveHorizontal > 0f || moveVertical > 0f)) {
				isDashing = true;
				dashPenalizesMovementSpeed = true;
				StartCoroutine (Dash ());
			}


			if (!isThrowing && pressThrow > 0.05f && this.gameObject.GetComponent<PlayerRelicHandler> ().HasRelic () == true) {

				isThrowing = true;
				StartCoroutine(Throw ());
			
			}

			//Charge explosionCounter 
			explosionCounter++;
		} else {

			//RESET IF PLAYER IS NOT ALIVE
			isDashing = false;
			isJumping = false;
		
		}
			
		LimitWalkingDistance();
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
		if (GetSurfaceTag () == "Water" || GetLastSurfaceTag() == "Water" ) {
			force /= 4;
		}

		if (isJumping) {
			force.x *= jumpXZscalar;
			force.z *= jumpXZscalar;
		}

		//If player is running towards camera, slow down velocity
		if (rb.velocity.z < 0) {
			if (force.z < 0) {
				force.z /= 2;
			}
		}

		if (dashPenalizesMovementSpeed)
			force /= 30f;

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


	/**
	 * Dashes the player in the direction of the left stick quickly
	 */
	IEnumerator Dash(){

		float hor = moveHorizontal * accelerationRate;
		float ver = moveVertical * accelerationRate;

		float y = 0;

		//Add force to prevent player getting stuck in edge
		if (GetSurfaceTag () == "water")
			y = 0.1f;

		Vector3 force = new Vector3 (hor, y, ver);

		force = force.normalized;

		int frame = 0;

		//Add force
		while (frame < numDashFrames) {

			rb.AddForce (force * Time.deltaTime * dashPower);
			frame++;

			yield return new WaitForSeconds (0.01f);
		}

		//Wait for movement speed penalty
		yield return new WaitForSeconds (0.3f);

		dashPenalizesMovementSpeed = false;

		//Wait for cool down
		yield return new WaitForSeconds (dashCoolDownForSeconds);

		//Reset dashing
		isDashing = false;

	}
		

	IEnumerator Jump(){

		float x = 0; float z = 0;
			
//		x = moveHorizontal * accelerationRate * horizontalJumpScalar;
//		z = moveVertical * accelerationRate * horizontalJumpScalar;

		int numFrames = 7;
		int index = 0;

		rb.velocity /= 3f;

		float jumpPower = GetJumpPower () / (float)numFrames;

		while (pressJump && index < numFrames) {

			Vector3 force = new Vector3 ( x, jumpPower, z);

			rb.AddForce ( force * Time.deltaTime );
			index++;

			yield return new WaitForSeconds (0.01f);
		}

		while (!IsGrounded () || pressJump) {
			yield return new WaitForSeconds (0.01f);
		}

		isJumping = false;
	}



	/**
	 * Builds up force, then throws the relic and removes it as child
	 */ 
	IEnumerator Throw(){

		Vector3 force = Vector3.zero;

		//Add some min force
		//force *= 50; /** GetDirection ()*/;

		int numFrames = 100;
		int index = 1;

		while (pressThrow > 0.05f && index < numFrames) {

			index++;
			yield return new WaitForSeconds(0.01f);
		}

		//Wait until button is released
		while (pressThrow > 0.05f) {
			yield return new WaitForSeconds(0.01f);
		}

		force = GetDirection () * 25;

		force *= index * 20;

		StartCoroutine( player.GetComponentInChildren<RelicController> ().Throw( force ) );

		this.gameObject.GetComponent<PlayerRelicHandler> ().ReleaseRelic ();

		isThrowing = false;
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

//		if (dis < nearWalkZone) {
//
//			Vector3 oldVel = rb.velocity;
//			float counterForce = maxVelocity * (dis-nearWalkZone)/walkZoneWidth;
//
//			Vector3 counterVel = new Vector3 (0, 0, -counterForce);
//
//			rb.velocity = oldVel + counterVel;
//		}
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

		return jumpPower * 100f;

	}

	/**
	 * Return the MaxVelocity, multiplied if push button is pressed
	 */
	private float GetMaxVelocity(){
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
	 * Shoots a ray down and updates surfaceTag and lastSurfaceTag
	 */
	private void UpdateSurfaceTags(){
		
		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast (rb.position, Vector3.down, out hit, 0.8f)) {
			lastSurfaceTag = hit.collider.gameObject.tag;
			surfaceTag =  hit.collider.gameObject.tag;
		} else {
			surfaceTag = null;
		}
	}

	private string GetSurfaceTag(){
		return surfaceTag;
	}

	private string GetLastSurfaceTag (){
		return lastSurfaceTag;
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
		if (rightStickDirection.magnitude > 0.01f) {
			direction = new Vector3 (rightStickDirection.x, 0f, rightStickDirection.y).normalized;
		
		//Check if left stick is used
		} else if (leftStickDirection.magnitude > 0.01f) {
			
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

	private bool IsAboveHeight(float height){
		bool isGrounded = Physics.Raycast (player.transform.position, -Vector3.up, height);//distToGround);
		return isGrounded;
	}


	/**
	 * 
	 * TODO If we use this function multiply by Time.deltaTime in the AddForce method!!!
	 */
	//	private void Explode(){
	//
	//		//Get the center of minion
	//		Vector3 center = this.gameObject.transform.position;
	//
	//		//Radius to check for collision
	//		float radius = 3;
	//
	//		//Get objects from collision
	//		Collider[] hitColliders = Physics.OverlapSphere(center, radius);
	//
	//		//Set variables
	//		int i = 0;
	//	
	//		while (i < hitColliders.Length) {
	//
	//			if (hitColliders [i].gameObject.tag == "Environment") {
	//
	//				Rigidbody rb = hitColliders [i].GetComponent<Rigidbody> ();
	//
	//				if (rb != null) {
	//
	//					Vector3 objectPosition = hitColliders [i].transform.position;
	//					Vector3 vectorFromCenterToObject = objectPosition - center;
	//
	//					float distance = vectorFromCenterToObject.magnitude;
	//
	//					rb.AddForce (vectorFromCenterToObject * (2500 - distance * 250));
	//					Debug.Log (i + "| " + hitColliders [i].name + " @" + objectPosition);
	//
	//				}
	//			}
	//			i++;
	//		}
	//	}

}