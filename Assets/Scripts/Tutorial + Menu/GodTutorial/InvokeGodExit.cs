using UnityEngine;
using System.Collections;

public class InvokeGodExit : MonoBehaviour
{

    public Material mat2;

    void OnCollisionEnter(Collision col)
    {

        if (col.gameObject.name == "intactObject7") // starts only when full crystal is summoned
        {
            Debug.Log("it wooorkssss");
            float fadeTime = GameObject.Find("FadingScenes").GetComponent<FadingScenes>().BeginFade(1);
            GetComponentInParent<Renderer>().sharedMaterial = mat2;
            Invoke("ChangeLevel", 3.0f);
        }
    }


    void ChangeLevel()

    {
        Application.Quit();

    }
}