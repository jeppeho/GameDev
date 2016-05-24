using UnityEngine;
using System.Collections;

public class ColorHandler : MonoBehaviour {

	public Material elementMaterial;
	public Material[] skyboxes;

	private enum ColorScheme { red, green, blue, purple };
	private ColorScheme colorScheme;

	private Color[] materialColor;
	public Texture[] sparkTextures;
	public Material waterMaterial;
	public Texture[] waterTextures;
	public Cubemap[] cubemaps;


	// Use this for initialization
	void Start () {
		materialColor = new Color[4];

		SetColors(ColorScheme.blue);
//		UpdateMaterialColor();

	}
	
	// Update is called once per frame
	void Update () {
		
		
	}
		
	public void SetSkybox(int index){
		//Debug.Log ("********Setting skybox with color index = " + index);
		RenderSettings.skybox = skyboxes [index];

		RenderSettings.customReflection = cubemaps [index];
		RenderSettings.fogColor = GetFogColor (index);

	}

	public void SetWater(int index){
		Debug.Log ("********Setting water with color index = " + index);

		waterMaterial.SetColor("_horizonColor", GetWaterColor (index));
		waterMaterial.SetTexture("_ColorControl", waterTextures [index]);

	}

	IEnumerator SetGoalAsync(int index)
	{
		yield return new WaitForEndOfFrame ();

		//Debug.Log ("********Setting goal with color index = " + index);

		Color[] colors = GetColorMaterialArray ( GetColorSchemeByIndex(index) );

        ParticleSystem partsys = GameObject.FindGameObjectWithTag("GoalLight").GetComponent<ParticleSystem>();
		partsys.startColor = colors [3];
	}

	IEnumerator SetHandglowAsync(int index)
	{
		yield return new WaitForEndOfFrame ();

		//Debug.Log ("********Setting handglow with color index = " + index);

		Color[] colors = GetColorMaterialArray ( GetColorSchemeByIndex(index) );

		ParticleSystem partsys = GameObject.Find("StoneHandModel 1")
			.transform.FindChild("ball")
				.transform.FindChild("glowRed").GetComponent<ParticleSystem> ();

		//Debug.Log (partsys.ToString ());
		partsys.startColor = colors [3];
	}

	public void SetColorScheme(int index){

		SetColors( GetColorSchemeByIndex(index) );

		SetSkybox (index);

		SetWater (index);

		StartCoroutine (SetGoalAsync(index));

		StartCoroutine (SetHandglowAsync(index));

		UpdateMaterialColor();

	}


	private void SetMinionSparkTexture(GameObject minion, int index){

		GameObject impactSparks = null;// = minion.gameObject.Find ("impactSparks");

		foreach (Transform t in minion.transform) {

			if(t.name.Contains("impactSparks")){
				impactSparks = t.gameObject;
				break;
			}
		
		}

		impactSparks.GetComponent<Renderer> ().material.SetTexture("_MainTex", sparkTextures[ index ]);
	}

	private Color GetFogColor(int index){

		Color color = new Color (0, 0, 0);

		if (index == 0) {
			color = new Color(148f/255f, 10f/255f, 0f/255f, 1f);
		} 
		else if (index == 1) {
			color = new Color(31f/255f, 216f/255f, 131f/255f, 1f);
		} 
		else if (index == 2) {
			color = new Color(50f/255f, 129f/255f, 211f/255f, 1f);
		} 
		else if (index == 3) {
			color = new Color(186f/255f, 60f/255f, 247f/255f, 1f);
		}

		return color;
	}

	private Color GetWaterColor(int index){

		Color color = new Color (0, 0, 0);

		if (index == 0) {
			color = new Color(148f/255f, 10f/255f, 0f/255f, 1f);
		} 
		else if (index == 1) {
			color = new Color(31f/255f, 216f/255f, 131f/255f, 1f);
		} 
		else if (index == 2) {
			color = new Color(50f/255f, 129f/255f, 211f/255f, 1f);
		} 
		else if (index == 3) {
			color = new Color(186f/255f, 60f/255f, 247f/255f, 1f);
		}

		return color;
	}



	public void SetMinionColor(GameObject minion, int index){
		
		Color[] colors = GetColorMaterialArray ( GetColorSchemeByIndex(index) );

		//Set the material color
		foreach (Transform t in minion.transform) {

			foreach (Transform tt in t.transform) {

				if (tt.name.Contains ("character:minion")) {

					//Get renderer material
					Material m = tt.GetComponent<Renderer> ().material;

					//Set colors
					m.SetColor ("_Color", colors [0]);
					m.SetColor ("_SpecColor", colors [1]);
					m.SetColor ("_EmissionColor", colors [2]);

					//Apply changes to material on Renderer 
					tt.GetComponent<Renderer> ().material = m;
				}
			}
		}

		//Set the color of the impactSparks
		SetMinionSparkTexture(minion, index);
	}


	public void SetMinionColorOLD(GameObject minion, int godIndex){

		Color[] colors = GetColorMaterialArray ( GetColorSchemeByIndex(godIndex) );

		Renderer[] childRends = minion.GetComponentsInChildren<Renderer> ();

		for (int i = 0; i < childRends.Length; i++)
		{
			Material m = childRends [i].material;
			m.SetColor("_Color", colors [0]);
			m.SetColor("_SpecColor", colors [1]);
			m.SetColor("_EmissionColor", colors [2]);
		}
	}
		


