using UnityEngine;
using System.Collections;

public class MoveCamera : MonoBehaviour {

	public float cameraSpeed;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

		//Debug.Log ("Moving camera");
		Vector3 position = this.gameObject.transform.position;

		position.z += cameraSpeed * Time.deltaTime; 

		this.gameObject.transform.position = position;
	}
}
