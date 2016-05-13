using UnityEngine;
using System.Collections;

public class ColorHandler : MonoBehaviour {

	public Material elementMaterial;
	public Material[] skyboxes;

	private enum ColorScheme { red, green, blue, purple };
	private ColorScheme colorScheme;

	private Color[] materialColor;


	// Use this for initialization
	void Start () {
		materialColor = new Color[3];

		SetColors(ColorScheme.blue);
//		UpdateMaterialColor();

	}
	
	// Update is called once per frame
	void Update () {
		
		
	}
		
	public void SetSkybox(int index){
		Debug.Log ("********Setting skybox with color index = " + index);
		RenderSettings.skybox = skyboxes [index];
		RenderSettings.fogColor = GetFogColor (index);

	}

	public void SetColorScheme(int index){

		SetColors( GetColorSchemeByIndex(index) );
		SetSkybox (index);
		UpdateMaterialColor();

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


	public void SetMinionColor(GameObject minion, int godIndex){

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
	}




	private Color[] GetColorMaterialArray(ColorScheme c){
	
		Color[] colors = new Color[3];

		switch (c) {

		case ColorScheme.red:
			colors [0] = new Color(114f/255f, 23f/255f, 23f/255f, 80f/255f);
			colors [1] = new Color(172f/255f, 40f/255f, 40f/255f, 255f / 255f);
			colors [2] = new Color(0.2f, 0.05490196f, 0.02352941f, 0.2f);
			//fogColor = new Color (0.8f, 0, 0);
			break;

		case ColorScheme.green:
			colors [0] = new Color(59f/255f, 113f/255f, 21f/255f, 80f/255f);
			colors [1] = new Color(71f/255f, 172f/255f, 40f/255f, 255f / 255f);
			colors [2] = new Color(0.108f, 0.3f, 0.03200001f, 0.2f);
			break;

		case ColorScheme.blue:
			colors [0] = new Color(22f/255f, 65f/255f, 113f/255f, 80f/255f);
			colors [1] = new Color(40f/255f, 106f/255f, 172f/255f, 255f / 255f);
			colors [2] = new Color(0.0433071f, 0.2716535f, 0.5f, 0.5f);
			break;

		case ColorScheme.purple:
			colors [0] = new Color(101f/255f, 2f/255f, 113f/255f, 80f/255f);
			colors [1] = new Color(115f/255f, 40f/255f, 172f/255f, 255f / 255f);
			colors [2] = new Color(0.2092105f, 0.03157895f, 0.3f, 0.3f);
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
