﻿using UnityEngine;
using System.Collections;

public class PlayerRelicHandler : MonoBehaviour {

	private Rigidbody rb;

	string playerState;

	
	// Update is called once per frame
	void Update () {

		//COULD I CHECK THIS IN THE PLAYERMANAGER???
		//Get state of player
		playerState = this.gameObject.GetComponent<PlayerManager> ().GetState ();

		//If player not active, release object
		if (playerState != "active") {

			if ( HasRelic() ) {
				
				ReleaseRelic ();
			
			}
		}
	}

	void OnCollisionEnter(Collision collision){

		if (playerState == "active") {

			if(!HasRelic()){

				if (collision.gameObject.tag == "Relic") {

					//Set player as parent to Relic
					collision.gameObject.GetComponent<RelicManager>().SetParent(this.gameObject.transform);
				}
			}
		}
	}


	public bool HasRelic(){

		foreach(Transform t in transform){
			if (t.tag == "Relic")
				return true;
		}

		return false;
	}

	private GameObject GetRelic(){

		GameObject relic = null;

		foreach(Transform t in transform){
			if (t.tag == "Relic")
				relic = t.gameObject;
		}
	
		return relic;

//		if (relic == null)
//			Debug.Log ("Couldn't find relic in GetRelic() in PlayerRelicHandler");
//			

	}


	public void ReleaseRelic(){
		GameObject.Find ("Relic").GetComponent<RelicManager>().ReleaseFromParent();
	}
}
