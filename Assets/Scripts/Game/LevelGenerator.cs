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

	private int halfWidth = 6;

	//Gameobjects
	public GameObject planePrefab;
	public GameObject waterSmallPrefab;
	public GameObject stackCube;


	//Level cubes
	//public GameObject groundCube;
	public GameObject lavaCube;
	//public GameObject cliffCube;
	//public GameObject bridgeCube;
	//public GameObject woodCube;

	//REAL MODELS
	public GameObject[] groundEdges;
	public GameObject groundMiddle;
	public GameObject[] pillars;
	public GameObject[] pillars_hex;
	public GameObject[] bridgeSteps;
	public GameObject[] sides;
	public GameObject[] steppingStones;
	public GameObject cliff;


	private int prevPillarIndex = -1;
	private int prevFirstbridgeStepIndex = -1;
	private int prevSecondbridgeStepIndex = -1;
	private int prevGroundEdgeIndex = -1;
	private int prevSidesIndex = -1;
	private int prevSteppingStoneIndex = -1;


	//Ground slices
//	public GameObject startSlice;
//	public GameObject goalSlice;
//	public GameObject groundSlice;
//	public GameObject lavaSlice;
//	public GameObject lavaStoneSlice;
//	public GameObject waterSlice;
//	public GameObject holeSlice;

