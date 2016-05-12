using UnityEngine;
using System.Collections;

public class PlayerScore : MonoBehaviour {


	private float damage = 1;
	private float startDamage = 1;
	private float time = 0;


	void OnLevelWasLoaded(){
		damage = startDamage;
		time = 0;
	}


	
	// Update is called once per frame
	void Update () {

		if (Time.frameCount % 60 == 0) {
			Debug.Log ("//////////////////");
			Debug.Log ("damage = " + damage + " | time = " + time);
			Debug.Log ("Final Score = " + GetFinalScore());
		}


	}


	public int GetFinalScore(){

		return Mathf.FloorToInt (time / damage * time) ;

	}

	public void IncreaseDamage(float increase){
		Debug.Log ("increasing damage = " + increase + " total damage = " + damage);
		damage += increase;
	}

	public float GetDamage(){
		return this.damage;
	}

	public void IncreaseTime(float increase){
		//Debug.Log ("Increasing with " + increase + ", total = " + GetTime ());
		time += increase;
	}

	public float GetTime(){
		return this.time;
	}



}
