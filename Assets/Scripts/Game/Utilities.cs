using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Utilities : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
		
	void Update(){
		if (Input.GetKey ("r"))
			ReloadScene ();
	}


	public static void ReloadScene(){
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}
