using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour {

	private static AudioClip music_red;
	private static AudioClip music_green;

	// Use this for initialization
	void Start () {
		Init ();

		string path = "Audio/Music/";
		music_red = Resources.Load(path+"Runtheme_01") as AudioClip;
		music_green = Resources.Load(path+"Runtheme_02") as AudioClip;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Init()
	{
		AudioClip m;

			if (SceneManager.GetActiveScene().Equals(SceneManager.GetSceneByName("LevelGenerator")))
			{
				switch (GetGodColor ())
				{
					case 0:
					m = music_red; //Red
					break;
					case 1:
					m = music_red; //Blue
					break;
					case 2:
					m = music_green; //Green
					break;
					case 3:
					m = music_green; //Purple
					break;
					default:
					m = music_red; //Menu-theme
					break;
				}
			}

			else
			{
			m = music_red; //Menu-theme
			}
	}

	private int GetGodColor ()
	{
		//Get renderer material
		Material m = GameObject.Find("StoneHandModel 1").transform.FindChild("ball").GetComponent<Renderer>().material;

		//Check color
		Color myColor = m.GetColor ("_Color");

		int c = -1;

		if (myColor.Equals (new Color (114f / 255f, 23f / 255f, 23f / 255f, 80f / 255f)))
		{	c = 0;} //red
		else if (myColor.Equals (new Color (22f/255f, 65f/255f, 113f/255f, 80f/255f)))
		{	c = 1; } //blue
		else if (myColor.Equals (new Color (59f/255f, 113f/255f, 21f/255f, 80f/255f)))
		{	c = 2; } //green
		else if (myColor.Equals (new Color (101f/255f, 2f/255f, 113f/255f, 80f/255f)))
		{	c = 3; } //purple

		return c;
	}
}
