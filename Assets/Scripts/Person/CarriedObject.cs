using UnityEngine;
using System.Collections;

public class CarriedObject : MonoBehaviour {


	GameObject carriedObject;
	Vector3 slotPosition;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

		//@todo
		//only if player is alive
		if (carriedObject != false) {
			slotPosition = this.transform.position;
			slotPosition.y += 1f;
			carriedObject.transform.position = slotPosition;
			if (Input.GetKey ("p")) {
				DropObject ();
			}
		}
	}


	void OnCollisionEnter(Collision collision){
		if (collision.gameObject.tag == "Relic") {

			//Set relic as carriedObject
			carriedObject = GameObject.FindGameObjectWithTag ("Relic");

			//Set player as parent to Relic
			carriedObject.transform.SetParent (this.gameObject.transform, false);

			//Freeze relic rotation
			carriedObject.GetComponent<Rigidbody> ().freezeRotation = true;
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
}
