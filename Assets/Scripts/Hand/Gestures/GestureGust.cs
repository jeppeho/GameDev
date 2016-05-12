using UnityEngine;
using System.Collections;

public class GestureGust : Gesture {

	//Derived: leapManager
	//Derived: gestureManager
	//Derived: handManager
	//Derived: thisSpell
	private float charge;
	private float pulsebase;
	private Vector3 gustCenter;
	private Vector3 palmWorldPosition;
	private float gustProgression;
	public float yThreshold = 5.0f;
	public bool released = false;

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
			&& leapManager.PalmBetweenY (yThreshold, Mathf.Infinity)
			&& leapManager.GetFingerIsExtendedPattern (false, false, false, false, false)
			&& leapManager.GetHandGrabStrength() >= 0.8f
		)
		{
			gestureManager.activeSpell = thisSpell;
			gustProgression = leapManager.GetPalmPosition ().y;
			gestureManager.setHandColor(Color.magenta);

			charge = 0;
			released = false;

			audioManager.PlayLoop("gustLoop", handManager.audioplayerCasting);
		}

		//----------------------------------
		// Maintain
		//----------------------------------

		float palmY = leapManager.GetPalmPosition ().y;

		palmWorldPosition = leapManager.GetPalmWorldPosition ();
		gustCenter = new Vector3 (palmWorldPosition.x, 5f, palmWorldPosition.z);

		if (gestureManager.activeSpell.Equals(thisSpell))
		{
			//Debug.Log ("Gust is active!");
			if (
				leapManager.PalmNormalNear (gestureManager.calibratedDown, 0.95f)
			&& leapManager.GetFingerIsExtendedPattern (false, false, false, false, false)
			&& leapManager.GetHandGrabStrength () >= 0.6f
			//&& leapManager.PalmNearIgnore (tempPalmWorldPosition, 7f, false, true, true)
			&& palmY < gustProgression + 1.5f
			)
			{

				//Handle charge
				charge = Mathf.Min(1, charge+ 0.0075f);
				pulsebase = (pulsebase + (charge *50f * Time.deltaTime)) % (2 * Mathf.PI);
				gestureManager.setHandColor (Color.Lerp (Color.grey, Color.magenta, Mathf.Sin(pulsebase)));

				//If the hand is actually moving downwards, make some wind
				if (palmY < gustProgression - 1.5f)
				{
					if (!released)
					{
						audioManager.Play("gustRelease", handManager.audioplayerCasting);
						released = true;
					}

					//Determine charge for this particular fixed update (NOTE - a different use of 'charge' than in GestureHurricane!)
					float pull = gustProgression - palmY;

					//Create drag
					float searchRadiusSqr = 280.0f;

					GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
					GameObject[] objects = GameObject.FindGameObjectsWithTag ("Interactables");
					GameObject[] intactObjects = GameObject.FindGameObjectsWithTag ("Shatterables");

					foreach (GameObject o in intactObjects)
					{
						ShatterOnCollision scrA = o.GetComponent<ShatterOnCollision> ();
						ShatterIndexOnCollision scrB = o.GetComponent<ShatterIndexOnCollision> ();

						if (scrA != null || scrB != null)
						{
							//get pull direction (and distance to hand)
							Vector3 dir = gustCenter - o.transform.position;

							if (dir.sqrMagnitude <= searchRadiusSqr-Random.Range(0f, 50f))
							{
								IntactObject scr2 = o.GetComponentInChildren<IntactObject> ();
								if (scr2 != null)
								{	scr2.SetHitvector (dir.normalized * 0.2f);		}

								if (scrA != null)
								{	scrA.ShatterObject ();	}
								else if (scrB != null)
								{	scrB.ShatterObject ();	}
							}
						}
					}
						
					foreach (GameObject o in objects)
					{
						Rigidbody rb = o.GetComponent<Rigidbody> ();
						if (rb != null) {

							//get pull direction (and distance to hand)
							Vector3 dir = gustCenter - rb.position;

							if (dir.sqrMagnitude <= searchRadiusSqr) { //Ignore if above threshold

								//Calculate drag, based on distance to hand (reverse proportional), and scale by totalForceMultiplier
								float force = Mathf.Clamp (searchRadiusSqr - dir.sqrMagnitude, 0f, searchRadiusSqr) * charge * pull * 6.0f;
								//Set magnitude to force
								dir = dir.normalized * force;
								//Rotate pull, to create hurricane effect
								dir = Quaternion.Euler (0f, 15f, 0f) * dir; 

								//Apply
								rb.AddForce (dir);
							}

						}
					}

					foreach (GameObject o in players)
					{
						Rigidbody rb = o.GetComponent<Rigidbody> ();
						if (rb != null) 
						{
							//get pull direction (and distance to hand)
							Vector3 dir = gustCenter - rb.position;
							//Calculate drag, based on distance to hand (reverse proportional)
							float force = Mathf.Clamp (searchRadiusSqr - dir.sqrMagnitude, 0, searchRadiusSqr) * pull * 0.001f;
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
				gustProgression = -10f; //Incapacitate gustProgression
				gestureManager.clearActiveSpell ();
				if (!released)
				{	audioManager.Play ("summonStop", handManager.audioplayerCasting);	}
			}
		}
	}
}
