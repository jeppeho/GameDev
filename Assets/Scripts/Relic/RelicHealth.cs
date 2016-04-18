using UnityEngine;
using System.Collections;

public class RelicHealth : MonoBehaviour {

	public float startHealth = 1000;
	private float health;

	private Rigidbody rb;
	private RelicManager manager;

	// Use this for initialization
	void Start () {
		health = startHealth;
		rb = GetComponent<Rigidbody> ();
		manager = GetComponent<RelicManager> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {



		if (!manager.HasParent ())
			DrainEnergy (1);

		//UpdateColor ();
	}




	/**
	 * On collision drain energy, linearly to velocity between colliding objects
	 * Right now it doesn't matter if it has a parent.
	 */
	void OnCollisionEnter(Collision col){
		
		float relativeVelocity = col.relativeVelocity.magnitude;

		if (!manager.HasParent ()) {

			//Only drain if above some threshold
			if (relativeVelocity > 3f) {
				float drain = Mathf.FloorToInt (relativeVelocity * 2);

				//Limit max energy drain
				if (drain > 30f)
					drain = 30f;

				DrainEnergy (drain);
			}
		} else {
			if (relativeVelocity > 20) {

				//Remove from parent
				manager.RemoveParent ();
			}
		}
	}

	/**
	 * Drain energy and update the color
	 */
	public void DrainEnergy(float drain){
		health -= drain;

		//UpdateColor ();
		if(drain > 1)
			UpdateColor ();
		else if (Time.frameCount % 30 == 0)
			this.GetComponent<Renderer> ().material.color = new Color (1, 1, 1);
		else
			UpdateColor ();
			
		Debug.Log ("Draining energy with drain = " + drain);

//		if (drain > 1) {
//			//Flash when draining energy
//
//			this.GetComponent<Renderer> ().material.color =  new Color(1, 1, 1);
//		} else {
//			UpdateColor ();
//		}

		if (health < 0) {
			manager.RemoveParent ();
			DeleteObject ();
		}
	}


	/**
	 * Updates the color on the basis of how much health is left
	 */
	private void UpdateColor(){
		this.GetComponent<Renderer> ().material.color =  new Color(1, GetNormalizedHealth(), GetNormalizedHealth());
		this.GetComponent<Renderer> ().material.color =  new Color(GetNormalizedHealth(), 0, 0);
	}

	/**
	 * Returns the remaining health in percent
	 */
	private float GetNormalizedHealth(){
		float normalizedHealth = health / startHealth;

		return normalizedHealth;
	}

	private void DeleteObject(){
		Destroy (this.gameObject);
	}
}
