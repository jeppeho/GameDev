using UnityEngine;
using System.Collections;

public class IntactObject : MonoBehaviour {

	private bool untouched = true;
	private int breakForce = 20;


	void OnCollisionEnter(Collision col){

		//If collider layer is Hand
//		if (col.gameObject.layer == 8)

		//If threshold force is used
		if (col.relativeVelocity.magnitude > breakForce) {
			//Set untouched to false
			untouched = false;
		}
	}

	public bool IsUntouched(){
		return untouched;
	}
}
