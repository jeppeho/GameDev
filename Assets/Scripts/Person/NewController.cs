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

	float accelerationRate = 10f;
	private float maxVelocity = 5;

	// Use this for initialization
	void Start () {
		rb = this.gameObject.GetComponent<Rigidbody> ();
		player = this.gameObject;

		//Keeps the tempo, while changing mass;
		accelerationRate = 200 * rb.mass;
	}
	
	// Update is called once per frame
	void Update () {

		SetInputButtonValues ();

		Move ();

		TestJump ();

		FallDown ();

		Jumping ();

		//Debug.Log ("VELOCITY @" + rb.velocity);
		Debug.Log("isJumping = " + isJumping);
	}

	private void Move(){

		rb.AddForce (moveHorizontal * accelerationRate, 0, moveVertical * accelerationRate);

		if(rb.velocity.magnitude > maxVelocity)
			rb.velocity = Vector3.Normalize(rb.velocity) * maxVelocity;
	}

	private void FallDown(){
		if (!IsGrounded ()) {
			Debug.Log ("Falling doooooown");
			rb.AddForce (new Vector3 (0, -1f, 0));
		} else {
			Debug.Log ("Landed");
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

	private void TestJump(){

		if (Input.GetKey ("u")) {
			Vector3 pos = this.gameObject.transform.position;
			pos.y += 1;
			this.gameObject.transform.position = pos;
		}

	}

	private void Jumping(){

		if (pressJump && !isJumping) {
			isJumping = true;
			StartCoroutine (Jump ());
		}

	}

	IEnumerator Jump(){
		Debug.Log ("Couroutine");
		int frame = 0;
		int maxFrame = 10;

		while (pressJump && frame < maxFrame) {
			Debug.Log ("frame " + frame + " < maxFrame " + maxFrame);
			Vector3 pos = new Vector3();

			pos.y += 2f - 2f * (frame / 10);
			rb.AddForce (pos);
			frame += 1;
			yield return new WaitForSeconds (0f);
		}

	}



	//Gets the input from the controller and maps it to variables
	private void SetInputButtonValues(){
		
		moveHorizontal = Input.GetAxis( prefix + "_Horizontal" );
		moveVertical = Input.GetAxis (prefix + "_Vertical");
		pressJump = (Input.GetAxis (prefix + "_Jump") == 1) ? true : false;
		pressPush = (Input.GetAxis (prefix + "_Push") == 1) ? true : false;

	}
}
