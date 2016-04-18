using UnityEngine;
using System.Collections;

public class LevelGenerator : MonoBehaviour {

	public int grassPercentage;
	public int waterPercentage;
	public int canyonPercentage;
	public int gapPercentage;


	NoiseGenerator NG;
	//LevelManager LM;
	//GameObject[] cubes;

	private int startingCell = 5;

	private int levelWidth = 10;

	private int numCubeWidth = 6;

	//Gameobjects
	public GameObject planePrefab;
	public GameObject waterSmallPrefab;
	public GameObject stackCube;


	//Level cubes
	public GameObject groundCube;
	public GameObject waterCube;
	public GameObject cliffCube;
	public GameObject bridgeCube;
	public GameObject woodCube;


	//Ground slices
	public GameObject startSlice;
	public GameObject goalSlice;
	public GameObject groundSlice;
	public GameObject lavaSlice;
	public GameObject lavaStoneSlice;
	public GameObject waterSlice;
	public GameObject holeSlice;

	public GameObject lavaSteppingStone;

	private enum AreaType { grass, water, canyon, gap, start, goal }; 
	private AreaType[] levelAreas; 

	private float[] levelAreaNoise;
	private float[] canyonNoise;
	private float[] cliffBridgeNoise;
	private float[] cliffBridgeNoise2;
	private float[] waterStoneNoise;
	private float[] cubeStackNoise;
	private float[] cubeStackHeightNoise;
	private float[] woodSteppingStones1;
	private float[] woodSteppingStones2;

	private Vector3[] respawnPoints;
	private float respawnpointOffset;

	// Use this for initialization
	void Start () {

		NG = new NoiseGenerator ();

		//GenerateLevel ();
		//NG.CreateVisualAxes ();

		//float[] someNoise = NG.GetPerlinNoise1D (0, 4, 0.6f, -4, 4);
		//NG.CreateVisualRepresentationSingle (someNoise, 20);

		GenerateLevelNew ();


	}


	private void GenerateLevelNew(){

		//Instantiate arrays
		levelAreas = new AreaType[NG.GetNumSamples()];
		respawnPoints = new Vector3[NG.GetNumSamples () * LevelManager.numPlayers];
		//cubes = new GameObject[NG.GetNumSamples];

		//Create noise arrays
//		levelAreaNoise = NG.GetPerlinNoise1D (3, 10, 0.6f, -1, 1);
//		SetLevelAreaArray ();

		generateAcceptableLevel ();

		canyonNoise = NG.GetPerlinNoise1D (4, 7, 0.6f, -numCubeWidth +1, numCubeWidth -1);
		cliffBridgeNoise = NG.GetPerlinNoise1D (1, 6, 0.5f, -numCubeWidth + 0.5f, (numCubeWidth / 4) );
		cliffBridgeNoise2 = NG.GetPerlinNoise1D (4, 6, 1f, (-numCubeWidth / 4), numCubeWidth - 0.5f);
		waterStoneNoise = NG.GetPerlinNoise1D (6, 10, 0.7f, -numCubeWidth, numCubeWidth);
		cubeStackNoise = NG.GetPerlinNoise1D (8, 10, 0.8f, -1, 1); //NG.GetRandomNoise1D (-1, 1);
		cubeStackHeightNoise = NG.GetPerlinNoise1D (6, 10, 1f, 2, 7);
		woodSteppingStones1 = NG.GetPerlinNoise1D (4, 10, 0.8f, -numCubeWidth + 1, 0);
		woodSteppingStones2 = NG.GetPerlinNoise1D (4, 10, 0.8f, 0, numCubeWidth - 1);

		for (int sample = 0; sample < NG.GetNumSamples (); sample++) {

			float y = levelAreaNoise [sample];

			//TEMPORARY
		 	if (levelAreas [sample] == AreaType.start) 
				CreateGrassCube (sample);


			if (levelAreas [sample] == AreaType.canyon) {

				CreateCanyonCube (sample);

			} else if (levelAreas [sample] == AreaType.water) {

				CreateWaterCube (sample);

			} else if (levelAreas [sample] == AreaType.gap) {
				
				CreateBridges (sample);

			} else if (levelAreas [sample] == AreaType.grass) {

				CreateGrassCube (sample);

			} else {
				Debug.Log ("Level area type not defined @" + sample + " | " + levelAreas [sample]);
			}
		}

		LevelManager.respawnPoints = respawnPoints;

		for (int i = 0; i < NG.GetNumSamples (); i++) {
			Debug.Log (i + "] @" + LevelManager.GetRespawnPoint (i));
		}
	}


