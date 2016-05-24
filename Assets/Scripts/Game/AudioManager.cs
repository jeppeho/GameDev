using UnityEngine;
using System;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour {

	private static AudioClip handMoveLoop;
	private static AudioClip summonInit;
	private static AudioClip summonLoop;
	private static AudioClip summonStop;
	private static AudioClip gustLoop;
	private static AudioClip gustRelease;
	private static AudioClip deathN;

	private static List<AudioClip> summonInstance = new List<AudioClip>();
	private static List<AudioClip> rockImpact = new List<AudioClip>();
	private static List<AudioClip> rockShatter = new List<AudioClip>();
	private static List<AudioClip> rockClick = new List<AudioClip>();
	private static List<AudioClip> relicCatch = new List<AudioClip>();
	private static List<AudioClip> relicThrow = new List<AudioClip> ();
	private static List<AudioClip> relicDamage = new List<AudioClip> ();
	private static List<AudioClip> footstepsFast = new List<AudioClip>();
	private static List<AudioClip> footstepsSlow = new List<AudioClip>();

	private static List<AudioClip> jumpM = new List<AudioClip>();
	private static List<AudioClip> landM = new List<AudioClip>();
	private static List<AudioClip> deathM = new List<AudioClip>();

	private static List<AudioClip> jumpF = new List<AudioClip>();
	private static List<AudioClip> landF = new List<AudioClip>();
	private static List<AudioClip> deathF = new List<AudioClip>();

	[HideInInspector]
	public GameObject audioplayerCasting;

	void Awake()
	{
		//Load all sounds from resources
		string path = "Audio/Godhand/";
		handMoveLoop = Resources.Load(path+"handMoveLoop") as AudioClip;
		summonLoop = Resources.Load(path+"summonLoop") as AudioClip;
		summonStop = Resources.Load(path+"gestureStop") as AudioClip;
		gustLoop = Resources.Load(path+"gustLoop") as AudioClip;
		gustRelease = Resources.Load(path+"gustRelease") as AudioClip;

		summonInstance = LoadSet(path+"summonInstance", 7);
		rockClick = LoadSet(path+"rockClick", 8);

		path = "Audio/Environment/";
		rockImpact = LoadSet(path+"rockImpact", 12);
		rockShatter = LoadSet(path+"rockShatter", 6);

		path = "Audio/Relic/";
		relicCatch = LoadSet(path+"relicCatch", 8);
		relicThrow = LoadSet(path+"relicThrow", 8);
		relicDamage = LoadSet(path+"relicDamage", 8);

		path = "Audio/Players/";
		footstepsFast = LoadSet(path+"footstepsFast", 4);
		footstepsSlow = LoadSet(path+"footstepsSlow", 4);
		jumpM = LoadSet(path+"jumpM", 6);
		landM = LoadSet(path+"landM", 6);
		deathM = LoadSet(path+"deathM", 5);
		jumpF = LoadSet(path+"jumpF", 6);
		landF = LoadSet(path+"landF", 6);
		deathF = LoadSet(path+"deathF", 5);
		deathN = Resources.Load(path+"deathN") as AudioClip;
	}

	void Start()
	{
		audioplayerCasting = GameObject.Find("audioplayerCasting");
	}

	public void Play(string s, GameObject caller)
	{
		AudioSource player = caller.GetComponent<AudioSource>();
		AudioClip clip = FindClip (s);
		try
		{
			player.clip = clip;
			player.loop = false;
			player.Play();
		}
		catch (Exception e)
		{	/*Debug.Log("A sound was triggered, but the caller doesn't have an audiosource component.");*/	}
	}

	public void Play(string s, GameObject caller, int n, int targetSource)
	{
		AudioSource player = caller.GetComponents<AudioSource>()[targetSource];
		AudioClip clip = FindClip (s, n);
		try
		{
			player.clip = clip;
			player.loop = false;
			player.volume = 1f;
			player.Play();
		}
		catch (Exception e)
		{	/*Debug.Log("A sound was triggered, but the caller doesn't have an audiosource component.");*/	}
	}

	public void Play(string s, float volume, GameObject caller)
	{
		AudioSource player = caller.GetComponent<AudioSource>();
		AudioClip clip = FindClip (s);
		try
		{
			player.clip = clip;
			player.loop = false;
			player.volume = volume;
			player.Play();
		}
		catch (Exception e)
		{	/*Debug.Log("A sound was triggered, but the caller doesn't have an audiosource component.");*/	}
	}

	public void Stop(GameObject caller)
	{
		AudioSource player = caller.GetComponent<AudioSource>();
		try
		{
			player.Stop();
		}
		catch (Exception e)
		{	/*Debug.Log("A stop was triggered, but the caller doesn't have an audiosource component.");*/	}
	}

	public void PlayLoop(string s, GameObject caller)
	{
		AudioSource player = caller.GetComponent<AudioSource>();
		AudioClip clip = FindClip (s);

		//if (!player.isPlaying)
		{
			try {
				player.clip = clip;
				player.loop = true;
				player.Play ();
			} catch (Exception e) {
				Debug.Log ("A sound was triggered, but the caller doesn't have an audiosource component.");
			}
		}
	}

	private AudioClip FindClip(string s)
	{
		switch (s)
		{
		case "handMoveLoop":
			return handMoveLoop;
			break;
		case "rockImpact":
			return FindClipRandom (rockImpact);
			break;
		case "rockShatter":
			return FindClipRandom (rockShatter);
			break;
		case "rockClick":
			return FindClipRandom (rockClick);
			break;
		//case "summonInit":
			//return summonInit;
			//break;
		case "summonLoop":
			return summonLoop;
			break;
		case "summonStop":
			return summonStop;
			break;
		case "gustLoop":
			return gustLoop;
			break;
		case "gustRelease":
			return gustRelease;
			break;
		case "summonInstance":
			return FindClipNumber(summonInstance, 0);
			break;
		case "relicCatch":
			return FindClipRandom (relicCatch);
			break;
		case "relicThrow":
			return FindClipRandom (relicThrow);
			break;
		case "relicDamage":
			return FindClipRandom (relicDamage);
			break;
		case "footstepsFast":
			return FindClipRandom (footstepsFast);
			break;
		case "footstepsSlow":
			return FindClipRandom (footstepsSlow);
			break;
		case "jumpM":
			return FindClipRandom (jumpM);
			break;
		case "landM":
			return FindClipRandom (landM);
			break;
		case "deathM":
			return FindClipRandom (deathM);
			break;
		case "jumpF":
			return FindClipRandom (jumpF);
			break;
		case "landF":
			return FindClipRandom (landF);
			break;
		case "deathF":
			return FindClipRandom (deathF);
			break;
		case "deathN":
			return deathN;
			break;

		default:
			return null;
			break;
		}
	}

	private AudioClip FindClip(string s, int n)
	{
		switch (s)
		{
		case "summonInstance":
			return FindClipNumber(summonInstance, n);
			break;
		default:
			return null;
			break;
		}
	}

	private AudioClip FindClipRandom(List<AudioClip> list)
	{
		return list[UnityEngine.Random.Range(0,list.Count)];
	}

	private AudioClip FindClipNumber(List<AudioClip> list, int n)
	{
		return list[n];
	}

	public void SetVolume(float v, GameObject caller)
	{
		AudioSource player = caller.GetComponent<AudioSource>();
		player.volume = v;
	}

	private List<AudioClip> LoadSet (string s, int n)
	{
		List<AudioClip> result = new List<AudioClip>();

		for (int i = 0; i < n; i++)
		{
			result.Add( Resources.Load(s+"_"+i.ToString("00")) as AudioClip );
		}

		return result;
	}
	/*
	public void PlayLoopWithInit(string s, string l, GameObject caller)
	{
		AudioSource[] players = caller.GetComponents<AudioSource>();
		AudioClip clipInit = FindClip ("s");
		AudioClip clipLoop = FindClip ("l");
		try
		{
			player[0].clip = clipInit;
			player[0].loop = false;

			player[1].clip = clipLoop;
			player[1].loop = true;

			player[0].Play();
			StartCoroutine(FadePlayers())
		}
		catch (Exception e)
		{	Debug.Log("A sound was triggered, but the caller doesn't have an audiosource component.");	}
	}*/
}
