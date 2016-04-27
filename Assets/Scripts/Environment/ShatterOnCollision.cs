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
		intactObject.GetComponent<IntactObject> ().SetBreakForce (this.transform.localScale.x / 2f);

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
		Vector3 force = intactObject.GetComponent<IntactObject> ().GetHitVector ();

		//Push shard to the ground
		force.y = -500;

		force *= Time.deltaTime * 4000;

		foreach (Transform t in shatteredObject.transform) {
			//Debug.Log ("name = " + t.name);
			t.GetComponent<Rigidbody> ().AddForce (force);
		}
	}



}
