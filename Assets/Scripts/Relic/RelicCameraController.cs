using UnityEngine;
using System.Collections;

public class RelicCameraController : MonoBehaviour {

	Transform t;
	Quaternion rotation;

	// Use this for initialization
	void Start () {
		t = this.gameObject.GetComponent<Transform> ();
		rotation = t.rotation;
	}
	
	// Update is called once per frame
	void Update () {
		KeepCameraCentered ();
		KeepCameraRotation ();
	}

	/**
	 * Function for mainting some rotation and position values
	 * When camera is following relic
	 */
	public void KeepCameraCentered(){

		Vector3 parent_position = GameObject.Find ("Relic").GetComponent<Transform> ().position;//this.gameObject.GetComponentInParent<Transform> ().position;
		Vector3 pos = t.position;
		//t.position = new Vector3 (5, 5, -6);

		pos.x = 5;
		pos.y = 5;
		pos.z = parent_position.z - 6;
//		Debug.Log("Pos @" + t.position);
		t.position = pos;
	}

	public void KeepCameraRotation(){
		t.rotation = rotation;
	}
}
