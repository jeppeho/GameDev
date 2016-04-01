using UnityEngine;
using System.Collections;
using Leap;

public class SphereHandModel : MonoBehaviour {


	int NUM_FINGERS = 5;
	int NUM_BONES = 4;
	private Frame frame;

	private Vector3[,] finger_bone_positions = new Vector3[5, 4];
	private Vector3 palm_bone_position = new Vector3();
	private float boneMass = 0.1f;
	private GameObject[,] finger_bones;
	private GameObject palm_bone;
	public GameObject FingerBone;
	public float fingerBoneScale;
	public Color handColor;

	private bool firstUpdate = true;

	private LeapVariables manager;

	// Use this for initialization
	void Start () {

		manager = this.gameObject.GetComponent<LeapVariables> ();
		//Create finger bones
		finger_bones = new GameObject[NUM_FINGERS, NUM_BONES];

		for (int i = 0; i < NUM_FINGERS; i+=1) {
			for (int g = 0; g < NUM_BONES; g+=1) {
				//Debug.Log ("Creating Sphere");

				//Insert prefab fingerbone in spheres array
				//finger_bones [i, g] = Instantiate(FingerBone, new Vector3(5,20,190), Quaternion.Euler(0,0,0)) as GameObject;
				//finger_bones [i, g].GetComponent<Rigidbody> ().MovePosition (new Vector3 (5, 1, 190));
				finger_bones [i, g] = Instantiate(FingerBone) as GameObject;
				//finger_bones [i, g].GetComponent<Rigidbody> ().MovePosition (new Vector3 (5, 1, 190));

				//Change scale of fingerbone
				finger_bones[i,g].GetComponent<BoneBehavior>().setScale( new Vector3(0.2f*fingerBoneScale, 0.2f*fingerBoneScale, 0.2f*fingerBoneScale) );
				finger_bones[i,g].GetComponent<BoneBehavior>().setMass(boneMass);


				finger_bones[i,g].GetComponent<BoneBehavior>().SetPositionY(200);

				//finger_bones[i,g].transform.position = new ;
				//spheres[i, g].transform.localScale = new Vector3 (scale, scale, scale);
			}
		}

		//Create palm bone
		palm_bone = Instantiate(FingerBone, new Vector3(5,1,-130), Quaternion.Euler(0,0,0)) as GameObject;
		palm_bone.GetComponent<Rigidbody> ().MovePosition (new Vector3 (5, 1, -130));
		palm_bone.GetComponent<BoneBehavior> ().setScale (new Vector3 (0.4f*fingerBoneScale, 0.4f*fingerBoneScale, 0.4f*fingerBoneScale));
		palm_bone.GetComponent<BoneBehavior> ().setMass (boneMass);



	}
	
	// Update is called once per frame
	void Update () {

		//First update of hand and finger positions to prevent destruction of stage
		if (firstUpdate) {
			UpdatePalmBonePosition ();
			UpdateFingerBonesPositions ();
			firstUpdate = false;
		}

		if (manager.HandIsValid()) {
			UpdatePalmBonePosition();
			UpdateFingerBonesPositions();
			/*if (checkForWall (hand) == true) {
				CreateWall (hand);
			} else{
				CreateHand (hand);
			}*/
			CreateHand (manager.GetHand());
		} else {
			MakeBonesFlyUp ();
		}
	}

	public void SetHandColor(Color c)
	{
		handColor = c;
	}

	/**
	 * 	THIS SHOULD BE MOVED TO THE LEAP MANAGER SCRIPT
	 * 	Checks if finger is extended
	 */
	private void isFingerExtended(Finger finger){

		Debug.Log (finger.Type + " is exnteded = " + finger.IsExtended);
		
	}


