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

	GameObject player;
	Rigidbody rb;

	private Vector3 direction;

	private bool isJumping = false;

	public float accelerationRate;
	public float maxAcceleration;
	public float maxVelocity;
	public float jumpPower;

	int falldownCounter = 0;


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

		GetInputButtonValues ();

		UpdateDirection ();

		//Check if player should throw relic
		if (pressThrow)
			ThrowRelic ();

		//Move on X and Z axis
		if(moveHorizontal != 0 || moveVertical != 0)
			Move ();

		//Check if player should fall or land
		if (!IsGrounded ()) {
			FallDown ();
		}else{
			Landing();
		}
			
		//Check if player is jumping
		if (pressJump && !isJumping) {
			isJumping = true;
			Jump2 ();
			//StartCoroutine (Jump ());
		}
	}


	/**
	 * Throws the relic and removes it as child
	 */ 
	private void ThrowRelic(){

		//Get relic
		GameObject relic = this.transform.Find("Relic").gameObject;
	
		//Throw relic and remove as child
		if (relic) {
			player.GetComponentInChildren<RelicController> ().Throw();
			this.gameObject.GetComponent<CarriedObject>().ReleaseCarriedObject ();
		}
	}


	/**
	 * 	Updates the direction showing the facing of the player
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
	 * Add force on X and Z axis based on controller input
	 */
	private void Move(){

		//Add force to rigidbody
		float hor = moveHorizontal * accelerationRate;
		float ver = moveVertical * accelerationRate;

		Vector3 force = new Vector3 (hor, 0, ver);

		if (force.magnitude > maxAcceleration)
			force = Vector3.Normalize (force) * maxAcceleration;

		//Add force
		rb.AddForce (force * Time.deltaTime);


		//Make vector to check ground velocity
		Vector2 ground_speed = new Vector2 (rb.velocity.x, rb.velocity.z);

		//If magnitude of x and z velocity is exceeded, then stomp it
		if (ground_speed.magnitude > GetMaxVelocity()) {

			//Set speed to the maxVelocity
			ground_speed = Vector3.Normalize (ground_speed) * GetMaxVelocity();

			//Convert to 3 dimension vector
			Vector3 updatedVel = new Vector3 (ground_speed.x, rb.velocity.y, ground_speed.y);
			rb.velocity = updatedVel;
		}


	}

	/**
	 * Returns true if player is grounded
	 */
	private bool IsGrounded(){
		bool isGrounded = Physics.Raycast (player.transform.position, -Vector3.up, 0.4f);//distToGround);
		return isGrounded;
	}

	private void Jump2(){
		rb.AddForce ( new Vector3(0, GetJumpPower(), 0) );
	}

	private void FallDown(){
		rb.AddForce (new Vector3 (0, -1.3f, 0));
		falldownCounter = 3;
	}

	private void Landing(){

		if (falldownCounter == 0)
			isJumping = false;
		else
			falldownCounter--;
	}
		
	/**
	 * Return the jumppower, multiplied if push button is pressed
	 */
	public float GetJumpPower(){
		if (pressPush)
			return jumpPower * 1.3f;
		else
			return jumpPower;
	}

	/**
	 * Return the MaxVelocity, multiplied if push button is pressed
	 */
	public float GetMaxVelocity(){
		if (pressPush)
			return maxVelocity * 1.3f;
		else
			return maxVelocity;
	}

	//Gets the input from the controller and maps it to variables
	private void GetInputButtonValues(){
		moveHorizontal = Input.GetAxis( prefix + "_Horizontal" );
		moveVertical = Input.GetAxis (prefix + "_Vertical");
		pressJump = (Input.GetAxis (prefix + "_Jump") == 1) ? true : false;
		pressPush = (Input.GetAxis (prefix + "_Push") == 1) ? true : false;
		pressThrow = (Input.GetAxis (prefix + "_Throw") == 1) ? true : false;

	}
}