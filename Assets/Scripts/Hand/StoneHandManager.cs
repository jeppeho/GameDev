using UnityEngine;
using System.Collections;

[RequireComponent (typeof(LeapManager))]

public class StoneHandManager : MonoBehaviour {

	//All bone objects
	private GameObject[,] finger_bones;
	private GameObject[] palm_bones;

	//Model used for fingers
	public GameObject FingerBoneModel;

	//Bone counts
	private int NUM_FINGERS = 5;
	private int NUM_FINGERBONES = 4;
	private int NUM_PALMBONES = 1;

	//The position of all bones
	private Vector3[,] fingerBonePositions = new Vector3[5, 4];
	private Quaternion[,] fingerBoneRotations = new Quaternion[5, 4];
	private Vector3[] palmBonePositions = new Vector3[1];
	private Quaternion[] palmBoneRotations = new Quaternion[1];

	//The global scale of the bones
	public float fingerBoneScale = 1.5f;
	//The position of the core (the center of the palm?)
	private Vector3 corePosition = new Vector3();

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

	void Awake()
	{
		manager = this.gameObject.GetComponent<LeapManager> ();
		finger_bones = new GameObject[NUM_FINGERS, NUM_FINGERBONES];
		palm_bones = new GameObject[NUM_PALMBONES];
	}
	// Use this for initialization
	void Start () {

		//Create GameObject-arrays
		manager = this.gameObject.GetComponent<LeapManager> ();
		finger_bones = new GameObject[NUM_FINGERS, NUM_FINGERBONES];
		palm_bones = new GameObject[NUM_PALMBONES];

		//Create fingerbones
		for (int i = 0; i < NUM_FINGERS; i++) {
			for (int j = 0; j < NUM_FINGERBONES; j++) {
				//Debug.Log ("Creating Sphere");

				//Insert prefab fingerbone in array
				finger_bones [i, j] = Instantiate(FingerBoneModel, new Vector3(0,5,-130), Quaternion.Euler(0,0,0)) as GameObject;
				finger_bones [i, j].GetComponent<BoneManager> ().scale = 0.2f*fingerBoneScale;
			}
		}

		//Create palm-bones
		for (int i = 0; i < NUM_PALMBONES; i++) {
			palm_bones [i] = Instantiate (FingerBoneModel, new Vector3(0,5,-130), Quaternion.Euler (0, 0, 0)) as GameObject;
			palm_bones [i].GetComponent<BoneManager> ().scale = 0.4f*fingerBoneScale;
		}
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
		//For palm
		for (int i = 0; i < NUM_PALMBONES; i++) {
			
			//Update stored positions/rotations
			palmBonePositions [i] = manager.GetPalmWorldPosition(); //EXTEND HERE, TO ACCOMODATE MORE PALMBONES!
			palmBoneRotations [i] = Quaternion.FromToRotation(Vector3.zero, manager.GetPalmNormal()); //EXTEND HERE, TO ACCOMODATE MORE PALMBONES!

			//Update bones
			palm_bones [i].GetComponent<BoneManager>().targetPosition = palmBonePositions[i];
		}

		//For fingers
		for (int i = 0; i < NUM_FINGERS; i++) {
			for (int j = 1; j < NUM_FINGERBONES; j++) {
				//Update stored positions/rotations
				fingerBonePositions[i, j] = manager.GetBoneCenterWorldPosition(i,j);
				fingerBoneRotations [i, j] = Quaternion.FromToRotation(Vector3.zero, manager.GetBoneRotation (i, j));

				//Update bones
				finger_bones [i, j].GetComponent<BoneManager>().targetPosition = fingerBonePositions[i,j];
			}
		}
	}

	private void MakeBonesFlyUp()
	{
		Vector3 position;
		float upSpeed = 0.02f;
		float horizontalRange = 0.2f;

		//For palm
		for (int i = 0; i < NUM_PALMBONES; i++) {
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
		for (int i = 0; i < NUM_FINGERS; i++) {
			for (int j = 1; j < NUM_FINGERBONES; j++) {

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