	/**
	 * Returns true if middle and/or index finger is extended
	 */
	/*
	private bool checkForWall(Hand hand){

		//Get the fingers
		FingerList fingers = hand.Fingers;


		//Check if only middle and index is extended
		bool fingerWall = true;

		foreach (Finger finger in fingers) {

			//THUMB
			if (finger.Type == Finger.FingerType.TYPE_THUMB && finger.IsExtended != false)
				fingerWall = false;

			//INDEX
			if (( finger.Type == Finger.FingerType.TYPE_INDEX && finger.IsExtended != true ) && (finger.Type == Finger.FingerType.TYPE_MIDDLE  && finger.IsExtended != true) )
				fingerWall = false;
			
			//MIDDLE
//			if (finger.Type == Finger.FingerType.TYPE_MIDDLE  && finger.IsExtended != true)
//				fingerWall = false;
			//RING
			if (finger.Type == Finger.FingerType.TYPE_RING && finger.IsExtended != false)
				fingerWall = false;
			//PINKY
			if (finger.Type == Finger.FingerType.TYPE_PINKY && finger.IsExtended != false)
				fingerWall = false;
		}

		return fingerWall;
	}
	*/



	/**
	 * Sets all bones to the positions of index finger bones
	 * Changes mass as well.
	 * 
	 */

	/*
	private void CreateWall(Hand hand){

		//Get the fingers
		FingerList fingers = hand.Fingers;

		//For each finger calculate the position of each bone
		foreach (Finger finger in fingers) {

			if (finger.Type == Finger.FingerType.TYPE_INDEX) {
				SetFingerBonePositions ( finger, 0 );
				SetFingerBonePositions ( finger, 1 );
				SetFingerBonePositions ( finger, 2 );
				SetFingerBonePositions ( finger, 3 );
				SetFingerBonePositions ( finger, 4 );
			}
		}

		//Set positions of each sphere to that of the finger bone
		SetSpherePositionsToBonePositions ();

		//Set position of spheres to the same as bone_positions
		for (int i = 0; i < NUM_FINGERS; i++) {
			for (int g = 0; g < NUM_BONES; g++) {

				float distance = finger_bones [i, g].GetComponent<BoneBehavior> ().getDistanceToDesiredPosition ();

				float mass = ( distance / 1000);

				finger_bones [i, g].GetComponent<BoneBehavior> ().setMass ( mass );
				Debug.Log("Mass has been set to " + finger_bones [i, g].GetComponent<BoneBehavior> ().getMass () );
			}
		}


	}
	*/

	/**
	 * Sets the position of var finger_bone_positions
	 * and calls methods to set positions of bones and
	 * lastly it maps the mass of each bone to the distance 
	 * to the desired position from their current position.
	 */
	private void CreateHand(Hand hand){

		//Get the fingers
		FingerList fingers = hand.Fingers;

		UpdatePalmBonePosition ();
		UpdateFingerBonesPositions ();

		//Set position of spheres to the same as bone_positions
		for (int i = 0; i < NUM_FINGERS; i++) {
			for (int g = 0; g < NUM_BONES; g++) {

				float distance = finger_bones [i, g].GetComponent<BoneBehavior> ().getDistanceToDesiredPosition ();

				float mass = ( distance * distance / 10);

				finger_bones [i, g].GetComponent<BoneBehavior> ().setMass ( mass );
				//Debug.Log("Mass has been set to " + finger_bones [i, g].GetComponent<BoneBehavior> ().getMass () );
			}
		}
	
	}


