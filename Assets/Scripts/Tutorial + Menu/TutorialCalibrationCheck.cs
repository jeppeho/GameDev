using UnityEngine;
using System.Collections;

public class TutorialCalibrationCheck : MonoBehaviour {

        public GestureManager gestureManager;
        public CalibrationManager calibrationManager;

    void Start()
    {
        
        GameObject f = GameObject.Find("HandController");
        gestureManager = f.GetComponent<GestureManager> ();
        GameObject g = GameObject.Find("LeapControllerBlockHand");
        calibrationManager = g.GetComponent<CalibrationManager>();

    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.layer == 8) // check if colides with hand
        {
            //// -> if hand is within the box collider, then start calibration ! 
            gestureManager.calibrationTriggered = true;
               
        }
    }


    
}
