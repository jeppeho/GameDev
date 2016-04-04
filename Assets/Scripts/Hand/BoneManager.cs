using UnityEngine;
using System.Collections;

[RequireComponent (typeof(SphereHandModel))]

public class BoneManager : MonoBehaviour {

	//The actual position of this bone object
	private Vector3 position;
	//The position of the bone recorded by the LEAP (in Unity world-space), that the bone attempts to snap to
	public Vector3 targetPosition;
	//The scale
	public float scale;
	//The maximum mass, applied when the bone becomes solid
	public float boneMass;
	//The script controlling this bone
	private SphereHandModel modelController;

	//The speed which with the bone attempts to fly to its target position, when at full traction, but not yet in place
	public float tractionSpeed = 40; { get; public set; }
	//The speed which with the bone can attempt to remain in its target position, when at full structure
	public float snappedSpeed = 80;
	//The amount which with the traction rebuilds per update (i.e. how quickly the hand becomes whole again)
	public float tractionRegeneration = 0.25f; { get; public set; }
	//The amount that the traction can go below 0, leaving a deadzone of the bone not having traction
	public float tractionBuffer = 1;
	//The threshold distance to its desired position the bone has to get in, before the magnetic algorith takes over (a more snappy one)
	public float magneticThreshold = 2;
	//By which factor the speed is multiplied, when doing magnetic snap at max speed; the larger, the more snap
	public float magneticFactor = 3;

	//The force that snaps the bone back in place (traction) of this particular bone, which rebuilds over time; when below 0, it'll be treated as 0
	private float localTraction = 1; //...-1
	//The force that snaps the bone back in place (traction) that is acctually applied; it's a combination of the local traction and the hand's general integrity
	private float combinedTraction = 1; //...-1
	//The structural interity of the bone, controlling the mass and the bone's relationship with the hand; while bones have a structure of 1, they follow their targetPosision closely, and can affect objects
	private float structure = 1; //0-1

	//The bone's rigidbody
	private Rigidbody rb;
	//The bone's material
	private Material material;

	// Use this for initialization
	void Start () {

		modelController = GameObject.Find ("HandController").GetComponent<SphereHandModel> ();
		rb = gameObject.GetComponent<Rigidbody> ();
		material = GetComponent<Renderer> ().material;

		gameObject.transform.localScale = scale;

	}
	
	// Update is called once per frame
	void Update () {
	
	}
		
	// Update is called once per frame
	void FixedUpdate () {

		//Update position
		position = gameObject.GetComponent<Rigidbody> ().transform.position
		
		//Update mass
		rb.mass = boneMass * structure;

		//Move towards hand - add the nessicary force towards desired position, based on traction, targetPosition etc.
		Vector3 newVelocity;

		//Calculate where to fly to, and how far there is
		Vector3 direction = targetPosition - position;

		//If the bone is knocked loose..
		if (structure < 1)
		{
			//How the bone's currently moving
			Vector3 oldVelocity = rb.velocity;
			//How the bone would move at full traction (until it gets under magneticThreshold that is)
			Vector3 newVelocity = direction * tractionSpeed * Time.deltaTime;

			//If the bone is close enough to start snapping..
			if (direction.magnitude <= magneticThreshold) {

				//Create a revease formula that is equal to magneticThreshold when direction.magnitude is equal to magneticThreshold, but then rises by magneticFactor
				float targetMagnitude = (magneticThreshold*magneticFactor) + magneticThreshold - direction.magnitude*magneticFactor;

				//Scale the new velocity, so it applies the magnetic effect
				newVelocity.Scale (new Vector3 (targetMagnitude, targetMagnitude, targetMagnitude));
			}

			//Lerp towards the new velocity, as traction increases
			newVelocity = Vector3.Lerp (oldVelocity, newVelocity, combinedTraction);

			//If the bone is close enough (and has traction enough) to regain structure, and become part of the hand again...
			if (combinedTraction >= 0.99f && direction.magnitude <= 0.025f) {
				//It resets the structure
				structure = 1;
			}
		}

		//Otherwise, try to snap instantly
		else
		{
			newVelocity = direction * snappedSpeed * Time.deltaTime;
		}

		//Either way, apply the desired velocity
		rb.velocity = newVelocity;

		//Recharge traction
		if (localTraction < 1)
		{	localTraction = Mathf.Min(1, localTraction + tractionRegeneration/1000 * Time.deltaTime);	}

		//Calculate a new combined traction, from 
		combinedTraction = Mathf.Min (localTraction, modelController.globalTraction);
	}
		
	void OnCollisionEnter(Collision col)
	{
		//
		if (col.gameObject.layer == "Environment") {
			float customImpact;

			if (combinedTraction >= 1) { //Impact trying to get back to hand
				customImpact = Mathf.Clamp ((col.relativeVelocity.sqrMagnitude - 800f) / col.rigidbody.mass * 100, 0, 1);
				localTraction = 1-customImpact;
			}
			else //Regular impact
			{
				customImpact = 0;
				localTraction = 0;
			}

			Debug.Log (customImpact);
			modelController.addGlobalTraction(-customImpact / 5);
			//Kill some velocity
			rb.velocity.Scale(Vector3.zero);
			//Debug.Log(col.relativeVelocity.sqrMagnitude);
		}
	}
}