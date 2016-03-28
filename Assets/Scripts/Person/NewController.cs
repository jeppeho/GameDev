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
	public float maxVelocity;
	private int numJumpFrames = 20;



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
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		GetInputButtonValues ();

		//Set power if push button is pushed
//		if (pressPush)
//			SetPower (16, 8);
//		else
//			SetPower (10, 5);


		UpdateDirection ();

		if (pressThrow)
			ThrowRelic ();

		if(moveHorizontal != 0 || moveVertical != 0)
			Move ();

		if (!IsGrounded ()) {
			falldownCounter = 3;
		}

		FallDown ();

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

		force *= Time.deltaTime;

		if (force.magnitude > maxVelocity)
			force = Vector3.Normalize (force) * maxVelocity;

		rb.AddForce (force);

		//Check if max velocity exceeded
//		if(rb.velocity.magnitude > maxVelocity)
//			rb.velocity = Vector3.Normalize(rb.velocity) * maxVelocity;

	}

	int falldownCounter = 0;

	private void FallDown(){
		if (!IsGrounded ()) {

			float y = rb.velocity.y;

			rb.AddForce (new Vector3 (0, -1.3f, 0));

		} else {

			if (falldownCounter == 0)
				isJumping = false;
			else
				falldownCounter--;
		}
	}


	/**
	 * Returns true if player is grounded
	 */
	private bool IsGrounded(){
		bool isGrounded = Physics.Raycast (player.transform.position, -Vector3.up, 0.4f);//distToGround);
		return isGrounded;
	}
		

	private void Jumping(){

		if (pressJump && !isJumping) {
			isJumping = true;
			StartCoroutine (Jump ());
		}
	}

	IEnumerator Jump(){
		
		int frame = 0;

		numJumpFrames = 5;

		while (pressJump && frame < numJumpFrames) {

			Vector3 force = new Vector3();

			force.y += 1f - 1f * (frame / numJumpFrames);
			//force.y += 30f - 30f * (frame / numJumpFrames);
			//force *= Time.deltaTime;
			rb.AddForce ( force );
			Debug.Log ("Frame " + frame + " / " + numJumpFrames);
			frame += 1;
			yield return new WaitForSeconds (0f);
		}
	}

	private void Jump2(){
		if(pressPush)
			rb.AddForce ( new Vector3(0, 20, 0) );
		else
			rb.AddForce ( new Vector3(0, 10, 0) );
	}
		

	private void SetPower(int numFrames, float maxVelocity){
		numJumpFrames = numFrames;
		maxVelocity = maxVelocity;
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
