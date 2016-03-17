using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using DG.Tweening;

public class PersonPrefixController : MonoBehaviour {

	public string prefix;

	GameObject player;
	private float moveHorizontal;
	private float moveVertical;
	private bool pressPush;
	private bool pressJump;
	private bool prevPressJump;
	private bool isJumping = false;
	private bool isFalling = false;

	private int maxJumpFrames = 80;
	private int jumpedFrames;
	private float jumpVelocityX;
	private float jumpVelocityZ;

	private bool jumping;
	float velocityX, velocityY, velocityZ;

	private float speed;
	public float highSpeed = 8;
	public float lowSpeed = 6;
	public float jumpPower = 20; //Might be obsolete now
	public float jumpHeight = 20;
	Vector3 position;


	public AnimationCurve anim;
	Vector3 highPos;
	//Vector3 lowPos;

	public float regularPushPower;
	public float extraPushPower;


	// Use this for initialization
	void Start () {
		jumpedFrames = maxJumpFrames;
		speed = lowSpeed;
	}

	// Update is called once per frame
	void Update () {

		//get values from axis (keys)
		moveHorizontal = Input.GetAxis( prefix + "_Horizontal" );
		moveVertical = Input.GetAxis (prefix + "_Vertical");
		pressJump = (Input.GetAxis (prefix + "_Jump") == 1) ? true : false;
		pressPush = (Input.GetAxis (prefix + "_Push") == 1) ? true : false;


		//Get the player object
		player = this.gameObject;

		//Get current position
		position = this.gameObject.transform.position;





		increaseSpeed ();

		//Move Person on X and Z axis
		Move ();

		//Jump Person
		Jump ();

		//Set mass to push object
		Push ();


		//Debug.Log("X = " + velocityX + ", Y = " + velocityY+ ", Z = " + velocityZ);

		//Move player
		position.x += velocityX;
		position.y += velocityY;
		position.z += velocityZ;


		//Reset velocity
		velocityX = 0;
		velocityY = 0;
		velocityZ = 0;

		//Update position
		this.gameObject.transform.position = position;
	}

	void Jump(){

		//Check for hitting other object, then stop jump

		//Check if player is grounded
		float distToGround = player.GetComponent<Collider>().bounds.extents.y;
		Debug.Log ("distToGRound = " + distToGround);
		bool isGrounded = Physics.Raycast (player.transform.position, -Vector3.up, 0.4f);//distToGround);

		//Set velocity for X and Z axis just when button is pressed
		if (prevPressJump != pressJump && !isJumping) {
			jumpVelocityX = velocityX;
			jumpVelocityZ = velocityZ;
		}

		if (isGrounded) {
			isFalling = false;
			isJumping = false;
			//reset jumpFrames
			jumpedFrames = maxJumpFrames;
		}

		if (pressJump && isGrounded) {
			isJumping = true;
			isFalling = false;
		} 

		if ( (!isGrounded && jumpedFrames <= 0 ) || (!isGrounded && !pressJump) ) {
			isFalling = true;
			isJumping = false;
		}


		if (!isGrounded) {
			velocityX = jumpVelocityX / 1.2f;
			velocityZ = jumpVelocityZ / 1.2f;
		}

		if (isJumping) {

			float scale = (float)jumpedFrames /  (float)maxJumpFrames;
			Debug.Log ("Scale = " + scale); 

			velocityY = jumpHeight * Time.deltaTime * scale;

			jumpedFrames--;
		}
			
		//reset jumpFrames
		if (isGrounded) {
			jumpedFrames = maxJumpFrames;
		}
			
		prevPressJump = pressJump;	


		//Debug.Log ("What is up?");
		if(isJumping) Debug.Log("isJumping");
		//Debug.Log ("frames Count = " + jumpedFrames);
		if(isFalling) Debug.Log("isFalling");
		if(isGrounded) Debug.Log("isGrounded");
	}


	void Jump2(){

		//Check for hitting other object, then stop jump

		//Check if player is grounded
		bool isGrounded = Physics.Raycast (player.transform.position, -Vector3.up, 0.4f);

		//Set velocity for X and Z axis just when button is pressed
		if (prevPressJump != pressJump && !isJumping) {
			jumpVelocityX = velocityX;
			jumpVelocityZ = velocityZ;
		}
			

		if (pressJump && jumpedFrames > 0) {
			isJumping = true;
		} 
		if(!pressJump && isGrounded){
			isJumping = false;
		}

		if (!isGrounded) {
			velocityX = jumpVelocityX / 2;
			velocityZ = jumpVelocityZ / 2;
		}

		if (isJumping) {
			//Debug.Log ("jumping");
			velocityY = jumpPower * Time.deltaTime;

			jumpedFrames--;
			if (jumpedFrames < 0)
				isJumping = false;
		}
			

//		if (isGrounded)
//			jumpedFrames = maxJumpFrames;

		prevPressJump = pressJump;	
		
		Debug.Log ("isGrounded = " + isGrounded);
		Debug.Log("Jump frame = " + jumpedFrames);

	}


	void increaseSpeed(){

		if (pressPush)
			speed = highSpeed;
		else
			speed = lowSpeed;
	}


	void JumpOLD(){

		//Jump
		if (Input.GetAxis (prefix +  "_Jump") == 1) {
			if (!jumping) {
				jumping = true;
				highPos = position;
				highPos.y += 1.5f;
				transform.DOMove (highPos, jumpPower * Time.deltaTime).SetEase (anim);
			}
		}

		//Reset jumping when key released
		if (Input.GetAxis (prefix +  "_Jump") == 0)
			jumping = false;

	}

	void Move(){

		//Move on the X-axis
		if(moveHorizontal != 0)
			velocityX = moveHorizontal * speed * Time.deltaTime;

		//Move on the Z-axis
		if(moveVertical != 0)
			velocityZ = moveVertical * speed * Time.deltaTime;
		
	}

	void Push(){
		
		if (Input.GetAxis ( prefix + "_Push") == 1) {
			//Debug.Log("Pushing Square");
			setMass (extraPushPower);
		} else {
			setMass (regularPushPower);
		}
	}

	void setMass(float mass){

		//Set mass
		this.gameObject.GetComponent<Rigidbody>().mass = mass;
	}
}
