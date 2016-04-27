using UnityEngine;
using System.Collections;

public class GestureGust_Legacy : Gesture {

	//Derived: leapManager
	//Derived: gestureManager
	//Derived: thisSpell
	private Vector3 tempGustCenter;
	private Vector3 tempPalmWorldPosition;
	private float gustProgression;

	// Use this for initialization
	void Start () {
		base.Init ();
		thisSpell = "gust";
		Debug.Log ("Set thisSpell: " + thisSpell.ToString());
	}

	// Update is called once per frame
	void FixedUpdate () {

		//----------------------------------
		// Initiate
		//----------------------------------

		if (
			gestureManager.noSpellActive()
			&& leapManager.PalmNormalNear (gestureManager.calibratedDown, 0.75f)
			&& leapManager.PalmBetweenY (4.75f, Mathf.Infinity)
			&& leapManager.GetFingerIsExtendedPattern (false, false, false, false, false)
			&& leapManager.GetHandGrabStrength() >= 0.8f
		)
		{
			gestureManager.activeSpell = thisSpell;
			tempPalmWorldPosition = leapManager.GetPalmWorldPosition ();

			gustProgression = leapManager.GetPalmPosition ().y;

			tempGustCenter = new Vector3 (tempPalmWorldPosition.x, 3f, tempPalmWorldPosition.z);
			gestureManager.setHandColor(Color.magenta);
		}

		//----------------------------------
		// Maintain
		//----------------------------------

		float palmY = leapManager.GetPalmPosition ().y;

		if (gestureManager.activeSpell.Equals(thisSpell))
		{
			//Debug.Log ("Gust is active!");
			if (
				leapManager.PalmNormalNear (gestureManager.calibratedDown, 0.95f)
				&& leapManager.GetFingerIsExtendedPattern (false, false, false, false, false)
				&& leapManager.GetHandGrabStrength () >= 0.8f
				&& leapManager.PalmNearIgnore (tempPalmWorldPosition, 7f, false, true, true)
				&& palmY < gustProgression + 0.5f
			)
			{
				//If the hand is actually moving downwards, make some wind
				if (palmY < gustProgression)
				{
					//Determine charge for this particular fixed update (NOTE - a different use of 'charge' than in GestureHurricane!)
					float charge = gustProgression - palmY;

					//Create drag
					float searchRadiusSqr = 250.0f;

					GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
					GameObject[] objects = GameObject.FindGameObjectsWithTag ("Interactables");

					foreach (GameObject o in objects)
					{
						Rigidbody rb = o.GetComponent<Rigidbody> ();
						if (rb != null) {

							//get pull direction (and distance to hand)
							Vector3 dir = tempGustCenter - rb.position;

							if (dir.sqrMagnitude <= searchRadiusSqr) { //Ignore if above threshold

								//Calculate drag, based on distance to hand (reverse proportional), and scale by totalForceMultiplier
								float force = Mathf.Clamp (searchRadiusSqr - dir.sqrMagnitude, 0f, searchRadiusSqr) * charge * 4.0f;
								//Set magnitude to force
								dir = dir.normalized * force;
								//Rotate pull, to create hurricane effect
								dir = Quaternion.Euler (0f, 15f, 0f) * dir; 

								//Apply
								rb.AddForce (dir);

								Debug.Log ("Applied force " + force.ToString("00.0000") + " from charge " + charge.ToString());
							}

						}
					}

					foreach (GameObject o in players)
					{
						Rigidbody rb = o.GetComponent<Rigidbody> ();
						if (rb != null) 
						{
							//get pull direction (and distance to hand)
							Vector3 dir = tempGustCenter - rb.position;
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

					gustProgression = palmY;
				}
			}

			else
			{
				gestureManager.clearActiveSpell ();
			}
		}
	}
}
