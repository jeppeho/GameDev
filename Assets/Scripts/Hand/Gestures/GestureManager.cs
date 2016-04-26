using UnityEngine;
using System.Collections;

public class GestureManager : MonoBehaviour {

	private LeapManager leapManager;
	public string activeSpell; //{ get; set; }

	// Use this for initialization
	void Start () {
		activeSpell = "none";
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log("activeSpell is " + activeSpell.ToString());
	}

	public void setHandColor(Color c)
	{
		GameObject.Find ("HandController").GetComponent<StoneHandManager> ().handColor = c;
	}

	public void clearActiveSpell()
	{
		activeSpell = "none";
		setHandColor(Color.grey);
	}

	public bool noSpellActive()
	{
		return (activeSpell.Equals("none"));
	}

	public string getActiveSpell()
	{
		return activeSpell;
	}

	public void setActiveSpell(string s)
	{
		activeSpell = s;
	}
}
