﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour {

	private int godColor;

	private static AudioClip music_red;
	private static AudioClip music_green;
	private static AudioClip music_blue;
	private static AudioClip music_purple;
	private static AudioClip music_menu;

	// Use this for initialization
	void Start () {
		
		string path = "Audio/Music/";
		music_red = Resources.Load(path+"Runtheme_red") as AudioClip;
		music_green = Resources.Load(path+"Runtheme_green") as AudioClip;
		music_blue = Resources.Load(path+"Runtheme_blue") as AudioClip;
		music_purple = Resources.Load(path+"Runtheme_purple") as AudioClip;
		music_menu = Resources.Load(path+"Runtheme_menu") as AudioClip;

        Init();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void Init()
	{
		AudioClip m;

		//Debug.Log ("MM: Checking scene..");
			if (SceneManager.GetActiveScene().Equals(SceneManager.GetSceneByName("LevelGenerator")))
			{
			//Debug.Log ("MM: Yes, it's the level..");
			switch (GetGodColor())
				{
					case 0:
					m = music_red; //Red
					break;
					case 1:
					m = music_green; //Green
					break;
					case 2:
					m = music_blue; //Blue
					break;
					case 3:
					m = music_purple; //Purple
					break;
					default:
					m = music_red; //Menu-theme
					break;
				}

			Debug.Log ("MusicManager: ..Playing "+m.ToString());
			}

			else
			{
				m = music_menu; //Menu-theme
			}

		this.GetComponents<AudioSource> () [0].Stop ();
		this.GetComponents<AudioSource> () [1].Stop ();
		this.GetComponents<AudioSource> () [0].clip = m;
		/*
		//Only autoplay if somewhere in menu!
        if (!SceneManager.GetActiveScene().Equals(SceneManager.GetSceneByName("LevelGenerator")))
        {
			PlayMusic ();
        }
		*/
	}

	public void PlayMusic()
	{
		AudioSource src = this.GetComponents<AudioSource> () [1];
		src.Stop(); //Make the winjingle-source stop

		src = this.GetComponents<AudioSource> () [0];
		src.volume = 1f;
		src.Play(); //Make the music-source play
	}

	public void MinionsWin()
	{
		Debug.Log ("JINGLE PLAYED!!");
		StartCoroutine(FadeMusic());
		StartCoroutine(DelayMinionsWin());
	}
		
	IEnumerator FadeMusic()
	{
		AudioSource src = this.GetComponents<AudioSource> () [0];

		while (src.volume > 0)
		{
			src.volume -= 0.75f*Time.unscaledDeltaTime;
			yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(Time.unscaledDeltaTime));
		}
	}

	IEnumerator DelayMinionsWin()
	{
		yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(0.5f));

		AudioSource src = this.GetComponents<AudioSource> () [1];
		src.Play(); //Make the music-source play
	}

	private int GetGodColor ()
	{
        /*
		//Debug.Log ("|||||||||||||||||||||||||||||MM: Checking god color... " );
		//Get renderer material
		Material m = GameObject.Find("StoneHandModel 1").transform.FindChild("ball").GetComponent<Renderer>().material;

		//Debug.Log ("|||||||||||||||||||||||||||||MM: Extracting color from "+ GameObject.Find("StoneHandModel 1").transform.FindChild("ball").ToString());
		//Check color
		Color myColor = m.GetColor ("_Color");

		int c = -1;

		if (myColor.g < 40f/255f && myColor.g >= 10f/255f)
		{	c = 0;} //red
		else if (myColor.g >= 85f/255f)
		{	c = 1; } //green
		else if (myColor.g < 85f/255f && myColor.g >= 40f/255f)
		{	c = 2; } //blue
		else if (myColor.g < 10f/255f)
		{	c = 3; } //purple
        */
		/*
		if (myColor.Equals (new Color (114f / 255f, 23f / 255f, 23f / 255f, 80f / 255f)))
		{	c = 0;} //red
		else if (myColor.Equals (new Color (22f/255f, 65f/255f, 113f/255f, 80f/255f)))
		{	c = 1; } //blue
		else if (myColor.Equals (new Color (21f/255f, 113f/255f, 71f/255f, 80f/255f)))
		{	c = 2; } //green
		else if (myColor.Equals (new Color (101f/255f, 2f/255f, 113f/255f, 80f/255f)))
		{	c = 3; } //purple
        */
        int c = GameObject.Find("SystemRunner").GetComponent<SystemManager>().GetCurrentGodMaterialIndex();
		return c;

        Debug.Log("|||||||||||||||||||||||||||||MM: Determined that god was color no. " + c.ToString());
	}
}
