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

	private int maxJumpTime = 20;
	private int jumpTime;
	private float jumpVelocityX;
	private float jumpVelocityZ;

	private bool jumping;
	float velocityX, velocityY, velocityZ;

	private float speed;
	private float highSpeed = 8;
	private float lowSpeed = 3;
	private float jumpHeight = 20;
	public float forwardJumpScale; //How should the jump be scaled on the X and Z-axis.
	public float jumpWaitingTime = 0;
	Vector3 position;


	public AnimationCurve anim;
	Vector3 highPos;
	//Vector3 lowPos;

	public float regularPushPower;
	public float extraPushPower;


	// Use this for initialization
	void Start () {
		jumpTime = maxJumpTime;
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
		velocityY = 0;

		//Update position
		this.gameObject.transform.position = position;
	}



	/**
	 * Returns the absolute value of the speed, by using 
	 * pythagoras to calculate the speed with velocity X and Z.
	 */
	float getCurrentSpeed(){
		//
		float speed = Mathf.Abs( Mathf.Sqrt( Mathf.Pow(velocityX, 2) + Mathf.Pow(velocityZ, 2) ) );
		//Debug.Log ("[" + velocityX + "][" + velocityZ + " = " +  speed);
		return speed;
	}

	float getAbsoluteCurrentSpeed(){
		return Mathf.Abs (getCurrentSpeed ());
	}


	float maxVelocity = 0.1f;
	float decelerationSpeed = 0.008f;

	void Move(){

		//Debug.Log ("moveHorizontal = " + moveHorizontal);
		int accelerationTime = 300;
		//int accelerationTime = 7;

		//X-Axis
		float absVelocityX = Mathf.Abs (velocityX);

		//If button or stick is pressed and target velocity not reached
		if (moveHorizontal != 0 && absVelocityX < Mathf.Abs(moveHorizontal) * maxVelocity) {

			//Value for making velocity negative value if moveHorizontal is negative value
			int value = 1;
			if (moveHorizontal < 0)
				value = -1;

			//get change of velocity
			float velocityChange = value * moveHorizontal * moveHorizontal / accelerationTime;

			//if (getAbsoluteCurrentSpeed () > 0.01f)
			//		velocityChange *= getAbsoluteCurrentSpeed () / 5;

			//Add speed to velocity
			velocityX += velocityChange;
			//velocityX += value * moveHorizontal * moveHorizontal / accelerationTime;


		} else { //If button or stick is NOT pressed AND/OR target velocity is exceeded	

			//If velocity is close to zero
			if (absVelocityX < decelerationSpeed)
				velocityX = 0;

			else //Decelerate velocity
			if (velocityX > 0)
				velocityX -= decelerationSpeed;
			else
				velocityX += decelerationSpeed;
		}


		//Z-Axis
		float absVelocityZ = Mathf.Abs (velocityZ);

		//If button or stick is pressed and target velocity not reached
		if (moveVertical != 0 && absVelocityZ < Mathf.Abs(moveVertical) * maxVelocity) {

			//Value for making velocity negative value if moveHorizontal is negative value
			int value = 1;
			if (moveVertical < 0)
				value = -1;

			//get change of velocity
			float velocityChange = value * moveVertical * moveVertical / accelerationTime;

			//if (getAbsoluteCurrentSpeed () > 0.01f)
			//		velocityChange *= getAbsoluteCurrentSpeed () / 5;

			//Add speed to velocity
			velocityZ += velocityChange;
			//velocityZ += value * moveVertical * moveVertical / accelerationTime;

		} else { //If button or stick is NOT pressed AND/OR target velocity is exceeded	

			//If velocity is close to zero
			if (absVelocityZ < decelerationSpeed)
				velocityZ = 0;

			else //Decelerate velocity
				if (velocityZ > 0)
					velocityZ -= decelerationSpeed;
				else
					velocityZ += decelerationSpeed;
		}


		//Print out current speed
		//Debug.Log("Print speed:");
		//float print = getCurrentSpeed();
	}


	void MoveOLD(){

		//Reset velocity
		velocityX = 0;
		velocityZ = 0;

		//Move on the X-axis
		if(moveHorizontal != 0)
			velocityX = moveHorizontal * speed * Time.deltaTime;

		//Move on the Z-axis
		if(moveVertical != 0)
			velocityZ = moveVertical * speed * Time.deltaTime;

	}


	/**
	 * This method will perfom a jump for a specified period 
	 * if the player is grounded and the jump button is pressed.
	 */
	void Jump(){

		//Check if player is grounded
		float distToGround = player.GetComponent<Collider>().bounds.extents.y;
		bool isGrounded = Physics.Raycast (player.transform.position, -Vector3.up, 0.4f);//distToGround);

		if (isGrounded) {
			isJumping = false;

			//Reset jumpFrames
			jumpTime = maxJumpTime;

			//Decrease waiting time
			jumpWaitingTime--;
		}

		//Jump if button is pressed, player is on ground and waiting period has passed
		if (pressJump && isGrounded && jumpWaitingTime < 0) {
			isJumping = true;
			jumpWaitingTime = 5;
			if (prevPressJump != pressJump) {
				velocityX *= 2;
				velocityY *= 2;
			}
		} 

		//Set jumping to false if times up or button released
		if ( jumpTime <= 0 || !pressJump ) {
			isJumping = false;
		}

		if (!isGrounded) {
			velocityX *= 0.93f;
			velocityZ *= 0.93f;
		}
			
		//If player is jumping increase on the Y-axis
		if (isJumping) {
			//Make the jump lose it's energy over time 
			float scale = (float)jumpTime /  (float)maxJumpTime;

			velocityY = jumpHeight * Time.deltaTime * scale;

			jumpTime--;
		}
			
		prevPressJump = pressJump;	
	}




	void increaseSpeed(){

		if (pressPush)
			maxVelocity = highSpeed;
		else
			maxVelocity = lowSpeed;
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
