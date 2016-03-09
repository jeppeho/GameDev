using UnityEngine;
using System.Collections;

public class PickupObject : MonoBehaviour {

	public GameObject holdSlot;

	// Use this for initialization
	void Start () {
		holdSlot = GameObject.Find("HoldSlot");
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnCollisionEnter(Collision other){

		Debug.Log ("OTHER object is: " + other);

		//holdSlot = other;
	}
}
