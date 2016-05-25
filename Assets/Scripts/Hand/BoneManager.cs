using UnityEngine;
using System.Collections;

public class BoneManager : MonoBehaviour {

	//The actual position of this bone object
	private Vector3 position;
	//The position of the bone recorded by the LEAP (in Unity world-space), that the bone attempts to snap to
	[HideInInspector]
	public Vector3 targetPosition;
	//The rotation of the bone recorded by the LEAP (in Unity world-space), that the bone attempts to adhere to
	[HideInInspector]
	public Quaternion targetRotation;
	//The scale
	[HideInInspector]
	public float scale;
	//The maximum mass, applied when the bone becomes solid
	//[HideInInspector]
	public float boneMass = 1.5f;
	//The script controlling this bone
	private StoneHandManager modelController;
	//The audio manager
	AudioManager audioManager;

	//The speed which with the bone attempts to fly to its target position, when at full traction, but not yet in place; the actual speed is lerped between 0 and this
	//[HideInInspector]
	public float tractionSpeed = 650f;
	//The speed which with the bone can attempt to remain in its target position, when at full structure
	//[HideInInspector]
	public float snappedSpeed = 2500f;
	//The amount which with the traction rebuilds per update (i.e. how quickly the hand becomes whole again)
	//[HideInInspector]
	public float tractionRegeneration = 0.8f;
	//The amount that the traction can go below 0, leaving a deadzone of the bone not having traction
	//[HideInInspector]
	public float tractionBuffer = 0;
	//The threshold distance to its desired position the bone has to get in, before the magnetic algorith takes over (a more snappy one)
	//[HideInInspector]
	public float magneticThreshold = 1.8f;
	//By which factor the speed is multiplied, when doing magnetic snap at max speed; the larger, the more snap
	//[HideInInspector]
	public float magneticFactor = 2.2f;
	//The threshold (impact magnitude^2) below which the hand won't shatter
	//[HideInInspector]
	public float shatterThreshold = 1000f;
	//The factor by which all impact magnitudes are multiplied; the higher, the more easily the hand shatters
	public float shatterFactor = 0.0001f;

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
	[HideInInspector]
	public Color handColor;

	// Use this for initialization
	void Start () {

		targetRotation = new Quaternion (0, 0, 0, 1);
		modelController = GameObject.Find ("HandController").GetComponent<StoneHandManager> ();
		audioManager = GameObject.Find ("AudioManager").GetComponent<AudioManager> ();

		rb = gameObject.GetComponent<Rigidbody> ();
		material = GetComponent<Renderer> ().material;

		gameObject.transform.localScale = new Vector3(modelController.fingerBoneScale,modelController.fingerBoneScale,modelController.fingerBoneScale);
	}
	
	// Update is called once per frame
	void Update () {
	}
		
