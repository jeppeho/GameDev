using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SystemCheatCodes : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if (Input.GetKey ("m")) 
			RestartToMainMenu ();
		

		if (Input.GetKey ("s")) 
			RestartCurrentScene ();

		for (int i = 0; i <= 6; i++) {

			if (Input.GetKey (i.ToString())) {

				GoToScene(i);
			}
		
		}

		//if(Input.GetKey("1") || Input.GetKey("2") || Input.GetKey("3") ||  Input.GetKey("4") || Input.GetKey("5") || Input.GetKey("6") || 
	} 

	/* Delete the relic if it is in the scene */
	private void DeleteRelic(){
		GameObject relic = GameObject.FindGameObjectWithTag ("Relic");
		if(relic != null)
			Destroy (relic);
	}


	private void RestartToMainMenu(){
		DeleteRelic ();
		Application.LoadLevel (1);
	
	}

	private void GoToScene(int index){

		DeleteRelic ();
		Application.LoadLevel (index);
	}

	private void RestartCurrentScene(){

		DeleteRelic ();

		int index = SceneManager.GetActiveScene ().buildIndex;
		Application.LoadLevel (index);
	}
}
