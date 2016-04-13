using UnityEngine;
using System.Collections;

public class RelicHealth : MonoBehaviour {

	public float startHealth = 1000;
	private float health;

	private Rigidbody rb;

	// Use this for initialization
	void Start () {
		health = startHealth;
		rb = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		Debug.Log ("GetNormalizedHealth() = " + GetNormalizedHealth ());

		if (!HasParent ())
			DrainEnergy (1);

		UpdateColor ();
	}



	/**
	 * If game object has a parent return true
	 * else return false
	 */
	private bool HasParent(){
		return transform.parent;
	}

	/**
	 * On collision drain energy, linearly to velocity between colliding objects
	 * Right now it doesn't matter if it has a parent.
	 */
	void OnCollisionEnter(Collision col){
		
		float relicForce = rb.velocity.magnitude;

		float drain = Mathf.FloorToInt (col.relativeVelocity.magnitude * 2);

		//Limit max energy drain
		if (drain > 30f)
			drain = 30f;

		DrainEnergy (drain);
	}

	/**
	 * Drain energy
	 */
	private void DrainEnergy(float drain){
		health -= drain;
		if(health % 100 == 0)
			Debug.Log("Health = " + health);

		if (health < 0) {
			DeleteObject ();
		}
	}

	private void UpdateColor(){



		this.GetComponent<Renderer> ().material.color =  new Color(1, GetNormalizedHealth(), GetNormalizedHealth());
	}

	/**
	 * Returns the remaining health in percent
	 */
	private float GetNormalizedHealth(){
		float normalizedHealth = health / startHealth;

		return normalizedHealth;
	}

	private void DeleteObject(){
		Debug.Log ("DESTROYING");
		Destroy (this.gameObject);
	}
}
