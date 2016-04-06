using UnityEngine;
using System.Collections;

public class CoreManager : MonoBehaviour {

	//The actual position of the core
	private Vector3 position;
	//The position of the core recorded by the LEAP (in Unity world-space), that the core attempts to snap to
	[HideInInspector]
	public Vector3 targetPosition;

	//The script controlling this bone
	private StoneHandManager modelController;

	//The core's rigidbody
	private Rigidbody rb;

	//The speed which with the core can attempt to remain in its target position, when at full structure (which is always for the core)
	[HideInInspector]
	public float snappedSpeed = 2500f;

	void Start()
	{
		modelController = GameObject.Find ("HandController").GetComponent<StoneHandManager> ();
		rb = gameObject.GetComponent<Rigidbody> ();
	}

	void FixedUpdate () {

		//Update position
		position = gameObject.GetComponent<Rigidbody> ().transform.position;

		//Calculate where to fly to, and how far there is
		Vector3 direction = targetPosition - position;

		//Then apply
		Vector3 newVelocity = direction * snappedSpeed * Time.deltaTime;
		rb.velocity = newVelocity;
	}
}
