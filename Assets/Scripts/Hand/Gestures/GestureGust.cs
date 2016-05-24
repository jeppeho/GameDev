using UnityEngine;
using System.Collections;

public class GestureGust : Gesture {

	//Derived: leapManager
	//Derived: gestureManager
	//Derived: handManager
	//Derived: thisSpell
	Light levelLight;
	float levelLightBaseIntensity;
	float lightflicker;
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

		levelLight = GameObject.Find ("LevelLight").GetComponent<Light> ();
		levelLightBaseIntensity = levelLight.intensity;

		thisSpell = "gust";
		//Debug.Log ("Set thisSpell: " + thisSpell.ToString());
	}

    public float GetCharge()
    {
        return charge;
    }

	// Update is called once per frame
	void FixedUpdate () {

		//----------------------------------
		// Initiate
		//----------------------------------

		if (
			gestureManager.noSpellActive()
            && leapManager.PalmNormalNearIgnore(gestureManager.calibratedDown, 0.65f, false, true, false)
			&& leapManager.PalmBetweenY (yThreshold, Mathf.Infinity)
			&& leapManager.GetFingerIsExtendedPattern (false, false, false, false, false)
			&& leapManager.GetHandGrabStrength() >= 0.8f
		)
		{
			gestureManager.activeSpell = thisSpell;
			gustProgression = leapManager.GetPalmPosition ().y;
            gestureManager.setHandColor(Color.yellow);
            gestureManager.glowController.Burst(1f);

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
				leapManager.PalmNormalNearIgnore (gestureManager.calibratedDown, 0.85f, false, true, false)
			&& leapManager.GetFingerIsExtendedPattern (false, false, false, false, false)
			&& leapManager.GetHandGrabStrength () >= 0.6f
			//&& leapManager.PalmNearIgnore (tempPalmWorldPosition, 7f, false, true, true)
			&& palmY < gustProgression + 1.5f
			)
			{
                if (charge < 0.1f)
                {
                    gestureManager.glowController.setIntensity(1f - charge*10f);
                }
                else
                {
                    gestureManager.glowController.setIntensity(charge);
                }
                gestureManager.glowController.Flicker(charge);

				//Handle charge
				charge = Mathf.Min(1, charge+ 0.0075f);
				pulsebase = (pulsebase + (charge *20f * Time.deltaTime)) % (2 * Mathf.PI);
                gestureManager.setHandColor(Color.Lerp(Color.white, Color.yellow, Mathf.Sin(pulsebase)));

				//If the hand is actually moving downwards, make some wind
				if (palmY < gustProgression - 1.5f)
				{
					//Flash lights
					FlashLights(charge);

					if (!released)
					{
						audioManager.Stop(handManager.audioplayerCasting);
						audioManager.Play("gustRelease", handManager.audioplayerCastingGust);
						released = true;
					}

                    gestureManager.glowController.setIntensity(0);
                    gestureManager.glowController.Burst(0.5f);


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

				levelLight.intensity = levelLightBaseIntensity;
                gestureManager.glowController.setIntensity(0);

				if (!released)
				{	audioManager.Play ("summonStop", handManager.audioplayerCasting);	}
			}
		}


		//Handle flickering

		if (lightflicker > 0)
		{
			lightflicker -= 0.05f;

			//Flash the lights!
			levelLight.intensity = levelLightBaseIntensity + Random.Range (-0.4f, 2f) * lightflicker;
		}
		else
		{
			levelLight.intensity = levelLightBaseIntensity;
		}
	}

	private void FlashLights(float n)
	{
		lightflicker = n;
	}
}
