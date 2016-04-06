using UnityEngine;
using System.Collections;

[RequireComponent (typeof(LeapManager))]

public class StoneHandManager : MonoBehaviour {

	//All bone objects
	private GameObject[,] finger_bones;
	private GameObject[] palm_bones;
	private GameObject core;

	//Object used, containing all bones
	public GameObject handObject;

	//Bone counts
	private int fingerCount = 5;
	private int fingerBoneCount = 4;
	private int palmBoneCount = 11;

	//The position of all bones
	private Vector3[,] fingerBonePositions = new Vector3[5, 4];
	private Quaternion[,] fingerBoneRotations = new Quaternion[5, 4];
	private Vector3[] palmBonePositions = new Vector3[11];
	private Quaternion[] palmBoneRotations = new Quaternion[11];
	private Vector3[] palmBoneOffsets = new Vector3[11];
	private Vector3 corePosition;

	//The global scale of the bones
	public float fingerBoneScale = 40f;

	//The force that snaps all bones back in place (traction), which rebuilds over time; when below 0, it'll be treated as 0
	[HideInInspector]
	public float globalTraction = 1; //...-1
	//The amount which with the traction rebuilds per update (i.e. how quickly the hand becomes whole again)
	public float globalTractionRegeneration = 0.5f;
	//The amount that the traction can go below 0, leaving a deadzone of the bone not having traction
	public float globalTractionBuffer = 0f;

	//The LeapManager
	private LeapManager manager;
	//Other variables
	private bool firstUpdate = true;
	public Color handColor;

	// Use this for initialization
	void Start () {

		//Create GameObject-arrays
		manager = this.gameObject.GetComponent<LeapManager> ();
		finger_bones = new GameObject[fingerCount, fingerBoneCount];
		palm_bones = new GameObject[palmBoneCount];

		//Locate core
		core = handObject.transform.FindChild ("ball").gameObject;

		//Locate fingerbones
		for (int i = 0; i < fingerCount; i++) {
			for (int j = 0; j < fingerBoneCount; j++) {
				//Insert prefab fingerbone in array
				Transform fingerContainer = handObject.transform.FindChild ("fingers");
				switch (i)
				{
				default:
					finger_bones [i, j] = fingerContainer.FindChild ("thumb_" + j.ToString ()).gameObject; break;
				case 1:
					finger_bones [i, j] = fingerContainer.FindChild("point_"+j.ToString()).gameObject; break;
				case 2:
					finger_bones [i, j] = fingerContainer.FindChild("middle_"+j.ToString()).gameObject; break;
				case 3:
					finger_bones [i, j] = fingerContainer.FindChild("ring_"+j.ToString()).gameObject; break;
				case 4:
					finger_bones [i, j] = fingerContainer.FindChild("pinky_"+j.ToString()).gameObject; break;
				}

				finger_bones [i, j].GetComponent<BoneManager> ().scale = fingerBoneScale;
				Debug.Log ("Found bone! " + finger_bones [i, j].ToString () + "| Scaled to "+fingerBoneScale.ToString());
			}
		}

		//Locate palm-bones
		for (int i = 0; i < palmBoneCount; i++) {
			Transform palmContainer = handObject.transform.FindChild ("palm");
			palm_bones [i] = palmContainer.FindChild("palm_"+(i).ToString()).gameObject;
			palmBoneOffsets [i] = palm_bones [i].transform.localPosition;
			palm_bones [i].GetComponent<BoneManager> ().scale = fingerBoneScale*0.75f;
			Debug.Log ("Found bone! " + palm_bones [i].ToString () + "| Offset of "+palmBoneOffsets[i].ToString());
		}
		/*
		//Locate core
		for (int i = 0; i < palmBoneCount; i++) {
			Transform palmContainer = handObject.transform.FindChild ("palm");
			palm_bones [i] = palmContainer.FindChild("palm_"+(i).ToString()).gameObject;
			palm_bones [i].GetComponent<BoneManager> ().scale = fingerBoneScale;
			Debug.Log ("Found bone! " + palm_bones [i].ToString () + "| Scaled to "+fingerBoneScale.ToString());
		}
		*/
	}
	
	// Update is called once per frame
	void Update () {
		//First update of hand and finger positions to prevent destruction of stage
		if (firstUpdate) {
			manager = this.gameObject.GetComponent<LeapManager> ();
			UpdateBonePositions ();
			firstUpdate = false;
		}

		if (manager.HandIsValid()) {
			UpdateBonePositions();
		} else {
			MakeBonesFlyUp ();
		}
	}

