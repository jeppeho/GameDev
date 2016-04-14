using UnityEngine;
using System.Collections;

public class RelicController : MonoBehaviour {

	public float cameraZOffsetBound = 4f;

	//private Vector3 targetPosition;
	private Rigidbody rb;
	private Transform parent;

	private bool movingToTarget;

	private GameObject camera;

	//Bounds - should be put in a LevelManager script
	float minX, maxX, minY, minZ;


	// Use this for initialization
	void Start () {
		rb = this.gameObject.GetComponent<Rigidbody> ();
		UpdateFreezeRotation (false);
		movingToTarget = false;

		minX = 0; maxX = 10.5f; minY = 0; minZ = -150; //Bounds - should be put in a LevelManager script

		camera = GameObject.Find ("LeapControllerBlockHand");
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (GetParent() == null) {

			//Check if below ground
			if (GetPosition ().y < 0.2f) {
			
				rb.AddForce (new Vector3 (0, 2000f, 0));
			
			} else {

				PushToCenter ();

				//If relic is about to go behind camera
				if (GetPosition ().z < camera.GetComponent<CameraController> ().GetPosition ().z + cameraZOffsetBound) {
					PushForward ();
				}

			}

			//Make sure velocity doesn't go nuts
			CapVelocity ();

			//OLD WAY
//			if (!IsWithinBounds ()) {
//
//				if (GetPosition ().y < 1f) {
//				
//					rb.AddForce (new Vector3 (0, 40f, 0));
//
//				} else {
//					
//					if (movingToTarget == false) {
//						//movingToTarget = true;
//						StartCoroutine (MoveTowardsCenter ());
//					}
//				}
//			}
		} else {
			//Follow parent
			FlyAboveParent ();
		}
	}


	private void CapVelocity(){

		float maxSpeed = 10f;

		if (rb.velocity.magnitude > maxSpeed) {
			rb.velocity = rb.velocity.normalized * maxSpeed;
		}
	}

	private void PushToCenter(){
	
		Vector3 force = new Vector3 ();

		if (Mathf.Abs (GetPosition ().x) > 4)
			force.x = 20 * GetPosition ().x * -1;

		force.x += Random.Range (-20, 20);

		rb.AddForce ( force );
	}

	private void PushForward(){
		rb.AddForce( new Vector3( 0, 0, 550 ) );
	}


	/**
	 * Check if the relic is within the bounds of the level
	 */
	public bool IsWithinBounds(){
		//if (GetPosition ().x > maxX || GetPosition ().x < minX || GetPosition ().y < minY || GetPosition().z < minZ)
		//LevelManager lm = new LevelManager();
		if (GetPosition ().x < LevelManager.MIN_X || GetPosition ().x > LevelManager.MAX_X || GetPosition ().y < LevelManager.MIN_Y || GetPosition().z < LevelManager.RELIC_MINZ)
			return false;
		else
			return true;
	}
		

	/*
	 * Returns true if the relic is below ground level, otherwise false
	 */
	public bool IsBelowGround(){
		if (GetPosition ().y < 0)
			return true;
		else
			return false;
	}

	//Updates the position to be above the parents head
	private void FlyAboveParent(){
		Vector3 slotPosition = GetParent().position;
		slotPosition.y += 2f;
		UpdatePosition( slotPosition );
	}


	/**
	 * Moves the relic to the center of the level and a bit forward
	 */
	IEnumerator MoveTowardsCenter(){

		Vector3 cameraPos = camera.GetComponent<CameraController> ().GetPosition ();
		Debug.Log ("cameraPos  = " + cameraPos);
		//Get target position for dropped carried object
		Vector3 target = new Vector3 (5, 1, cameraPos.z );
		Debug.Log ("Ball Target = " + target);
		Vector3 vectorToTarget = target - GetPosition (); 
		vectorToTarget.Normalize ();

		//Move carried object towards target position
		while(!IsWithinBounds()){
			rb.AddForce( vectorToTarget );

			yield return new WaitForSeconds (0f);
		}

		if (IsWithinBounds()) {
			movingToTarget = false;
		}
	}


	/**
	 * Moves the relic to the center of the level and a bit forward
	 */
//	IEnumerator MoveTowardsCenterOLD(){
//
//		//Get target position for dropped carried object
//		Vector3 target = new Vector3 (5, GetPosition ().y, GetPosition ().z + (3f * LevelManager.SPEED) );
//
//		Vector3 vectorToTarget = target - GetPosition (); 
//		vectorToTarget.Normalize ();
//		
//		//Move carried object towards target position
//		while(!IsWithinBounds()){
//			rb.AddForce( vectorToTarget );
//		
//			yield return new WaitForSeconds (0f);
//		}
//
//		if (IsWithinBounds()) {
//			movingToTarget = false;
//		}
//	}
		

	public void Throw(float force){

		int count = 0;
		//Debug.Log ("throwDirection = " + parent.GetComponent<NewController> ().GetDirection ());
		Vector3 bodyVelocity = this.gameObject.GetComponentInParent<Rigidbody>().velocity;
		Vector3 throwDirection = this.gameObject.GetComponentInParent<NewController> ().GetDirection ();

		Debug.Log ("orig. throwDirection = " + throwDirection);
		Vector3.Normalize (throwDirection);

		throwDirection.y += 0.65f;
		throwDirection *= 250 + 1000 * Mathf.Pow(force,1.8f);
		Debug.Log ("bodyVelocity = " + bodyVelocity);
		Debug.Log ("throwDirection = " + throwDirection);
		RemoveParent ();

		rb.AddForce ( /*bodyVelocity +*/ throwDirection );
	
		count++;
	}


	/**
	 * Updates the freezeRotation parameter, with the inputted bool value
	 */
	public void UpdateFreezeRotation(bool rotate){
		rb.freezeRotation = rotate;
	}

	/**
	 * Set current rotation to zero
	 */
	public void ResetRotation(){
		rb.transform.localRotation = new Quaternion();
	}

	/**
	 * Set a parent to the relic
	 * Minimize the scale
	 * Freeze rotation
	 * And resets rotation to zero.
	 */
	public void SetParent(Transform parent){
		this.transform.SetParent (parent, false);
		UpdateScale (0.8f);
		UpdateFreezeRotation (true);
		ResetRotation ();
	}

	/**
	 * Sets the parent variable to null,
	 * updates the scale
	 * and unfreezes the rotation
	 */
	public void RemoveParent(){
		this.transform.parent = null;
		UpdateScale (1f);
		UpdateFreezeRotation (false);
	}

	public void UpdateScale(float scale){
		rb.transform.localScale = new Vector3 (scale, scale, scale);
	}


	public float GetScale(){
		return rb.transform.localScale.x;
	}
		

	public Transform GetParent(){
		return this.transform.parent;
	}

	public void UpdatePosition(Vector3 position){
		rb.transform.position = position;
	}

	public Vector3 GetPosition (){
		return rb.transform.position;
	}
}
