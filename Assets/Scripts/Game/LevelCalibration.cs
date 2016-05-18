using UnityEngine;
using System.Collections;

public class LevelCalibration : MonoBehaviour {

	public GameObject calibrationText;
	public bool calibrationDone;

	// Use this for initialization
	void Start () {

		calibrationDone = false; // set true for testing
		calibrationText.SetActive(true); // set false for testing
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
