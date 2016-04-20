using UnityEngine;
using System.Collections;

public class GestureSummon : Gesture {

	//Derived: leapManager
	//Derived: gestureManager
	//Derived: thisSpell
	private Vector3 tempPalmWorldPosition;
	private Vector3 tempPalmPosition;
	public GameObject summonedObject;
	private int summonCounter;

	// Use this for initialization
	void Start () {
		base.Init ();
		Vector3 tempPalmWorldPosition = leapManager.GetPalmWorldPosition ();
		Vector3 tempPalmPosition = leapManager.GetPalmPosition ();
		thisSpell = GestureManager.spell.summon;
	}
	
	// Update is called once per frame
	void Update () {

		//----------------------------------
		// Initiate
		//----------------------------------

		if (
			gestureManager.noSpellActive()
			&& leapManager.PalmNormalNear (Vector3.down, 0.35f)
			&& leapManager.PalmBetweenY (-0.45f, -10f)
			&& leapManager.GetFingerIsExtendedPattern (true, true, true, true, true)
			//&& manager.GetPalmVelocity().magnitude < 5f
		)
		{
			gestureManager.activeSpell = thisSpell;
			summonCounter = 1;
			tempPalmWorldPosition = leapManager.GetPalmWorldPosition ();
			tempPalmPosition = leapManager.GetPalmPosition ();
			gestureManager.setHandColor(Color.yellow);
		}

		//----------------------------------
		// Maintain
		//----------------------------------

		if (gestureManager.activeSpell == thisSpell)
		{
			if (
				leapManager.PalmNormalNear (Vector3.down, 0.45f)
				&& leapManager.GetFingerIsExtendedPattern (true, true, true, true, true)
				&& leapManager.PalmNearIgnore (tempPalmPosition, 5f, false, true, true)) {
				float palmY = leapManager.GetPalmPosition ().y;
				float nextBoxY = -0.2f + summonCounter * 1.1f;

				if (palmY > nextBoxY + 0.5) {
					if (summonCounter == 1) //As the first block is being summoned, update palm position
					{	tempPalmWorldPosition = leapManager.GetPalmWorldPosition ();	}

					GameObject block = Instantiate (summonedObject, new Vector3 (tempPalmWorldPosition.x, nextBoxY, tempPalmWorldPosition.z), Quaternion.Euler (0, 0, 0)) as GameObject;
					summonCounter++;
				}
			}
			else
			{
				gestureManager.clearActiveSpell ();
			}
		}
	}
}
