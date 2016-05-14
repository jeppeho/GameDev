using UnityEngine;
using System.Collections;

public class TutorialCalibrationCheck : MonoBehaviour {

    void OnCollisionEnter(Collision col)
    {

        if (col.gameObject.layer == 8) // check if colides with hand
        {
            Debug.Log("hand is in calibration zone");
            //// -> if hand is within the box collider, then start calibration ! 

        }
    }
}
