using UnityEngine;
using System.Collections;

public class IntactObjectStart : MonoBehaviour {

	private int breakForce = 0;


	void Update(){

		if (Input.GetKey ("l")) {
			Invoke( "ChangeLevel", 3.0f );
		}

	}

	void OnCollisionEnter(Collision col){

		//If collider layer is not Hand
		if (col.gameObject.layer != 10) {

			//If threshold force is used
			if (col.relativeVelocity.magnitude > breakForce) {

				float fadeTime = GameObject.Find("FadingScenes").GetComponent<FadingScenes>().BeginFade (1);
				Invoke( "ChangeLevel", 3.0f );
			}
		}

	}
		

	void ChangeLevel() {
		Application.LoadLevel ("LevelGenerator");  
	}
}