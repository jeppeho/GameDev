using UnityEngine;
using System.Collections;

public class SystemManager : MonoBehaviour {

	private InputControllerHandler inputHandler;

//	int godPlayerIndex = -1;
//	public Material[] materials;

	int currentGodMaterialIndex = 3; //to become minion color
	int minionWinnerPlayerIndex = 0; //The minion to change color on
	int minionWinnerMaterialIndex = 0; //to become world color



	// Use this for initialization
	void Start () {
		Debug.Log ("SystemManager has started !!!");
		inputHandler = this.gameObject.GetComponent<InputControllerHandler> ();
	}

	void Awake() {
		//Keep the system manager from destroying when changing scenes
		DontDestroyOnLoad(transform.gameObject);

	}

	void OnLevelWasLoaded(int level){
		
		Debug.Log ("OnLevelWasLoaded !!! @" + level);

		//Main menu
		if (level == 1) {

			SetMainMenuBoundaries ();

			int num = inputHandler.GetNumAcceptedControllers ();
			Debug.Log ("numControllers = " + num );

			//Put in a minion for each connected controller
			inputHandler.CreateMinionsForControllers (false);


			//If less than three connected controllers
//			if (num < 3) {
//			
//				//Put in a keyboard minion
//				inputHandler.CreateMinion(new Vector3(3, 2, 0), "KEYBOARD", 3, false);
//			}

		//Levelgenerator
		} else if (level == 2) {

			SetLevelBoundaries ();

			ActivateActivatedMinions ();

			UpdateColorScheme ();


		}
	}



	private void UpdateColorScheme(){

		Debug.Log ("Running: UpdateColorScheme()");

		//Change color on winning minion
		GameObject[] minions = inputHandler.GetMinionsArray ();

		minions[minionWinnerPlayerIndex].GetComponent<PlayerManager> ().SetMaterial (currentGodMaterialIndex);

		//Update currentGodMaterial to that of the winner
		currentGodMaterialIndex = minionWinnerMaterialIndex;

		//SET SKYBOX AND DIRECTIONAL LIGHTING AND WHATEVS
		//using:
		//currentGodMaterialIndex
	}


	public InputControllerHandler GetInputHandler(){
		return this.inputHandler;
	}


//
//	public void SetGodIndex(int index){
//		this.godPlayerIndex = index;
//	}
//

//	/**
//	 * Sets new material for the minions
//	 */
//	private void SetColorsForMinions(){
//
//		GameObject[] minions = inputHandler.GetMinionsArray ();
//
//		//Put in a minion for each activated controller
//		for (int i = 0; i < minions.Length; i++) {
//		
//			if (i != godPlayerIndex) {
//				minions [i].GetComponent<PlayerManager> ().SetMaterial (i);
//			}
//		}
//	}

	private void ActivateActivatedMinions(){

		Debug.Log ("Running: ActivateActivatedMinions()");

		GameObject[] minions = inputHandler.GetMinionsArray ();

		//Put in a minion for each activated controller
		for (int i = 0; i < minions.Length; i++) {

			if (minions [i].GetComponent<PlayerManager> ().GetState () == "inactive") {
				minions [i].SetActive (false);
			} else {
				minions [i].SetActive (true);
			}

			minions [i].transform.position = new Vector3 (inputHandler.GetXPositionForMinion(i), 1, 0);
		}
	}


	private void CreateMinions(){

		//Put in a minion for each connected controller
		inputHandler.CreateMinionsForControllers (false);

	}

	public void SetMinionWinner(int playerIndex /*, int materialIndex*/){
		
		this.minionWinnerPlayerIndex = playerIndex;
		//this.minionWinnerMaterialIndex = materialIndex;

		Debug.Log ("minionWinnerPlayerIndex" + minionWinnerPlayerIndex);
		Debug.Log ("minionWinnerMaterialIndex" + minionWinnerMaterialIndex);

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
