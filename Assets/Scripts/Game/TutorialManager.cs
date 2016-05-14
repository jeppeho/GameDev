using UnityEngine;
using System.Collections;

public class TutorialManager : MonoBehaviour {

	private float cameraSpeed = 2f;

	public GameObject text;
    public GameObject calibrationText;

    private bool calibrationDone;

	// Use this for initialization
	void Start () {
		text.SetActive(false);
        
        calibrationDone = true; // set true for testing
        StartCalibration();
        calibrationText.SetActive(false); // set false for testing
    }
	
	// Update is called once per frame
	void Update () {

       if(calibrationDone == true) { 
        MoveForward();
        }

    }

    private void StartCalibration()
    {
        


    }

	private void MoveForward(){

		Vector3 currentPosition = this.gameObject.transform.position;

		if(currentPosition.z < -0)
			{
			currentPosition.z += cameraSpeed * Time.deltaTime; 

			this.gameObject.transform.position = currentPosition;
			}
		else
		{	
			text.SetActive(true);
		}
	}

}