	private void UpdateMaterialColor(){

		elementMaterial.SetColor("_Color", materialColor [0]);
		elementMaterial.SetColor("_SpecColor", materialColor [1]);
		elementMaterial.SetColor("_EmissionColor", materialColor [2]);
	}

	private void UpdateFogColor(){
	}

	private void SetGoalLightColor(){}

	private void SetColors(ColorScheme c){
		
		Color[] colors = GetColorMaterialArray (c);

		materialColor [0] = colors[0];
		materialColor [1] = colors[1];
		materialColor [2] = colors[2];
		materialColor [3] = colors[3];
	}




	private Color[] GetColorMaterialArray(ColorScheme c){
	
		Color[] colors = new Color[4];

		switch (c) {

		case ColorScheme.red:
			colors [0] = new Color(114f/255f, 23f/255f, 23f/255f, 80f/255f);
			colors [1] = new Color(172f/255f, 40f/255f, 40f/255f, 255f / 255f);
			colors [2] = new Color(0.2f, 0.05490196f, 0.02352941f, 0.2f);
			colors [3] = new Color(0.949f, 0.353f, 0.251f, 0.176f);
			//fogColor = new Color (0.8f, 0, 0);
			break;

			//OLD GREEN
//		case ColorScheme.green:
//			colors [0] = new Color(59f/255f, 113f/255f, 21f/255f, 80f/255f);
//			colors [1] = new Color(71f/255f, 172f/255f, 40f/255f, 255f / 255f);
//			colors [2] = new Color(0.108f, 0.3f, 0.03200001f, 0.2f);
//			break;
		case ColorScheme.green:
			colors [0] = new Color(0.082f, 0.443f, 0.278f, 0.313f);
			colors [1] = new Color(0.156f, 0.674f, 0.439f, 255f / 255f);
			colors [2] = new Color(0.032f, 0.3f, 0.1447448f, 0.3f);
			colors [3] = new Color(0.337f, 0.678f, 0.451f, 0.176f);
			break;

		case ColorScheme.blue:
			colors [0] = new Color(22f/255f, 65f/255f, 113f/255f, 80f/255f);
			colors [1] = new Color(40f/255f, 106f/255f, 172f/255f, 255f / 255f);
			colors [2] = new Color(0.0433071f, 0.2716535f, 0.5f, 0.5f);
			colors [3] = new Color(0.341f, 0.588f, 0.874f, 0.176f);
			break;

		case ColorScheme.purple:
			colors [0] = new Color(101f/255f, 2f/255f, 113f/255f, 80f/255f);
			colors [1] = new Color(115f/255f, 40f/255f, 172f/255f, 255f / 255f);
			colors [2] = new Color(0.2092105f, 0.03157895f, 0.3f, 0.3f);
			colors [3] = new Color(0.411f, 0.368f, 0.756f, 0.176f);
			break;

		default:
			Debug.Log ("No color found in ColorHandler()");	
			break;
		}

		return colors;
	
	}

	private ColorScheme GetColorSchemeByIndex(int index){

		switch(index){

		case 0:
			return ColorScheme.red;
		case 1:
			return ColorScheme.green;
		case 2:
			return ColorScheme.blue;
		case 3:
			return ColorScheme.purple;
		default:
			Debug.Log("Couldn't access colorscheme by color in GetColorSchemeByIndex()");
			return ColorScheme.red;
		}
	}


//	private void SetColorsOLD(ColorScheme c){
//
//		Debug.Log ("Settings colors to " + c);
//
//		switch (c) {
//
//		case ColorScheme.red:
//			Debug.Log ("@Red");
//			materialColor [0] = new Color(114f/255f, 23f/255f, 23f/255f, 80f/255f);
//			materialColor [1] = new Color(172f/255f, 40f/255f, 40f/255f, 255f / 255f);
//			materialColor [2] = new Color(0.2f, 0.05490196f, 0.02352941f, 0.2f);
//			//fogColor = new Color (0.8f, 0, 0);
//			break;
//
//		case ColorScheme.green:
//			Debug.Log ("@Green");
//			materialColor [0] = new Color(59f/255f, 113f/255f, 21f/255f, 80f/255f);
//			materialColor [1] = new Color(71f/255f, 172f/255f, 40f/255f, 255f / 255f);
//			materialColor [2] = new Color(0.108f, 0.3f, 0.03200001f, 0.2f);
//			break;
//
//		case ColorScheme.blue:
//			Debug.Log ("@Blue");
//			materialColor [0] = new Color(22f/255f, 65f/255f, 113f/255f, 80f/255f);
//			materialColor [1] = new Color(40f/255f, 106f/255f, 172f/255f, 255f / 255f);
//			materialColor [2] = new Color(0.0433071f, 0.2716535f, 0.5f, 0.5f);
//			break;
//
//		case ColorScheme.purple:
//			Debug.Log ("@Purple");	
//			Debug.Log ("Hei");
//			materialColor [0] = new Color(101f/255f, 2f/255f, 113f/255f, 80f/255f);
//			materialColor [1] = new Color(115f/255f, 40f/255f, 172f/255f, 255f / 255f);
//			materialColor [2] = new Color(0.2092105f, 0.03157895f, 0.3f, 0.3f);
//			break;
//
//		default:
//			Debug.Log ("No color found in ColorHandler()");	
//			break;
//		}
//	}
}
