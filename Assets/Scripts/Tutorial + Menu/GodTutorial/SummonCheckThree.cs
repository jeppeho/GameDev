using UnityEngine;
using System.Collections;

public class SummonCheckThree : MonoBehaviour {

    public SummonManager summonManager;
    public Material mat2;

    void Start()
    {
        GameObject f = GameObject.Find("LeapControllerBlockHand");
        summonManager = f.GetComponent<SummonManager>();
        summonManager.summonThree = false;
    }

    void OnCollisionEnter(Collision col)
    {

        if (col.gameObject.name == "intactObject7") // starts only when full crystal is summoned
        {
            Debug.Log("SummonSuccess2");
            summonManager.summonThree = true;
            GetComponentInParent<Renderer>().sharedMaterial = mat2;
        }
    }
}