	// Update is called once per frame
	void FixedUpdate ()
	{
		if (rb == null)
		{
			gameObject.GetComponent<Rigidbody> (); //This is just a quick fix.. not sure why nessicary!
		}

		//Update position and scale
		position = rb.transform.position;

		//Update mass
		rb.mass = boneMass * structure;

		//Move towards hand - add the nessicary force towards desired position, based on traction, targetPosition etc.
		Vector3 newVelocity;
		Quaternion newRotation;

		//Calculate where to fly to, and how far there is
		Vector3 direction = targetPosition - position;

		//If the bone is knocked loose..
		if (structure < 1) {
			//How the bone's currently moving
			Vector3 oldVelocity = rb.velocity;
			Quaternion oldRotation = rb.rotation;

			//How the bone would move at full traction (until it gets under magneticThreshold that is)
			float currentSpeed = Mathf.Max (oldVelocity.magnitude, Mathf.Lerp (0, tractionSpeed, combinedTraction));
			newVelocity = direction * currentSpeed * Time.deltaTime;


			//If the bone is close enough to start snapping..
			if (direction.magnitude <= magneticThreshold) {

				//Create a revease formula that is equal to magneticThreshold when direction.magnitude is equal to magneticThreshold, but then rises by magneticFactor
				float targetMagnitude = (magneticThreshold * magneticFactor) + magneticThreshold - direction.magnitude * magneticFactor;

				//Scale the new velocity, so it applies the magnetic effect
				newVelocity.Scale (new Vector3 (targetMagnitude, targetMagnitude, targetMagnitude));
			}

			//Lerp towards the new velocity, as traction increases
			newVelocity = Vector3.Lerp (oldVelocity, newVelocity, combinedTraction);

			//Do the same with quaternion, but only when closing in on actual position
			//if (combinedTraction >= 0.75f)
			//{
			newRotation = Quaternion.Lerp (oldRotation, targetRotation, Mathf.Clamp (1 - direction.magnitude, 0, 1));
			//}
			//else
			//{
			//	newRotation = oldRotation;
			//}

			//Decline structure (structure declines as soon as it gets below 1)
			if (structure < 1) {
				structure = Mathf.Max (0, structure - 0.075f);
			}
		}
		//If structure is fine, try snapping instead
		else {
			newVelocity = direction * snappedSpeed * Time.deltaTime;

			//And rotate it into place
			newRotation = targetRotation;
		}
			
		//Either way, now check if the new velocity has been scaled too far, and surpasses the needed magnitude
		//if (newVelocity.magnitude > direction.magnitude)
		//{	newVelocity = direction;	}

		//Then apply the desired velocity
		rb.velocity = newVelocity;
		rb.rotation = newRotation;

		//Check if the bone is now close enough (and has traction enough) to regain structure, and become part of the hand again...
		if (structure < 1) {
			//Reset the structure
			if (direction.magnitude <= 0.1f)
			{
				structure = 1;
				localTraction = 1;
				if (Random.value > 0.5f)
				{	modelController.PlayClickback (gameObject);	}
			}
		}

		//Recharge localTraction
		if (localTraction < 1)
		{	localTraction = Mathf.Min(1, localTraction + tractionRegeneration * Time.deltaTime);	}

		UpdateCombinedTraction ();

		//Recolor
//		handColor = GameObject.Find("HandController").GetComponent<StoneHandManager>().handColor;
//		material.color = Color.Lerp (Color.grey, handColor, combinedTraction-0.5f);
	}
		
	void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.layer == 12) { //I.e. ObjectsKinematic
			//?
		}
		if (col.gameObject.layer == 13) { //I.e. ObjectsHeavy
			
			float customImpact;

			//Impact shattering the hand
			if (structure >= 1) { 
				customImpact = Mathf.Clamp ((col.relativeVelocity.sqrMagnitude - shatterThreshold) * shatterFactor, 0, 1+tractionBuffer);

				localTraction = 1-customImpact;
				modelController.addGlobalTraction (-customImpact*2);

				UpdateCombinedTraction ();
				//Begin to decline structure; part of the trick is that this takes a few 10th of a second, leaving the bones to do some subsequent impact
				structure -= 0.01f;

				//Handle audio
				PlayImpact(col.impulse.magnitude);
			}
			else //Regular impact
			{
				customImpact = 0;
				localTraction = 0;
			}

			//Kill some velocity
			rb.velocity.Scale(Vector3.zero);
		}
		if (col.gameObject.layer == 15)   //I.e. Event Object
		{
			Destroy(col.gameObject);
		}
	}
	
	private void PlayImpact(float magnitude)
	{
		//Handle audio
		if (magnitude > 5f)
		{
			audioManager.Play ("rockImpact", Mathf.Clamp((magnitude+20) / 200 + Random.Range(-0.1f, 0.1f),0,1), gameObject);
		}
    }

	private void UpdateCombinedTraction()
	{
		//Calculate a new combined traction, from local and global traction; unlike local and globalTraction combinedTraction is clamped
		combinedTraction = Mathf.Min(1, Mathf.Min (localTraction, modelController.globalTraction));

		//If globalTraction is knocked below 1, break structure
		if (modelController.globalTraction < 1) {
			structure = 0;
		}
	}
}