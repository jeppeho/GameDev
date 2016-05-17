using UnityEngine;
using System.Collections;

public class ShatterOnCollision : MonoBehaviour {

	AudioManager audioManager;

	private GameObject intactObject;
	private GameObject shatteredObject;
	private bool isUntouched = true;
	private bool prevIsUntouched = true;
	private int cooldown = 0;

	// Use this for initialization
	void Start () {

		audioManager = GameObject.Find ("AudioManager").GetComponent<AudioManager> ();

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
		intactObject.GetComponent<IntactObject> ().SetBreakForce (1f/*this.transform.localScale.x / 3f*/);

		//Deactive shattered shards
		shatteredObject.SetActive (false);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		if (intactObject.active)
		{
			isUntouched = intactObject.GetComponent<IntactObject> ().IsUntouched ();
	
			//Check if this is first frame, after the crystal has been touched
			if (isUntouched == false && prevIsUntouched == true) {
				ShatterObject ();
			}

			prevIsUntouched = isUntouched;
		}

		if (cooldown > 0) {
			cooldown--;
			if (cooldown <= 0) {
				resetLayers ();
			}
		}
	}
		
	public void ShatterObject(){

		intactObject.SetActive (false);
		shatteredObject.SetActive (true);

		//Add force from hand to shards
		Vector3 impactForce = intactObject.GetComponent<IntactObject> ().GetHitVector ();

		//handForce = handForce.normalized * 5;

		//For each shard
		foreach (Transform t in shatteredObject.transform) {
			
			t.GetComponent<Rigidbody> ().mass = 10;//15f;
			t.GetComponent<Rigidbody> ().drag = 0.25f;
			t.GetComponent<Rigidbody> ().angularDrag = 0.8f;

			//Add hand vector as force to hand
			t.GetComponent<Rigidbody> ().AddForce (impactForce * 50);

			t.gameObject.layer = 17; //Set to CollisionfreeObject and wait for 5 frames
			cooldown = 5;
		}

		audioManager.Play ("rockShatter", Random.Range(0.75f, 1f), this.gameObject);
	}

	private void resetLayers()
	{
		foreach (Transform t in shatteredObject.transform) {
			t.gameObject.layer = 13; //Set to ObjectsHeavy
		}
	}

}
