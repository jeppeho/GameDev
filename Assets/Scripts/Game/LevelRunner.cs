using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelRunner : MonoBehaviour {

	private enum GameState { running, paused, godWin, minionWin }; 
	private GameState gameState; 

	private GameObject relic;

	private float pauseGameButton;
	private float prevPauseGame;


	// Use this for initialization
	void Start () {

		relic = GameObject.Find ("Relic");
		Debug.Log (relic);
		gameState = GameState.running;

	}

	// Update is called once per frame
	void Update () {

		UpdateState ();

		if (gameState == GameState.paused) {
			Time.timeScale = 0f;
			StartCoroutine( PauseGame () );
		} 

		else 
			Time.timeScale = 1;

			if (gameState == GameState.running) {

			} 
		else if (gameState == GameState.godWin) {
			SceneManager.LoadScene("LevelGenerator");
		}
		else if (gameState == GameState.minionWin) {

		}

	}


	private void UpdateState(){

		UpdateButtonInput ();

		//Check if god win
		if (relic.GetComponent<RelicHealth> ().GetHealth () < 0f) {
	
			gameState = GameState.godWin;
			
		} else if (prevPauseGame < 0.05f && pauseGameButton > 0.05f /*|| Input.GetKeyUp ("k")*/ ) {

			if(gameState == GameState.running)
				gameState = GameState.paused;
			else if(gameState == GameState.paused)
				gameState = GameState.running;
		
		} 

		prevPauseGame = pauseGameButton;
				

			//Check if minions win

		//}
	}

	private void UpdateButtonInput(){
		pauseGameButton = Input.GetAxisRaw ("PauseGame");
	}



	/**
	 * Coroutine running while game is paused, to register button inputs.
	 * Update or FixedUpdate doesn't run while Time.timeScale == 0.
	 */
	IEnumerator PauseGame(){

		while (gameState == GameState.paused) {

			UpdateButtonInput ();

			if(prevPauseGame < 0.01f && pauseGameButton > 0.01){
				gameState = GameState.running;
			}

			prevPauseGame = pauseGameButton;

			yield return null;
		}
	}
}
