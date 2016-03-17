﻿using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {

	private Rigidbody rb;
	private MeshCollider collider;
	private float lastVelY;
	private enum state {active, dead, invulnerable};
	private state playerState;
	public float impactResistance = 3f;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
		collider = GetComponentInChildren<MeshCollider>();
	}

	// Update is called once per frame
	void FixedUpdate () {
		//Track falling velocity, so we have something to compare to, when landing on environment
		lastVelY = rb.velocity.y;
		//Check to see if player has fallen off grid
		if (transform.position.y <= -1f)
		{
			DeathFall ();
		}
	}

	//Check for impacts with environment
	void OnCollisionEnter(Collision col)
	{
		//Make a new impact velocity, by subtracting fall speed
		Vector3 impact = new Vector3 (col.relativeVelocity.x, col.relativeVelocity.y + lastVelY, col.relativeVelocity.z);
		if (col.gameObject.tag == "Environment" && impact.magnitude >= impactResistance)
		{
			DeathSquished();
		}
	}

	//Death by squishing (i.e. high relative y-velicity)
	private void DeathSquished()
	{
		Death();
	}

	//Death by falling (i.e. falling below ground)
	private void DeathFall()
	{
		Death();
	}

	//Death
	public void Death()
	{
		if (playerState == state.active)
		{
			Debug.Log ("Died!");
			playerState = state.dead;
			rb.freezeRotation = false;
			rb.AddTorque(new Vector3(Random.Range(0.2f,1),0,Random.Range(0.2f,1)));
			StartCoroutine(CountdownToRespawn(1f));
		}
	}

	//Respawning
	private void Respawn()
	{
		transform.position = new Vector3 (5, 0.5f, GameObject.Find ("Main Camera").transform.TransformPoint(Vector3.zero).z + 10);
		transform.rotation = new Quaternion ();
		GetComponent<Rigidbody> ().freezeRotation = true;
	}

	IEnumerator CountdownToRespawn(float time)
	{
		yield return new WaitForSeconds (time);
		playerState = state.invulnerable;
		StartCoroutine(FlashOnRespawn(1.5f));
		Respawn();
	}

	IEnumerator FlashOnRespawn(float time)
	{
		bool transparent = false;
		Renderer[] childRends = GetComponentsInChildren<Renderer> ();
		float subTime = 0;

		while (subTime < time) {
			transparent = !transparent;
			for (int i=0; i < childRends.Length; i++)
			{
				Color c = childRends [i].material.color;
				if (transparent)
				{	childRends [i].material.color = new Color(c.r, c.g, c.b, 0.2f);	}
				else
				{	childRends [i].material.color = new Color(c.r, c.g, c.b, 0.6f);	}
			}
			yield return new WaitForSeconds (0.1f);
			subTime += 0.1f;
		}

		//Reset colors
		for (int i=0; i < childRends.Length; i++)
		{
			Color c = childRends [i].material.color;
			childRends [i].material.color = new Color(c.r, c.g, c.b, 1f);
		}

		//And change state back
		playerState = state.active;
	}
}