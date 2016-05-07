using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	LevelGenerator l;

	float[] cameraPositions;
	float[] lookAts;

	// Use this for initialization
	void Start () {
		l = GameObject.Find ("LevelGenerator").GetComponent<LevelGenerator> ();

		cameraPositions = new float[l.levelLength];
		lookAts = new float[l.levelLength];

		SetLookAtsArray ();
		SetCameraPositionsArray ();
	}
	
	// Update is called once per frame
	void Update () {
			
		if (GetZPosition() > 0f) {
			UpdateCameraPosition ();
			UpdateCameraRotation ();
		}
	}




	private void UpdateCameraRotation(){

		Quaternion rotation = this.transform.parent.rotation;

		rotation.y = GetLerpedRotation() / 60f;

		this.transform.parent.rotation = rotation;
	}

	private void UpdateCameraPosition(){

		Vector3 currentPos = this.transform.parent.position;

		currentPos.x = GetLerpedPosition ();

		this.transform.parent.position = currentPos;
	}


	private float GetLerpedRotation(){
	
		int floor_z = Mathf.FloorToInt (GetZPosition ());

		float t = GetZPosition () - floor_z;

		float rotY = Mathf.Lerp (lookAts[floor_z], lookAts[floor_z + 1], t);

		return rotY;
	}

	private float GetLerpedPosition(){
	
		int floor_z = Mathf.FloorToInt (GetZPosition ());

		float t = GetZPosition () - floor_z;

		float rotY = Mathf.Lerp (cameraPositions[floor_z], cameraPositions[floor_z + 1], t);

		return rotY;
	}


	private void SetLookAtsArray(){

		for (int i = 0; i < l.levelLength; i++)
			lookAts [i] = SetAveragedCameraPosition (i, 7, 12);
	}

	private void SetCameraPositionsArray(){

		for (int i = 0; i < l.levelLength; i++)
			cameraPositions [i] = SetAveragedCameraPosition (i, 0, 5);
	}


	private float SetAveragedCameraPosition (int z, int start, int end){

		float a = 0f;

		for (int i = start; i < end; i++) {

			a += l.GetCameraPositionAtIndex ( GetAcceptedLevelIndex ( z + i ) );
		}

		//Divide by number of samples
		a /= (end - start);

		return a;
	}


	/**
	 * Returns and index that is not out of bounds
	 */
	private int GetAcceptedLevelIndex(int z){

		if (z < 0)
			z = 0;
		else if (z >= l.levelLength)
			z = l.levelLength - 1;

		return z;
	}



	public Vector3 GetPosition(){
		return this.transform.position;
	}

	public float GetZPosition(){
		return this.transform.position.z;
	}



//
//	private float GetAveragedInComingPositions(int start, int end){
//	
//		int z = Mathf.FloorToInt( GetZPosition () );
//
//		float a = 0;
//		float b = 0;
//
//		for (int i = start; i <= end; i++) {
//		
//			if(i < end)
//				a += l.GetCameraPositionAtIndex ( GetAcceptedLevelIndex( z + i ) );
//			if(i > start)
//				b += l.GetCameraPositionAtIndex ( GetAcceptedLevelIndex( z + i ) );
//		
//		}
//
//		a /= (end - start);
//		b /= (end - start);
//	
//		float t = GetZPosition () - Mathf.FloorToInt( GetZPosition() );
//		Debug.Log ("t = " + t);
//		float x = Mathf.Lerp (a, b, t);
//
//		return x;
//	}
//
//	private void SetLookAt(){
//
//		Quaternion rotation = this.transform.parent.rotation;
//		Debug.Log ("rotation = " + rotation);
//		rotation.y = GetAveragedInComingPositions(7, 12) / 60;
//		//rotation.x = 30f;
//		Debug.Log ("- rotation = " + rotation);
//		this.transform.parent.rotation = rotation;
//
//	}
//		
//
//	private float GetNextLookAt(){
//	
//		int z = Mathf.FloorToInt( GetZPosition () );
//
//		float z3 = l.GetCameraPositionAtIndex ( GetAcceptedLevelIndex( z + 7 ) );
//		float z4 = l.GetCameraPositionAtIndex ( GetAcceptedLevelIndex( z + 8 ) ) ;
//		float z5 = l.GetCameraPositionAtIndex ( GetAcceptedLevelIndex( z + 9 ) ) ;
//		float z6 = l.GetCameraPositionAtIndex ( GetAcceptedLevelIndex( z + 10 ) ) ;
//		float z7 = l.GetCameraPositionAtIndex ( GetAcceptedLevelIndex( z + 11 ) ) ;
//		float z8 = l.GetCameraPositionAtIndex ( GetAcceptedLevelIndex( z + 12 ) ) ;
//
//		float a = (z3 + z4 + z5 + z6 + z7) / 5;
//		float b = (z4 + z5 + z6 + z7 + z8) / 5;
//
//		float t = GetZPosition () - Mathf.FloorToInt( GetZPosition() );
//		Debug.Log ("t = " + t);
//		float x = Mathf.Lerp (a, b, t);
//
//		return x;
//
//	}
//
//
//
//	private void SetLookAtOLD(){
//
//		Quaternion rotation = this.transform.parent.rotation;
//		Debug.Log ("rotation = " + rotation);
//		rotation.y = GetNextLookAt ()/ 60;
//		//rotation.x = 30f;
//		Debug.Log ("- rotation = " + rotation);
//		this.transform.parent.rotation = rotation;
//
//	}
//
//
//	private float GetNextXPosition(){
//
//		Debug.Log ("/////////Finding the next position!");
//
//		int z = Mathf.FloorToInt( GetZPosition () );
//
//		float z3 = l.GetCameraPositionAtIndex ( GetAcceptedLevelIndex( z + 0) );
//		float z4 = l.GetCameraPositionAtIndex ( GetAcceptedLevelIndex( z + 1 ) ) ;
//		float z5 = l.GetCameraPositionAtIndex ( GetAcceptedLevelIndex( z + 2 ) ) ;
//		float z6 = l.GetCameraPositionAtIndex ( GetAcceptedLevelIndex( z + 3 ) ) ;
//		float z7 = l.GetCameraPositionAtIndex ( GetAcceptedLevelIndex( z + 4 ) ) ;
//		float z8 = l.GetCameraPositionAtIndex ( GetAcceptedLevelIndex( z + 5 ) ) ;
//
//		float a = (z3 + z4 + z5 + z6 + z7) / 5;
//		float b = (z4 + z5 + z6 + z7 + z8) / 5;
//
//
//		float t = GetZPosition () - Mathf.FloorToInt( GetZPosition() );
//
//		float x = Mathf.Lerp (a, b, t);
//
//		return x;
//	}
//
//
//	private void SetXPosition(){
//	
//		Vector3 currentPos = this.transform.parent.position;
//
//		float nextX = GetNextXPosition ();
//
//		Debug.Log ("Next camera X = " + nextX);
//
//		currentPos.x = nextX;
//
//		this.transform.parent.position = currentPos;
//	}

}
