using UnityEngine;
using System.Collections;

public class LevelGenerator : MonoBehaviour {

	NoiseGenerator NG;

	private int startingCell = 5;

	private int levelWidth = 10;

	//Gameobjects
	public GameObject planePrefab;
	public GameObject waterSmallPrefab;
	public GameObject cubestackPrefab;
	public GameObject blockPrefab;

	//Ground slices
	public GameObject startSlice;
	public GameObject goalSlice;
	public GameObject groundSlice;
	public GameObject lavaSlice;
	public GameObject lavaStoneSlice;
	public GameObject waterSlice;
	public GameObject holeSlice;

	public GameObject lavaSteppingStone;

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
		float[] planeNoise = NG.GetPerlinNoise1D (3, 10, 0.6f, -1, 1);
		float[] steppingStoneNoise = NG.GetPerlinNoise1D (0, 5, 0.8f, -0.5f, 1);
		float[] steppingStoneNoise2 = NG.GetPerlinNoise1D (0, 5, 0.8f, -1, 0.5f);
		float[] steppingStoneNoise3 = NG.GetRandomNoise1D (-0.5f, 1);
		//NG.CreateVisualRepresentationSingle (planeNoise, 20);
		NG.CreateVisualRepresentationSingle (planeNoise, 20);

		//Set start planes
		for (int i = 0; i < startingCell; i++)
			CreateStartSlice (new Vector3 (0, 0, i));

		//Set level planes
		for (int sample = startingCell; sample < NG.GetNumSamples (); sample++) {

			float n = planeNoise [sample];

			if (n < -0.6f && n > -1f) {
				CreateLavaSlice (new Vector3 (0, 0, sample));
				if (steppingStoneNoise3 [sample] > 0) {
					CreateLavaSteppingStone (new Vector3 (steppingStoneNoise [sample] * levelWidth / 2, 0, sample), sample);
				}
				CreateLavaSteppingStone (new Vector3 (steppingStoneNoise2[sample] * levelWidth / 2, 0, sample), sample);
			}else if(n < -0.5f)
				CreateLavaStoneSlice (new Vector3 (0, 0, sample));
			else if (n > 0.0f && n < 0.2f)
				CreateWaterSlice (new Vector3 (0, 0, sample));
			else if (n > 0.6f && n < 0.8f)
				CreateWaterSlice (new Vector3 (0, 0, sample));
			else
				CreateGroundSlice (new Vector3 (0, 0, sample));	
		}

		//Set goal planes
		for(int i = 0; i < 20; i++)
			CreateGoalSlice (new Vector3 (0, 0, NG.GetNumSamples() + i));


		//GENERATE CUBESTACKS
		float[] stackNoise = NG.GetRandomNoise1D(-1, 1);
		float[] stackPositionNoise = NG.GetRandomNoise1D(-5, 5);
		float[] stackWidthNoise = NG.GetPerlinNoise1D (0, 3, 0.5f, 0.5f, 2.5f);

		//STACKS PLACED AT RANDOM WITH A PERLIN NOISE TO DECIDE WIDTH
		for (int sample = 0; sample < NG.GetNumSamples (); sample++)
			if (stackNoise[sample] < 0.2f && stackNoise[sample] > -0.2f) 
				CreateCubeStack(new Vector3(stackPositionNoise[sample], 0, sample), stackWidthNoise[sample]);
//
//		//FEW BIG OR MANY SMALL STACKS - OR NONE..?
//		float[] stackVarNoise = NG.GetPerlinNoise1D (0, 3, 0.5f, 0f, 0.5f);
//		for (int sample = startingCell; sample < NG.GetNumSamples (); sample++)
//			if (stackNoise[sample] < stackVarNoise[sample] && stackNoise[sample] > -stackVarNoise[sample]) 
//				CreateCubeStack(new Vector3(stackPositionNoise[sample], 0, sample), 3f - stackVarNoise[sample]*6 );

	
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

	private void CreateLavaSteppingStone(Vector3 position, int sample){
		GameObject stone = Instantiate(lavaSteppingStone, position, Quaternion.identity) as GameObject;
		Vector3 offsetPosition = stone.transform.position;

		float r = 0.2f;
		offsetPosition += new Vector3(Random.Range(-r, r), 0,Random.Range(-r, r)); 
		stone.transform.position = offsetPosition;

		float[] scaleNoise = NG.GetPerlinNoise1D (0, 3, 0.5f, 1f, 3);
		//float[] heightNoise = NG.GetPerlinNoise1D (0, 3, 0.3f, 0.2f, 1);
		Vector3 scale_v = new Vector3 (scaleNoise[sample], 0.2f, 1.2f);
		stone.transform.localScale =  scale_v;
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
	private void CreateLavaStoneSlice(Vector3 position){
		GameObject ground = Instantiate(lavaStoneSlice, position, Quaternion.identity) as GameObject;
	}

	private void CreateWaterSlice(Vector3 position){
		GameObject ground = Instantiate(waterSlice, position, Quaternion.identity) as GameObject;
	}
	private void CreateStartSlice(Vector3 position){
		GameObject ground = Instantiate(startSlice, position, Quaternion.identity) as GameObject;
	}
	private void CreateGoalSlice(Vector3 position){
		GameObject ground = Instantiate(goalSlice, position, Quaternion.identity) as GameObject;
	}

}
