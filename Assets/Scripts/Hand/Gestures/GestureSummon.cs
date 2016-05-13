using UnityEngine;
using System;
using System.Collections;


public class GestureSummon : Gesture {

	//Derived: leapManager
	//Derived: gestureManager
	//Derived: handManager
	//Derived: thisSpell
	private LevelGenerator levelGenerator;
	private Vector3 tempPalmWorldPosition;
	private Vector3 tempPalmPosition;
	public GameObject summonedObjectType;
	private GameObject shard;
	private GameObject[] subShard;
	private int blockCounter;

	private float origoY = 0;

	// Use this for initialization
	void Start () {
		base.Init ();
        
        try { levelGenerator = GameObject.Find("LevelGenerator").GetComponent<LevelGenerator>(); }
        catch(Exception e)
        { }

		thisSpell = "summon";

		subShard = new GameObject[7];

		origoY = 0;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (levelGenerator != null) {
			origoY = levelGenerator.GetLevelAreaHeights () [Mathf.Clamp (Mathf.FloorToInt (leapManager.GetPalmWorldPosition ().z), 0, 299)] * 1.25f;
		} else {
			origoY = 0f;
		}
		//----------------------------------
		// Initiate
		//----------------------------------

		if (
			gestureManager.noSpellActive()
			&& leapManager.PalmNormalNear (gestureManager.calibratedDown, 0.35f)
			&& leapManager.GetPalmSmoothedVelocity() <= 1.0f
			&& leapManager.PalmBetweenY (origoY + 2f, -10f)
			&& leapManager.GetFingerIsExtendedPattern (true, true, true, true, true)
			//&& manager.GetPalmVelocity().magnitude < 5f
		)
		{
			gestureManager.activeSpell = thisSpell;
			blockCounter = 1;
			tempPalmWorldPosition = leapManager.GetPalmWorldPosition ();
			tempPalmPosition = leapManager.GetPalmPosition ();

			gestureManager.setHandColor(Color.yellow);
            gestureManager.glowController.Burst(1f);

			audioManager.PlayLoop("summonLoop", handManager.audioplayerCasting);
		}

		//----------------------------------
		// Maintain
		//----------------------------------

		if (gestureManager.activeSpell.Equals(thisSpell))
		{
			if (
				leapManager.PalmNormalNear (gestureManager.calibratedDown, 0.45f)
			&& leapManager.GetFingerIsExtendedPattern (true, true, true, true, true)
			&& leapManager.PalmNearIgnore (tempPalmPosition, 5f, false, true, true)
			)
			{
                gestureManager.glowController.setIntensity(0.5f);

				float palmY = leapManager.GetPalmPosition ().y;
				float nextBlockY = origoY + 1.2f + blockCounter * 0.75f; //Insert something about relative height here!

				if (palmY > nextBlockY && blockCounter < 7)
				{
					//As the first block is being summoned, update palm position and summon frame-object
					if (blockCounter == 1)
					{ 
						tempPalmWorldPosition = leapManager.GetPalmWorldPosition ();
						shard = Instantiate (summonedObjectType, new Vector3 (tempPalmWorldPosition.x, origoY -0.6f, tempPalmWorldPosition.z + 1f), Quaternion.Euler (0, 0, 0)) as GameObject;
						ShatterIndexOnCollision shatterManager = shard.GetComponent<ShatterIndexOnCollision> ();

						//Index subshards
						foreach (Transform t in shard.transform) {

							//Find intact child, by checking begining of name
							if (t.name.StartsWith ("intactObject")) {
								//Read index, and add 00 as prefix, because bug-safely.
								string strIndex = t.name.Insert (12, "00").Substring (12, 3);
								int index = 1;

								if (strIndex != null) {
									index = int.Parse (strIndex);
								}

								subShard [index - 1] = t.gameObject;
							}

							subShard [0].GetComponent<Rigidbody>().AddTorque(new Vector3(0,-50,0),ForceMode.Impulse);
							subShard [0].GetComponent<Rigidbody>().AddForce(Vector3.up*0.1f);
						}

						//Animate shard
						//Rigidbody shardRb = shard.GetComponent<Rigidbody>();
						//shardRb.rotation = new Quaternion (0, 1, 0, 0);
					}

					//For all other blocks, just activate and deactivate appropriately
					else
					{

						//Dublicate rotation etc.
						Rigidbody oldRb = subShard [blockCounter-1].GetComponent<Rigidbody>();

						Quaternion rot = oldRb.rotation;
						Vector3 pos = oldRb.position;
						Vector3 angVel = oldRb.angularVelocity;

						//Deactivate and activate
						for (int i = 0; i < subShard.Length; i++)
						{
							subShard [i].SetActive (false);
						}

						subShard [blockCounter].SetActive(true);

						//Dublicate rotation etc.
						Rigidbody newRb = subShard [blockCounter].GetComponent<Rigidbody>();

						newRb.rotation = rot;
						newRb.position = pos;
						newRb.angularVelocity = angVel;

						//A a bit of torque
						subShard [blockCounter].GetComponent<Rigidbody>().AddTorque(new Vector3(0,-50,0),ForceMode.Impulse);
					}

					//Either way, count the block, and play a sound
					audioManager.Play("summonInstance", handManager.audioplayerCastingSupporting, blockCounter-1, blockCounter-1);
                    gestureManager.glowController.Burst(0.5f);

					blockCounter++;
				}
			}
			else
			{
				gestureManager.clearActiveSpell ();
                gestureManager.glowController.setIntensity(0);
				audioManager.Play("summonStop", handManager.audioplayerCasting);
			}
		}
	}
}
