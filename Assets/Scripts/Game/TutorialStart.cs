using UnityEngine;
using System.Collections;

public class TutorialStart : MonoBehaviour {

    void OnCollisionEnter(Collision col)
    {

        if (col.gameObject.name == "intactObject7") // starts only when full crystal is summoned
        {
            Debug.Log("it wooorkssss");
            float fadeTime = GameObject.Find("FadingScenes").GetComponent<FadingScenes>().BeginFade(1);
            Invoke("ChangeLevel", 3.0f);
        }
    }


    void ChangeLevel()
    {
        Application.LoadLevel("LevelGenerator");
    }
}
