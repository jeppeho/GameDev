using UnityEngine;
using System.Collections;

public class NewController : MonoBehaviour {

	//Prefix for controller type
	public string prefix;

	//Buttons
	private float moveHorizontal;
	private float moveVertical;
	private bool pressPush;
	private bool pressJump;
	private bool pressThrow;
	private bool pressExplode;

	GameObject player;
	Rigidbody rb;

	private Vector3 direction;

	private bool isJumping = false;

	public float accelerationRate = 10;
	public float maxAcceleration = 5;
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
		maxAcceleration *= LevelManager.SPEED;
	}

	
	// Update is called once per frame
	void FixedUpdate () {

		//Get state of player, eg. dead, active etc.
		playerState = this.gameObject.GetComponent<PlayerManager> ().GetState ();

		if (playerState == "active" || playerState == "invulnerable") {
			
			GetInputButtonValues ();

			UpdateDirection ();

			//Check if player should throw relic
			if (pressThrow && this.gameObject.GetComponent<CarriedObject>().isCarrying() == true)
				Throw ();

			//If explosionCounter is above target, then make explosion
			if (pressExplode) {
				Debug.Log ("explision = " + explosionCounter);
				if (explosionCounter > 200) {
					
					//StartCoroutine (ExplodeLayers ());
					Explode();
					explosionCounter = 0;
				}
			}

			//Charge explosionCounter 
			explosionCounter++;

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
			
			//Check if player is jumping
			if (pressJump && !isJumping) {
				isJumping = true;
				Jump ();
			}

			if(pressPush)
				Suicide ();
		}
	}


	private void AutoRun(){
		rb.AddForce(new Vector3(0,0,0.15f));
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
	 * Throws the relic and removes it as child
	 * TODO Check if gameObject is present
	 */ 
	private void Throw(){

		//Get relic
		GameObject relic = this.transform.Find("Relic").gameObject;
	
		//Throw relic and remove as child
		if (relic) {
			player.GetComponentInChildren<RelicController> ().Throw();
			this.gameObject.GetComponent<CarriedObject>().ReleaseCarriedObject ();
		}
	}


	/**
	 * 	Updates the direction showing the facing of the player.
	 */
	private void UpdateDirection (){

		//Update if magnitude is below threshold
		if(rb.velocity.magnitude > 1f)
			direction = rb.velocity;
	}

	public Vector3 GetDirection(){
		return direction;
	}


	/**
	 * Add force on X and Z axis based on controller input.
	 */
	private void Move(){

		//Add force to rigidbody
		float hor = moveHorizontal * accelerationRate;
		float ver = moveVertical * accelerationRate;

		Vector3 force = new Vector3 (hor, 0, ver);

		if (force.magnitude > maxAcceleration)
			force = Vector3.Normalize (force) * maxAcceleration;
	
		//Get CameraZ position
		float cameraZ = GameObject.Find ("LeapControllerBlockHand").GetComponent<CameraController> ().GetZPosition ();
		//float cameraZ = GameObject.Find ("Relic").GetComponent<CameraController> ().GetZPosition ();

		//Only add force if player is behind invisisble wall
		if (rb.position.z < cameraZ + 3f) {

			rb.AddForce (force * Time.deltaTime);
		}


		//Make 2D vector to check ground velocity
		Vector2 ground_speed = new Vector2 (rb.velocity.x, rb.velocity.z);

		//If magnitude of x and z velocity is exceeded, then set it to max value.
		if (ground_speed.magnitude > GetMaxVelocity()) {

			//Set speed to the maxVelocity
			ground_speed = Vector3.Normalize (ground_speed) * GetMaxVelocity();

			//Convert to 3 dimension vector
			Vector3 updatedVel = new Vector3 (ground_speed.x, rb.velocity.y, ground_speed.y);
			rb.velocity = updatedVel;
		}


		//If player is running towards camera, slow down velocity
		if (rb.velocity.z < 0) {
			Vector3 vel = rb.velocity;
			vel.z /= 2;
			rb.velocity = vel;
		}
			
	}


	/**
	 * Returns true if player is grounded.
	 */
	private bool IsGrounded(){
		bool isGrounded = Physics.Raycast (player.transform.position, -Vector3.up, 0.4f);//distToGround);
		return isGrounded;
	}
		
	/**
	 * Add upwards force
	 */
	private void Jump(){
		rb.AddForce ( new Vector3(0, GetJumpPower(), 0) );
	}


	/**
	 * Add additional downward force to falling
	 */
	private void FallDown(){
		rb.AddForce (new Vector3 (0, -1f, 0));
		falldownCounter = 3;
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
		pressJump = (Input.GetAxis (prefix + "_Jump") == 1) ? true : false;
		pressPush = (Input.GetAxis (prefix + "_Push") == 1) ? true : false;
		pressThrow = (Input.GetAxis (prefix + "_Throw") == 1) ? true : false;
		pressExplode = (Input.GetAxis (prefix + "_Explode") == 1) ? true : false;

	}

	public void Suicide(){

			this.gameObject.GetComponent<PlayerManager> ().Suicide ();
	}
}