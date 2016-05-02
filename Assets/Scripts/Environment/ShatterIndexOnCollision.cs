using UnityEngine;
using System.Collections;

public class ShatterIndexOnCollision : MonoBehaviour {

	private GameObject[] intactObject;
	private GameObject[] shatteredObject;

	public int instances;
	public int startingIndex;

	private bool isUntouched = true;
	private bool prevIsUntouched = true;
	private int cooldown = 0;
	private int finalIndex;

	// Use this for initialization
	void Start () {

		intactObject = new GameObject[instances];
		shatteredObject = new GameObject[instances];

		Debug.Log ("Got this far...");

		//Go through all child objects
		foreach (Transform t in transform) {

			//Find shattered child and intactchild, by checking begining of name
			if (t.name.StartsWith("intactObject"))
			{
				//Read index, and add 00 as prefix, because bug-safely.
				string strIndex = t.name.Insert(12, "00").Substring (12, 3);
				int index = 1;

				if (strIndex != null)
				{
					index = int.Parse (strIndex);
				}

				intactObject [index-startingIndex] = t.gameObject;
				Debug.Log ("Found intact obj. no. " + index.ToString());
			}

			else if (t.name.StartsWith("shatteredObject"))
			{
				//Read index, and add 00 as prefix, because bug-safely.
				string strIndex = t.name.Insert(15, "00").Substring (15, 3);
				int index = 1;

				if (strIndex != null) {
					index = int.Parse (strIndex);
				}

				shatteredObject [index-startingIndex] = t.gameObject;
				Debug.Log ("Found shattered obj. no. " + index.ToString());
			}




		}

		//Minimize pieces a bit
		for (int i = 0; i < instances; i++) 
		{
			Vector3 scale = shatteredObject[i].transform.localScale;
			scale *= 0.95f;

			shatteredObject [i].transform.localScale = scale;

			//Set breakforce, based on scale
			intactObject [i].GetComponent<IntactObject> ().SetBreakForce (1f);

			//Deactive shattered shards
			shatteredObject [i].SetActive (false);
		}
	}

	// Update is called once per frame
	void FixedUpdate () {
		
		for (int i = 0; i < instances; i++)
		{
			if (intactObject [i].active)
			{
				IntactObject childScript = intactObject [i].GetComponent<IntactObject> ();
				isUntouched = childScript.IsUntouched ();

				childScript.UpdateCooldown ();

				//Check if this is first frame, after the crystal has been touched
				if (isUntouched == false && prevIsUntouched == true) {
					finalIndex = i;
					ShatterObject (finalIndex);
				}

				prevIsUntouched = isUntouched;
			}
		}

		if (cooldown > 0) {
			cooldown--;
			if (cooldown <= 0) {
				resetLayers (finalIndex);
			}
		}
	}

	private void ShatterObject(int n){
		
		intactObject[n].SetActive (false);
		shatteredObject[n].SetActive (true);

		//Add force from hand to shards
		Vector3 impactForce = intactObject[n].GetComponent<IntactObject> ().GetHitVector ();

		//Push shard to the ground
		//handForce.y = -500;

		//For each shard
		foreach (Transform t in shatteredObject[n].transform) {

			t.GetComponent<Rigidbody> ().mass = 15f;
			t.GetComponent<Rigidbody> ().drag = 0.25f;
			t.GetComponent<Rigidbody> ().angularDrag = 0.8f;
			t.GetComponent<Rigidbody> ().useGravity = true;

			//Add hand vector as force to hand
			t.GetComponent<Rigidbody> ().AddForce (impactForce * 75);

			t.gameObject.layer = 17; //Set to CollisionfreeObject
			cooldown = 5;
			Debug.Log ("Shattering pillar!");
		}
	}

	private void resetLayers(int n)
	{
		foreach (Transform t in shatteredObject[n].transform) {
			t.gameObject.layer = 13; //Set to ObjectsHeavy
		}
	}
}
