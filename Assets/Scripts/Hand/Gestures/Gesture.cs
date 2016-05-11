using UnityEngine;
using System.Collections;

public class Gesture : MonoBehaviour {

	protected LeapManager leapManager;
	protected GestureManager gestureManager;
	protected StoneHandManager handManager;
	protected AudioManager audioManager;

	protected string thisSpell;

	// Use this for initialization
	protected void Init () {
		leapManager = this.gameObject.GetComponent<LeapManager> ();
		gestureManager = this.gameObject.GetComponent<GestureManager> ();
		handManager =  this.gameObject.GetComponent<StoneHandManager> ();
		audioManager = GameObject.Find ("AudioManager").GetComponent<AudioManager> ();
		Debug.Log ("Base.init was called by " + this.ToString());
		Debug.Log ("Two managers were found: " + leapManager.ToString() + " , " + gestureManager.ToString());
	}
}
