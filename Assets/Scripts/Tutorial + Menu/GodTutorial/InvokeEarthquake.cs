using UnityEngine;
using System.Collections;

public class InvokeEarthquake : MonoBehaviour {

    private int breakForce = 0;

    void Update()
    {

        //Use L key to go to level  
        //if (Input.GetKey("l"))
        //    Invoke("ChangeLevel", 0.1f);

    }

    void OnCollisionEnter(Collision col)
    {

        //If collider layer is not Hand
        if (col.gameObject.layer != 10)
        {

            //If threshold force is used
            if (col.relativeVelocity.magnitude > breakForce)
            {

                float fadeTime = GameObject.Find("FadingScenes").GetComponent<FadingScenes>().BeginFade(1);
                Invoke("ChangeLevel", 3.0f);
            }
        }
    }


    void ChangeLevel()
    {
        Application.LoadLevel("TutorialGodEarthquake");
    }
}
