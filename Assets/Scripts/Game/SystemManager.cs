using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/**
 * the system manager class has information about
 * 
 */

public class SystemManager : MonoBehaviour {

	private InputControllerHandler inputHandler;
	private ColorHandler colorHandler;

//	int godPlayerIndex = -1;
//	public Material[] materials;

	int currentGodMaterialIndex = 0; //to become minion color
	int minionWinnerPlayerIndex = 2; //The minion to change color on
	int minionWinnerMaterialIndex = 2; //to become world color

	int prevLevel = 0;

	GameObject[] minions;
	int[] minionColorIndexes;


	void Awake() {
		//Keep the system manager from destroying when changing scenes
		DontDestroyOnLoad(transform.gameObject);
	}


	// Use this for initialization
	void Start () {

		//Get the input handler
		inputHandler = GetComponent<InputControllerHandler> ();
		colorHandler = GetComponent<ColorHandler> ();

		prevLevel = 0;

		minionColorIndexes = new int[ 3 ];

		SceneManager.LoadScene("MainMenu");
	}


	public int GetCurrentGodMaterialIndex(){
		return currentGodMaterialIndex; 
	}


		
	/**
	 * Updates the colors on the minions and update the minionColorIndexes as well
	 */
	private void UpdateMinionColors(){

		for (int i = 0; i < 3; i++) {

			int colorIndex = i;

			if (i >= currentGodMaterialIndex)
				colorIndex++;

			colorHandler.SetMinionColor (minions [i], colorIndex);
			minionColorIndexes[i] = colorIndex;
		}

	}


	/**
	 * Scene order
	 * 0) The opening load scene
	 * 1) Main menu
	 * 2) Tutorial lobby scene
	 * 3) Tutorial basics
	 * 4) Tutoroal Earthquake
	 * 5) Tutorial Summon
	 * 6) The Level!!!
	 * 
	 */
	int testIndex = 0;

	void OnLevelWasLoaded(int level){
		
//		Debug.Log (testIndex + "]*****OnLevelWasLoaded | level = " + level + " | prevLevel = " + prevLevel);
//		Debug.Log ("God: " + currentGodMaterialIndex + ", winMinion: " + minionWinnerPlayerIndex + ", minionMaterial: " + minionWinnerMaterialIndex);
//		testIndex++;
		Time.timeScale = 1f;

		colorHandler.SetColorScheme (currentGodMaterialIndex);

		//ResetActivatedMinionsToStartOfLevel ();

		//First time in the main menu after game start
		if (level == 1 && prevLevel == 0) {

			inputHandler.CreateThreeMinions (2f);

			minions = inputHandler.GetMinionsArray ();

			colorHandler.SetColorScheme (currentGodMaterialIndex);
			UpdateMinionColors ();
		}

		//Everytime you go to main menu
		if (level == 1) {
			//Debug.Log ("level == 1");

			SetMenuBoundaries ();

			ActivateAllMinionGameObjects ();
		}

		//Everytime last scene was main menu
		if (prevLevel == 1) {
			//Debug.Log ("prevLevel == 1");

			DeactivateUnusedPlayers ();
		}

		//If not the opening load scene or the level
		if (level != 0 && level != 6) {
			//Debug.Log ("level != 0 && level != 6");

			PositionMinionsInRow(1f);
		}

		//If previous scene was the level
		if (prevLevel == 6) {
			//Debug.Log ("prevLevel == 6");
		
			//UpdateWinnersAndLosers ();

			colorHandler.SetColorScheme (currentGodMaterialIndex);
			UpdateMinionColors ();
		}


		//Everytime you go to the level
		if (level == 6) {
			//Debug.Log ("level == 6");

			DeactivateUnusedPlayers ();

			SetLevelBoundaries ();
			PositionMinionsInRow (5f);
		}


		prevLevel = level;
	}


	public void UpdateWinnersAndLosersForLevelRunner(int playerIndex, int materialIndex){

		//Store current values
		int prevGod = currentGodMaterialIndex;
		int prevWinner = minionColorIndexes [playerIndex];

		//Update with new values
		minionColorIndexes [playerIndex] = prevGod;
		currentGodMaterialIndex = prevWinner;

		//Set indeces
		minionWinnerPlayerIndex = playerIndex;
		minionWinnerMaterialIndex = materialIndex;
	
	}


