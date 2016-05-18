using UnityEngine;
using System.Collections;

public class UIcalibration : MonoBehaviour {

	GameObject leap;
	CalibrationManagerNew c;

	// Use this for initialization
	void Start () {
	
		leap = GameObject.Find ("LeapControllerBlockHand");
		c = leap.GetComponent<CalibrationManagerNew> ();

	}
	
	// Update is called once per frame
	void Update () {
		if (c.calibrationDone) {
			this.gameObject.SetActive (false);
		}

	}
}
