using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LevelBuilder : MonoBehaviour {

	public bool UseActivationAndDeactivation = true;

	private List<GameObject> levelElements = new List<GameObject>();
	private LevelGenerator levelGenerator;
	private float farClip;

	private int lastActivatedElement = 0;
	private int lastDeactivatedElement = 0;

	Transform levelElementParent;

	// Use this for initialization
	void Start () {

		//levelGenerator = GameObject.Find("LevelGenerator");
		levelElementParent = GameObject.Find ("LevelElements").GetComponent<Transform>();
		levelGenerator = this.gameObject.GetComponent<LevelGenerator> ();

		//Sort the list based on position.z value
		List<GameObject> sortedLevelElements = levelElements.OrderBy(go => go.GetComponent<Transform>().position.z).ToList ();

		//Get the farClipPlaneFromCamera 
		farClip = 120; //GameObject.Find ("Camera").GetComponent<Camera>().farClipPlane;

		//Get and add all level elements to the levelElements list
		GetAllLevelElements ();

		if (UseActivationAndDeactivation) {

			//Deactive all elements
			DeactivateAllLevelElements ();

			//Activate close elements from start
			ActivateOncomingLevelElements (farClip);
		}

		AddElevation ();
	}
	
	// Update is called once per frame
	void Update () {

		if (Time.frameCount % 40 == 0) {
			
			GameObject camera = GameObject.Find ("LeapControllerBlockHand");
			float cameraZ = camera.GetComponent<CameraController> ().GetZPosition ();

			if (UseActivationAndDeactivation) {

				//Deactive passed elements
				DeactivatePassedLevelElements (cameraZ);

				//Activate incoming elements
				ActivateOncomingLevelElements (cameraZ + farClip);
			}
		}
	}


//	private void PreactivateElements(){
//
//		for (int i = 0; i < levelElements.Count; i++)
//			if(levelElements [i].name == "WaterBasicNightime(Clone)")
//				levelElements [i].SetActive (false);
//	}

	private void AddElevation(){

		float[] levelAreaNoise = levelGenerator.GetLevelAreaNoise ();

		foreach (Transform t in levelElementParent) {

			int index = Mathf.FloorToInt (t.position.z);

			float y = levelGenerator.GetLevelAreaHeights()[ Mathf.Clamp(index, 0, levelGenerator.levelLength - 1) ];

			if (t.name == "WaterBasicNightime(Clone)")
				y -= 0.5f;

			//If water
			else if (t.name == "WaterCube(Clone)")
				y -= 1.5f;
			//If stepping stone
			else if (t.name == "STEPPING_STONE(Clone)")
				y += 0f;//0.25f;

			Vector3 elevatedPosition = new Vector3 (t.position.x, y, t.position.z);

			t.position = elevatedPosition;


			/*
			int z = Mathf.FloorToInt (t.position.z);
			LevelGenerator.AreaType type = levelGenerator.GetLevelAreas () [z];
			switch (type) {

			case LevelGenerator.AreaType.start:
				y = levelAreaNoise [z] + 0.3f;
				break;
			case LevelGenerator.AreaType.lava:
				//If water
				if (t.name == "WaterCube(Clone)")
					y = -0.2f;
				//If stepping stone
				else if (t.name == "STEPPING_STONE(Clone)")
					y = 0.05f;
				//all other elements
				else
					y = 0;
				break;
			case LevelGenerator.AreaType.lowGround:
				y = levelAreaNoise [z] + 0.3f;
				break;
			case LevelGenerator.AreaType.cliff:
				y = levelAreaNoise [z] + 0.3f;
				break;
			case LevelGenerator.AreaType.highGround:
				y = levelAreaNoise [z] + 0.3f;
				break;
			case LevelGenerator.AreaType.bridge:
				y = 1;
				break;
			default:
				break;
			}

			y *= 5f;

			Vector3 elevatedPosition = new Vector3 (t.position.x, y, t.position.z);

			t.position = elevatedPosition;
			*/
		}


	}


	public void GetAllLevelElements(){

		foreach (Transform t in levelElementParent) {

			//Debug.Log ("t.gameObject.name = " + t.gameObject.name);

			if(t.gameObject.name != "WaterBasicNightime(Clone)" /* && t.gameObject.name != "Light" */)
				levelElements.Add (t.gameObject);
		}
	
	}


	public void AddLevelElement(GameObject go){
			levelElements.Add (go);
	}


	/**
	 * Set all elements in list to false
	 */
	private void DeactivateAllLevelElements(){
		for (int i = 0; i < levelElements.Count; i++)
			levelElements [i].SetActive (false);
	}


	/**
	 * Activate all elemetns that is closer than the activation point
	 */
	private void ActivateOncomingLevelElements(float activationPoint){

		for (int i = lastActivatedElement; i < levelElements.Count; i++) {

			float z = levelElements [i].GetComponent<Transform> ().position.z;

			if (z < activationPoint) {
				levelElements [i].SetActive (true);
			} else {
				lastActivatedElement = i;
				break;
			}
		}
	}


	/**
	 * Deactivate all elements that are behind the camera
	 */
	private void DeactivatePassedLevelElements(float cameraZ){

		for (int i = lastDeactivatedElement; i < levelElements.Count; i++) {

			float z = levelElements [i].GetComponent<Transform> ().position.z;


			if (z < cameraZ - 0.5f) {

				StartCoroutine (ReleaseGameObject(levelElements [i]));
			}
//			if (z + 15 < cameraZ) {
//				levelElements [i].SetActive (false);
//			} 
			else {
				lastDeactivatedElement = i;
				break;
			}
		}
	}


	/**
	 * Makes the gameObject fall down
	 */
	IEnumerator ReleaseGameObject (GameObject g){

		float fallSpeed = 0.01f;
		int frame = 0;

		if(g.name.Contains("Pillar"))
			yield return new WaitForSeconds (1.5f);

		if( g.name.Contains("cliff") )
			yield return new WaitForSeconds (1f);

		int z = Mathf.FloorToInt( g.transform.position.z );

		bool isNextToWater = false;

		//If current area is lowGround
		if (levelGenerator.GetLevelAreas () [z] == LevelGenerator.AreaType.lowGround) {
			//If one of following areas is lava
			for (int i = 0; i < 2; i++) {

				if (levelGenerator.GetLevelAreas () [z + i] == LevelGenerator.AreaType.lava) {
					isNextToWater = true;
				}
			}
		}
			

		//Don't lower the floor to the water
		if( !isNextToWater && !g.name.Contains("WaterCube") && !g.name.Contains("canyon") && !g.name.Contains("STEPPING") ){
		
			while (frame < 150) {
				Vector3 position = g.transform.position;
				//position.y -= 0.02f;
				position.y -= fallSpeed * Time.deltaTime;
				fallSpeed += 0.02f;
				g.transform.position = position;
				frame++;
				yield return new WaitForSeconds (0.01f);
			}
		
			g.SetActive (false);

		} else {

			yield return new WaitForSeconds (8f);
			g.SetActive (false);
		
		}
	}

}
