using UnityEngine;
using System.Collections;

public class CarriedObject : MonoBehaviour {


	GameObject carriedObject;
	Rigidbody rb;
	private Vector3 slotPosition;
	bool dropping;

	string playerState;

	// Use this for initialization
	void Start () {
		dropping = false;
	}
	
	// Update is called once per frame
	void Update () {

		//Get state of player
		playerState = this.gameObject.GetComponent<PlayerManager> ().GetState ();

		//Check if carried object contains an object
		if (carriedObject != null) {

			//Update rigidbody of carried object
			rb = carriedObject.gameObject.GetComponent<Rigidbody> ();

			//If not dropping game object
			if (dropping == false) {

				//Check if player is dead
				if (playerState == "dead") {

					StartCoroutine (DropObjectInCenter ());
				
				} else {

					//Update position
					slotPosition = this.transform.position;
					slotPosition.y += 1f;
					carriedObject.transform.position = slotPosition;
				}

				//If key is pressed
				if (Input.GetKey ("p")) {
					StartCoroutine(DropObjectInCenter ());
				}	
			}
		}
	}


	void OnCollisionEnter(Collision collision){

		if (playerState == "active") {
			if (collision.gameObject.tag == "Relic") {

				//Set relic as carriedObject
				carriedObject = GameObject.FindGameObjectWithTag ("Relic");

				//Set player as parent to Relic
				carriedObject.transform.SetParent (this.gameObject.transform, false);

				//Freeze relic rotation
				rb.freezeRotation = true;
			}
		}
	}
		

	void DropObject(){

		//add value to z-position
		slotPosition.z += 2f;

		//Set it a bit forward into scene
		carriedObject.transform.position = slotPosition;

		//Remove parent object
		carriedObject.transform.parent = null;

		//Scale up (it has been scaled down when picking up)
		carriedObject.transform.localScale /= 0.3f;

		//Remove carried object
		carriedObject = null;
	}

	IEnumerator DropObjectInCenter(){

		dropping = true;

		//Unfreeze relic rotation
		rb.freezeRotation = false;

		//Position of the carried object
		Vector3 position = rb.transform.position;

		//Move carried object upwards
		while (position.y < 0f) {
			position = rb.transform.position;
			rb.AddForce ( new Vector3(0, 50f, 0));
			Debug.Log ("Relic.y = " + position.y);
			yield return new WaitForSeconds (0f);
			
		}

		int frame = 0;
		//Create vector from current position towards center
		position = rb.transform.position;

		//Get target position for dropped carried object
		Vector3 target = new Vector3 (5, position.y, position.z + 5f);

		//Get the vector towards the target
		Vector3 vectorToTarget = (target - position);
		vectorToTarget.Normalize ();
		vectorToTarget *= 50;

		//Move carried object towards target position
		while(frame < 10){
			rb.AddForce( vectorToTarget );
			yield return new WaitForSeconds (0f);
			frame += 1;
		}


		//Remove parent object
		carriedObject.transform.parent = null;

		//Scale up (it has been scaled down when picking up)
		carriedObject.transform.localScale /= 0.3f;

		//Remove carried object
		carriedObject = null;

		dropping = false;
	}


	IEnumerator DropObjectInCenterNEW(){

		dropping = true;

		//Unfreeze relic rotation
		rb.freezeRotation = false;

		//Create vector from current position towards center
		Vector3 position = rb.transform.position;
		Vector3 center = position;
		center.x = 5;
		center.y = position.y + 5f;
		center.z = position.z + 5f;

		Vector3 directionToCenter = (center - position);
		Debug.Log ("1. CENTER vec = " + directionToCenter);
		directionToCenter.Normalize ();
		Debug.Log ("2. CENTER vec = " + directionToCenter);
		directionToCenter *= 30;


		while (rb.transform.position.y < 5f) {
			rb.AddForce( new Vector3(0,3f,0));
			position = rb.transform.position;
			yield return new WaitForSeconds (0f);
		}

		//Move carried object towards center
//		int frame = 0;
//		while (frame < 10) {
//			rb.AddForce( directionToCenter );
//			yield return new WaitForSeconds (0f);
//			frame += 1;
//		}

		//Remove parent object
		carriedObject.transform.parent = null;

		//Scale up (it has been scaled down when picking up)
		carriedObject.transform.localScale /= 0.3f;

		//Remove carried object
		carriedObject = null;

		dropping = false;
	}
}
