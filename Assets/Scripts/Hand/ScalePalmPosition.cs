using UnityEngine;
using System.Collections;
using Leap;


/**
 * This class is used to translate the position of the hand 
 * to be displayed in a certain area of the screen, ie. full
 * screen from left to right and top to bottom.
 */

public class ScalePalmPosition : MonoBehaviour {

	private Frame frame;
	public float scale = 2f;
	private float offset = 5f;

	private float z_offset;

	float prev_x_change;
	float prev_z_change;
	float prev_y_change;

	// Use this for initialization
	void Start () {
		prev_x_change = 0;
	}
	
	// Update is called once per frame
	void Update () {

		//Get frame from parent object
		frame = this.gameObject.GetComponentInParent<LeapVariables> ().getFrame();

		MoveHandsRelative();
		//MoveHandsIncrementally ();
		//MoveHands2();

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

	void MoveHands2(){

		//Get hand position
		Vector hand_position = GetSinglePalmPosition ();


		//Get current gameObject position
		Vector3 position = this.gameObject.transform.position;

		//get camera position
		Vector3 camera_position = this.gameObject.transform.parent.position;

		Debug.Log ("Camera @" + camera_position + " | LEAP @" + position);
		//Subtract the camera position on the x-axis
		//So the center point will be zero



		//Multiply the x coord from the hand with some scale






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

		//Get transformation values
		//float x_change = hands.x / 50f;
		float x_change = hands.x / 50f;
		float z_change = hands.z / 1000f;
		float y_change = hands.y / 500f;
		Debug.Log ("hands.x = " + hands.x);

		//Subtract the change from last frame from the position
		position.x -= prev_x_change;
		//position.x += prev_z_change;
		position.y -= prev_y_change;

		Debug.Log ("before position.x = " + position.x);

		//Add transformation
		position.x += x_change;
		position.z -= z_change;
		position.y += y_change;

		Debug.Log ("after position.x = " + position.x);

		//Update prev values
		prev_x_change = x_change;
		prev_z_change = z_change;
		prev_y_change = y_change;


		//Set boundaries on X-axis
		float minX = 4f;
		float maxX = 6f;

		if (position.x < minX)
			position.x = minX;
		else if (position.x > maxX)
			position.x = maxX;

		//Set boundaries on Z-axis
		float minZ = 6f - parent_position.z;
		float maxZ = 4f - parent_position.z;

		if (position.z < -minZ) {
			position.z = -minZ;
		} else if (position.z > -maxZ) {
			position.z = -maxZ;
		}

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



	/**
	 * Returns the palm position of the first hand
	 * Of more than one hand, the first hand is the 
	 * first had that have entered. 
	 */
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

	/**
	 * Returns the average palm position of both hands or
	 * the palm position of a single hand (if only one present.
	 */
	Vector GetAveragePalmPosition(){

		HandList hands = frame.Hands;

		Hand firstHand = hands [0];
		Hand secondHand = hands [1];

		Vector firstHandPalmPosition = firstHand.PalmPosition;
		Vector secondHandPalmPosition = secondHand.PalmPosition;

		Vector avgPalmPosition = firstHandPalmPosition + secondHandPalmPosition;

		Debug.Log ("Palm position: " + avgPalmPosition);

		return avgPalmPosition;
		//return firstHand.PalmPosition;
	}
}
