using UnityEngine;
using System.Collections;

public class StoneHandManager : MonoBehaviour {

	int NUM_FINGERS = 5;
	int NUM_BONES = 4;

	//The position of all bones:
	//[0]
	private Vector3[,] bonePositions = new Vector3[6, 4];
	private Quaternion[,] boneRotations = new Quaternion[6, 4];
	private Vector3 corePosition = new Vector3();
	private float boneMass = 5f;

	private GameObject[,] finger_bones;
	private GameObject palm_bone;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
