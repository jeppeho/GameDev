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
	private int lastRenderingModeChangeIndex = 0;

	Transform levelElementParent;

	GameObject camera;

	// Use this for initialization
	void Start () {

		//levelGenerator = GameObject.Find("LevelGenerator");
		levelElementParent = GameObject.Find ("LevelElements").GetComponent<Transform>();
		levelGenerator = this.gameObject.GetComponent<LevelGenerator> ();
		camera = GameObject.Find ("LeapControllerBlockHand");
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

		ActivateGoalArea ();

		AddElevation ();

		//Set all crystals to opaque rendering mode
		SetAllCrystalsRenderingModeOpaque ();
		//Set the nearest crystals to transparent rendering mode
		SetTransparencyBasedOnDistance (camera.transform.position.z);

	}
	
	// Update is called once per frame
	void Update () {

		if (Time.frameCount % 40 == 0) {
			
			//GameObject camera = GameObject.Find ("LeapControllerBlockHand");
			float cameraZ = camera.GetComponent<CameraController> ().GetZPosition ();

			SetTransparencyBasedOnDistance (cameraZ);

			if (cameraZ > levelGenerator.levelLength) {
				EndState ();
			}

			else if (UseActivationAndDeactivation) {

				//Deactive passed elements
				DeactivatePassedLevelElements (cameraZ);

				//Activate incoming elements
				ActivateOncomingLevelElements (cameraZ + farClip);
			}
		}
	}


	private void EndState(){

		for (int i = lastDeactivatedElement; i < levelElements.Count; i++) {

			StartCoroutine (ReleaseEndGameObjects (levelElements [i]));
		
		}
	}


	private void AddElevation(){

		float[] levelAreaNoise = levelGenerator.GetLevelAreaNoise ();

		foreach (Transform t in levelElementParent) {

			int index = Mathf.FloorToInt (t.position.z);

			float y = levelGenerator.GetLevelAreaHeights()[ Mathf.Clamp(index, 0, levelGenerator.levelLength - 1) ];

			if (t.name.Contains ("WaterPlane"))
				y -= 0.8f;
			else
			//If water
				if (t.name == "WaterCube(Clone)")
				y -= 2f;
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

			if(!t.gameObject.name.Contains("Water") /* && t.gameObject.name != "Light" */)
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


	private void ActivateGoalArea(){
	
		for (int i = 0; i < levelElements.Count; i++) {

			if (levelElements [i].name.Contains ("Goal")) {
				levelElements [i].SetActive (true);
			}
		}
	}


	/**
	 * Activate all elements that are closer than the activation point
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

	private void SetTransparencyBasedOnDistance(float cameraZ){


		for (int i = lastRenderingModeChangeIndex; i < levelElements.Count; i++) {

			float z = levelElements [i].GetComponent<Transform> ().position.z;

			if (z < cameraZ + 27f) {
				if (levelElements [i].name.Contains ("Pillar")) {

					foreach (Transform intact in levelElements [i].transform) {

						if (intact.name.Contains ("intactObject")) {
							SetTransparent (intact.gameObject);
						}
					}
				}
			} else {
				lastRenderingModeChangeIndex = i;
				break;
			}
		}
	}


	private void SetTransparencyBasedOnDistanceOLD(float cameraZ){
	
		foreach (Transform t in levelElementParent) {

			if (t.transform.position.z < cameraZ + 27f) {

				if (t.name.Contains ("Pillar")) {

					foreach (Transform intact in t) {

						if (intact.name.Contains ("intactObject")) {
							Debug.Log ("Changing alpha for " + intact.name);
							SetTransparent (intact.gameObject);
						}
					}
				}
			}
		}
	}


	private void SetAllCrystalsRenderingModeOpaque(){

		foreach (Transform t in levelElementParent) {
			//Debug.Log ("testing " + t.name);

			if(t.name.Contains("Pillar")){
				
				foreach (Transform intact in t) {
					
					if (intact.name.Contains ("intactObject")) {
						SetOpaque (intact.gameObject);
					}
				}

			}
		}
	}

	/**
	 * Sets the rendering mode of the inputted GameObject to opaque rendering mode
	 * and it updates some parameters so it can be done at runtime
	 */
	private void SetOpaque(GameObject g){

		Material material = g.gameObject.GetComponent<Renderer> ().material;

		//OPAQUE
		material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
		material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
		material.SetInt("_ZWrite", 1);
		material.DisableKeyword("_ALPHATEST_ON");
		material.DisableKeyword("_ALPHABLEND_ON");
		material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
		material.renderQueue = -1;

		g.gameObject.GetComponent<Renderer> ().material = material;
	}

	/**
	 * Sets the rendering mode of the inputted GameObject to transparent rendering mode
	 * and it updates some parameters so it can be done at runtime
	 */
	private void SetTransparent(GameObject g){

		Material material = g.gameObject.GetComponent<Renderer> ().material;

		//TRANSPARENT
		material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
		material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
		material.SetInt("_ZWrite", 0);
		material.DisableKeyword("_ALPHATEST_ON");
		material.DisableKeyword("_ALPHABLEND_ON");
		material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
		material.renderQueue = 3000;

		g.gameObject.GetComponent<Renderer> ().material = material;
	}

//	private void SetAlphaChannel(GameObject g, float alpha){
//	
//		Material material = g.gameObject.GetComponent<Renderer> ().material;
//
//
//
////		Color c = material.GetColor ("_Color");
////		Color sc = material.GetColor ("_SpecColor");
////		Color e = material.GetColor ("_EmissionColor");
////
////		material.SetFloat("_Mode", 0f);
////
////		c.a = alpha;
////		sc.a = alpha;
////		e.a = alpha;
////
////		material.SetColor ("_Color", c);
////		material.SetColor ("_SpecColor", sc);
////		material.SetColor ("_EmissionColor", e);
//
//
//		//OPAQUE
//		material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
//		material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
//		material.SetInt("_ZWrite", 1);
//		material.DisableKeyword("_ALPHATEST_ON");
//		material.DisableKeyword("_ALPHABLEND_ON");
//		material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
//		material.renderQueue = -1;
//
//		//TRANSPARENT
//		material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
//		material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
//		material.SetInt("_ZWrite", 0);
//		material.DisableKeyword("_ALPHATEST_ON");
//		material.DisableKeyword("_ALPHABLEND_ON");
//		material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
//		material.renderQueue = 3000;
//
//
//		g.gameObject.GetComponent<Renderer> ().material = material;
//	}



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

	IEnumerator MakeObjectFlyUp (GameObject g){
		Debug.Log ("Starting up MakeObjectFlyUp");

		Vector3 pos = g.transform.position;

		float initY = pos.y;

		pos.y -= 5;

		g.transform.position = pos;

		while (pos.y < initY) {
			
			pos.y += Time.deltaTime;
			g.transform.position = pos;


			yield return new WaitForEndOfFrame ();
		}
	}


	IEnumerator ReleaseEndGameObjects(GameObject g){

		float fallSpeed = 0.01f;
		int frame = 0;

		while (frame < 150) {
			Vector3 position = g.transform.position;
			//position.y -= 0.02f;
			position.y -= fallSpeed * Time.deltaTime;
			fallSpeed += 0.02f;
			g.transform.position = position;
			frame++;
			yield return new WaitForSeconds (0.01f);
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
					break;
				} else if (levelGenerator.GetLevelAreas () [z - i] == LevelGenerator.AreaType.lava) {
					isNextToWater = true;
					break;
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
