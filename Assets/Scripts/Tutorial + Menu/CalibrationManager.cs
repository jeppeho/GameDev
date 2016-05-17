using UnityEngine;
using System.Collections;

public class CalibrationManager : MonoBehaviour {

	private float cameraSpeed = 2f;

	public GameObject UItext;
    public GameObject calibrationText;
    public GameObject UIMinions;

    public bool calibrationDone;

	// Use this for initialization
	void Start () {
		UItext.SetActive(false);
        UIMinions.SetActive(false);
        
        calibrationDone = false; // set true for testing
        calibrationText.SetActive(true); // set false for testing
    }
	
	// Update is called once per frame
	void Update () {

       if(calibrationDone == true) {
            StartCoroutine(Wait());
            
        }

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
			UItext.SetActive(true);
            UIMinions.SetActive(true);
		}
	}


    IEnumerator Wait()
    {
        Debug.Log("waiting?");
        yield return new WaitForSeconds(2f);
        MoveForward();
        calibrationText.SetActive(false);
        //tutorialManager.calibrationDone = true;
    }
}