//	public GameObject lavaSteppingStone;

	private enum AreaType { grass, water, canyon, gap, start, goal }; 
	private AreaType[] levelAreas; 

	private float[] levelAreaNoise;
	private float[] canyonNoise;
	private float[] cliffBridgeNoise;
	private float[] cliffBridgeNoise2;
	private float[] waterStoneNoise;
	private float[] pillarNoise;
	private float[] pillarPositionNoise;
	private float[] cubeStackNoise;
	private float[] cubeStackHeightNoise;
	private float[] woodSteppingStones1;
	private float[] woodSteppingStones2;
	private float[] cliffHeightNoise;

	private Vector3[] respawnPoints;
	private float respawnpointOffset;

	// Use this for initialization
	void Start () {

		NG = new NoiseGenerator ();

		//Instantiate arrays
		levelAreas = new AreaType[NG.GetNumSamples()];
		respawnPoints = new Vector3[NG.GetNumSamples () * LevelManager.numPlayers];

		generateAcceptableLevel ();

		//Create noise array to use with level elements
		canyonNoise = NG.GetPerlinNoise1D (4, 7, 0.6f, -halfWidth +1, halfWidth -1);
		cliffBridgeNoise = NG.GetPerlinNoise1D (1, 6, 0.5f, -halfWidth + 0.5f, (halfWidth / 4) );
		cliffBridgeNoise2 = NG.GetPerlinNoise1D (4, 6, 1f, (-halfWidth / 4), halfWidth - 0.5f);
		waterStoneNoise = NG.GetPerlinNoise1D (6, 10, 0.7f, -halfWidth, halfWidth);
		pillarNoise = NG.GetPerlinNoise1D (8, 10, 0.8f, -1, 1);
		pillarPositionNoise = NG.GetPerlinNoise1D (8, 10, 0.8f, -1, 1);
		cubeStackNoise = NG.GetPerlinNoise1D (8, 10, 0.8f, -1, 1);
		cubeStackHeightNoise = NG.GetPerlinNoise1D (6, 10, 1f, 2, 7);
		woodSteppingStones1 = NG.GetPerlinNoise1D (4, 10, 0.8f, -halfWidth -1, halfWidth + 1);
		woodSteppingStones2 = NG.GetPerlinNoise1D (4, 10, 0.8f, -halfWidth -1, halfWidth + 1);
		cliffHeightNoise = NG.GetPerlinNoise1D (6, 10, 1f, 300, 600);


		NG.CreateVisualRepresentationSingle (levelAreaNoise, 20f);

		for (int sample = 0; sample < NG.GetNumSamples (); sample++) {

			float y = levelAreaNoise [sample];

			CreateSides (sample);

			//TEMPORARY
			if (levelAreas [sample] == AreaType.start){
				//CreateGround (sample);
				CreateGroundSection(sample);

			} else if (levelAreas [sample] == AreaType.canyon) {

				//CreateCanyonSlice (sample);
				CreateCliffSection(sample);

			} else if (levelAreas [sample] == AreaType.water) {

				CreateLavaSection (sample);

			} else if (levelAreas [sample] == AreaType.gap) {

				//CreateBridges (sample);
				CreateBridgeStep (sample);

			} else if (levelAreas [sample] == AreaType.grass) {

				//CreateGround(sample);
				CreateGroundSection(sample);

			} else {
				Debug.Log ("Level area type not defined @" + sample + " | " + levelAreas [sample]);
			}
		}

		LevelManager.respawnPoints = respawnPoints;

		NG.ConvertSamplesToUnits (levelAreaNoise);

	}


	/**
	 * Checks that no areas are just a single line. If prev and following
	 * area is different, and those two are the same, then it modifies the middle one to be 
	 * that of those two.
	 */
	private void RemoveIsolatedAreas(){
	
		for (int sample = startingCell + 1; sample < NG.GetNumSamples() - 1; sample++) {

			//Remove areas of length 1
			if (levelAreas[sample - 1] != levelAreas[sample] 
				&& levelAreas [sample + 1] != levelAreas [sample]
				&& levelAreas[sample - 1] == levelAreas [sample + 1])
			{
				levelAreas [sample] = levelAreas [sample - 1]; 
			}

			//Remove areas of length 2
			if (levelAreas [sample] == levelAreas [sample + 1]
			   && levelAreas [sample - 1] != levelAreas [sample]
			   && levelAreas [sample - 1] == levelAreas [sample + 2]) 
			{
				levelAreas[sample] = levelAreas [sample - 1]; 
				levelAreas[sample + 1] = levelAreas [sample - 1]; 
			}
		}
	}


	private void generateAcceptableLevel(){

		bool accepted = false;
		int numTests = 10000;

		//To keep score of how many tries it took to get an accepted
		int acceptedNum = 0;

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


			if (accepted == true)
				Debug.Log (acceptedNum + " tries to generate accepted level!");
			else
				acceptedNum++;
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
			else if (n > 0.2f && n < 0.4f) {

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



	//----------------------------------
	// METHODS FOR CREATING A SECTION OF THE LEVEL
	// A section is one step deep on the Z-axis
	//----------------------------------



	private void CreateGroundSection(int z){

		Vector3 position = new Vector3( 0, 0, z );
	
		//Instantiate middle part
		GameObject middle = Instantiate (groundMiddle, position, Quaternion.identity) as GameObject;


		//If next area type is not grass
		if (z < NG.GetNumSamples() - 2) {
			AreaType nextArea = levelAreas [z + 1];
			AreaType nextNextArea = levelAreas [z + 2];
			if ( (nextNextArea == AreaType.water || nextNextArea == AreaType.gap ) 
				&& nextArea == AreaType.grass ) {

				CreateGroundEdge (z, false);

			}
		}

		//If previous area type is not grass
		if (z >= 2) {
			AreaType prevArea = levelAreas [z - 1];
			AreaType prevPrevArea = levelAreas [z - 2];
			if ( (prevPrevArea == AreaType.water || prevPrevArea == AreaType.gap )
				&& prevArea == AreaType.grass) {

				CreateGroundEdge (z, true);

			}
		}

		//Insert pillars
//		if ( (pillarNoise [z] > 0.5f && pillarNoise[z] < 0.6f)
//			|| (pillarNoise [z] > -0.1f && pillarNoise[z] < 0.0f)
//			|| (pillarNoise[z] > -0.6f && pillarNoise[z] < -0.5f)
//		) {

		if (z > 0 && z < NG.GetNumSamples ()) {

			if (levelAreas [z - 1] == AreaType.grass && levelAreas [z + 1] == AreaType.grass) {

				if (levelAreaNoise [z] < 0.6f) {

					if (z % 10 == 0) {

						CreatePillar (z, false);

					}
				}

				if (levelAreaNoise [z] > -0.5f) {
					if (z % 3 == 0 && z % 10 != 0) {
		
						CreateCrystalGroupSmall (z);

					}
				}
			}
		}


		//Set respawn point
		SetRespawnPointOnGrass (z);
	}
		

	private void CreateLavaSection(int sample){

		Vector3 position = new Vector3 (0, -1, sample);

		for (int i = -halfWidth - 10; i < halfWidth + 10; i++) {
			position.x = i + 0.5f;
			GameObject cube = Instantiate (lavaCube, position, Quaternion.identity) as GameObject;
		}


		//Don't check array out of bounds
		if ( sample > 0 && sample < NG.GetNumSamples() - 2 ) {

			//Only if prev and next area is water
			if (levelAreas [sample - 1] == AreaType.water && levelAreas [sample + 1] == AreaType.water) {

				if (sample % 4 == 0) {

					CreateSteppingStone(woodSteppingStones1, sample);

					if (levelAreaNoise [sample] < 0.0f) {

						if (sample % 8 == 0) {

							CreatePillar (sample, true);

						}
					}
						
					SetRespawnPointOnSteppingStone (sample, woodSteppingStones1 [sample]);
					SetRespawnPointOnSteppingStone (sample + 1, woodSteppingStones1 [sample]);
				} 
				if ((sample + 2) % 4 == 0) {

					CreateSteppingStone(woodSteppingStones2, sample);

					SetRespawnPointOnSteppingStone (sample, woodSteppingStones2 [sample]);
					SetRespawnPointOnSteppingStone (sample + 1 , woodSteppingStones2 [sample]);
				}
			}
		}
	}



	private void CreateCliffSection(int z){

		Vector3 position = new Vector3 ( 0, 0, z );

		GameObject middle = Instantiate (groundMiddle, position, Quaternion.identity) as GameObject;

		if (z % 2 == 0) {

			for (int x = -halfWidth - 10; x < halfWidth + 10; x += 2) {

				//Check if to close to road
				float difference = (x + 0.5f) - canyonNoise [z];

				if ( Mathf.Abs ( difference) > 4f) {

					position.x = Random.Range (x - 0.2f, x + 0.2f);

					GameObject cliffy = Instantiate (cliff, position, Quaternion.identity) as GameObject;
					cliffy.name = "normal cliff";
					float height = cliffHeightNoise [z] + Mathf.Abs ( difference ) * 30;
					float width = Random.Range (110, 150);
					cliffy.transform.localScale = new Vector3 (width, height, width);

					float rotationZ = difference * 1.4f;
					cliffy.transform.RotateAround (position, new Vector3 (0, 0, 1), -rotationZ);

					//ADD NARROWING AREA IN FRONT OF CANYON
					if (levelAreas [z-2] != AreaType.canyon) {

						for (int i = 1; i < Mathf.Abs(difference) - 4 && i < 5; i+=2) {

							position.x += Random.Range (-0.2f, 0.2f);
							position.z = z - i ;

							GameObject cliffy2 = Instantiate (cliff, position, Quaternion.identity) as GameObject;
							cliffy2.name = "extra cliff";
							//float height2 = 1000f;
							cliffy2.transform.localScale = new Vector3 (width, height, width);


//							float rotationZ2 = difference * 1.4f;
							cliffy.transform.RotateAround (position, new Vector3 (0, 0, 1), -rotationZ);

							position.z = z;
						}
					}
						
						
				}
			}
		}

		SetRespawnPointInCanyon (z);

	}





	//----------------------------------
	// METHODS FOR INSTANTIATING AND PLACING OBJECTS
	// Objects such as pillars, sides and stepping stones
	//----------------------------------


	private void CreateGroundEdge(int z, bool invertOnZ){

		Vector3 position = new Vector3 (0, 0, z);


		int index = NG.GetRandomButNotPreviousValue (0, groundEdges.Length, prevGroundEdgeIndex);
		GameObject edge = Instantiate (groundEdges [index], position, Quaternion.identity) as GameObject;

		Vector3 scale = edge.transform.localScale;
		scale.y = 5;
		edge.transform.localScale = scale;

		//Invert on z-axis
		if (invertOnZ) {
			edge.transform.RotateAround (position, new Vector3 (0, 1, 0), 180);
		}

		prevGroundEdgeIndex = index;

	}


	private void CreateGapSideEdge(int z, bool invertOnZ){

		Vector3 position = new Vector3 (-12, 0, z);
		int rotation = 90;
		//Invert on z-axis
		if (invertOnZ) {
			rotation += 180;
			position.x = 12;
		}

		int index = NG.GetRandomButNotPreviousValue (0, groundEdges.Length, prevGroundEdgeIndex);
		GameObject edge = Instantiate (groundEdges [index], position, Quaternion.identity) as GameObject;

		Vector3 scale = edge.transform.localScale;
		scale.y = 5;
		edge.transform.localScale = scale;

		edge.transform.RotateAround (position, new Vector3 (0, 1, 0), rotation);

		prevGroundEdgeIndex = index;

	}



	private void CreatePillar(int z, bool createHex){

		float x = pillarPositionNoise [z];

		int pillarIndex = NG.GetRandomButNotPreviousValue (0, pillars.Length, prevPillarIndex);

		if (pillarIndex == 1)
			x -= halfWidth;
		else if (pillarIndex == 2)
			x += halfWidth;
			

		GameObject pillar;

		Vector3 position = new Vector3( x, 0, z );
		if(createHex)
			pillar = Instantiate (pillars_hex [pillarIndex], position, Quaternion.identity) as GameObject; 
		else
			pillar = Instantiate (pillars [pillarIndex], position, Quaternion.identity) as GameObject; 

		//Set size
		float size = Random.Range (110, 130);
		pillar.transform.localScale = new Vector3 (size, size, size);

		//Rotate around Y-axis
		int rotation = 0;
		if (pillarIndex == 1)
			rotation = Random.Range (120, 180);
		else if (pillarIndex == 2)
			rotation = Random.Range (0, -60);

		else
			rotation = Random.Range (0, 360);
		

		pillar.transform.RotateAround( position, new Vector3( 0, 1, 0), rotation );

		//Update
		prevPillarIndex = pillarIndex;

	}

	private void CreateCrystalGroupSmall(int z){

		float x = Random.Range (-halfWidth + 1f, halfWidth - 1f);

		Vector3 position = new Vector3( x, 0, z );

		for (int i = 0; i < pillars.Length; i++) {
			GameObject pillar = Instantiate (pillars [i], position, Quaternion.identity) as GameObject;
			float scale = Random.Range (35, 50);
			pillar.transform.localScale = new Vector3 (scale, scale, scale);
			//Rotate around Y-axis
			pillar.transform.RotateAround( new Vector3(x - 1f, 0, z), new Vector3(0,1,0), 120 * i);
			pillar.transform.RotateAround( position, new Vector3(0,1,0), Random.Range(0, 360));
		
		}
	}



	private void CreateSteppingStone(float[] noise, int z){
		Debug.Log ("CreateSteppingStone");
		int index = NG.GetRandomButNotPreviousValue (0, steppingStones.Length, prevSteppingStoneIndex);
		GameObject steppingStone = Instantiate(steppingStones[index], new Vector3(noise[z], 0, z), Quaternion.identity) as GameObject;

		int scale = Random.Range (30, 50);
		steppingStone.transform.localScale = new Vector3 (scale, scale, scale);

		prevSteppingStoneIndex = index;
	}


	private void CreateSides(int z){

		//Put in every 5th slice or add an edge
		if (z % 10 == 0) {

			//float scale = Random.Range(90, 110);
			int min = 130;
			int max = 150;

			Vector3 scalar = new Vector3 (Random.Range(min, max), Random.Range(min, max), Random.Range(min, max));
			int rotationVariator = 30;
			float x = halfWidth + 10;

			Vector3 positionLeft = new Vector3 (-x, 0, z);
			Vector3 positionRight = new Vector3 (x, 0, z);

			//Get index for model
			int index = NG.GetRandomButNotPreviousValue (0, sides.Length, prevSidesIndex);

			GameObject leftSide = Instantiate (sides [index], positionLeft, Quaternion.identity) as GameObject;
			leftSide.transform.RotateAround (positionLeft, new Vector3 (0, 1, 0), Random.Range(-rotationVariator, rotationVariator) + 180);
			leftSide.transform.localScale = scalar;

			//Get new index for model
			index = NG.GetRandomButNotPreviousValue (0, sides.Length, index);

			GameObject rightSide = Instantiate (sides [index], positionRight, Quaternion.identity) as GameObject;
			rightSide.transform.RotateAround (positionRight, new Vector3 (0, 1, 0), Random.Range(-rotationVariator, rotationVariator));
			rightSide.transform.localScale = scalar;

			prevSidesIndex = index;
		}
	}



	private void CreateBridgeStep(int z){

		Vector3 position1 = new Vector3 ( cliffBridgeNoise [z], 0, z );
		Vector3 position2 = new Vector3 ( cliffBridgeNoise2 [z], 0, z );

		int firstBridgeIndex = NG.GetRandomButNotPreviousValue (0, bridgeSteps.Length, prevFirstbridgeStepIndex);
		int secondBridgeIndex = NG.GetRandomButNotPreviousValue (0, bridgeSteps.Length, prevSecondbridgeStepIndex);

		//Instantiate steps
		GameObject step1 = Instantiate (bridgeSteps[firstBridgeIndex], position1, Quaternion.identity) as GameObject;
		GameObject step2 = Instantiate (bridgeSteps[secondBridgeIndex], position2, Quaternion.identity) as GameObject;

		//Rotate around Y-axis
		step1.transform.RotateAround( position1, new Vector3(0,1,0), Random.Range(0, 360));
		step2.transform.RotateAround( position2, new Vector3(0,1,0), Random.Range(0, 360));

		//Set scale
		float size = Random.Range (27, 37);
		step1.transform.localScale = new Vector3 (size, 500, size);
		size = Random.Range (27, 37);
		step2.transform.localScale = new Vector3 (size, 500, size);

		if (z % 20 == 0) {
			CreateGapSideEdge (z, true);
			CreateGapSideEdge (z, false);
		}

		SetRespawnPointOnBridge (z, position1.x);

	}



	/**
	 * Used as stepping stone over water
	 */
//	private void CreateWoodCube(int z){
//
//		float x = waterStoneNoise [z];
//
//		//Limit width on the X-axis
//		x = x / numCubeWidth * (numCubeWidth - 1);
//
//		//Randomize position a bit
//		x += Random.Range (-1, 1);
//		z += Random.Range (-1, 1);
//
//		Vector3 position = new Vector3 (x, 0.2f, z);
//
//		GameObject cube = Instantiate (woodCube, position, Quaternion.identity) as GameObject;
//	}


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


	//----------------------------------
	// RESPAWN METHODS
	//----------------------------------

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




	//	private void CreateCubeStack(int z){
	//
	//		int numCubes = Mathf.FloorToInt (cubeStackHeightNoise [z]);
	//
	//		float x = Random.Range (-numCubeWidth + 0.5f, numCubeWidth - 0.5f);
	//
	//		for (int i = 1; i < numCubes; i++) {
	//		
	//			Vector3 position = new Vector3( x, i, z );
	//			GameObject cubestack = Instantiate(stackCube, position, Quaternion.identity) as GameObject;
	//	
	//		}
	//	
	//	}

	//	private void CreateArchway(int z){
	//
	//		float length = Random.Range (2f, 5f);
	//		float x = Random.Range (-numCubeWidth + length / 2, numCubeWidth - length / 2);
	//		float thickness = Random.Range (1f, 1.6f);
	//		int height = Random.Range (1, 5);
	//
	//		for (int i = 1; i < height; i++) {
	//			GameObject cubeLeft = Instantiate(stackCube, new Vector3( x - length / 2, i * thickness, z), Quaternion.identity) as GameObject;
	//			GameObject cubeRight = Instantiate(stackCube, new Vector3( x + length / 2, i * thickness, z), Quaternion.identity) as GameObject;
	//			cubeLeft.transform.localScale = new Vector3 (thickness, 1, thickness);
	//			cubeRight.transform.localScale = new Vector3 (thickness, 1, thickness);
	//		}
	//
	//		GameObject top = Instantiate(stackCube, new Vector3( x, height, z), Quaternion.identity) as GameObject;
	//		top.transform.localScale = new Vector3 (length + thickness, thickness, thickness);
	//	
	//	}


	//	private void CreateGrassCube(int z){
	//
	//		Vector3 position = new Vector3( 0, 0, z );
	//
	//		for (int i = -numCubeWidth; i < numCubeWidth; i++) {
	//			position.x = i + 0.5f;
	//			GameObject cube = Instantiate (groundCube, position, Quaternion.identity) as GameObject;
	//		}
	//
	//		if ( (pillarNoise [z] > 0.6f && pillarNoise[z] < 0.7f)
	//			|| (pillarNoise [z] > -0.1f && pillarNoise[z] < 0.1f)
	//			|| (pillarNoise[z] > -0.6f && pillarNoise[z] < -0.5f)
	//		) {
	//
	//			CreatePillar(z);
	//		}
	//
	//		SetRespawnPointOnGrass (z);
	//	}


	//	private void CreateBridges(int z){
	//
	//		//Vector3 position = new Vector3 ( 0, 0.2f, z );
	//		Vector3 position1 = new Vector3 ( cliffBridgeNoise [z], 0.2f, z );
	//		Vector3 position2 = new Vector3 ( cliffBridgeNoise2 [z], 0.2f, z );
	//
	//
	//		//position.x = cliffBridgeNoise [z];
	//		GameObject cube = Instantiate (bridgeCube, position1, Quaternion.identity) as GameObject;
	//		cube.transform.Rotate(new Vector3(0, -GetBrigdeStepRotation(z, cliffBridgeNoise),0));
	//
	//		//position.x = cliffBridgeNoise2 [z];
	//		GameObject cube2 = Instantiate (bridgeCube, position2, Quaternion.identity) as GameObject;
	//		//Rotate bridge cube
	//		cube2.transform.Rotate(new Vector3(0, -GetBrigdeStepRotation(z, cliffBridgeNoise2), 0) );
	//
	//		SetRespawnPointOnBridge (z, position1.x);
	//	}



	//	private void CreateWaterSteppingStone(float[] noise, int z){
	//		GameObject cube = Instantiate (woodCube, new Vector3(noise[z], -0.9f, z), Quaternion.identity) as GameObject;
	//	}



	//
	//	private void CreateCanyonCube(int z){
	//
	//		Vector3 position = new Vector3 ( 0, 0, z );
	//
	//		for (int x = -numCubeWidth; x < numCubeWidth; x++) {
	//			position.x = x + 0.5f;
	//
	//			//Set Y position
	//			float difference = Mathf.Abs( (x + 0.5f) - canyonNoise[z] );
	//
	//			if (difference > 3f)
	//				position.y = 4f;
	//			else
	//				position.y = 0f;
	//
	//			GameObject cube = Instantiate (cliffCube, position, Quaternion.identity) as GameObject;
	//			cube.transform.localScale = new Vector3 (1, 1, 1);
	//		}
	//
	//		//Create cubestack
	////		if (cubeStackNoise [z] > 0.6f) {
	////			//CreateCubeStack (z);
	////			CreatePillar (z);
	////		}
	//
	//		SetRespawnPointInCanyon (z);
	//	}


	//	private void CreateCanyonSection(int z){
	//
	//		Vector3 position = new Vector3( 0, 0, z );
	//
	//		//Instantiate middle part
	//		GameObject middle = Instantiate (groundMiddle, position, Quaternion.identity) as GameObject;
	//
	//		/*
	//		 * Temporary solution, we need actual model
	//		 */
	//		for (int x = -numCubeWidth * 2; x < numCubeWidth * 2; x++) {
	//			position.x = x + 0.5f;
	//
	//			//Set Y position
	//			float difference = Mathf.Abs( (x + 0.5f) - canyonNoise[z] );
	//
	//			if (difference > 3f) {
	//				position.y = 4f;
	//				GameObject cube = Instantiate (cliffCube, position, Quaternion.identity) as GameObject;
	//				//cube.transform.localScale = new Vector3 (1, 1, 1);
	//			}
	//
	//
	//
	//		}
	//			
	//		SetRespawnPointInCanyon (z);
	//	}


	//	private void CreateGround(int z){
	//
	//		//Great ground floor
	//		CreateGroundSection (z);
	//
	//		//Insert pillars
	//		if ( (pillarNoise [z] > 0.5f && pillarNoise[z] < 0.6f)
	//			|| (pillarNoise [z] > -0.1f && pillarNoise[z] < 0.0f)
	//			|| (pillarNoise[z] > -0.6f && pillarNoise[z] < -0.5f)
	//		) {
	//
	//			CreatePillar(z);
	//		}
	//
	//		//Set respawn point
	//		SetRespawnPointOnGrass (z);
	//	}
}