	/**
	 * Checks that no areas are just a single line. If prev and following
	 * area is different, and those two are the same, then it modifies the middle one to be 
	 * that of those two.
	 */
	private void RemoveIsolatedAreas(){
	
		for (int sample = startingCell + 1; sample < NG.GetNumSamples() - 1; sample++) {

			if (levelAreas[sample - 1] != levelAreas[sample] 
				&& levelAreas [sample + 1] != levelAreas [sample]
				&& levelAreas[sample - 1] == levelAreas [sample + 1])
			{
				levelAreas [sample] = levelAreas [sample - 1]; 
			}
		}
	}


	private void generateAcceptableLevel(){

		bool accepted = false;
		int numTests = 10000;

		Debug.Log ("Generating acceptable level with " + numTests + " tries");

		while (!accepted) {

			accepted = true;
		
			levelAreaNoise = NG.GetPerlinNoise1D (3, 10, 0.6f, -1, 1);
			SetLevelAreaArray ();

			if (GetCountOfAreaType (AreaType.water) < waterPercentage)
				accepted = false;

			if (GetCountOfAreaType (AreaType.canyon) < canyonPercentage)
				accepted = false;

			if (GetCountOfAreaType (AreaType.gap) < gapPercentage)
				accepted = false;

			if (GetCountOfAreaType (AreaType.grass) < grassPercentage)
				accepted = false;
			
				
			numTests--;
			//Dont let it continue forever
			if (numTests < 0) {
				accepted = true;
				Debug.Log ("No succes for generating acceptable level");
			}
		}

		Debug.Log ("WATER = " + GetCountOfAreaType (AreaType.water));
		Debug.Log ("CANYON = " + GetCountOfAreaType (AreaType.canyon));
		Debug.Log ("GAP = " + GetCountOfAreaType (AreaType.gap));
		Debug.Log ("GRASS = " + GetCountOfAreaType (AreaType.grass));
	}


	/**
	 * Returns the percentage-wise represenation of a certain area type
	 * in the levelAreas array
	 */
	private float GetCountOfAreaType(AreaType type){
		
		int count = 0;

		for (int sample = startingCell; sample < NG.GetNumSamples (); sample++) {
			if (levelAreas [sample] == type) {
				count++;		
			}
		}

		float percent = count / (float)NG.GetNumSamples () * 100; 		  

		return percent;

	}

	/**
	 * Based on values from the levelAreaNoise array it sets
	 * the values with the LevelArea enum type
	 */
	private void SetLevelAreaArray(){

		//SET START LINE
		for (int sample = 0; sample < startingCell; sample++)
			levelAreas[sample] = AreaType.start;


		//SET OBSTACLE COURSE
		for (int sample = startingCell; sample < NG.GetNumSamples (); sample++) {

			float n = levelAreaNoise [sample];

			if (n > -1f && n < -0.3f) {

				levelAreas[sample] = AreaType.water;
			}
			else if (n > 0f && n < 0.2f) {

				levelAreas[sample] = AreaType.canyon;

			} else if( (n > 0.7f && n < 1f ) /*|| (n > 0.4f && n < 0.6f )*/){
				
				levelAreas[sample] = AreaType.gap;

			} else {
				
				levelAreas[sample] = AreaType.grass;

			}
		}

		//SET GOAL AREA
		for (int sample = NG.GetNumSamples() - 6; sample < NG.GetNumSamples(); sample++)
			levelAreas[sample] = AreaType.goal;


		//MODIFY AREAS OF LENGTH 1
		RemoveIsolatedAreas ();
	
	}

	private void CreateCubeStack(int z){

		int numCubes = Mathf.FloorToInt (cubeStackHeightNoise [z]);

		float x = Random.Range (-numCubeWidth + 0.5f, numCubeWidth - 0.5f);

		for (int i = 1; i < numCubes; i++) {
		
			Vector3 position = new Vector3( x, i, z );
			GameObject cubestack = Instantiate(stackCube, position, Quaternion.identity) as GameObject;
	
		}
	
	}

