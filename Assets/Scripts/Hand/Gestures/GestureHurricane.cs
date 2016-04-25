using UnityEngine;
using System.Collections;

public class GestureHurricane : Gesture {

	//Derived: leapManager
	//Derived: gestureManager
	//Derived: thisSpell
	private float charge;
	private float pulsebase;
	private float totalForce = 0;
	private float totalForceMultiplier = 1f;
	private float totalForceLimit = 5000f;
	// Use this for initialization
	void Start () {
		base.Init ();
		thisSpell = "hurricane";
		Debug.Log ("Set thisSpell: " + thisSpell.ToString());
	}

	// Update is called once per frame
	void FixedUpdate () {

		//----------------------------------
		// Initiate
		//----------------------------------

		if (
			gestureManager.noSpellActive()
			&& leapManager.PalmNormalNear (Vector3.down, 0.75f)
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

		if (gestureManager.activeSpell.Equals(thisSpell))
		{
			//Debug.Log ("Hurricane is active!");
			if (
			leapManager.PalmNormalNear (Vector3.down, 0.95f)
			&& leapManager.PalmBetweenY (3f, Mathf.Infinity)
			&& leapManager.GetFingerIsExtendedPattern (false, false, false, false, false)
			&& leapManager.GetHandGrabStrength () >= 0.8f
			//&& charge <1
			)
			{
				charge = Mathf.Min(1, charge+ 0.006f);
				pulsebase = (pulsebase + (charge *35 * Time.deltaTime)) % (2 * Mathf.PI);
				gestureManager.setHandColor (Color.Lerp (Color.grey, Color.blue, Mathf.Sin(pulsebase)));
					
				//Create drag
				totalForce *= 0.5f;
				float searchRadiusSqr = 200f * Mathf.Pow(totalForceMultiplier, 0.5f);
				Vector3 center = leapManager.GetPalmWorldPosition () + new Vector3 (0, 2, 0);

				GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
				GameObject[] objects = GameObject.FindGameObjectsWithTag ("Environment");

				foreach (GameObject o in objects)
				{
					Rigidbody rb = o.GetComponent<Rigidbody> ();
					if (rb != null) {
						
						//get pull direction (and distance to hand)
						Vector3 dir = center - rb.position;

						if (dir.sqrMagnitude <= searchRadiusSqr) { //Ignore if above threshold
							
							//Calculate drag, based on distance to hand (reverse proportional), and scale by totalForceMultiplier
							float force = Mathf.Clamp (searchRadiusSqr - dir.sqrMagnitude, 0, searchRadiusSqr) * charge * 3.0f;
							//Set magnitude to force
							dir = dir.normalized * force * totalForceMultiplier;
							//Rotate pull, to create hurricane effect
							dir = Quaternion.Euler (0, 12f, 0) * dir; 

							//Apply
							rb.AddForce (dir);

							//Add force to totalForce, calculating a limit for next update's force-pulls
							totalForce += force;
						}
					}
				}
					
				totalForceMultiplier = Mathf.Clamp( (totalForceLimit - totalForce)/(totalForceLimit*0.5f) , 0f , 1f );
				Debug.Log (totalForceMultiplier.ToString());

				foreach (GameObject o in players)
				{
					Rigidbody rb = o.GetComponent<Rigidbody> ();
					if (rb != null) 
					{
						//get pull direction (and distance to hand)
						Vector3 dir = center - rb.position;
						//Calculate drag, based on distance to hand (reverse proportional)
						float force = Mathf.Clamp (searchRadiusSqr - dir.sqrMagnitude, 0, searchRadiusSqr) * charge * 0.001f;
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
					//GameObject.Find ("HandController").GetComponent<SphereHandModel> ().ExplodeHand(2,center,50);
				}
			}
		}
	}
}