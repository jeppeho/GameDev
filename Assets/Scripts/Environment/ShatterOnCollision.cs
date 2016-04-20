using UnityEngine;
using System.Collections;

public class ShatterOnCollision : MonoBehaviour {

	private GameObject intactObject;
	private GameObject shatteredObject;

	// Use this for initialization
	void Start () {

		//Go through all child objects
		foreach (Transform t in transform) {

			//Find shattered child and intactchild
			if (t.name == "shatteredObject")
				shatteredObject = t.gameObject;
			else if (t.name == "intactObject")
				intactObject = t.gameObject;
			
		}

		shatteredObject.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {

		if (intactObject.GetComponent<IntactObject> ().IsUntouched () == false)
			ShatterObject ();
	}

	private void ShatterObject(){
		intactObject.SetActive (false);
		shatteredObject.SetActive (true);
	}



}