	void FixedUpdate()
	{
		//Recharge globalTraction
		if (globalTraction < 1)
		{	globalTraction = Mathf.Min(1, globalTraction + globalTractionRegeneration * Time.deltaTime);	}
	}

	private void UpdateBonePositions()
	{
		//For core
		corePosition = manager.GetPalmWorldPosition();
		core.GetComponent<BoneManager>().targetPosition = corePosition;

		//For palm
		for (int i = 0; i < palmBoneCount; i++) {

			//Scale all offsets, based on grib strength
			//---

			//Update stored positions/rotations
			palmBoneRotations [i] = manager.GetPalmRotation() /** Quaternion.Euler(Vector3.up * 180) * Quaternion.Euler(Vector3.forward * 180)*/; //EXTEND HERE, TO ACCOMODATE MORE PALMBONES!
			//palmBonePositions [i] = manager.GetPalmWorldPosition () +palmBoneOffsets [i];

			Vector3 newOffset = -palmBoneOffsets [i];
			newOffset = Quaternion.Euler (manager.GetPalmRotation().eulerAngles) * newOffset;
			palmBonePositions [i] = manager.GetPalmWorldPosition () + newOffset * fingerBoneScale*0.75f;

			//Update bones
			palm_bones [i].GetComponent<BoneManager>().targetPosition = palmBonePositions[i];
			palm_bones [i].GetComponent<BoneManager>().targetRotation = palmBoneRotations[i];
		}

		//For fingers
		for (int i = 0; i < fingerCount; i++) {
			for (int j = 0; j < fingerBoneCount; j++) {

				//Treat positions differently, depending on if it's thumb or some other finger
				switch(i)
				{
				case 0: //Thumb
					switch(j)
					{
					case 0:
						fingerBonePositions [i, j] = manager.GetBoneCenterWorldPosition (i, 1); break;
					case 1:
						fingerBonePositions [i, j] = manager.GetBoneBaseWorldPosition (i, 2); break;
					case 2:
						fingerBonePositions [i, j] = manager.GetBoneCenterWorldPosition (i, 2); break;
					case 3:
						fingerBonePositions [i, j] = manager.GetBoneBaseWorldPosition (i, 3); break;
					} break;
				default: //Other fingers
					//Update stored positions/rotations
					fingerBonePositions [i, j] = manager.GetBoneCenterWorldPosition (i, j);
					break;
				}

				fingerBoneRotations [i, j] = manager.GetBoneRotation (i, j);

				//Update bones
				finger_bones [i, j].GetComponent<BoneManager>().targetPosition = fingerBonePositions[i,j];
				finger_bones [i, j].GetComponent<BoneManager>().targetRotation = fingerBoneRotations[i,j];
			}
		}
	}

	private void MakeBonesFlyUp()
	{
		Vector3 position;
		float upSpeed = 0.02f;
		float horizontalRange = 0.2f;
			
		//For palm
		for (int i = 0; i < palmBoneCount; i++) {
			//Get current position
			position = palm_bones [i].GetComponent<BoneManager>().targetPosition;
			
			//Add velocity on Y-axis
			position.y += upSpeed;
			position.x += Random.Range ( -horizontalRange, horizontalRange );
			position.z += Random.Range ( -horizontalRange, horizontalRange );

			//Set new position
			palm_bones [i].GetComponent<BoneManager>().targetPosition = position;
		}

		//For fingers
		for (int i = 0; i < fingerCount; i++) {
			for (int j = 1; j < fingerBoneCount; j++) {

				//Get current position
				position = finger_bones [i, j].GetComponent<BoneManager>().targetPosition;

				//Add velocity on Y-axis
				position.y += upSpeed;
				position.x += Random.Range ( -horizontalRange, horizontalRange );
				position.z += Random.Range ( -horizontalRange, horizontalRange );

				//Set new position
				finger_bones [i, j].GetComponent<BoneManager>().targetPosition = position;
			}
		}
	}

	public void addGlobalTraction(float n)
	{
		globalTraction = Mathf.Clamp(globalTraction+n, -globalTractionBuffer,1);
	}

	//For debug only
	public void SetHandColor(Color c)
	{
		handColor = c;
	}
}
