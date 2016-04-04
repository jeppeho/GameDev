using UnityEngine;
using System.Collections;

public class GestureHurricane : Gesture {

	//Derived: leapManager
	//Derived: gestureManager
	//Derived: thisSpell
	private float charge;
	private float pulsebase;

	// Use this for initialization
	void Start () {
		base.Init ();
		thisSpell = GestureManager.spell.hurricane;
	}

	// Update is called once per frame
	void Update () {

		//----------------------------------
		// Initiate
		//----------------------------------

		if (
			gestureManager.noSpellActive()
			&& leapManager.PalmNormalNear (Vector3.down, 0.65f)
			&& leapManager.PalmBetweenY (3f, Mathf.Infinity)
			&& leapManager.GetFingerIsExtendedPattern (false, false, false, false, false)
			&& leapManager.GetHandGrabStrength() >= 0.8f
		)
		{
			gestureManager.activeSpell = thisSpell;
			Debug.Log ("Started huricaning");
			charge = 0;
		}
			
		//----------------------------------
		// Maintain
		//----------------------------------

		if (gestureManager.activeSpell == thisSpell)
		{
			if (
				leapManager.PalmNormalNear (Vector3.down, 0.85f)
				&& leapManager.PalmBetweenY (3f, Mathf.Infinity)
				&& leapManager.GetFingerIsExtendedPattern (false, false, false, false, false)
				&& leapManager.GetHandGrabStrength () >= 0.8f
				&& charge <1
				)
				{
					charge = Mathf.Min(1, charge+ 0.005f);
					pulsebase = (pulsebase + (charge *35 * Time.deltaTime)) % (2 * Mathf.PI);
					gestureManager.setHandColor (Color.Lerp (Color.grey, Color.white, Mathf.Sin(pulsebase)));
					
					//Create drag
					Vector3 center = leapManager.GetPalmWorldPosition () + new Vector3 (0, 2, 0);

					GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
					GameObject[] objects = GameObject.FindGameObjectsWithTag ("Environment");

					foreach (GameObject o in objects)
					{
						Rigidbody rb = o.GetComponent<Rigidbody> ();
						if (rb != null) 
						{
							//get pull direction (and distance to hand)
							Vector3 dir = center - rb.position;
							//Calculate drag, based on distance to hand (reverse proportional)
							float force = Mathf.Clamp (250f - dir.sqrMagnitude, 0, 250f) * charge * 3.0f;
							//Set magnitude to force
							dir = dir.normalized * force;
							//Rotate pull, to create hurricane effect
							dir = Quaternion.Euler (0, 30f, 0) * dir; 

							//Apply
							rb.AddForce (dir);
						}
					}
					
					foreach (GameObject o in players)
					{
						Rigidbody rb = o.GetComponent<Rigidbody> ();
						if (rb != null) 
						{
							//get pull direction (and distance to hand)
							Vector3 dir = center - rb.position;
							//Calculate drag, based on distance to hand (reverse proportional)
							float force = Mathf.Clamp (250f - dir.sqrMagnitude, 0, 250f) * charge * 0.005f;
							//Set magnitude to force
							dir = dir.normalized * force;
							//Rotate pull, to create hurricane effect
							dir = Quaternion.Euler (0, 30f, 0) * dir; 
							
							//Apply
							rb.AddForce (dir);
						}
					}
				}
			else
			{
				gestureManager.clearActiveSpell ();
				if (charge >= 0.5f)
				{
					Vector3 center = leapManager.GetPalmWorldPosition ();
					GameObject[] objects = GameObject.FindGameObjectsWithTag ("Environment");
					foreach (GameObject o in objects)
					{
						Rigidbody rb = o.GetComponent<Rigidbody> ();
						if (rb != null) 
						{
							rb.AddExplosionForce (250,center,100);
						}
					}

					//ExplodeHand
					GameObject.Find ("HandController").GetComponent<SphereHandModel> ().ExplodeHand(2,center,50);
				}
			}
		}
	}
}