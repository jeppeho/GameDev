using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GestureManager : MonoBehaviour {

	private LeapManager leapManager;
	public CalibrationManager calibrationManager;
    public CalibrationManagerNew calibrationManagerNew;
    private int calibrationFrames = 60;
	private List<Vector3> calibratedDownFrame;
	[HideInInspector]
	public Vector3 calibratedDown = Vector3.down;

	[HideInInspector]
	public string activeSpell; //{ get; set; }
    [HideInInspector]
    public GlowControl glowController;

	public bool calibrateOnStart;
    public bool calibrationTriggered; // check if hand is within calibration area

	// Use this for initialization
	void Start () {
		leapManager = this.gameObject.GetComponent<LeapManager> ();
        GameObject g = GameObject.Find("LeapControllerBlockHand");
        calibrationManagerNew = g.GetComponent<CalibrationManagerNew>();
		calibrationManager = g.GetComponent<CalibrationManager>();
        glowController = GameObject.Find("StoneHandModel 1").GetComponentInChildren<GlowControl>();
        //Debug.Log("Found hand + glow");
		activeSpell = "none";

		calibratedDownFrame = new List<Vector3> ();
		calibratedDown = new Vector3 (0f, -1f, -0.1f); //Default down-vector

        calibrateOnStart = false;
        calibrationTriggered = false;

		if (calibrateOnStart)
		{
			Debug.Log ("Calibrating... ");
		}
	}
	
	// Update is called once per frame
	void Update () {

		if (calibrationTriggered) // only start calibration when hand is inside the calibration area
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
        return (activeSpell.Equals("none") || activeSpell == null);

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
					//Debug.Log ("Calibration done! Down-vector is " + calibratedDown.ToString ());
                    // tell tutorial manager that calibration is done, and only than move forward to menu
                    calibrationManagerNew.calibrationDone = true;
					calibrationManager.calibrationDone = true;
                    setHandColor(Color.grey);
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
