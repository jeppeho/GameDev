﻿using UnityEngine;
using System.Collections;

public class RelicHealth : MonoBehaviour {

	private AudioManager audioManager;

	public float startHealth = 500;
	public float maxHandDistanceToTakeEnergy = 2f;
	private float health;
    private float damageBuffer;

	private Rigidbody rb;
	private RelicManager manager;
	private GameObject sphere;
	private PlayerScore ps;


	Color specularColor;
	Color baseColor;

	private GameObject handCore;
	private ParticleSystem trail;
	private ParticleSystem glow;
	private GameObject impactSparks;

	// Use this for initialization
	void Awake () {

		audioManager = GameObject.Find ("AudioManager").GetComponent<AudioManager> ();

		health = startHealth;
		rb = GetComponent<Rigidbody> ();
		manager = GetComponent<RelicManager> ();
		ps = this.gameObject.GetComponent<PlayerScore> ();

		sphere = GameObject.Find ("RelicSphere");

		specularColor = new Color (1f, 0.8f, 0.3f, 1f); // new Color (1, 0.5f, 0.5f);//
		baseColor = new Color (1f, 0.7f, 0.2f, 1f); // new Color (1, 0.5f, 0.5f);//

		//Get the core ball of the hand 
		handCore = GameObject.Find ("ball");
		trail = GameObject.Find ("RelicSphere/trailRelic").GetComponent<ParticleSystem>();
		glow = GameObject.Find ("RelicSphere/glowRelic").GetComponent<ParticleSystem>();

		impactSparks = GameObject.Find ("RelicSphere/impactSparks");
		impactSparks.SetActive (false);
	}


	void OnLevelWasLoaded(){

		//Reset health
		health = startHealth; 

		//Turn of impact sparks
		if (impactSparks != null) {
			impactSparks.SetActive (false);
		}
	}


	// Update is called once per frame
	void FixedUpdate () {

		SetShaderSpecularColor (specularColor * GetNormalizedHealth());
		SetShaderColor (GetBaseColorBasedOnHealth());

		SetTrail ();

        //Refill buffer
        damageBuffer = Mathf.Clamp(damageBuffer + 1f * Time.deltaTime, 0f, 1f);
	}

	/**
	 * Maps the trail and glow of the relic to the health
	 */
	private void SetTrail(){

		float rate = GetNormalizedHealth () * 100f;

		if (rate < 10f)
			rate = 10f;
		
		SetParticleSystemEmissionRate (trail, rate);
		SetParticleSystemEmissionRate (glow, rate);

		//Set max particles for the glow particle system
		glow.maxParticles = (int)rate;

	}

	/**
	 * Sets the emission rate of the parameter particleSystem, to the provided rate
	 */
	private void SetParticleSystemEmissionRate(ParticleSystem ps, float newRate){

		ParticleSystem.EmissionModule em = ps.emission;

		var rate = em.rate;

		rate.constantMax = newRate;
		rate.constantMin = newRate;

		em.rate = rate;
	}




	/**
	 * On collision drain energy, linearly to velocity between colliding objects
	 * Right now it doesn't matter if it has a parent.
	 */
	void OnCollisionEnter(Collision col){

		float relativeVelocity = col.relativeVelocity.magnitude;

        //////////

		//Only if relic collides with crystals
		if (col.gameObject.layer == 13) {

			//If relic is not carried
			if (!manager.HasParent ()) {

				//Only drain if above some threshold
				if (relativeVelocity > 3f) {

                    //Cap to 20
                    relativeVelocity = Mathf.Clamp(relativeVelocity, 0f, 20f);

                    //Drain
                    DrainEnergy(relativeVelocity);
				}

			//If relic is being carried
			} else {

				//Increase damage score for player
				ps.IncreaseDamage (Mathf.Clamp(relativeVelocity, 0, 30));

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

        //Apply buffer
        drain *= damageBuffer;

        //Round it off
        drain = Mathf.FloorToInt(drain);

        Debug.Log(">>> RELIC @ " + health.ToString("000") + "| Damaged by " + drain.ToString() + " pts., with a buffer of " + (damageBuffer * 100).ToString() + "%");

        //Update buffer, reducing by up to 50% on maximum damage
        damageBuffer -= drain / 40;

		//If enough hit, after applying buffer...
		if (drain > 1){
            
		    StartCoroutine (RunImpactSparks ());

		    //drain health
		    health -= drain;

			SetShaderSpecularColor (new Color (1, 1, 1));
			SetShaderColor (new Color (1, 1, 1));

			//~~SOUND~~

			//Play hit-sound
			audioManager.Play("relicDamage", Mathf.Clamp(drain/2f, 0f, 1f), this.gameObject);

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


	IEnumerator RunImpactSparks(){

		impactSparks.SetActive (true);

		yield return new WaitForSeconds (1f);

		impactSparks.SetActive (false);

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

				//Set the visual relic to inactive 
				t.gameObject.SetActive(false);
			}
		}
	}

}
