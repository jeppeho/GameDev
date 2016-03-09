using UnityEngine;
using System.Collections;
using Leap;

public class PowerUpHand : MonoBehaviour {

	private Frame frame;

	private int NUM_FINGERS = 5;
	private int charge = 0;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

		//Get current frame
		frame = this.gameObject.GetComponent<LeapVariables> ().getFrame();
		Hand hand = frame.Hands[0];


		float palm_height = hand.PalmPosition.y;
		float total_finger_height = 0;
		int numFingersOutOfBounds = 0;


		//Get average
		for (int i = 1; i < NUM_FINGERS; i++) {

			Finger finger = hand.Fingers [i];

			//Get the outer joint position
			Vector joint_position = finger.JointPosition ((Finger.FingerJoint)(3));

			if (Mathf.Abs (palm_height - joint_position.y) > 10){
				numFingersOutOfBounds += 1;
			}
				

			total_finger_height += joint_position.y;

			//finger_heights[i] = joint_position.y;
		}

		//Get the avg height of all fingers
		float avg_finger_height = total_finger_height / NUM_FINGERS;

		//Get the difference between fingers and palm
		float fingers_palm_diff = Mathf.Abs (palm_height - avg_finger_height);


		if(fingers_palm_diff < 10 && numFingersOutOfBounds < 2 ){
			//Debug.Log("POWERING THE HAND OF GOD!");
			increaseCharge ();
		}else{
			decreaseCharge();
		}

		//Debug.Log ("Charge = " + charge);
			

	}


	void increaseCharge(){
		if(charge < 200)
			charge++;
	}

	void decreaseCharge(){
		if (charge > 0)
			charge-= 3;
	}

	void setCharge(int newCharge){
		charge = newCharge;
	}
}
