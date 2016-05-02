using UnityEngine;
using System.Collections;

public class CameraScrollingTutorial : MonoBehaviour {

	private float cameraSpeed = 2f;

	public GameObject text;

	// Use this for initialization
	void Start () {
		text.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {

		MoveForward ();
	
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
