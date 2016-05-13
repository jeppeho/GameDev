using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelRunner : MonoBehaviour {

	private enum GameState { running, paused, godWin, minionWin }; 
	private GameState gameState; 

	private GameObject relic;

	private float pauseGameButton;
	private float prevPauseGame;

	public Canvas pauseMenu;
	public Canvas godWinMenu;
	public Canvas minionWinMenu;

	LevelGenerator lg;
	SystemManager sm;

	GameObject[] minions;

	private bool GameOver; 


	// Use this for initialization
	void Start () {

		//Deactivate menus
		pauseMenu.gameObject.SetActive (false);
		godWinMenu.gameObject.SetActive (false);
		minionWinMenu.gameObject.SetActive (false);

		lg = GameObject.Find ("LevelGenerator").GetComponent<LevelGenerator>();
		sm = GameObject.Find ("SystemRunner").GetComponent<SystemManager>();

		minions = sm.GetInputHandler ().GetMinionsArray ();

		relic = GameObject.Find ("Relic");

		//Set initial gameState
		gameState = GameState.running;

		GameOver = false;
	}

	// Update is called once per frame
	void Update () {

		//if(!GameOver){
			
			UpdateState ();

			if (gameState == GameState.paused) {
				PauseGame ();
				//StartCoroutine( PauseGameKeepRegisteringInput () );
			} 
			if (gameState == GameState.running) {
				Time.timeScale = 1f;
			} 

			if (gameState == GameState.godWin) {
				GameOver = true;
				Time.timeScale = 0f;
				godWinMenu.gameObject.SetActive (true);
			}

			if (gameState == GameState.minionWin) {
				GameOver = true;
				FindTheWinningMinion ();
				Time.timeScale = 0f;
				minionWinMenu.gameObject.SetActive (true);
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
		else if( relic.gameObject.GetComponent<Transform>().position.z > /*lg.levelLength +*/ 10f){
			
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


	private void ReleaseRelicForAllPlayers(){

		for(int i = 0; i < minions.Length; i++){

			PlayerRelicHandler h = minions [i].GetComponent<PlayerRelicHandler> ();

			if (h.HasRelic ()) {
				h.ReleaseRelic ();
				break;
			}
		}
	}


	private void FindTheWinningMinion(){

		Debug.Log ("//////FINDING THE WINNER");
		Debug.Log ("We have " + minions.Length +  " minions");

		int winningMinionIndex = -1;
		float highestScore = -1000; 

		for (int i = 0; i < minions.Length; i++) {

			if (minions [i].activeSelf == true) {

				float minionScore = minions [i].GetComponent<PlayerManager> ().GetPlayerScore ().GetFinalScore ();
				Debug.Log ("minion #" + i + " score = " + minionScore + " and highscore = " + highestScore);

				//If score is so far the highest then update the index of the winning minion
				if (minionScore > highestScore) {
					highestScore = minionScore;
					winningMinionIndex = i;
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

		Debug.Log ("Highscore is now = " + highestScore + " for minion #" + winningMinionIndex);

		int materialIndex = minions [winningMinionIndex].GetComponent<PlayerManager> ().GetCurrentMaterialIndex ();

		sm.SetMinionWinner (winningMinionIndex, materialIndex);
	}


	private void UpdateButtonInput(){
		pauseGameButton = Input.GetAxisRaw ("PauseGame");
	}

	public void PauseGame(){
		//StartCoroutine ( PauseGameKeepRegisteringInput() );
		pauseMenu.gameObject.SetActive (true);
		Time.timeScale = 0f;
		//this.gameState = GameState.paused;
	}

	public void ResumeGame(){
		this.gameState = GameState.running;
		pauseMenu.gameObject.SetActive (false);
		Time.timeScale = 1f;
		Debug.Log ("Resuming game");

	}

	public void GoToMainMenu(){
		Time.timeScale = 1f;
		Debug.Log ("Going to main menu with time scale = " + Time.timeScale );
		ReleaseRelicForAllPlayers ();
		SceneManager.LoadScene("Tutorial Start");
	}

	public void RestartLevel(){
		Time.timeScale = 1f;
		ReleaseRelicForAllPlayers ();
		SceneManager.LoadScene("LevelGenerator");
	}


	/**
	 * Coroutine running while game is paused, to register button inputs.
	 * Update or FixedUpdate doesn't run while Time.timeScale == 0.
	 */
//	IEnumerator PauseGameKeepRegisteringInput(){
//
//		pauseMenu.gameObject.SetActive (true);
//
//		while (gameState == GameState.paused) {
//
//			UpdateButtonInput ();
//
//			if(prevPauseGame < 0.01f && pauseGameButton > 0.01f){
//				gameState = GameState.running;
//			}
//
//			prevPauseGame = pauseGameButton;
//
//			yield return null;
//		}
//	}
}
