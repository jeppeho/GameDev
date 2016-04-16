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

	RelicManager manager;


	// Use this for initialization
	void Start () {
		rb = this.gameObject.GetComponent<Rigidbody> ();
		manager = GetComponent<RelicManager> ();
		manager.UpdateFreezeRotation (false);
		movingToTarget = false;

		minX = 0; maxX = 10.5f; minY = 0; minZ = -150; //Bounds - should be put in a LevelManager script

		camera = GameObject.Find ("LeapControllerBlockHand");
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (!manager.HasParent()) {

			//Check if below ground
			if (manager.GetPosition ().y < 0.2f) {
			
				rb.AddForce (new Vector3 (0, 2000f, 0));
			
			} else {

				PushToCenter ();

				//If relic is about to go behind camera
				if (manager.GetPosition ().z < camera.GetComponent<CameraController> ().GetPosition ().z + cameraZOffsetBound) {
					PushForward ();
				}
			}

			//Make sure velocity doesn't go nuts
			CapVelocity ();

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

		if (Mathf.Abs (manager.GetPosition ().x) > 4)
			force.x = 20 * manager.GetPosition ().x * -1;

		force.x += Random.Range (-20, 20);

		rb.AddForce ( force );
	}

	private void PushForward(){
		rb.AddForce( new Vector3( 0, 0, 550 ) );
	}



	//Updates the position to be above the parents head
	private void FlyAboveParent(){
		Vector3 slotPosition = manager.GetParent().position;
		slotPosition.y += 2f;
		manager.UpdatePosition( slotPosition );
	}


	/**
	 * Moves the relic to the center of the level and a bit forward
	 */
	IEnumerator MoveTowardsCenter(){

		Vector3 cameraPos = camera.GetComponent<CameraController> ().GetPosition ();

		//Get target position for dropped carried object
		Vector3 target = new Vector3 (5, 1, cameraPos.z );
		Vector3 vectorToTarget = target - manager.GetPosition (); 
		vectorToTarget.Normalize ();

		//Move carried object towards target position
		while(!manager.IsWithinBounds()){
			rb.AddForce( vectorToTarget );

			yield return new WaitForSeconds (0f);
		}

		if (manager.IsWithinBounds()) {
			movingToTarget = false;
		}
	}

		

	public void Throw(float force){

		int count = 0;
		//Debug.Log ("throwDirection = " + parent.GetComponent<NewController> ().GetDirection ());
		Vector3 bodyVelocity = this.gameObject.GetComponentInParent<Rigidbody>().velocity;
		Vector3 throwDirection = this.gameObject.GetComponentInParent<NewController> ().GetDirection ();

		Vector3.Normalize (throwDirection);

		throwDirection.y += 0.65f;
		throwDirection *= 250 + 1000 * Mathf.Pow(force,1.8f);

		manager.RemoveParent ();

		rb.AddForce ( /*bodyVelocity +*/ throwDirection );

		count++;
	}


}
