﻿using UnityEngine;
using System;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour {

	private static AudioClip handMoveLoop;
	private static AudioClip summonInit;
	private static AudioClip summonLoop;
	private static AudioClip summonInstance;
	private static List<AudioClip> rockImpact = new List<AudioClip>();
	private static List<AudioClip> rockClick = new List<AudioClip>();

	public GameObject audioplayerCasting;

	void Awake()
	{
		//Load all sounds from resources
		string path = "Audio/Godhand/";
		handMoveLoop = Resources.Load(path+"handMoveLoop") as AudioClip;
		summonInit = Resources.Load(path+"summonInit") as AudioClip;
		summonLoop = Resources.Load(path+"summonLoop") as AudioClip;
		summonInstance = Resources.Load(path+"summonInstance") as AudioClip;
		rockClick = LoadSet(path+"rockClick", 8);

		path = "Audio/Environment/";
		rockImpact = LoadSet(path+"rockImpact", 16);
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
		{	Debug.Log("A sound was triggered, but the caller doesn't have an audiosource component.");	}
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
		{	Debug.Log("A sound was triggered, but the caller doesn't have an audiosource component.");	}
	}

	public void PlayLoop(string s, GameObject caller)
	{
		AudioSource player = caller.GetComponent<AudioSource>();
		AudioClip clip = FindClip (s);
		try
		{
			player.clip = clip;
			player.loop = true;
			player.Play();
		}
		catch (Exception e)
		{	Debug.Log("A sound was triggered, but the caller doesn't have an audiosource component.");	}
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
		case "rockClick":
			return FindClipRandom (rockClick);
			break;
		case "summonInit":
			return summonInit;
			break;
		case "summonLoop":
			return summonLoop;
			break;
		case "summonInstance":
			return summonInstance;
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
