using UnityEngine;
using System.Collections;

public class PlayerRelicHandler : MonoBehaviour {


	public GameObject carriedObject = null;
	private Rigidbody rb;
	//private bool dropping;

	string playerState;

	// Use this for initialization
	void Start () {
		//dropping = false;
	}
	
	// Update is called once per frame
	void Update () {

		//COULD I CHECK THIS IN THE PLAYERMANAGER???
		//Get state of player
		playerState = this.gameObject.GetComponent<PlayerManager> ().GetState ();

		//If player not active, release object
		if (playerState != "active") {

			if (carriedObject != null) {
				ReleaseRelic ();
			}
		}
	}

	public bool HasRelic(){
		if (carriedObject != null)
			return true;
		else
			return false;
	}


	void OnCollisionEnter(Collision collision){
		//Debug.Log ("Running OnCollisionEnter() in CarriedObject script");

		if (playerState == "active") {

			if (carriedObject == null) {

				if (collision.gameObject.tag == "Relic") {

					//Set relic as carriedObject
					carriedObject = collision.gameObject;

					//Set player as parent to Relic
					carriedObject.GetComponent<RelicManager>().SetParent(this.gameObject.transform);
				}
			}
		}
	}

	public void ReleaseRelic(){
		carriedObject.GetComponent<RelicManager>().ReleaseFromParent();
		carriedObject = null;
	}
		
	/**
	 * Not in use anymore...
	 */
//	IEnumerator DropObjectInCenter(){
//
//		dropping = true;
//
//		//Unfreeze relic rotation
//		rb.freezeRotation = false;
//
//		//Position of the carried object
//		Vector3 position = rb.transform.position;
//
//		//Move carried object upwards
//		while (position.y < 0f) {
//			position = rb.transform.position;
//			rb.AddForce ( new Vector3(0, 50f, 0));
//			Debug.Log ("Relic.y = " + position.y);
//			yield return new WaitForSeconds (0f);
//			
//		}
//
//		int frame = 0;
//		//Create vector from current position towards center
//		position = rb.transform.position;
//
//		//Get target position for dropped carried object
//		Vector3 target = new Vector3 (5, position.y, position.z + 5f);
//
//		//Get the vector towards the target
//		Vector3 vectorToTarget = (target - position);
//		vectorToTarget.Normalize ();
//		vectorToTarget *= 50;
//
//		//Move carried object towards target position
//		while(frame < 10){
//			rb.AddForce( vectorToTarget );
//			yield return new WaitForSeconds (0f);
//			frame += 1;
//		}
//
//
//		//Remove parent object
//		carriedObject.transform.parent = null;
//
//		//Scale up (it has been scaled down when picking up)
//		carriedObject.transform.localScale /= 0.3f;
//
//		//Remove carried object
//		carriedObject = null;
//
//		dropping = false;
//	}
}
