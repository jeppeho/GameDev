using UnityEngine;
using System.Collections;
using Leap;

public class LeapGestures : MonoBehaviour {

	private Controller controller;
	private Frame frame;


	void Awake(){
		controller = this.gameObject.GetComponent<LeapVariables> ().GetController();

		//Enable each gesture
		controller.EnableGesture (Gesture.GestureType.TYPE_CIRCLE);
		controller.EnableGesture (Gesture.GestureType.TYPE_KEY_TAP);
		controller.EnableGesture (Gesture.GestureType.TYPE_SCREEN_TAP);
		controller.EnableGesture (Gesture.GestureType.TYPE_SWIPE);
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

		//Get the current frame
		frame = this.gameObject.GetComponent<LeapVariables> ().GetFrame();

		//MoveHands();
		GetGestures ();
	}

	void GetGestures(){

		//Get the gestures in the frame 
		GestureList gestures = frame.Gestures();

		//Write to console of a gesture is recognozed
		for (int i = 0; i < gestures.Count; i++) {

			if (gestures [i].Type == Gesture.GestureType.TYPE_CIRCLE)
				Debug.Log ("The gesture is a circle");
			if (gestures [i].Type == Gesture.GestureType.TYPE_SCREEN_TAP)
				Debug.Log ("The gesture is a screen tap");
			if (gestures [i].Type == Gesture.GestureType.TYPE_SWIPE)
				Debug.Log ("The gesture is a swipe");
			if (gestures [i].Type == Gesture.GestureType.TYPE_KEY_TAP)
				Debug.Log ("The gesture is a key tap");
		}
	
	}
}
