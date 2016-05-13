using UnityEngine;
using System.Collections;

public class SystemManager : MonoBehaviour {

	private InputControllerHandler inputHandler;

//	int godPlayerIndex = -1;
//	public Material[] materials;

	int currentGodMaterialIndex = 3; //to become minion color
	int minionWinnerPlayerIndex = 0; //The minion to change color on
	int minionWinnerMaterialIndex = 0; //to become world color

	int prevLevel = -1;

	GameObject[] minions;

	// Use this for initialization
	void Start () {
		
		Debug.Log ("SystemManager has started !!!");

		//Get the input handler
		inputHandler = this.gameObject.GetComponent<InputControllerHandler> ();

		//Set boundaries for players to move within
		SetMainMenuBoundaries ();

		//Put in a minion for each connected controller
		inputHandler.CreateMinionsForControllers (false);

		minions = inputHandler.GetMinionsArray ();
	}
		

	void OnLevelWasLoaded(int level){
		
		Debug.Log ("OnLevelWasLoaded !!! @" + level);

		//Main menu
		if (level == 0) {

			SetMainMenuBoundaries ();

			//Put in a minion for each connected controller
			inputHandler.CreateMinionsForControllers (false);

			minions = inputHandler.GetMinionsArray ();

		//If going from main menu to LevelGenerator
		} 

		if (level == 1 && prevLevel != 1) {

			DeactivateUnusedPlayers();

		}

		//Levelgenerator
		if (level == 1) {

			SetLevelBoundaries ();

			ActivateActivatedMinions ();

			if (prevLevel == 1)
				UpdateColorScheme ();	
		}

		prevLevel = level;
	}



	void Awake() {
		//Keep the system manager from destroying when changing scenes
		DontDestroyOnLoad(transform.gameObject);
	}


	private void UpdateColorScheme(){

		Debug.Log ("Running: UpdateColorScheme()");

		//Change color on winning minion
		GameObject[] minions = inputHandler.GetMinionsArray ();


		Debug.Log ("Setting material #"  + minionWinnerMaterialIndex+ " for player " + minionWinnerPlayerIndex);
		minions[minionWinnerPlayerIndex].GetComponent<PlayerManager> ().SetMaterial (currentGodMaterialIndex);

		//Update currentGodMaterial to that of the winner
		currentGodMaterialIndex = minionWinnerMaterialIndex;

		//SET SKYBOX AND DIRECTIONAL LIGHTING AND WHATEVS
		//using:
		//currentGodMaterialIndex

		currentGodMaterialIndex = minionWinnerMaterialIndex;
	}


	public InputControllerHandler GetInputHandler(){
		return this.inputHandler;
	}
		

	private void ActivateActivatedMinions(){

		Debug.Log ("Running: ActivateActivatedMinions()");

		//Put in a minion for each activated controller
		for (int i = 0; i < minions.Length; i++) {

			//Move to start position
			minions [i].transform.position = new Vector3 (inputHandler.GetXPositionForMinion(i), 2, 0);
			minions [i].GetComponent<PlayerManager> ().SetState (PlayerManager.state.active);
			//minions[i].GetComponent<PlayerManager
		}
	}


	private void DeactivateUnusedPlayers(){

		Debug.Log ("Running DeactivateUnusedPlayers() ");

		for (int i = 0; i < minions.Length; i++) {

			Debug.Log ("Check for deactivating players " + i);

			string playerState = minions [i].GetComponent<PlayerManager> ().GetState ();

			if (playerState == "inactive" ) {
				minions [i].SetActive (false);
				Debug.Log ("Removing player " + i);
			} else {
				minions [i].SetActive (true);
				Debug.Log ("Keeping player " + i);
			}
		}
	}


	private void CreateMinions(){

		//Put in a minion for each connected controller
		inputHandler.CreateMinionsForControllers (false);

	}

	public void SetMinionWinner(int playerIndex, int materialIndex){
		
		this.minionWinnerPlayerIndex = playerIndex;
		this.minionWinnerMaterialIndex = materialIndex;

		Debug.Log ("God #" + currentGodMaterialIndex + " has been defeated");
		Debug.Log ("Minion #" + minionWinnerPlayerIndex + " is the winner");
		Debug.Log ("With material #" + minionWinnerMaterialIndex);

	}
		

	private void SetMainMenuBoundaries(){

		LevelManager.MOVE_MAXZ = 20f;
		LevelManager.MOVE_MINZ = -20f;
	}

	private void SetLevelBoundaries(){

		LevelManager.MOVE_MAXZ = 10f;
		LevelManager.MOVE_MINZ = -9f;
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
}
