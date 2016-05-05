using UnityEngine;
using System.Collections;

public class InputControllerHandler : MonoBehaviour {

	public GameObject minion;

	//Contains strings with names of supported controllers
	string[] supportedControllers;

	//Contains strings with full names of all connected controllers
	string[] connectedControllers;

	//Contains the connected accepted controllers
	string[] acceptedControllers;


	// Use this for initialization
	void Start () {
		supportedControllers = new string[] { "Sony Computer Entertainment Wireless Controller", "Sony PLAYSTATION(R)3 Controller" };

		connectedControllers = Input.GetJoystickNames ();

		acceptedControllers = new string[ GetNumAcceptedControllers () ];
		Debug.Log ("Controllers.Length = " + acceptedControllers.Length);

		RegisterAcceptedControllers ();

		//Create minions
		CreateMinionsForControllers ();
		CreateMinion(new Vector3(3, 2, 0), "KEYBOARD");
		Debug.Log ("Keyboard minion");
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
	private int GetNumAcceptedControllers(){

		int matches = 0;

		for (int i = 0; i < connectedControllers.Length; i++) {
			Debug.Log (Input.GetJoystickNames () [i]);

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


	/**
	 * Creates a player for each connected and accepted controller
	 * 
	 */
	private void CreateMinionsForControllers(){
		Debug.Log ("Creating " + acceptedControllers.Length + "minionssss");
		for (int player = 0; player < acceptedControllers.Length; player++) {

			int index = player + 1;

			CreateMinion(new Vector3 (0, -1, -3), acceptedControllers[player] + index.ToString());
		}
	}

	private void CreateMinion(Vector3 position, string prefix){

		GameObject min = Instantiate (minion, position, Quaternion.identity) as GameObject;

		//Set controller prefix on minion
		min.GetComponent<NewController>().prefix = prefix;

	}

}
