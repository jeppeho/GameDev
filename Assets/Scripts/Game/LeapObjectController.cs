using UnityEngine;
using System.Collections;

public class LeapObjectController : MonoBehaviour {


	private float cameraStartSpeed = 0.2f;
	private float cameraNormalSpeed = 1f;
	private float cameraSpeed;

	public int slowStartNumFrames = 200;

	public bool freezeCameraForDebug = false;



	// Use this for initialization
	void Start () {

		cameraSpeed = cameraStartSpeed;
		//Scale the cameraspeed
		cameraSpeed *= LevelManager.SPEED;

		StartCoroutine (slowStart ());
	}

	// Update is called once per frame
	void Update () {
		if (!freezeCameraForDebug) {
			MoveForward ();
		}
	}


	IEnumerator slowStart(){

		int i = 0;

		while(i < slowStartNumFrames){

			SetCameraSpeed( Mathf.Lerp (cameraStartSpeed, cameraNormalSpeed, (float)i / (float)slowStartNumFrames) );
			i++;

			yield return new WaitForSeconds(0.02f);
		}
	}
		

	/**
	 * Multiplies parameter speed with LevelManager.SPEED
	 */
	private void SetCameraSpeed(float speed){
		cameraSpeed = speed * LevelManager.SPEED;
	}

	private void MoveForward(){
		Vector3 position = this.gameObject.transform.position;

		position.z += cameraSpeed * Time.deltaTime; 

		this.gameObject.transform.position = position;
	}

	public Vector3 GetPosition(){
		return this.transform.position;
	}

	public float GetZPosition(){
		return this.transform.position.z;
	}
}
