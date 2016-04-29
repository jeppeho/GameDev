using UnityEngine;
using System.Collections;

public class ShatterOnCollision : MonoBehaviour {

	private GameObject intactObject;
	private GameObject shatteredObject;
	private bool isUntouched = true;
	private bool prevIsUntouched = true;

	// Use this for initialization
	void Start () {

		//Go through all child objects
		foreach (Transform t in transform) {

			//Find shattered child and intactchild, by checking begining of name
			if (t.name == "shatteredObject")
			{
				shatteredObject = t.gameObject;
			}
			else if (t.name == "intactObject")
				intactObject = t.gameObject;
		}

		//Minimize pieces a bit
		Vector3 scale = shatteredObject.transform.localScale;
		scale *= 0.95f;
		shatteredObject.transform.localScale = scale;

		//Set breakforce, based on scale
		intactObject.GetComponent<IntactObject> ().SetBreakForce (this.transform.localScale.x / 3f);

		//Deactive shattered shards
		shatteredObject.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {

		isUntouched = intactObject.GetComponent<IntactObject> ().IsUntouched ();
	
		//Check if this is first frame, after the crystal has been touched
		if (isUntouched == false && prevIsUntouched == true) {
			ShatterObject ();
		}

		prevIsUntouched = isUntouched;

	}


	private void ShatterObject(){

		intactObject.SetActive (false);
		shatteredObject.SetActive (true);

		//Add force from hand to shards
		Vector3 handForce = intactObject.GetComponent<IntactObject> ().GetHitVector ();

		handForce = handForce.normalized * 5;

		//For each shard
		foreach (Transform t in shatteredObject.transform) {

			t.GetComponent<Rigidbody> ().mass = 8f;
			t.GetComponent<Rigidbody> ().drag = 3f;
			t.GetComponent<Rigidbody> ().angularDrag = 0.5f;

			//Add hand vector as force to hand
			t.GetComponent<Rigidbody> ().AddForce (handForce * 1000);
		}
	}

}
