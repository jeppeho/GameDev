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

	GameObject player;
	Rigidbody rb;

	private bool isJumping = false;

	private float accelerationRate = 10f;
	public float maxVelocity = 5;
	int numJumpFrames = 10;



	// Use this for initialization
	void Start () {
		rb = this.gameObject.GetComponent<Rigidbody> ();
		player = this.gameObject;

		//Keeps the tempo, while changing mass;
		accelerationRate = 200 * rb.mass;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		GetInputButtonValues ();

		//Set power if push button is pushed
		if (pressPush)
			SetPower (16, 8);
		else
			SetPower (10, 5);

		Move ();

		FallDown ();

		Jumping ();

		//Debug.Log ("VELOCITY @" + rb.velocity);
		//Debug.Log("isJumping = " + isJumping);
	}


	/**
	 * Add force on X and Z axis based on controller input
	 */
	private void Move(){

		//Add force to rigidbody
		rb.AddForce (moveHorizontal * accelerationRate, 0, moveVertical * accelerationRate);

		//Check if max velocity exceeded
		if(rb.velocity.magnitude > maxVelocity)
			rb.velocity = Vector3.Normalize(rb.velocity) * maxVelocity;
	}

	private void FallDown(){
		if (!IsGrounded ()) {
			//Debug.Log ("Falling doooooown");
			rb.AddForce (new Vector3 (0, -1.3f, 0));
		} else {
			//Debug.Log ("Landed");
			isJumping = false;
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


		while (pressJump && frame < numJumpFrames) {
			//Debug.Log ("frame " + frame + " < maxFrame " + maxFrame);
			Vector3 force = new Vector3();

			force.y += 2f - 2f * (frame / numJumpFrames);
			rb.AddForce ( force );
			frame += 1;
			yield return new WaitForSeconds (0f);
		}
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

	}
}
