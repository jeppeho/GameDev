using UnityEngine;
using System.Collections;

public class LooseRockManager : MonoBehaviour {

	AudioManager audioManager;

	void Start()
	{
		audioManager = GameObject.Find ("AudioManager").GetComponent<AudioManager> ();
	}
//	void OnCollisionEnter(Collision col)
//	{
//		if (col.impulse.magnitude > 10f)
//		{
//			audioManager.Play ("rockImpact", Mathf.Clamp((col.impulse.magnitude+10) / 300 + Random.Range(-0.05f, 0.05f),0,1), gameObject);
//		}
//	}
}