	private void CreateArchway(int z){

		float length = Random.Range (2f, 5f);
		float x = Random.Range (-numCubeWidth + length / 2, numCubeWidth - length / 2);
		float thickness = Random.Range (1f, 1.6f);
		int height = Random.Range (1, 5);

		for (int i = 1; i < height; i++) {
			GameObject cubeLeft = Instantiate(stackCube, new Vector3( x - length / 2, i * thickness, z), Quaternion.identity) as GameObject;
			GameObject cubeRight = Instantiate(stackCube, new Vector3( x + length / 2, i * thickness, z), Quaternion.identity) as GameObject;
			cubeLeft.transform.localScale = new Vector3 (thickness, 1, thickness);
			cubeRight.transform.localScale = new Vector3 (thickness, 1, thickness);
		}

		GameObject top = Instantiate(stackCube, new Vector3( x, height, z), Quaternion.identity) as GameObject;
		top.transform.localScale = new Vector3 (length + thickness, thickness, thickness);
	
	}



	/**
	 * INSTANTIATING GROUND CUBES METHODS
	 */

	private void CreateGrassCube(int z){

		Vector3 position = new Vector3( 0, 0, z );

		for (int i = -numCubeWidth; i < numCubeWidth; i++) {
			position.x = i + 0.5f;
			GameObject cube = Instantiate (groundCube, position, Quaternion.identity) as GameObject;
		}

		if (cubeStackNoise [z] > 0.6f) {

			CreateCubeStack (z);
		
		} else if (cubeStackNoise [z] < -0.9f) {
			CreateArchway (z);
		}

		SetRespawnPointOnGrass (z);
	}
		


	private void CreateWaterCube(int sample){

		Vector3 position = new Vector3 (0, -1, sample);

		for (int i = -numCubeWidth; i < numCubeWidth; i++) {
			position.x = i + 0.5f;
			GameObject cube = Instantiate (waterCube, position, Quaternion.identity) as GameObject;
		}


		//Don't check array out of bounds
		if ( sample > 0 && sample < NG.GetNumSamples() - 2 ) {
			//Only if prev and next area is water
			if (levelAreas [sample - 1] == AreaType.water && levelAreas [sample + 1] == AreaType.water) {
				//CreateWoodCube (sample);
				if (sample % 4 == 0) {
					//CreateWoodCubes (sample);
					CreateWaterSteppingStone(woodSteppingStones1, sample);
					//Debug.Log("#1 steppingstone & sample + 2 = " + (sample + 2));

					SetRespawnPointOnSteppingStone (sample, woodSteppingStones1 [sample]);
					SetRespawnPointOnSteppingStone (sample + 1, woodSteppingStones1 [sample]);
				} 
				if ((sample + 2) % 4 == 0) {
					//Debug.Log("#2 steppingstone");
					CreateWaterSteppingStone(woodSteppingStones2, sample);

					SetRespawnPointOnSteppingStone (sample, woodSteppingStones2 [sample]);
					SetRespawnPointOnSteppingStone (sample +1 , woodSteppingStones2 [sample]);
				}
			}
		}
	}


	private void CreateWaterSteppingStone(float[] noise, int z){
		GameObject cube = Instantiate (woodCube, new Vector3(noise[z], -0.9f, z), Quaternion.identity) as GameObject;
	}



	/**
	 * Used as stepping stone over water
	 */
	private void CreateWoodCube(int z){

		float x = waterStoneNoise [z];

		//Limit width on the X-axis
		x = x / numCubeWidth * (numCubeWidth - 1);

		//Randomize position a bit
		x += Random.Range (-1, 1);
		z += Random.Range (-1, 1);

		Vector3 position = new Vector3 (x, 0.2f, z);

		GameObject cube = Instantiate (woodCube, position, Quaternion.identity) as GameObject;
	}


	private void CreateCanyonCube(int z){

		Vector3 position = new Vector3 ( 0, 0, z );

		for (int x = -numCubeWidth; x < numCubeWidth; x++) {
			position.x = x + 0.5f;

			//Set Y position
			float difference = Mathf.Abs( x - canyonNoise[z] );

			if (difference < 3f)
				position.y = 0;
			else
				position.y = difference;

			GameObject cube = Instantiate (cliffCube, position, Quaternion.identity) as GameObject;
			cube.transform.localScale = new Vector3 (1, difference/2, 1);
		}

		//Create cubestack
		if (cubeStackNoise [z] > 0.6f) {
			CreateCubeStack (z);
		}

		SetRespawnPointInCanyon (z);
	}


