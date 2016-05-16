﻿using UnityEngine;
using System.Collections;

public class RelicHealth : MonoBehaviour {

	public float startHealth = 1000;
	public float maxHandDistanceToTakeEnergy = 2f;
	private float health;

	private Rigidbody rb;
	private RelicManager manager;
	private GameObject sphere;
	private PlayerScore ps;


	Color specularColor;
	Color baseColor;

	private GameObject handCore;

	// Use this for initialization
	void Start () {
		health = startHealth;
		rb = GetComponent<Rigidbody> ();
		manager = GetComponent<RelicManager> ();
		ps = this.gameObject.GetComponent<PlayerScore> ();

		sphere = GameObject.Find ("RelicSphere");

		specularColor = new Color (1f, 0.8f, 0.3f, 1f); // new Color (1, 0.5f, 0.5f);//
		baseColor = new Color (1f, 0.7f, 0.2f, 1f); // new Color (1, 0.5f, 0.5f);//

		//Get the core ball of the hand 
		handCore = GameObject.Find ("ball");
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		SetShaderSpecularColor (specularColor * GetNormalizedHealth());
		SetShaderColor (GetBaseColorBasedOnHealth());

//
//		if (!manager.HasParent ()) {
//			if (GetDistanceToCore () < maxHandDistanceToTakeEnergy) {
//				
//				DrainEnergy (1);
//
//			} else {
//				
//				SetShaderSpecularColor (specularColor * GetNormalizedHealth());
//				SetShaderColor (GetBaseColorBasedOnHealth());
//
//			}
//		}
	}



	/**
	 * On collision drain energy, linearly to velocity between colliding objects
	 * Right now it doesn't matter if it has a parent.
	 */
	void OnCollisionEnter(Collision col){

		float relativeVelocity = col.relativeVelocity.magnitude;

		//Only if relic collides with crystals
		if (col.gameObject.layer == 13) {

			//If relic is not carried
			if (!manager.HasParent ()) {

				//Only drain if above some threshold
				if (relativeVelocity > 3f) {

					float drain = Mathf.FloorToInt (relativeVelocity * 2);

					//Limit max energy drain
					if (drain > 20f)
						drain = 20f;

					DrainEnergy (drain);
				
				}

			//If relic is being carried
			} else {

				//Increase damage score for player
				ps.IncreaseDamage (Mathf.Clamp(0, 30, relativeVelocity));

				//If collision is big enough
				if (relativeVelocity > 30f) {

					//Remove from parent
					DrainEnergy (30f);
					manager.ReleaseFromParent ();
				}
			}
		}
	}
		

	private Color GetSpecularColorBasedOnHealth(){
		Color color = specularColor * GetNormalizedHealth ();
		color.a = 1f;
		return color;
	}

	private Color GetBaseColorBasedOnHealth(){
		Color color = baseColor * GetNormalizedHealth();
		color.a = 1f;
		return color;
	}

	/**
	 * Returns the distance from the relic to the core of the god hand
	 */
	private float GetDistanceToCore(){

		Vector3 vectorToCore = rb.transform.position - handCore.GetComponent<CoreManager> ().GetHandCorePosition ();

		return vectorToCore.magnitude;
	}


	/**
	 * Drain energy and update the color
	 */
	public void DrainEnergy(float drain){

		Debug.Log ("Draining " + drain + " energy from relic");

		//drain health
		health -= drain;

		//If hard hit
		if (drain > 1){
			SetShaderSpecularColor (new Color (1, 1, 1));
			SetShaderColor (new Color (1, 1, 1));

		//If god hand takes health, then blink occasionally
		} else if (Time.frameCount % 5 == 0 || Time.frameCount + 1 % 5 == 0 || Time.frameCount + 2 % 5 == 0 || Time.frameCount + 1 % 5 == 0) {
			SetShaderSpecularColor (new Color (1, 1, 1));
			SetShaderColor (new Color (1, 1, 1));
		}

		if (health < 0) {
			manager.ReleaseFromParent ();
			DeleteVisualRelic ();
		}
	}

	public float GetHealth(){
		return this.health;
	}


	/**
	 * Updates the color on the basis of how much health is left
	 */
	private void UpdateColor(){
		this.GetComponent<Renderer> ().material.color =  new Color(1, GetNormalizedHealth(), GetNormalizedHealth());
		this.GetComponent<Renderer> ().material.color =  new Color(GetNormalizedHealth(), 0, 0);
	}

	private void FlashColor(){
		//this.GetComponent<Renderer> ().material.color = new Color (1, 1, 1);
//		this.GetComponent<Renderer> ().material.SetColor ("_Color", new Color (0, 0, 1));
//		this.GetComponent<Renderer> ().material.SetColor ("_SpecColor", new Color (0, 0, 1));
		//this.GetComponent<Renderer>().material.shader.
	}

	private void SetShaderSpecularColor(Color c){
		//this.GetComponentInChildren<Renderer> ().material.SetColor ("_SpecColor", c);
		sphere.GetComponent<Renderer>().material.SetColor ("_SpecColor", c);
	}

	private void SetShaderColor(Color c){
		//this.GetComponentInChildren<Renderer> ().material.SetColor ("_Color", c);
		sphere.GetComponent<Renderer>().material.SetColor ("_Color", c);
	}

	/**
	 * Returns the remaining health in percent
	 */
	private float GetNormalizedHealth(){
		float normalizedHealth = health / startHealth;

		return normalizedHealth;
	}

	private void DeleteVisualRelic(){

		//Go through children
		foreach (Transform t in this.transform) {

			//Find name of relic
			if (t.name == "RelicSphere") {

				//Destroy the sphere
				Destroy (t);
			}
		}
	}

}
