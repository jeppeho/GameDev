using UnityEngine;
using System.Collections;

public class PushObject : MonoBehaviour {

	private Rigidbody person_;

	// Use this for initialization
	void Start () {
		//Get rigidbody of gameObject
		person_ = this.gameObject.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
	
		if(Input.GetAxis("Push") == 1){
			//Debug.Log("Pushing Square");
			setMass (0.0008f);
		}else{
			setMass (0.0004f);
		}
	}

		void setMass(float mass){
			
			//Set mass
			person_.mass = mass;

			//Debug.Log ("Mass set to: " + person_.mass);
		}
}
