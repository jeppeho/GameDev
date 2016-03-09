using UnityEngine;
using System.Collections;
using Leap;

public class ScalePalmPosition : MonoBehaviour {

	private Controller controller;
	private Frame frame;
	public float scale = 2f;
	private float offset = 5f;

	private float z_offset;

	float prev_x_change;
	float prev_z_change;

	// Use this for initialization
	void Start () {
		controller = new Controller ();
		prev_x_change = 0;
	}
	
	// Update is called once per frame
	void Update () {
	
		frame = controller.Frame ();


		MoveHandsRelative();
		//MoveHandsIncrementally ();


	}
		
	/**
	 * Move hands incrementally. Last transformation is saved in position
	 */
	void MoveHandsIncrementally(){

		//Get palm position
		Vector hands = GetSinglePalmPosition ();

		//Get current gameObject position
		Vector3 position = this.gameObject.transform.position;

		//Add transformation
		position.x += hands.x / 2000f;

		position.z -= hands.z / 3000f;

		//Set boundaries on X-axis
		if (position.x < 0)
			position.x = 0;
		else if (position.x > 10)
			position.x = 10;




		//Set changes to position
		this.gameObject.transform.position = position;

	}

	/**
	 * Move hands relative to actual position of hands recognized by Leap
	 */
	void MoveHandsRelative(){

		//Get palm position
		Vector hands = GetSinglePalmPosition ();

		//Get current gameObject position
		Vector3 position = this.gameObject.transform.position;

		//Get parent position
		Vector3 parent_position = this.gameObject.transform.parent.position;
		//Debug.Log("This.z: " + position.z +  " Parent.z: " + parent_position.z);

		position.x -= prev_x_change;

		//Set change values from hand position
		float x_change = hands.x / 50f;
		float z_change = hands.z / 1000f;

		//Update prev values
		prev_x_change = x_change;
		prev_z_change = z_change;

		//Add transformation
		position.x += x_change;


		position.z -= z_change;

		//Set boundaries on X-axis
		if (position.x < 0)
			position.x = 0;
		else if (position.x > 10)
			position.x = 10;

		float z_near_bound = 6f - parent_position.z;
		float z_far_bound = 4f - parent_position.z;
		//Debug.Log ("Near: " + z_near_bound + " far " + z_far_bound);

		//Set boundaries on X-axis
		if (position.z < -z_near_bound) {
			position.z = -z_near_bound;
			//Debug.Log ("z_near_bound reached");
		} else if (position.z > -z_far_bound) {
			//Debug.Log ("z_far_bound reached");
			position.z = -z_far_bound;
		}
//
//		position.z -= parent_position.z;


		//Set changes to position
		this.gameObject.transform.position = position;

	}


	void MoveHands(){

		//Get palm position
		Vector hands = GetAveragePalmPosition ();

		//Get current gameObject position
		Vector3 position = this.gameObject.transform.position;
		//Debug.Log ("position: " + position);
		float oldX = position.x;

		//Subtract offset
		position.x -= offset;

		//Scale position
		position.x *= scale;


//		if (position.x > 10)
//			position.x = 10;
//		else if (position.x < 0)
//			position.x = 0;

		Debug.Log ("oldX: " + oldX + " --> " + position.x); 

		//X-axis movement
//		if (hands.x < -boundary)
//			position.x -= speed;
//		else if (hands.x > boundary)
//			position.x += speed;
//


		//Add offset
		position.x += offset;


		//Set changes to position
		//this.gameObject.transform.position = position;
		//this.gameObject.transform.position = hands;

	}

	Vector GetSinglePalmPosition(){

		//Get list of hands
		HandList hands = frame.Hands;

		//Get the first hand
		Hand firstHand = hands [0];

		//Get the palm position of first hand
		Vector firstHandPalmPosition = firstHand.PalmPosition;
	
		//Return palm position
		return firstHand.PalmPosition;
	}


	Vector GetAveragePalmPosition(){

		HandList hands = frame.Hands;

		Hand firstHand = hands [0];
		Hand secondHand = hands [1];

		Vector firstHandPalmPosition = firstHand.PalmPosition;
		Vector secondHandPalmPosition = secondHand.PalmPosition;

		Vector avgPalmPosition = firstHandPalmPosition + secondHandPalmPosition;

		Debug.Log ("Palm position: " + avgPalmPosition);

		return avgPalmPosition;
		return firstHand.PalmPosition;
	}
}
