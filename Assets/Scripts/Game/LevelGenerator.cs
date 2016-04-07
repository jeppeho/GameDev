using UnityEngine;
using System.Collections;

public class LevelGenerator : MonoBehaviour {

	NoiseGenerator NG;

	//Gameobjects
	public GameObject planePrefab;
	public GameObject waterSmallPrefab;
	public GameObject cubestackPrefab;
	public GameObject blockPrefab;

	//Ground slices
	public GameObject groundSlice;
	public GameObject lavaSlice;
	public GameObject waterSlice;
	public GameObject holeSlice;

	// Use this for initialization
	void Start () {

		NG = new NoiseGenerator ();

		//GenerateLevel ();
		//NG.CreateVisualAxes ();

		//float[] someNoise = NG.GetPerlinNoise1D (0, 4, 0.6f, -4, 4);
		//NG.CreateVisualRepresentationSingle (someNoise, 20);

		GenerateLevel ();

	}


	/**
	 * Current function for generating a level, with a plane and obstacles
	 * 
	 */
	private void GenerateLevel(){

		//GENERATE PLANE
		float[] planeNoise = NG.GetPerlinNoise1D (0, 10, 0.6f, -1, 1);
		NG.CreateVisualRepresentationSingle (planeNoise, 20);

		for (int sample = 0; sample < NG.GetNumSamples (); sample++) {

			float color = (planeNoise [sample] + 1) / 2;

			if (planeNoise [sample] < -0.7f && planeNoise [sample] > -1f)
				CreateLavaSlice (new Vector3 (0, 0, sample));
			else if (planeNoise [sample] > 0.0f && planeNoise [sample] < 0.2f)
				CreateWaterSlice (new Vector3 (0, 0, sample));
			else if (planeNoise [sample] > 0.6f && planeNoise [sample] < 0.8f)
				CreateWaterSlice (new Vector3 (0, 0, sample));
			else if (planeNoise [sample] > 0.8f && planeNoise [sample] < 0.9f) {
			}
			else
				CreateGroundSlice (new Vector3 (0, 0, sample));

			GameObject point = GameObject.CreatePrimitive (PrimitiveType.Cube);
			point.transform.localScale = new Vector3 (3, 10, 1);
			point.transform.position = new Vector3 (3, planeNoise[sample] * 10, sample * 1);
			point.GetComponent<Renderer> ().material.color =  new Color(1 - color, color, 0);
		}


		//GENERATE CUBESTACKS
		float[] stackNoise = NG.GetRandomNoise1D(-1, 1);
		float[] stackPositionNoise = NG.GetRandomNoise1D(-5, 5);
		float[] stackWidthNoise = NG.GetPerlinNoise1D (0, 3, 0.5f, 0.5f, 2.5f);

		//STACKS PLACED AT RANDOM WITH A PERLIN NOISE TO DECIDE WIDTH
//		for (int sample = 0; sample < PN.GetNumSamples (); sample++)
//			if (stackNoise[sample] < 0.2f && stackNoise[sample] > -0.2f) 
//				CreateCubeStack(new Vector3(stackPositionNoise[sample], 0, sample), stackWidthNoise[sample]);

		//FEW BIG OR MANY SMALL STACKS - OR NONE..?
		float[] stackVarNoise = NG.GetPerlinNoise1D (0, 3, 0.5f, 0f, 0.5f);
		for (int sample = 0; sample < NG.GetNumSamples (); sample++)
			if (stackNoise[sample] < stackVarNoise[sample] && stackNoise[sample] > -stackVarNoise[sample]) 
				CreateCubeStack(new Vector3(stackPositionNoise[sample], 0, sample), 3f - stackVarNoise[sample]*6 );

	
	}
		


	private void CreateCubeStack(Vector3 position, float width){
		GameObject cubestack = Instantiate(cubestackPrefab, position, Quaternion.identity) as GameObject;
		cubestack.transform.localScale = new Vector3(width, 1, width);
	}

	/**
	 * TODO method should search for pairs of cubestacks that are within reach and then determine 
	 * if a block should be put on top, connecting them. 
	 */
	private void CreateBlock(Vector3 position){
		GameObject block = Instantiate(blockPrefab, position, Quaternion.identity) as GameObject;

		float scale = Random.Range (5f, 10f);

		CreateCubeStack (new Vector3 (position.x - scale/2, 0 , position.z), 1);
		CreateCubeStack (new Vector3 (position.x + scale/2, 0 , position.z), 1);

		block.transform.localScale = new Vector3(scale, 1, 1);


	}


	/**
	 * INSTANTIATING GROUND SLICE FUNCTIONS
	 */
	private void CreateGroundSlice(Vector3 position){
		GameObject ground = Instantiate(groundSlice, position, Quaternion.identity) as GameObject;
	}
	private void CreateLavaSlice(Vector3 position){
		GameObject ground = Instantiate(lavaSlice, position, Quaternion.identity) as GameObject;
	}
	private void CreateWaterSlice(Vector3 position){
		GameObject ground = Instantiate(waterSlice, position, Quaternion.identity) as GameObject;
	}

}
