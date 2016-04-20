using UnityEngine;
using System.Collections;

public class Gesture : MonoBehaviour {

	protected LeapManager leapManager;
	protected GestureManager gestureManager;
	protected string thisSpell;

	// Use this for initialization
	protected void Init () {
		leapManager = this.gameObject.GetComponent<LeapManager> ();
		gestureManager = this.gameObject.GetComponent<GestureManager> ();
		Debug.Log ("Base.init was called by " + this.ToString());
		Debug.Log ("Two managers were found: " + leapManager.ToString() + " , " + gestureManager.ToString());
	}
}
