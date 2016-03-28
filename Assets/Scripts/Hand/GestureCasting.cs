using UnityEngine;
using System.Collections;

public class GestureCasting : MonoBehaviour {

	private LeapVariables manager;
	private enum spell {none, summon, blast};
	private spell activeSpell;
	private Vector3 tempPalmPosition;

	public GameObject summonedObject;

	private int summonCounter;
	private float blastCharge;

	// Use this for initialization
	void Start () {
		manager = this.gameObject.GetComponent<LeapVariables> ();
		activeSpell = spell.none;
		Vector3 tempPalmPosition = manager.GetPalmPosition ();
	}
	
	// Update is called once per frame
	void Update () {

		manager.DebugVariables ();

		//----------------------------------
		// Listen for new gestures
		//----------------------------------

		//Summon blocks (initiate)
		if (
			activeSpell == spell.none
			&& manager.PalmNormalNear (Vector3.down, 0.2f)
			&& manager.PalmBetweenY (-0.4f, -1.5f)
			&& manager.GetFingerPatternIsExtended (true, true, true, true, true)
			//&& manager.GetPalmVelocity().magnitude < 5f
		    )
		{
			activeSpell = spell.summon;
			summonCounter = 1;
			tempPalmPosition = manager.GetPalmPosition();
			Debug.Log("Started summoning!");
			setColor(Color.cyan);
		}

		//Blast (initiate)
		if (
			activeSpell == spell.none
			&& manager.PalmNormalNear (Vector3.down, 0.65f)
			&& manager.PalmBetweenY (0f, Mathf.Infinity)
			&& manager.GetFingerPatternIsExtended (false, false, false, false, false)
			&& manager.GetHandGrabStrength() >= 0.8
			)
		{
			activeSpell = spell.blast;
			blastCharge = 0;
			tempPalmPosition = manager.GetPalmPosition();
			Debug.Log("Started blasting!");
			setColor(Color.red);
		}

		//----------------------------------
		// Maintain active gestures
		//----------------------------------
		
		//Summon blocks
		if (activeSpell == spell.summon) //SPELL
			if (
				manager.PalmNormalNear (Vector3.down, 0.35f)
				&& manager.GetFingerPatternIsExtended (true, true, true, true, true)
				&& manager.PalmNearIgnore(tempPalmPosition, 2f, false, true, true)
			)
			{
					float palmY = manager.GetPalmPosition().y;
					float nextBoxY = -0.2f + summonCounter*1.1f;

				if (palmY > nextBoxY+0.5)
				{
				Vector3 summonPos = tempPalmPosition + GetOffsetFromParent();
					GameObject block = Instantiate(summonedObject, new Vector3(summonPos.x, nextBoxY, summonPos.z+0.5f-(0.1f*summonCounter)), Quaternion.Euler(0,0,0)) as GameObject;
					//block.GetComponent<Rigidbody>().MovePosition(new Vector3(tempPalmPosition.x, nextBoxY, tempPalmPosition.z));
					summonCounter++;
				}
			}
			else
			{
				activeSpell = spell.none;
				Debug.Log("Stopped summoning!");
				setColor(Color.grey);
			}

		//Blast
		if (activeSpell == spell.blast) //SPELL
			if (
				//manager.PalmNormalNear (Vector3.down, 0.65f)
				manager.GetFingerPatternIsExtended (false, false, false, false, false)
			    && manager.GetHandGrabStrength() >= 0.8
				//&& manager.PalmNear(tempPalmPosition, 1.0f)
				)
		{
			//Blast maintained
			blastCharge = Mathf.Min (blastCharge+0.025f, 1);
			setColor(Color.Lerp(Color.grey, Color.red, blastCharge+0.1f));
		}
		else
		{
			//Blast released, by releasing grab but not moving/tilting hand
			if (
				blastCharge >= 0.66f
				//&& manager.GetHandGrabStrength() <= 0.2
				)
			{
					GameObject[] objects = GameObject.FindGameObjectsWithTag("Environment");
					foreach (GameObject o in objects)
					{
						Rigidbody rb = o.GetComponent<Rigidbody>();
						if (rb != null)
					{	rb.AddExplosionForce(6000f * blastCharge, manager.GetPalmPosition()+GetOffsetFromParent(), 100f);	}
					}
			}
			activeSpell = spell.none;
			Debug.Log("Stopped summoning!");
			setColor(Color.grey);
		}
	}

	private void setColor(Color c)
	{
		GameObject.Find ("HandController").GetComponent<SphereHandModel> ().handColor = c;
	}

	private Vector3 GetOffsetFromParent(){
		return gameObject.GetComponentInParent<Transform> ().transform.position;
	}
}
