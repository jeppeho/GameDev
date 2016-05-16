using UnityEngine;
using System.Collections;

public class InputControllerHandler : MonoBehaviour {

	public GameObject minion;

	private GameObject[] minions;

	//Contains strings with names of supported controllers
	string[] supportedControllers;

	//Contains strings with full names of all connected controllers
	string[] connectedControllers;

	//Contains the connected accepted controllers
	string[] acceptedControllers;

	//Contains the connected accepted and active controllers
	string[] activeControllers;


	// Use this for initialization
	void Start () {

		Debug.Log ("Starting up the InputControllerHANDLER()");

		supportedControllers = new string[] { "Sony Computer Entertainment Wireless Controller", "Sony PLAYSTATION(R)3 Controller" };

		connectedControllers = Input.GetJoystickNames ();

		//For using the controllers connected via cables
		acceptedControllers = new string[ GetNumAcceptedControllers () ];
		//minions = new GameObject[ GetNumAcceptedControllers () ];

		Debug.Log ("Controllers.Length = " + acceptedControllers.Length);
		minions = new GameObject[3];

		RegisterAcceptedControllers ();

	}


	private string GetControllerAbbrevation(string name){

		string abbrevation = "";

		switch (name) {

		case "Sony Computer Entertainment Wireless Controller":
			abbrevation = "P";
			break;
		case "Sony PLAYSTATION(R)3 Controller":
			abbrevation = "P";
			break;
		}

		return abbrevation;
	}


	/**
	 * Returns the number of connected supported controllers
	 */
	public int GetNumAcceptedControllers(){

		int matches = 0;

		for (int i = 0; i < connectedControllers.Length; i++) {
			//Debug.Log (Input.GetJoystickNames () [i]);

			for (int g = 0; g < supportedControllers.Length; g++) {

				if (supportedControllers [g] == connectedControllers [i]) {
					matches++;
				}
			}
		}

		return matches;
	}



	/**
	 * Goes through connected controllers, and checks which are supported.
	 * If a controller is supported it finds the prefix for that controller
	 */
	private void RegisterAcceptedControllers(){

		int index = 0;

		for (int i = 0; i < connectedControllers.Length; i++) {

			for (int g = 0; g < supportedControllers.Length; g++) {

				if (supportedControllers [g] == connectedControllers [i]) {

					acceptedControllers[ index ] = GetControllerAbbrevation( connectedControllers[i] );

					index++;
					break;
				}
			}
		}
	}


	public void SetAbbreviation(){
	}


	/**
	 * Run this method to create three players, so bluetooth connected controllers can join
	 */
	public void PutInThreeMinions(bool active){

		//Space and width is for placing the characters on a single line
		float space = 3f;
		int numPlayers = 3;
		float width = space * (numPlayers - 1f);

		for (int playerIndex = 0; playerIndex < numPlayers; playerIndex++) {
			
			float x = -space + playerIndex * space;
			Vector3 position = new Vector3 (x, 1, -7);

			int controllerNameIndex = playerIndex + 1;

			string prefix = "P" + controllerNameIndex.ToString ();

			Debug.Log ("Creating player: " + prefix);

			CreateMinion( position, prefix, playerIndex, active);
		}
	
	}


	/**
	 * Creates a player for each connected and accepted controller
	 * 
	 */
	public void CreateMinionsForControllers(bool active){

		//Space and width is for placing the characters on a single line
		float space = 3;
		float width = space * ((float)acceptedControllers.Length - 1f);
		int numPlayers = acceptedControllers.Length;

		for (int playerIndex = 0; playerIndex < numPlayers; playerIndex++) {

			float x = GetXPositionForMinion(playerIndex);
			Vector3 position = new Vector3 (x, 1, 3);

			int controllerNameIndex = playerIndex + 1;

			CreateMinion( position, acceptedControllers[playerIndex] + controllerNameIndex.ToString(), playerIndex, active);
		}
	}

	/**
	 * Returns an x-position for a player, based on how many players and it's placement in the lineup
	 */
	public float GetXPositionForMinion(int index){

		float space = 3;
		float width = space * ((float)acceptedControllers.Length - 1f);

		float x = 0 - width / 2 + index * space;

		return x;
	}


	public void CreateMinion(Vector3 position, string prefix, int index, bool active){

		Debug.Log ("Create a minion for controller." + prefix + ", with index = " + index);

		GameObject min = Instantiate (minion, position, Quaternion.identity) as GameObject;

		//Set controller prefix on minion
		min.GetComponent<NewController>().prefix = prefix;

		//Set inactive as a start
		min.GetComponent<PlayerManager> ().SetPlayerActive(active);
		Debug.Log(prefix + " state = " + min.GetComponent<PlayerManager> ().GetState());

		minions [index] = min;


		Debug.Log ("Set color now!! @" + index);
		min.GetComponent<PlayerManager> ().SetMaterial (index);

	}

	public GameObject[] GetMinionsArray(){
		return this.minions;
	}
}
