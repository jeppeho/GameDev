using UnityEngine;
using System.Collections;


/**
 * the system manager class has information about
 * 
 */

public class SystemManager : MonoBehaviour {

	private InputControllerHandler inputHandler;
	private ColorHandler colorHandler;

//	int godPlayerIndex = -1;
//	public Material[] materials;

	int currentGodMaterialIndex = 3; //to become minion color
	int minionWinnerPlayerIndex = 0; //The minion to change color on
	int minionWinnerMaterialIndex = 0; //to become world color

	int prevLevel = 0;

	GameObject[] minions;
	int[] minionColorIndexes;

	//Temp var for playtest
	public bool GodWon = false;


	void Awake() {
		//Keep the system manager from destroying when changing scenes
		DontDestroyOnLoad(transform.gameObject);
	}

	// Use this for initialization
	void Start () {
		
		Debug.Log ("SystemManager has started !!!");

		//Get the input handler
		inputHandler = GetComponent<InputControllerHandler> ();
		colorHandler = GetComponent<ColorHandler> ();


		//Set boundaries for players to move within
		SetMainMenuBoundaries ();

		//Put in a minion for each connected controller
		//inputHandler.CreateMinionsForControllers (false);
		inputHandler.PutInThreeMinions (false);

		minions = inputHandler.GetMinionsArray ();
		minionColorIndexes = new int[ minions.Length ];

		colorHandler.SetColorScheme (currentGodMaterialIndex);
		//colorHandler.SetSkybox (currentGodMaterialIndex);

		//Set initial colors for players
		for (int i = 0; i < minions.Length; i++) {
			colorHandler.SetMinionColor (minions [i], i);
			minionColorIndexes[i] = i;
		}



	}
		

	void OnLevelWasLoaded(int level){

		//On level load set time scale to 1
		Time.timeScale = 1f;

		Debug.Log ("OnLevelWasLoaded !!! @" + level + " prevLevel = " + prevLevel);

		if (level == 0 && prevLevel == 2) {

			KillAllPlayers ();

			minions = inputHandler.GetMinionsArray ();

			colorHandler.SetColorScheme (currentGodMaterialIndex);

			//colorHandler.SetSkybox (currentGodMaterialIndex);

			//Set initial colors for players
			for (int i = 0; i < minions.Length; i++) {
				colorHandler.SetMinionColor (minions [i], i);
				minionColorIndexes[i] = i;
			}
		}

//		//Main menu
//		if (level == 0) {
//
//			SetMainMenuBoundaries ();
//
//		//If going from main menu to LevelGenerator
//		} 

		if (level == 2 && prevLevel != 2) {

			DeactivateUnusedPlayers();

		}

		//Levelgenerator, right after main menu
		if (level == 2 && prevLevel == 0) {

			colorHandler.SetSkybox (currentGodMaterialIndex);

			//Update levelboundaries for relic and players
			SetLevelBoundaries ();

			//Remove unused players
			ResetActivatedMinionsToStartOfLevel ();
		}

		//Levelgenerator again
		if (level == 2 && prevLevel == 2) {

			Debug.Log ("//////////Reloading levelGenerator");
			Debug.Log ("winner color = " + minionWinnerMaterialIndex);
			Debug.Log ("currentGodMaterialIndex = " + currentGodMaterialIndex);

			ResetActivatedMinionsToStartOfLevel ();

			UpdateAllColors ();
			Debug.Log ("GodWin = " + GodWon);
			UpdateWinnersAndLosers ();
		}

		prevLevel = level;
	}

	//		//Levelgenerator
	//		if (level == 1) {
	//
	//			if (prevLevel == 0) {
	//
	//				Debug.Log ("////Setting skybox from SystemManager");
	//				//Set skybox
	//				colorHandler.SetSkybox (currentGodMaterialIndex);
	//
	//				//Update levelboundaries for relic and players
	//				SetLevelBoundaries ();
	//
	//				//Remove unused players
	//				ResetActivatedMinions ();
	//			
	//			}
	//			else if (prevLevel == 1) {
	//				Debug.Log ("//////////Reloading levelGenerator");
	////				Debug.Log ("winner color = " + minionWinnerMaterialIndex);
	////				Debug.Log ("currentGodMaterialIndex = " + currentGodMaterialIndex);
	//
	//				ResetActivatedMinions ();
	//
	//				UpdateAllColors ();
	//
	//			}
	//		}

	public void UpdateWinnersAndLosers(){
		
		//Update values
		int prevGod = currentGodMaterialIndex;
		int prevWinner = minionColorIndexes [minionWinnerPlayerIndex];
		minionColorIndexes [minionWinnerPlayerIndex] = prevGod;
		currentGodMaterialIndex = prevWinner;

	}

	public void UpdateAllColors(){

		colorHandler.SetColorScheme ( minionColorIndexes[ minionWinnerPlayerIndex] );
		colorHandler.SetMinionColor (minions [minionWinnerPlayerIndex], currentGodMaterialIndex);
		//colorHandler.SetSkybox (minionWinnerPlayerIndex);

	}

	public void UpdateAllColorsOLD(){

		colorHandler.SetColorScheme ( minionColorIndexes[ minionWinnerPlayerIndex] );
		colorHandler.SetMinionColor (minions [minionWinnerPlayerIndex], currentGodMaterialIndex);
		//colorHandler.SetSkybox (minionWinnerPlayerIndex);

		//Update values
		int prevGod = currentGodMaterialIndex;
		int prevWinner = minionColorIndexes [minionWinnerPlayerIndex];
		minionColorIndexes [minionWinnerPlayerIndex] = prevGod;
		currentGodMaterialIndex = prevWinner;

	}


	public InputControllerHandler GetInputHandler(){
		return this.inputHandler;
	}
		

	private void ResetActivatedMinionsToStartOfLevel(){

		Debug.Log ("Running: ActivateActivatedMinions()");

		//Put in a minion for each activated controller
		for (int i = 0; i < minions.Length; i++) {

			//Move to start position
			minions [i].transform.position = new Vector3 (inputHandler.GetXPositionForMinion(i), 3, 0);
			minions [i].GetComponent<PlayerManager> ().SetState (PlayerManager.state.active);

		}
	}


	private void DeactivateUnusedPlayers(){

		for (int i = 0; i < minions.Length; i++) {

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



	public void SetMinionWinner(int playerIndex, int materialIndex){
		
		this.minionWinnerPlayerIndex = playerIndex;
		this.minionWinnerMaterialIndex = materialIndex;

	}

	public void KillAllPlayers(){

		for (int i = 0; i < minions.Length; i++) {

			Destroy (minions [i]);
		
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


	//	private void UpdateColorScheme(){
	//
	//		Debug.Log ("Running: UpdateColorScheme()");
	//
	//		//Change color on winning minion
	//		GameObject[] minions = inputHandler.GetMinionsArray ();
	//
	//		Debug.Log ("Setting material #"  + minionWinnerMaterialIndex+ " for player " + minionWinnerPlayerIndex);
	//		//minions[minionWinnerPlayerIndex].GetComponent<PlayerManager> ().SetMaterial (currentGodMaterialIndex);
	//
	//		//Update currentGodMaterial to that of the winner
	//		currentGodMaterialIndex = minionWinnerMaterialIndex;
	//	}

}
