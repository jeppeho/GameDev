using UnityEngine;
using System.Collections;

public class BoneBehavior : MonoBehaviour {


	private Vector3 position; //The position of this bone object
	private Vector3 desired_position; //The position of the bone recorded by the LEAP
	private Vector3 velocity; //The velocity of this bone object
	private Vector3 scale; 
	private float mass = 0;

	private float speed = 200;

	Rigidbody rigidbody;

	// Use this for initialization
	void Start () {

		rigidbody = gameObject.GetComponent<Rigidbody> ();

		//Set mass
		rigidbody.mass = mass;

		//Set size
		gameObject.transform.localScale = scale;

		position.y = 150f;

	}
	
	// Update is called once per frame
	void Update () {



		//Set object position to the current position of bone
		setPosition ( gameObject.GetComponent<Rigidbody> ().transform.position );

		rigidbody.mass = mass;

		//Set speed 
//		setSpeedBasedOnDistance ();
		setMagneticSpeed();

		//Set velocity of object
		rigidbody.velocity = setVelocityLinear();
	}


	/**
	 *	This method will decrease the speed of a bone, the
	 *	closer the bone is to it's desired location.
	 */
	public Vector3 setVelocityLinear(){
		//speed = 400;

		//NEW WAY  doesn't work, it flickers
		//Vector3 velocity = getVectorTowardsDesiredPosition() * getDistanceToDesiredPosition() * speed * Time.deltaTime;

		//OLD WAY - it works
		Vector3 direction = desired_position - position;
		velocity = direction * speed * Time.deltaTime;

		return velocity;
	}

	private void setMagneticSpeed(){

		float distance = getDistanceToDesiredPosition ();
		float minSpeed = 200;

		//If bone is relatively far away, use magnetic speed
		if (distance > 2f)
			speed = (20 - distance * distance) * 40;
		//Else use static speed
		else
			speed = 400;

		//Set a lower speed limit
		if (speed < minSpeed)
			speed = minSpeed;

		//OLD VERSION
//		if (distance > 3f) {
//			speed = 250;
//		} else if (distance > 1f) {
//			speed = 400 + (3 - distance ) * 400;
//		} else {
//			speed = 400;
//		}


	}


	/**
	 *	This method will increase the speed of the bone, the
	 *	closer it is to it's desired location.
	 *	When it is within some distance it should ease-in though.
	 */
	private void setSpeedBasedOnDistance(){

		if (getDistanceToDesiredPosition() > 1)
			speed = (50 + getDistanceToDesiredPosition () * 10);
		else
			speed = 10;
		
		if (speed < 1)
			speed = 1;	
	}


	/**
	 * 	Returns the vector from current position to desired position
	 */
	private Vector3 getVectorToDesiredPosition(){
		return desired_position - position;
	}

	public float getDistanceToDesiredPosition(){
		return getVectorToDesiredPosition ().magnitude;
	}

	public void setDesiredPosition(Vector3 desiredPosition){
		this.desired_position = desiredPosition;
	}

	public Vector3 getDesiredPosition(){
		return desired_position;
	}

	public void setPosition(Vector3 position){
		this.position = position;
	}

	public void SetPositionY(float y){
		this.position.y = y;
	}

	public Vector3 getPosition(){
		return this.position;
	}

	public void setScale(Vector3 scale){
		this.scale = scale;
	}

	public void setMass(float mass){
		this.mass = mass;
	}

	public float getMass(){
		return this.mass;
	}

}
