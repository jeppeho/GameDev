using UnityEngine;
using System.Collections;

public class IntactObject : MonoBehaviour {

	private bool untouched = true;
	private int breakForce = 20;
	private Vector3 hitVector;


	void OnCollisionEnter(Collision col){

		//If collider layer is not Hand
		if (col.gameObject.layer != 10) {

			//If threshold force is used
			if (col.relativeVelocity.magnitude > breakForce) {
				//Set untouched to false
				untouched = false;
				hitVector = col.relativeVelocity;
			}
		}
	}

	public bool IsUntouched(){
		return untouched;
	}

	public Vector3 GetHitVector(){
		return this.hitVector;
	}

	public void SetBreakForce(float force){
		this.breakForce = Mathf.FloorToInt(force);
	}
}
