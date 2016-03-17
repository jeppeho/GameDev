using UnityEngine;
using System.Collections;
using Leap;

public class SphereHandModel : MonoBehaviour {


	int NUM_FINGERS = 5;
	int NUM_BONES = 4;
	private Frame frame;

	private Vector3[,] finger_bone_positions = new Vector3[5, 4];
	private Vector3 palm_bone_position = new Vector3();

	private GameObject[,] finger_bones;
	private GameObject palm_bone;
	public GameObject FingerBone;



	// Use this for initialization
	void Start () {

		//Create finger bones
		finger_bones = new GameObject[NUM_FINGERS, NUM_BONES];

		for (int i = 0; i < NUM_FINGERS; i+=1) {
			for (int g = 0; g < NUM_BONES; g+=1) {
				Debug.Log ("Creating Sphere");

				//Insert prefab fingerbone in spheres array
				finger_bones [i, g] = Instantiate(Resources.Load("FingerBone", typeof(GameObject))) as GameObject;
				//spheres[i, g] = GameObject.CreatePrimitive(PrimitiveType.Sphere);

				//Change scale of fingerbone
				finger_bones[i,g].GetComponent<BoneBehavior>().setScale( new Vector3(0.2f, 0.2f, 0.2f) );
				finger_bones[i,g].GetComponent<BoneBehavior>().setMass(0.04f);
				//spheres[i, g].transform.localScale = new Vector3 (scale, scale, scale);
			}
		}

		//Create palm bone
		palm_bone = Instantiate(Resources.Load("FingerBone", typeof(GameObject))) as GameObject;
		palm_bone.GetComponent<BoneBehavior> ().setScale (new Vector3 (0.4f, 0.4f, 0.4f));
		palm_bone.GetComponent<BoneBehavior> ().setMass (0.08f);

	}
	
	// Update is called once per frame
	void Update () {

		//Get frame from parent object
		frame = this.gameObject.GetComponent<LeapVariables> ().getFrame();

		//Get the first hand
		Hand hand = frame.Hands[0];


		//Get the fingers
		FingerList fingers = hand.Fingers;


		//For each finger calculate the position of each bone
		foreach (Finger finger in fingers) {
				
			int finger_index = 0;
			Bone bone;

			//THUMB
			if (finger.Type == Finger.FingerType.TYPE_THUMB) {
				setFingerBonePositions (finger, 0);
			}else
			//INDEX
			if (finger.Type == Finger.FingerType.TYPE_INDEX) {
				setFingerBonePositions (finger, 1);
			}else
			//MIDDLE
			if (finger.Type == Finger.FingerType.TYPE_MIDDLE) {
				setFingerBonePositions (finger, 2);
			}else
			//RING
			if (finger.Type == Finger.FingerType.TYPE_RING) {
				setFingerBonePositions (finger, 3);
			}else
			//PINKY
			if (finger.Type == Finger.FingerType.TYPE_PINKY) {
				setFingerBonePositions (finger, 4);
			}


		}

		//Set positions of each sphere to that of the finger bone
		setSpherePositionsToBonePositions ();

		//Set position of palm bone
		if(hand.IsValid)
			setPalmBonePosition (hand);
	}

	private void setFingerBonePositions(Finger finger, int finger_index){
		finger_bone_positions [finger_index, 0] = finger.Bone (Bone.BoneType.TYPE_METACARPAL).Center.ToUnityScaled () * 12  + new Vector3(-0.2f, 0.5f, 0f);
		finger_bone_positions [finger_index, 1] = finger.Bone (Bone.BoneType.TYPE_PROXIMAL).Center.ToUnityScaled () * 12  + new Vector3(-0.2f, 0.5f, 0f);
		finger_bone_positions [finger_index, 2] = finger.Bone (Bone.BoneType.TYPE_INTERMEDIATE).Center.ToUnityScaled () * 12  + new Vector3(-0.2f, 0.5f, 0f);
		finger_bone_positions [finger_index, 3] = finger.Bone (Bone.BoneType.TYPE_DISTAL).Center.ToUnityScaled () * 12  + new Vector3(-0.2f, 0.5f, 0f);
	}

	private void setPalmBonePosition(Hand hand){
		palm_bone_position = hand.PalmPosition.ToUnityScaled () * 12  + new Vector3(-0.2f, 0.5f, 0f);
	
		palm_bone.GetComponent<BoneBehavior> ().setDesiredPosition (palm_bone_position);
	}


	private void setSpherePositionsToBonePositions(){
		//Set position of spheres to the same as bone_positions
		for (int i = 0; i < NUM_FINGERS; i++) {
			for (int g = 1; g < NUM_BONES; g++) {

				//Print out bone position
				//Debug.Log ("i = " + i + ", g = " + g + " | bone @" + finger_bone_positions [i, g]);

				//Set position of game sphere
				finger_bones [i, g].GetComponent<BoneBehavior>().setDesiredPosition( finger_bone_positions [i, g] );
				//spheres [i, g].transform.position = bone_positions [i, g];
			}
		}
	}

}
