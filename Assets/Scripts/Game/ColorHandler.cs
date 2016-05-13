using UnityEngine;
using System.Collections;

public class ColorHandler : MonoBehaviour {

	public Material[] materials;

	private enum ColorScheme { red, green, blue, purple };
	private ColorScheme colorScheme;

	private Color materialColor;
	private Color fogColor;


	// Use this for initialization
	void Start () {
		SetColors(ColorScheme.red);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
		
	private void SetSkybox(){}

	private void UpdateMaterialColor(){
		
	}

	private void UpdateFogColor(){
	}

	private void SetGoalLightColor(){}

	private void SetColors(ColorScheme c){

		switch (c) {

		case ColorScheme.red:
			Debug.Log ("Red");
			materialColor = new Color (1, 0, 0);
			fogColor = new Color (0.8f, 0, 0);
			break;
		case ColorScheme.green:
			break;
		case ColorScheme.blue:
			break;
		case ColorScheme.purple:
			break;
		default:
			Debug.Log ("No color found in ColorHandler()");	
			break;
		}


	}

}
