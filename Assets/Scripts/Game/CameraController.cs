using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	private float camera_speed = 1;

	// Use this for initialization
	void Start () {

		//Scale the cameraspeed
		camera_speed *= LevelManager.SPEED;
	}
	
	// Update is called once per frame
	void Update () {

		//Debug.Log ("Moving camera");
		Vector3 position = this.gameObject.transform.position;

		position.z += camera_speed * Time.deltaTime; 

		this.gameObject.transform.position = position;
	}

	public float GetZPosition(){
		Debug.Log ("Camera");
		return this.transform.position.z;
	}
}
