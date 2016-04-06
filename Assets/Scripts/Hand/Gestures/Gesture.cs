using UnityEngine;
using System.Collections;

public class Gesture : MonoBehaviour {

	protected LeapManager leapManager;
	protected GestureManager gestureManager;
	protected GestureManager.spell thisSpell;

	// Use this for initialization
	protected void Init () {
		leapManager = this.gameObject.GetComponent<LeapManager> ();
		gestureManager = this.gameObject.GetComponent<GestureManager> ();
	}
}
