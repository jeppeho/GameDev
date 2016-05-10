using UnityEngine;
using System.Collections;

public class AvoidElevation : MonoBehaviour {

	Rigidbody rb;

	// Use this for initialization
	void Start () {
		rb = this.gameObject.GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter(Collision col){

		if (col.gameObject.tag == "Environment") {

			//If moving forward
			if (rb.velocity.z > 0) {

				//BECAUSE THE MINION ORIGO IS NOT IN Y = 0... Puhaaaa.
				float minionOffset = 0.3f;

				if (col.transform.position.y > rb.position.y - minionOffset) {

					//rb.AddForce (new Vector3 (0, 2f, 0));// * Time.deltaTime);
					rb.AddForce (new Vector3 (0, 100f, 0) * Time.deltaTime);
				}
			}
		}
	}
}
