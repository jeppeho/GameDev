﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelRunner : MonoBehaviour {

	private enum GameState { running, paused, godWin, minionWin }; 
	private GameState gameState;
	private GameState prevGameState;

	private GameObject relic;

	private float pauseGameButton;
	private float prevPauseGame;

	private bool playedWinMusic;

	public Canvas pauseMenu;
	public Canvas godWinMenu;
	public Canvas minionWinMenu;

	LevelGenerator lg;
	SystemManager sm;
	GameObject leap;

	GameObject[] minions;
	int winningMinion = -1;

	// Use this for initialization
	void Start () {

		//Deactivate menus
		pauseMenu.gameObject.SetActive (false);
		godWinMenu.gameObject.SetActive (false);
		minionWinMenu.gameObject.SetActive (false);

		lg = GameObject.Find ("LevelGenerator").GetComponent<LevelGenerator>();
		sm = GameObject.Find ("SystemRunner").GetComponent<SystemManager>();
		leap = GameObject.Find ("LeapControllerBlockHand");

		minions = sm.GetInputHandler ().GetMinionsArray ();

		relic = GameObject.Find ("Relic");
		playedWinMusic = false;

		//Set initial gameState
		gameState = GameState.running;
		prevGameState = GameState.running;
	}


	/* This should deactivate button presses when menu is set on 
	 * BUT it doesn't work as timeScale is set to zero, and the event systems
	 * are therefore not turned on again after the WaitForSeconds
	 */
	IEnumerator DeactivateInput(){

		Transform godMenu = godWinMenu.GetComponent<Transform> ();
		Transform godEs = godMenu.Find ("EventSystem");

		//Transform pauseMenu = pauseMenu.GetComponent<Transform> ();
		Transform pauseEs = pauseMenu.GetComponent<Transform> ().Find ("EventSystem");

		//Transform minionMenu = minionWinMenu.GetComponent<Transform> ();
		Transform minionEs = minionWinMenu.GetComponent<Transform> ().Find ("EventSystem");

		godEs.gameObject.SetActive (false);
		minionEs.gameObject.SetActive (false);
		pauseEs.gameObject.SetActive (false);

		yield return new WaitForSeconds(0.5f);

		Debug.Log ("Turning input on");

		godEs.gameObject.SetActive (true);
		minionEs.gameObject.SetActive (true);
		pauseEs.gameObject.SetActive (true);

		//godWinMenu.transform.Find ("EventSystem").SetActive (true);

		yield return new WaitForEndOfFrame();
	}


	// Update is called once per frame
	void Update () {

		UpdateState ();

//		if (gameState != prevGameState) {
//			StartCoroutine(DeactivateInput ());
//		}
//		prevGameState = gameState;

		if (gameState == GameState.paused) {
			
			PauseGame ();
			//StartCoroutine( PauseGameKeepRegisteringInput () );
		} 
		if (gameState == GameState.running) {
			Time.timeScale = 1f;
		} 

		if (gameState == GameState.godWin) {
			Time.timeScale = 0f;
			godWinMenu.gameObject.SetActive (true);

			if (!playedWinMusic)
			{
				// ~~ SOUND ~~
				GameObject.Find("AudioManager").GetComponent<MusicManager>().MinionsWin();
				playedWinMusic = true;
			}
		}

		if (gameState == GameState.minionWin) {
			if (winningMinion == -1) {
				FindTheWinningMinion ();
			}
			//DeactiveLosingMinions (winningMinion);
			KeepWinningMinion (winningMinion);
			Time.timeScale = 1f;
			minionWinMenu.gameObject.SetActive (true);
		}

		//Stop camera when level end is reached
		if (leap.GetComponent<Transform> ().position.z > lg.levelLength + 10f) {

			leap.GetComponent<LeapObjectController> ().enabled = false;			
		}

//		else 
//			Time.timeScale = 1;
//
//			if (gameState == GameState.running) {
//
//			} 
//		else if (gameState == GameState.godWin) {
//			SceneManager.LoadScene("LevelGenerator");
//		}
//		else if (gameState == GameState.minionWin) {
//
//		}
		//}

	}

	private void UpdateState(){

		UpdateButtonInput ();

		//Check if god win
		if (relic.GetComponent<RelicHealth> ().GetHealth () < 0f) {

			gameState = GameState.godWin;

		} 
		else if( relic.gameObject.GetComponent<Transform>().position.z > lg.levelLength + 12f){

			gameState = GameState.minionWin;
		}

		else if (prevPauseGame < 0.05f && pauseGameButton > 0.05f /*|| Input.GetKeyUp ("k")*/ ) {

			if(gameState == GameState.running)
				gameState = GameState.paused;
			//			else if(gameState == GameState.paused)
			//				gameState = GameState.running;

		} 

		prevPauseGame = pauseGameButton;

	}


	private void FindTheWinningMinion(){

//		Debug.Log ("//////FINDING THE WINNER");
//		Debug.Log ("We have " + minions.Length +  " minions");

		int winningMinionIndex = -1;
		float highestScore = -1000; 

		for (int i = 0; i < minions.Length; i++) {

			Debug.Log ("Testing minion #" + i);

			if (minions [i].activeSelf == true) {
				//Debug.Log ("Minion #" + i + " is active");
				float minionScore = minions [i].GetComponent<PlayerManager> ().GetPlayerScore ().GetFinalScore ();
				//Debug.Log ("minion #" + i + " score = " + minionScore + " and highscore = " + highestScore);

				//If score is so far the highest then update the index of the winning minion
				if (minionScore > highestScore) {
					highestScore = minionScore;
					winningMinionIndex = i;
					//Debug.Log ("Setting the new winner to index " + winningMinionIndex);
				}

				//If score is the same as the highest
				else if (minionScore == highestScore) {

					//Then there's 50/50 chance if we update it
					if (Random.Range (0f, 1f) > 0.5f) {
						winningMinionIndex = i;
					}
				}
			}
		}

		//Debug.Log ("Highscore is now = " + highestScore + " for minion #" + winningMinionIndex);

		int materialIndex = minions [winningMinionIndex].GetComponent<PlayerManager> ().GetCurrentMaterialIndex ();

		//Update local variable
		winningMinion = winningMinionIndex;

		sm.UpdateWinnersAndLosersForLevelRunner (winningMinionIndex, materialIndex);
	}



	private void KeepWinningMinion(int index){
	
		Vector3 pos = minions [index].GetComponent<Transform>().position;
	
		pos.y = 1;


		minions [index].GetComponent<PlayerManager> ().SetState (PlayerManager.state.invulnerable);

		minions [index].GetComponent<Transform> ().position = pos;
	}


	private void ReleaseRelicForAllPlayers(){

		for(int i = 0; i < minions.Length; i++){

			PlayerRelicHandler h = minions [i].GetComponent<PlayerRelicHandler> ();

			if (h.HasRelic ()) {
				h.ReleaseRelic ();
				break;
			}
		}
	}


	private void DeactiveLosingMinions(int winningIndex){

		//Go through all minions
		for (int i = 0; i < minions.Length; i++) {

			//If it's not the winning minion
			if (i != winningIndex) {

				//If position is low (in the hole)
				if (minions [i].GetComponent<Transform> ().position.y < -3) {

					//Deactivate the minion
					minions [i].SetActive (false);
				}
			}

		}
	}


	private void UpdateButtonInput(){
		pauseGameButton = Input.GetAxisRaw ("PauseGame");
	}

	public void PauseGame(){
		pauseMenu.gameObject.SetActive (true);
		Time.timeScale = 0f;
		//this.gameState = GameState.paused;
	}

	public void ResumeGame(){
		this.gameState = GameState.running;
		pauseMenu.gameObject.SetActive (false);
		Time.timeScale = 1f;
		//Debug.Log ("Resuming game");

	}

	public void GoToMainMenu(){
		//Debug.Log ("Going to main menu");
		ReleaseRelicForAllPlayers ();
		SceneManager.LoadScene("MainMenu");
	}

	public void RestartLevel(){
		//FindTheWinningMinion ();
		ReleaseRelicForAllPlayers ();
		SceneManager.LoadScene("LevelGenerator");
	}
}
