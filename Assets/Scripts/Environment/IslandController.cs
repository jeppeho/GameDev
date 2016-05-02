using UnityEngine;
using System.Collections;

public class IslandController : MonoBehaviour {

	Transform t;

	public float threshold = 15;
	public float maxForce = 15;

	// Use this for initialization
	void Start () {
		t = GetComponent<Transform> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	void OnCollisionEnter(Collision col){
		
		//If collider layer is Hand
		if (col.gameObject.layer == 8) {

			Debug.Log ("col.force = " + col.relativeVelocity.magnitude);

			//If threshold force is used
			if (col.relativeVelocity.magnitude > threshold) {

				//If hand is moving downwards
				if (col.relativeVelocity.y < 0) {

					StartCoroutine (MoveDown (col.relativeVelocity.magnitude));
				
				}
			}
		}

	}

	IEnumerator MoveDown(float magnitude){

		Vector3 vel = new Vector3(0,0,0);

		int numCycles = 2;

		//vel.y += magnitude / 1200 / numCycles;

		vel.y = Mathf.Sqrt (magnitude) / 1000 / numCycles * maxForce;


		Debug.Log ("pos = " + t.position + " | vel = " + vel);

		int i = 0;

		while (i < numCycles) {
			t.position -= vel;
			i++;
			yield return new WaitForSeconds (0.1f);
		}

	}


	IEnumerator MoveDownOLD(float magnitude){

		Vector3 vel = new Vector3(0,0,0);

		int numCycles = 3;

		//vel.y += magnitude / 1200 / numCycles;

		Debug.Log ("pos = " + t.position + " | vel = " + vel);

		int i = 0;

		while (i < numCycles) {
			t.position -= vel;
			i++;
			yield return new WaitForSeconds (0.1f);
		}
	}



}
