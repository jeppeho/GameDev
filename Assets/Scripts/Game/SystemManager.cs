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

			SetMainMenuBoundaries ();

			//Put in a minion for each connected controller
			inputHandler.CreateMinionsForControllers (false);

			int num = inputHandler.GetNumAcceptedControllers ();

			Debug.Log ("numControllers = " + num );

			//If less than three connected controllers
//			if (num < 3) {
//			
//				//Put in a keyboard minion
//				inputHandler.CreateMinion(new Vector3(3, 2, 0), "KEYBOARD", 3, false);
//			}

		//Levelgenerator
		} else if (level == 2) {

			SetLevelBoundaries ();

			GameObject[] minions = inputHandler.GetMinionsArray ();

			for (int i = 0; i < minions.Length; i++) {


				if (minions [i].GetComponent<PlayerManager> ().GetState () == "inactive") {
					minions [i].SetActive (false);
				} else {
					minions [i].SetActive (true);
				}
				minions [i].transform.position = new Vector3 (inputHandler.GetXPositionForMinion(i), 1, 0);

			}

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


	private void SetMainMenuBoundaries(){

		LevelManager.MOVE_MAXZ = 20f;
		LevelManager.MOVE_MINZ = -20f;
	}

	private void SetLevelBoundaries(){

		LevelManager.MOVE_MAXZ = 10f;
		LevelManager.MOVE_MINZ = -9f;
	}

}
