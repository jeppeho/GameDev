using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LevelBuilder : MonoBehaviour {

	private List<GameObject> levelElements = new List<GameObject>();
	private float farClip;

	private int lastActivatedElement = 0;
	private int lastDeactivatedElement = 0;

	Transform levelElementParent;

	// Use this for initialization
	void Start () {

		//levelGenerator = GameObject.Find("LevelGenerator");
		levelElementParent = GameObject.Find ("LevelElements").GetComponent<Transform>();

		//Sort the list based on position.z value
		List<GameObject> sortedLevelElements = levelElements.OrderBy(go => go.GetComponent<Transform>().position.z).ToList ();

		//Get the farClipPlaneFromCamera 
		farClip = GameObject.Find ("Camera").GetComponent<Camera>().farClipPlane;

		//Get and add all level elements to the levelElements list
		GetAllLevelElements ();

		//Deactive all elements
		DeactivateAllLevelElements ();

		//Activate close elements from start
		ActivateOncomingLevelElements (farClip);
	}
	
	// Update is called once per frame
	void Update () {

		if (Time.frameCount % 40 == 0) {
			
			GameObject camera = GameObject.Find ("LeapControllerBlockHand");
			float cameraZ = camera.GetComponent<CameraController> ().GetZPosition ();

			DeactivatePassedLevelElements (cameraZ);
			ActivateOncomingLevelElements (cameraZ + farClip);
		}
	}


//	private void PreactivateElements(){
//
//		for (int i = 0; i < levelElements.Count; i++)
//			if(levelElements [i].name == "WaterBasicNightime(Clone)")
//				levelElements [i].SetActive (false);
//	}


	public void GetAllLevelElements(){

		foreach (Transform t in levelElementParent) {

			//Debug.Log ("t.gameObject.name = " + t.gameObject.name);

			if(t.gameObject.name != "WaterBasicNightime(Clone)")
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

//			if (z < cameraZ) {
//				StartCoroutine (ReleaseGameObject(levelElements [i]));
//			}
			if (z + 15 < cameraZ) {
				levelElements [i].SetActive (false);
				Debug.Log ("Setting inactive @ " + i); 
			} else {
				lastDeactivatedElement = i;
				Debug.Log ("Breaking out @ " + i); 
				break;
			}
		}
	}


	/**
	 * Makes the gameObject fall down
	 */
	IEnumerator ReleaseGameObject (GameObject g){

		int frame = 0;
		while (frame < 150) {
			Vector3 position = g.transform.position;
			position.y -= 0.01f;
			g.transform.position = position;
			frame++;
			yield return new WaitForSeconds (0.01f);
		}
		g.SetActive (false);
		Debug.Log ("Setting inactive @ " + g); 
	}

}
