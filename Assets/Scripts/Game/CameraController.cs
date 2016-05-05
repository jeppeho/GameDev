using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	LevelGenerator l;

	// Use this for initialization
	void Start () {
		l = GameObject.Find ("LevelGenerator").GetComponent<LevelGenerator> ();
	}
	
	// Update is called once per frame
	void Update () {
			SetXPosition ();
			SetLookAt ();
	}
		

	private float GetNextLookAt(){
	
		int z = Mathf.FloorToInt( GetZPosition () );

		float a = l.GetCameraPositionAtIndex ( GetAcceptedLevelIndex( z ) );

		float b = l.GetCameraPositionAtIndex (GetAcceptedLevelIndex (z + 10) );

		float t = GetZPosition () - Mathf.FloorToInt( GetZPosition() );

		float x = Mathf.Lerp (a, b, t);

		return x;

	}

	private void SetLookAt(){

		Vector3 target = new Vector3 (0, 0, GetZPosition() + 2 );

		target.y = 4.5f;//GetNextLookAt () * 3f;

		this.transform.LookAt (target);
	}


	private float GetNextXPosition(){

		Debug.Log ("/////////Finding the next position!");

		int z = Mathf.FloorToInt( GetZPosition () );

		float a = l.GetCameraPositionAtIndex ( GetAcceptedLevelIndex( z ) );
		float b = l.GetCameraPositionAtIndex (GetAcceptedLevelIndex (z + 1));

		float t = GetZPosition () - Mathf.FloorToInt( GetZPosition() );

		float x = Mathf.Lerp (a, b, t);

		return x;
	}


	private void SetXPosition(){
	
		Vector3 currentPos = this.transform.position;

		float nextX = GetNextXPosition ();

		Debug.Log ("Next camera X = " + nextX);

		currentPos.x = nextX;

		this.transform.position = currentPos;
	}


	/**
	 * Returns and index that is not out of bounds
	 */
	private int GetAcceptedLevelIndex(int z){

		if (z < 0)
			z = 0;
		else if (z > l.levelLength)
			z = l.levelLength;

		return z;
	}



	public Vector3 GetPosition(){
		return this.transform.position;
	}

	public float GetZPosition(){
		return this.transform.position.z;
	}
}
