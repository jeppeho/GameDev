using UnityEngine;
using System.Collections;

public class BoneBehavior : MonoBehaviour {

	private Material mat;
	private Vector3 position; //The position of this bone object
	private Vector3 desired_position; //The position of the bone recorded by the LEAP
	private Vector3 scale;
	private float mass = 0;

	private float speed = 500;
	private float minSpeed = 1;
	private float maxSpeed = 500;
	public float traction = 01;
	private float snapBackFactor = 55;

	Rigidbody rigidbody;

	// Use this for initialization
	void Start () {

		rigidbody = gameObject.GetComponent<Rigidbody> ();

		//Set mass
		rigidbody.mass = mass;

		//Set size
		gameObject.transform.localScale = scale;

		position.y = 150f;

		mat = GetComponent<Renderer> ().material;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		//Set object position to the current position of bone
		setPosition ( gameObject.GetComponent<Rigidbody> ().transform.position );

		rigidbody.mass = mass;

		//Set speed 
//		setSpeedBasedOnDistance ();
		//setMagneticSpeed();

		//Set velocity of object
		rigidbody.velocity = findVelocity ();
		if (traction < 1)
		{
			traction = Mathf.Min(1, 0.001f + traction * snapBackFactor * Time.deltaTime);
		}

		speed = Mathf.Lerp (minSpeed, maxSpeed, traction);
		mat.color = Color.Lerp (Color.white, Color.red, (traction-0.5f)*2);
	}

	void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.tag == "Environment") {

			float customImpact = Mathf.Clamp((col.relativeVelocity.sqrMagnitude- 100f) /250f, 0, 1);
			traction = 1-customImpact;
			//Kill some velocity
			rigidbody.velocity.Scale(Vector3.zero);
			//Debug.Log(col.relativeVelocity.sqrMagnitude);
		}
	}

	/**
	 *	This method will decrease the speed of a bone, the
	 *	closer the bone is to it's desired location.
	 */
	public Vector3 findVelocity(){
		//speed = 400;

		//NEW WAY  doesn't work, it flickers
		//Vector3 velocity = getVectorTowardsDesiredPosition() * getDistanceToDesiredPosition() * speed * Time.deltaTime;

		//OLD WAY - it works
		Vector3 newVelocity;

		Vector3 direction = desired_position - position;

		//If the joint is unhinged - i.e. not right next to hand
		if (traction >= 0.9f && direction.magnitude <= 0.02f) {
			newVelocity = direction * speed * Time.deltaTime;
			//Debug.Log("Hinged!");
		}
		//Otherwise..
		else
		{
			//Debug.Log("Unhinged!");
			Vector3 oldVelocity = rigidbody.velocity;
		
			Vector3 targetVelocity = direction * speed * Time.deltaTime;

			if (direction.magnitude <= 2f) {
				float targetMagnitude = 6f - direction.magnitude * 2f;
			
				targetVelocity.Scale (new Vector3 (targetMagnitude, targetMagnitude, targetMagnitude));
				//Debug.Log ("Scaled mag from " + direction.magnitude.ToString () + " to " + targetMagnitude.ToString ());
			}

			newVelocity = Vector3.Lerp (oldVelocity, targetVelocity, traction);
		}

		return newVelocity;
	}

	private void setMagneticSpeed(){

		float distance = getDistanceToDesiredPosition ();
		float minSpeed = 200;

		//If bone is relatively far away, use magnetic speed
		if (distance > 2f)
			traction = (20 - distance * distance) * 40;
		//Else use static speed
		else
			traction = 400;

		//Set a lower speed limit
		if (traction < minSpeed)
			traction = minSpeed;

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
			traction = (50 + getDistanceToDesiredPosition () * 10);
		else
			traction = 10;
		
		if (traction < 1)
			traction = 1;	
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
