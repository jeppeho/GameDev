using UnityEngine;
using System.Collections;

public class GestureManager : MonoBehaviour {

	private LeapManager leapManager;
	public enum spell {none, summon, hurricane};
	public spell activeSpell { get; set; }

	// Use this for initialization
	void Start () {
		activeSpell = spell.none;
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void setHandColor(Color c)
	{
		GameObject.Find ("HandController").GetComponent<StoneHandManager> ().handColor = c;
	}

	public void clearActiveSpell()
	{
		activeSpell = spell.none;
		setHandColor(Color.grey);
	}

	public bool noSpellActive()
	{
		return (activeSpell == spell.none);
	}
}
