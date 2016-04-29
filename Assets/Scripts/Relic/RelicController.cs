using UnityEngine;
using System.Collections;

public class RelicController : MonoBehaviour {

	private float cameraZOffsetBound = -3.5f;

	//private Vector3 targetPosition;
	private Rigidbody rb;
	private Transform parent;

	private bool movingToTarget;

	private GameObject camera;

	//Bounds - should be put in a LevelManager script
	float minX, maxX, minY, minZ;

	RelicManager manager;

	int escapeDirection = 1;
	float prevX = 0;


	// Use this for initialization
	void Start () {
		rb = this.gameObject.GetComponent<Rigidbody> ();
		manager = this.GetComponent<RelicManager> ();
		//	ATTENSION - from this line forth, nothing makes sense. The manager can be debugged, but no method within it can be called, without a null-point-exception. After 1,5 hours of trying to resolve this error, I give up. /Nils
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
				float wall = camera.GetComponent<CameraController> ().GetPosition ().z + cameraZOffsetBound;
				if (manager.GetPosition ().z < wall ) {
					PushForward ();
				}



				if (manager.GetPosition ().z < wall - 1f) {
					GetUnstuck (wall);
				}
			}

			//Make sure velocity doesn't go nuts
			SnapToPlayer();

			CapVelocity ();

		} else {
			//Follow parent
			FlyAboveParent ();
		}
	}


	/**
	 * When the relic is whitin reach of a player,
	 * it will addforce towards the position of the player
	 */
	private void SnapToPlayer(){

		float radius = 1.5f;

		Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, radius);

		for (int col = 0; col < hitColliders.Length; col++) {
		
			if (hitColliders [col].tag == "Player") {

				if (hitColliders [col].GetComponent<PlayerManager> ().GetState () == "active") {
					//Get vector towards player
					Vector3 vecTowardsPlayer = hitColliders [col].transform.position - rb.transform.position;

					//Negate the relative velocity between player and relic
					Vector3 playerVelocity = hitColliders [col].GetComponent<Rigidbody> ().velocity;
					Vector3 relativeVelocity = rb.velocity - playerVelocity;

					vecTowardsPlayer -= (relativeVelocity * Time.deltaTime);

					//Add downforce so it does not fly over player
					vecTowardsPlayer.y = Mathf.Abs (vecTowardsPlayer.y) - 2f;

					vecTowardsPlayer *= Time.deltaTime * 10000f;

					rb.AddForce (vecTowardsPlayer);
				}
			}
		}
	}

	/**
	 * Whenever the relic is behind the camera, fx if it's stuck somewhere, 
	 * this method will try to get in the camera FOV by pushing it forward and 
	 * move it from side to side. If to far behind, it will change its position to 
	 * right behind the camera.
	 */
	private void GetUnstuck(float wall){

		//Get Current z position for camera
		float cameraZ = camera.GetComponent<CameraController>().GetZPosition ();
		float relicZ = rb.transform.position.z;

		float relicX = rb.transform.position.x;

		float distToCamera = cameraZ - relicZ;

		int maxDist = 7;

		//If relic is right behind camera, then push it forward
		if (distToCamera < maxDist && distToCamera > 0) {

			Vector3 force = new Vector3 (relicX * -1 + distToCamera, distToCamera, distToCamera);

			force *= Time.deltaTime * 1000;

			rb.AddForce (force);


		} else if(distToCamera > maxDist){
			//If relic is to far behind camera, move it and push forward
			rb.transform.position = new Vector3 (relicX / 2, 5, cameraZ - maxDist);
			rb.AddForce (0, 0, Time.deltaTime * 3000);
		}

	}


	/**
	 * Whenever the relic is behind the camera, fx if it's stuck somewhere, 
	 * this method will try to get in the camera FOV by pushing it forward and 
	 * move it from side to side
	 */
	private void GetUnstuckOLD(float wall){

		float currentRelicX = rb.transform.position.x;

		//If relic is out to the sides or at the same position at the prev position
		if (currentRelicX > 4f || currentRelicX < -4f || prevX == currentRelicX)
			escapeDirection = -1;

		//Get Current z position for camera
		float currentCameraZ = manager.GetPosition ().z;

		//Get distance to camera wall
		float distanceToView = wall - currentCameraZ;

		float x = 100f * escapeDirection;

		float y = 0;
		if (currentCameraZ < wall - 1f) {
			if (rb.transform.position.y < 20f) {
				y =  200f;
			}
//			if (rb.transform.position.y < 4f) {
//				y = (4f - rb.transform.position.y) * 2000f;
//			}
		}

		rb.AddForce (new Vector3 (x, y, 100f * distanceToView));

		prevX = currentRelicX;

	}


	private void CapVelocity(){

		float maxSpeed = 10f;

		if (rb.velocity.magnitude > maxSpeed) {
			rb.velocity = rb.velocity.normalized * maxSpeed;
		}
	}

	private void PushToCenter(){
	
		Vector3 force = new Vector3 ();

		if (Mathf.Abs (manager.GetPosition ().x) > LevelManager.MAX_X / 3)
			force.x = 20 * manager.GetPosition ().x * -1;

		force.x += Random.Range (-20, 20);

		rb.AddForce ( force );
	}

	private void PushForward(){
		rb.AddForce( new Vector3( 0, 0, 20 ) );
	}



	//Updates the position to be above the parents head
	private void FlyAboveParent(){
		Vector3 slotPosition = manager.GetParent().position;
		slotPosition.y += 1.1f;
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

		Vector3 bodyVelocity = this.gameObject.GetComponentInParent<Rigidbody>().velocity;
		Vector3 throwDirection = this.gameObject.GetComponentInParent<NewController> ().GetDirection ();

		Vector3.Normalize (throwDirection);

		throwDirection.y += 0.1f;
		throwDirection *= 750 + 5000 * Mathf.Pow(force,2f);
		Debug.Log ("throw Force = " + force);

		manager.ReleaseFromParent ();

		//throwDirection *= Time.deltaTime * 13f; //For new simple prefab
		throwDirection *= Time.deltaTime * 100f; //For old prefab 

		rb.AddForce ( /*bodyVelocity +*/ throwDirection );

		count++;
	}


}