	public void UpdateAllColors(){

		//Set colors on obstacles, update skybox and update the elementsMaterial color
		colorHandler.SetColorScheme ( minionColorIndexes[ minionWinnerPlayerIndex] );

		//Set the winning minions material to that of the losing god
		colorHandler.SetMinionColor (minions [minionWinnerPlayerIndex], currentGodMaterialIndex);
	
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


	/**
	 * Repositions minions in a row and sets them to active if they are dead or invulnerable
	 */
	private void PositionMinionsInRow(float z){

		float space = 3f;
		int numPlayers = 3;
		float width = space * (numPlayers - 1f);

		for (int i = 0; i < minions.Length; i++) {

			PlayerManager pm = minions [i].GetComponent<PlayerManager> ();

			//Set position
			float x = -space + i * space;
			Vector3 position = new Vector3 (x, 2f, z);

			minions [i].transform.position = position;

			//If not inactive, set to active
			string currentState = pm.GetState ();

			if (currentState != "inactive") {

				pm.SetState (PlayerManager.state.active);

			}

			pm.ResetLights ();
		}

	}


	/**
	 * Repositions minions in a row and sets them to active if they are dead or invulnerable
	 */
	private void PositionMinionsInRowOLD(float z){

		float space = 3f;
		int numPlayers = 3;
		float width = space * (numPlayers - 1f);

		for (int i = 0; i < minions.Length; i++) {
		
			//Set position
			float x = -space + i * space;
			Vector3 position = new Vector3 (x, 2f, z);

			minions [i].transform.position = position;

			//If not inactive, set to active
			string currentState = minions [i].GetComponent<PlayerManager> ().GetState ();

			if (currentState != "inactive") {

				minions [i].GetComponent<PlayerManager> ().SetState (PlayerManager.state.active);
			
			}
		}

	}


//	private void ResetActivatedMinionsToStartOfLevel(float z){
//
//		//Debug.Log ("Running: ActivateActivatedMinions()");
//
//		//Put in a minion for each activated controller
//		for (int i = 0; i < minions.Length; i++) {
//
//			PlayerManager pm = minions [i].GetComponent<PlayerManager> ();
//
//			//Move to start position
//			minions [i].transform.position = new Vector3 (inputHandler.GetXPositionForMinion(i), 3, z);
//			//minions [i].GetComponent<PlayerManager> ().SetState (PlayerManager.state.active);
//
//			//If not inactive, set to active
//			string currentState = minions [i].GetComponent<PlayerManager> ().GetState ();
//			if (currentState != "inactive") {
//
//				pm.SetState (PlayerManager.state.active);
//			}
//			pm.ResetLightDetails ();
//
//		}
//	}


	private void DeactivateUnusedPlayers(){

		for (int i = 0; i < minions.Length; i++) {

			string playerState = minions [i].GetComponent<PlayerManager> ().GetState ();

			if (playerState == "inactive" ) {
				minions [i].SetActive (false);
			} else {
				minions [i].SetActive (true);
			}
		}
	}

	private void ActivateAllMinionGameObjects(){
	
		for (int i = 0; i < minions.Length; i++) {
			minions [i].gameObject.SetActive (true);
		}
	}


	private void SetMenuBoundaries(){

		LevelManager.MOVE_MAXZ = 40f;
		LevelManager.MOVE_MINZ = -20f;
	}

	private void SetLevelBoundaries(){

		LevelManager.MOVE_MAXZ = 18f;
		LevelManager.MOVE_MINZ = -9f;
	}




	//	public void SetMinionWinner(int playerIndex, int materialIndex){
	//		Debug.Log ("Setting the minion Winner in SystemManager");
	//		this.minionWinnerPlayerIndex = playerIndex;
	//		this.minionWinnerMaterialIndex = materialIndex;
	//
	//		Debug.Log ("minionWinnerPlayerIndex: " + minionWinnerPlayerIndex + " | minionWinnerMaterialIndex: " + minionWinnerMaterialIndex);
	//
	//	}

	//	public void UpdateWinnersAndLosers(){
	//		
	//		//Update values
	//		int prevGod = currentGodMaterialIndex;
	//		int prevWinner = minionColorIndexes [minionWinnerPlayerIndex];
	//		minionColorIndexes [minionWinnerPlayerIndex] = prevGod;
	//		currentGodMaterialIndex = prevWinner;
	//
	//	}

	//	public void KillAllPlayers(){
	//
	//		for (int i = 0; i < minions.Length; i++) {
	//
	//			Destroy (minions [i]);
	//		
	//		}
	//	}



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


	//	void OnLevelWasLoadedOLD(int level){
	//
	//		//On level load set time scale to 1
	//		Time.timeScale = 1f;
	//
	//		Debug.Log ("OnLevelWasLoaded !!! @" + level + " prevLevel = " + prevLevel);
	//
	//		colorHandler.SetColorScheme (currentGodMaterialIndex);
	//
	//		if (level == 0 && prevLevel == 5) {
	//
	//			KillAllPlayers ();
	//
	//			minions = inputHandler.GetMinionsArray ();
	//
	//			colorHandler.SetColorScheme (currentGodMaterialIndex);
	//
	//			//colorHandler.SetSkybox (currentGodMaterialIndex);
	//
	//			//Set initial colors for players
	//			for (int i = 0; i < minions.Length; i++) {
	//				colorHandler.SetMinionColor (minions [i], i);
	//				minionColorIndexes[i] = i;
	//			}
	//		}
	//
	////		//Main menu
	////		if (level == 0) {
	////
	////			SetMainMenuBoundaries ();
	////
	////		//If going from main menu to LevelGenerator
	////		} 
	//
	//		if (level == 5 && prevLevel != 5) {
	//
	//			DeactivateUnusedPlayers();
	//
	//		}
	//
	//		//Levelgenerator, right after main menu
	//		if (level == 5 && prevLevel == 0) {
	//
	//			colorHandler.SetSkybox (currentGodMaterialIndex);
	//
	//			//Update levelboundaries for relic and players
	//			SetLevelBoundaries ();
	//
	//			//Remove unused players
	//			ResetActivatedMinionsToStartOfLevel (5f);
	//		}
	//
	//		//Levelgenerator again
	//		if (level == 5 && prevLevel == 5) {
	//
	////			Debug.Log ("//////////Reloading levelGenerator");
	////			Debug.Log ("winner color = " + minionWinnerMaterialIndex);
	////			Debug.Log ("currentGodMaterialIndex = " + currentGodMaterialIndex);
	//
	//			ResetActivatedMinionsToStartOfLevel (5f);
	//
	//			UpdateAllColors ();
	//			Debug.Log ("GodWin = " + GodWon);
	//			UpdateWinnersAndLosers ();
	//		}
	//
	//		prevLevel = level;
	//	}

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



	//
	//	void StartOLD () {
	//
	//		Debug.Log ("SystemManager has started !!!");
	//
	//		//Get the input handler
	//		inputHandler = GetComponent<InputControllerHandler> ();
	//		colorHandler = GetComponent<ColorHandler> ();
	//
	//
	//		//Set boundaries for players to move within
	//		SetMainMenuBoundaries ();
	//
	//		//Put in a minion for each connected controller
	//		//inputHandler.CreateMinionsForControllers (false);
	//		inputHandler.PutInThreeMinions (false);
	//
	//		minions = inputHandler.GetMinionsArray ();
	//		minionColorIndexes = new int[ minions.Length ];
	//
	//		colorHandler.SetColorScheme (currentGodMaterialIndex);
	//		//colorHandler.SetSkybox (currentGodMaterialIndex);
	//
	//		//Set initial colors for players
	//		for (int i = 0; i < minions.Length; i++) {
	//
	//			colorHandler.SetMinionColor (minions [i], i);
	//			minionColorIndexes[i] = i;
	//		}
	//
	//		for (int i = 0; i < 3; i++) {
	//
	//			int colorIndex = i;
	//
	//			if (i >= currentGodMaterialIndex)
	//				colorIndex++;
	//
	//			colorHandler.SetMinionColor (minions [i], colorIndex);
	//
	//			minionColorIndexes[i] = colorIndex;
	//		}
	//
	//	}

}