	/**
	 * Adds upwards energy to the desired position of the bone,
	 * so they will fly up. 
	 * Movement on the X and Z axes are randomaized.
	 * 
	 */
	private void MakeBonesFlyUp(){

		Vector3 position;
		float upSpeed = 0.02f;
		float horizontalRange = 0.3f;

		//For each bone in hand
		for (int i = 0; i < NUM_FINGERS; i++) {
			for (int g = 1; g < NUM_BONES; g++) {

				//Get current position
				position = finger_bones [i, g].GetComponent<BoneBehavior>().getDesiredPosition();

				//Add velocity on Y-axis
				/*
				position.y += upSpeed;

				position.x += Random.Range ( -horizontalRange, horizontalRange );
				position.z += Random.Range ( -horizontalRange, horizontalRange );
				*/

				position.z -= upSpeed;
				
				position.x += Random.Range ( -horizontalRange, horizontalRange );
				position.y += Random.Range ( -horizontalRange, horizontalRange );

				//Set new position
				finger_bones [i, g].GetComponent<BoneBehavior>().setDesiredPosition( position );
				//finger_bones [i, g].GetComponent<BoneBehavior>().traction = 0;
			}
		}

		position = palm_bone.GetComponent<BoneBehavior> ().getDesiredPosition ();

		//Add upwards velocity on Y-axis
		position.y += upSpeed;

		//Add random values to X and Z-axes
		position.x += Random.Range (-horizontalRange, horizontalRange);
		position.z += Random.Range (-horizontalRange, horizontalRange);

		palm_bone.GetComponent<BoneBehavior> ().setDesiredPosition ( position );


	}
		
	/**
	 * Helper function to set the bone positions of a finger
	 * @param finger, the finger from which to get the positions
	 * @param finger_index, the index in the first dimension of the finger_bone_positions array
	 */
	private void SetFingerBonePositions(Finger finger, int finger_index){
		finger_bone_positions [finger_index, 0] = finger.Bone (Bone.BoneType.TYPE_METACARPAL).Center.ToUnityScaled () * 12  + new Vector3(-0.2f, 0.5f, 0f);
		finger_bone_positions [finger_index, 1] = finger.Bone (Bone.BoneType.TYPE_PROXIMAL).Center.ToUnityScaled () * 12  + new Vector3(-0.2f, 0.5f, 0f);
		finger_bone_positions [finger_index, 2] = finger.Bone (Bone.BoneType.TYPE_INTERMEDIATE).Center.ToUnityScaled () * 12  + new Vector3(-0.2f, 0.5f, 0f);
		finger_bone_positions [finger_index, 3] = finger.Bone (Bone.BoneType.TYPE_DISTAL).Center.ToUnityScaled () * 12  + new Vector3(-0.2f, 0.5f, 0f);
	}

	/**
	 * Sets the desiredPositions of the palm, incl. the offset from parent object
	 */

private void UpdatePalmBonePosition(){
	palm_bone_position = manager.GetPalmPosition();
	
	palm_bone_position += GetOffsetFromParent ();
	
	palm_bone.GetComponent<BoneBehavior> ().setDesiredPosition( palm_bone_position );
}
		

private void UpdateFingerBonesPositions(){
		//Set position of spheres to the same as bone_positions
		for (int i = 0; i < NUM_FINGERS; i++) {
			for (int g = 1; g < NUM_BONES; g++) {

				finger_bone_positions[i, g] = manager.GetBoneCenterPosition(i,g);
				
				//Add parents offset to finger_bone_positions
				AddOffsetFromParent(i, g);
			
				//Set position of game sphere
				finger_bones [i, g].GetComponent<BoneBehavior>().setDesiredPosition( finger_bone_positions[i,g] );
			}
		}
	}



	/**
	 * Sets the mass on all bones of the hand, to the specified input.
	 */
	private void SetMassOnAllBones(float mass){
		//Set position of spheres to the same as bone_positions
		for (int i = 0; i < NUM_FINGERS; i++) {
			for (int g = 0; g < NUM_BONES; g++) {
				
				finger_bones [i, g].GetComponent<BoneBehavior> ().setMass ( mass );
			}
		}
	}

	/**
	 * Adds offset from parent object to a finger bone
	 */
	private void AddOffsetFromParent(int i, int g){
		finger_bone_positions [i, g] += GetOffsetFromParent (); 
	}

	/**
	 * Get offset from parent object
	 */
	private Vector3 GetOffsetFromParent(){
		return gameObject.GetComponentInParent<Transform> ().transform.position;
	}



}
