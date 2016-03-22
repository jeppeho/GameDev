using UnityEngine;
using System.Collections;
using Leap;

public class LeapController : MonoBehaviour {

	private Frame frame;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

		frame = this.gameObject.GetComponent<LeapVariables> ().GetFrame();
		MoveHands ();
	}


	void MoveHands(){

		float speed = 0.03f;
		float boundary = 100;

		//Get palm position
		Vector hands = GetAveragePalmPosition ();

		//Get current gameObject position
		Vector3 position = this.gameObject.transform.position;


		//X-axis movement
		if (hands.x < -boundary)
			position.x -= speed;
		else if (hands.x > boundary)
			position.x += speed;

		//Y-axis movement
		if (hands.y < 200)
			position.y -= speed/2;
		else if (hands.y > 700)
			position.y += speed/2;

		//Z-axis movement
		if (hands.z < -boundary)
			position.z += speed;
		else if (hands.z > boundary)
			position.z -= speed;


		//Set changes to position
		this.gameObject.transform.position = position;

	}


	//TODO CHECK NUM HANDS
	int getNumHands(){

		HandList hands = frame.Hands;


		//Return number of hands
		return 0;
	}

	Vector GetAveragePalmPosition(){

		HandList hands = frame.Hands;

		Hand firstHand = hands [0];
		Hand secondHand = hands [1];

		Vector firstHandPalmPosition = firstHand.PalmPosition;
		Vector secondHandPalmPosition = secondHand.PalmPosition;

		Vector avgPalmPosition = firstHandPalmPosition + secondHandPalmPosition;

		//Debug.Log ("Palm position: " + avgPalmPosition);

		return avgPalmPosition;
	}

}