	private void CreateBridges(int z){

		//Vector3 position = new Vector3 ( 0, 0.2f, z );
		Vector3 position1 = new Vector3 ( cliffBridgeNoise [z], 0.2f, z );
		Vector3 position2 = new Vector3 ( cliffBridgeNoise2 [z], 0.2f, z );


		//position.x = cliffBridgeNoise [z];
		GameObject cube = Instantiate (bridgeCube, position1, Quaternion.identity) as GameObject;
		cube.transform.Rotate(new Vector3(0, -GetBrigdeStepRotation(z, cliffBridgeNoise),0));

		//position.x = cliffBridgeNoise2 [z];
		GameObject cube2 = Instantiate (bridgeCube, position2, Quaternion.identity) as GameObject;
		//Rotate bridge cube
		cube2.transform.Rotate(new Vector3(0, -GetBrigdeStepRotation(z, cliffBridgeNoise2), 0) );

		SetRespawnPointOnBridge (z, position1.x);
	}


	/**
	 * Finds the rotation for a bridge step, by comparing positions with 
	 * the prev and next bridge step.
	 */
	private float GetBrigdeStepRotation(int sample, float[] noise){

		float angle = 0;

		if ( sample > 0 && sample < NG.GetNumSamples() - 2 ) {

			//Set positions for prev, current and next bridge step
			Vector2 prevPos = new Vector2 ( -1f, noise[ sample - 1 ] );
			Vector2 thisPos = new Vector2 ( 0f, noise[ sample ] );
			Vector2 nextPos = new Vector2 ( 1f, noise[ sample + 1 ] );

			float prevAngle = GetAngleBetweenTwoSteps(prevPos, thisPos); 
			float nextAngle = GetAngleBetweenTwoSteps(thisPos, nextPos);

			angle = (prevAngle + nextAngle) / 2;
		}

		return angle;
	}

	/**
	 * Calculates the angle between two points
	 */
	private float GetAngleBetweenTwoSteps(Vector2 v1, Vector2 v2){

		//Find angle
		float opposite = v1.y - v2.y;
		float hypotenuse = Mathf.Sqrt(1 + Mathf.Pow (opposite, 2));
		float angle = Mathf.Asin ( opposite / hypotenuse );

		//Convert from radians to degrees
		angle *= Mathf.Rad2Deg;

		return angle;
	}


	public Vector3 GetRespawnPoint(/*int player,*/ int z){

		int offset = 10;
		int index = 0;

		for (int i = offset; i > 0; i--) {
			if (z + i < NG.GetNumSamples ()) {
				index = z + i;
			}
		
		}

		return this.respawnPoints[index /* * player */];
	}




	private void SetRespawnPointInCanyon(int z){

		for (int i = 0; i < LevelManager.numPlayers; i++) {

			respawnPoints [ GetRespawnIndex (z, i) ] = new Vector3(canyonNoise[z - i], 0.5f, z - i);
		}
	}

	private void SetRespawnPointOnBridge(int z, float x){
	
		for (int i = 0; i < LevelManager.numPlayers; i++) {

			respawnPoints [ GetRespawnIndex (z, i) ] = new Vector3(cliffBridgeNoise[z - i], 0.5f, z - i);
		}
	}

	private void SetRespawnPointOnSteppingStone(int z, float x){

		for (int i = 0; i < LevelManager.numPlayers; i++) {

			//x = (x + (LevelManager.numPlayers - 1) / 2f) / 2f;

			respawnPoints [ GetRespawnIndex (z, i) ] = new Vector3(x, 0.5f, z-0.5f);
		}
	}

	private void SetRespawnPointOnGrass(int z){

		for (int i = 0; i < LevelManager.numPlayers; i++) {

			float x = (float)(i - (LevelManager.numPlayers - 1) / 2f);

			respawnPoints [ GetRespawnIndex (z, i) ] = new Vector3(x, 1, z);
		}
	}

	private int GetRespawnIndex (int z, int player){
		return z * (player + 1);
	}


}
