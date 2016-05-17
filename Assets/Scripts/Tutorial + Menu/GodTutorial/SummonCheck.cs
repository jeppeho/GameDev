﻿using UnityEngine;
using System.Collections;

public class SummonCheck : MonoBehaviour {

    public SummonManager summonManager;
    public Material mat2;


    void Start()
    {
        GameObject f = GameObject.Find("LeapControllerBlockHand");
        summonManager = f.GetComponent<SummonManager>();
        summonManager.summonOne = false;     
    }

    void OnCollisionEnter(Collision col)
    {

        if (col.gameObject.name == "intactObject7") // starts only when full crystal is summoned
        {
            Debug.Log("SummonSuccess1");
            summonManager.summonOne = true;
            GetComponentInParent<Renderer>().sharedMaterial = mat2;
        }
    }


}