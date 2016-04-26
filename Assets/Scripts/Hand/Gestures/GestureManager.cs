using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GestureManager : MonoBehaviour {

	private LeapManager leapManager;
	private int calibrationFrames = 60;
	private List<Vector3> calibratedDownFrame;
	[HideInInspector]
	public Vector3 calibratedDown = Vector3.down;

	[HideInInspector]
	public string activeSpell; //{ get; set; }
	public bool calibrateOnStart;

	// Use this for initialization
	void Start () {
		leapManager = this.gameObject.GetComponent<LeapManager> ();
		activeSpell = "none";

		calibratedDownFrame = new List<Vector3> ();

		if (calibrateOnStart)
		{
			Debug.Log ("Calibrating... ");
		}
	}
	
	// Update is called once per frame
	void Update () {

		if (calibrateOnStart)
		{
			Calibrate ();
		}
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

	private void Calibrate()
	{
		//If there are frames left, do calibration
		if (calibrationFrames > 0)
		{
			if (leapManager.GetPalmSmoothedVelocity () < 5f && leapManager.PalmNormalNear (Vector3.down, 0.75f)) {
				calibrationFrames--;

				setHandColor (Color.Lerp (Color.green, Color.grey, Mathf.Round ((Time.frameCount % 10f) / 10f)));

				//How many frames taken into account, in order to determine a smoothed vector
				int range = 25;

				//Add data
				calibratedDownFrame.Add (leapManager.GetPalmNormal ());

				//Clean up
				if (calibratedDownFrame.Count >= range) {
					calibratedDownFrame.RemoveAt (0);
				}

				calibratedDown = Vector3.zero;

				//Find velocity
				for (int i = 0; i < calibratedDownFrame.Count; i++) {
					calibratedDown.x += calibratedDownFrame [i].x;
					calibratedDown.y += calibratedDownFrame [i].y;
					calibratedDown.z += calibratedDownFrame [i].z;
				}

				calibratedDown.x /= calibratedDownFrame.Count;
				calibratedDown.y /= calibratedDownFrame.Count;
				calibratedDown.z /= calibratedDownFrame.Count;

				//If this was the last frame, close calibration
				if (calibrationFrames <= 0) {
					Debug.Log ("Calibration done! Down-vector is " + calibratedDown.ToString ());
					setHandColor (Color.grey);
				}
			}

			//If hand is removed from position, reset calibration
			else
			{
				calibrationFrames = 60;
				setHandColor (Color.grey);
			}
		}
	}
}
