using UnityEngine;
using System.Collections;

public class LooseRockManager : MonoBehaviour {

	AudioManager audioManager;

	void Start()
	{
		audioManager = GameObject.Find ("AudioManager").GetComponent<AudioManager> ();
		gameObject.GetComponent<AudioSource> ().priority = 255;
	}
	void OnCollisionEnter(Collision col)
	{
		if (col.impulse.magnitude > 15f)
		{
			audioManager.Play ("rockImpact", Mathf.Clamp((col.impulse.magnitude+8) / 400 + Random.Range(-0.1f, 0.05f),0,1), gameObject);
		}
	}
}
