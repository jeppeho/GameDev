using UnityEngine;
using System.Collections;
using Leap;

public class GrabObjects : MonoBehaviour {

	private Frame frame;

	private int NUM_FINGERS = 5;
	private int NUM_JOINTS = 4;
	float THUMB_TRIGGER_DISTANCE = 50f;
	float PINCH_DISTANCE = 2f;

	Vector thumb_tip;

	private Vector3 prev_palm_position;

	private int _releaseCounter = 5;

	bool pinching_ = false;

	Collider grabbed_object = null;


	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
		frame = this.gameObject.GetComponent<LeapVariables> ().getFrame();

		UpdatePinch ( frame );

		if (grabbed_object != null) {

			float thumb_tip_multiplier = 20f; //Used to lift the things

			//Get relative movement based on palm position
			Hand hand = frame.Hands[0];
			Vector3 palmPosition = hand.PalmPosition.ToUnityScaled();
			Vector3 palmMovement = palmPosition - prev_palm_position;
			//Debug.Log("palmMovement : " + palmMovement);
			prev_palm_position = palmPosition;


			//Find the vector towards the thumb
			Vector3 distance_towards_thumb = thumb_tip.ToUnityScaled() * thumb_tip_multiplier - grabbed_object.transform.position;
			distance_towards_thumb *= 0.002f; //Limit the speed
	

			//I think gravity should be disabled when trying to make relative movement to palm or thumb
			//Make sure to set it to true on release
			//Maybe make an OnRelease() method
			grabbed_object.attachedRigidbody.useGravity = false;


			grabbed_object.attachedRigidbody.AddForce (palmMovement);
		}
			
	}

	void UpdatePinch(Frame frame){

		bool trigger_pinch = false;
		float avgTipDistance = 0;

		//Get thumb position
		Hand hand = frame.Hands [0];	
		thumb_tip = hand.Fingers [0].TipPosition;


		//Get average distance between tip of thumb and rest of the fingers
		for (int i = 1; i < NUM_FINGERS && !trigger_pinch; i++) {
		
			Finger finger = hand.Fingers [i];

			//Get the outer joint position
			Vector joint_position = finger.JointPosition ((Finger.FingerJoint)(3));

			//Get distance to thumb_tip
			Vector distanceToThumbTip = thumb_tip - joint_position;

			//Add distance to the avg distance
			avgTipDistance += distanceToThumbTip.Magnitude;		
		}

		avgTipDistance /= NUM_FINGERS;
		//Debug.Log ("avg tip dist: " + avgTipDistance + " num fingers: " + NUM_FINGERS);

		//if thumb and index finger joints are within distance
		if (avgTipDistance < THUMB_TRIGGER_DISTANCE && avgTipDistance > 0) {
			ResetReleaseCounter ();
			trigger_pinch = true;
			Debug.Log ("triggering pinch with dist: " + avgTipDistance);
		} else {
			if (pinching_ == true) {

				//Pinch
				if (_releaseCounter > 0) {
					decreaseReleaseCounter ();
					//Debug.Log ("ReleaseCounter: " + _releaseCounter);

				} else {
					pinching_ = false;
					grabbed_object.attachedRigidbody.useGravity = true;
					grabbed_object = null;
				}

				//pinching_ = false;


				//Release object

			}
		}


		if (trigger_pinch && !pinching_) {
			OnPinch (thumb_tip.ToUnityScaled() );
		}
	}


	void ResetReleaseCounter(){
		_releaseCounter = 5;
	}

	void decreaseReleaseCounter(){
		_releaseCounter -= 1;
	}


	/**
	 * When a pinch has been detected this method will
	 * find the closest object to the inputted position
	 * @param pinch_position, the position that it will find the closest object to
	 */
	void OnPinch(Vector3 pinch_position){
	
		//Debug.Log ("OnPinch at " + pinch_position); 
		//Include only the 9th layer
		LayerMask layer_mask = 1 << 9;// 

		//Get all objects within PINCH_DISTANCE
		Collider[] close_things = Physics.OverlapSphere (pinch_position, PINCH_DISTANCE, layer_mask);


		//Get the nearest object
		Vector3 nearest_distance = new Vector3 (PINCH_DISTANCE, 0.0f, 0.0f);
		for (int i = 0; i < close_things.Length; i++) {
			
			Vector3 new_distance = pinch_position - close_things [i].transform.position;
			if (new_distance.magnitude < nearest_distance.magnitude) {
				
				grabbed_object = close_things [i];
				nearest_distance = new_distance;
				//I guess this should be here
				pinching_ = true;
			}
		}

		//Debug.Log ("Grabbed object @" + grabbed_object.transform.position);
	
	}
}
