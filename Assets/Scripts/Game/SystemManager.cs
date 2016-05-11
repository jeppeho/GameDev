using UnityEngine;
using System.Collections;

public class SystemManager : MonoBehaviour {

	InputControllerHandler inputHandler;

	//The index of the current active scene
	int activeScene = 0;

	void Awake() {
		//Keep the system manager from destroying when changing scenes
		DontDestroyOnLoad(transform.gameObject);
	}

	void OnLevelWasLoaded(int level){

		activeScene = level;
		
		Debug.Log ("OnLevelWasLoaded !!! @" + level);

		//Main menu
		if (level == 1) {

			//Put in a minion for each connected controller
			inputHandler.CreateMinionsForControllers (false);

			int num = inputHandler.GetNumAcceptedControllers ();

			Debug.Log ("numControllers = " + num );

			//If less than three connected controllers
			if (num < 2) {
			
				//Put in a keyboard minion
				inputHandler.CreateMinion(new Vector3(3, 2, 0), "KEYBOARD", 3, false);
			}

		//Levelgenerator
		} else if (level == 2) {


			//Put in a minion for each activated controller

		}


	}

	// Use this for initialization
	void Start () {
		inputHandler = this.gameObject.GetComponent<InputControllerHandler> ();
	}

	// Update is called once per frame
	void Update () {

		if (activeScene == 1) {


		
		}

	}

}
