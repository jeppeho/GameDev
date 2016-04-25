using UnityEngine;
using System.Collections;

public class LevelGenerator : MonoBehaviour {

	public int levelLength = 400;
	public int minRegularGroundPercentage;
	public int minLavaPercentage;
	public int minCliffPercentage;
	public int minBridgePercentage;
	public bool useWater = true;

	NoiseGenerator NG;

	private int startingCell = 5;

	private int levelWidth = 10;

	private int halfWidth = 6;



	//Gameobjects
	public GameObject planePrefab;
	public GameObject waterSmallPrefab;
	public GameObject stackCube;


	//Level cubes - OBSOLETE WHEN WATER
	public GameObject lavaCube;

	//REAL MODELS
	public GameObject[] groundEdges;
	public GameObject groundMiddle;
	public GameObject[] pillars;
	public GameObject[] pillars_hex;
	public GameObject[] bridgeSteps;
	public GameObject[] sides;
	public GameObject[] steppingStones;
	public GameObject cliff;
	public GameObject waterPrefab;


	private int prevPillarIndex = -1;
	private int prevFirstbridgeStepIndex = -1;
	private int prevSecondbridgeStepIndex = -1;
	private int prevGroundEdgeIndex = -1;
	private int prevSidesIndex = -1;
	private int prevSteppingStoneIndex = -1;


	private enum AreaType { grass, lava, cliff, bridge, start, goal }; 
	private AreaType[] levelAreas; 

	private float[] levelAreaNoise;
	private float[] canyonNoise;
	private float[] bridgeNoise;
	private float[] bridgeNoise2;
	private float[] crystalNoise;
	private float[] crystalPositionNoise;
	private float[] lavaSteppingStonesNoise1;
	private float[] lavaSteppingStonesNoise2;
	private float[] cliffHeightNoise;

	private Vector3[] respawnPoints;
	private float respawnpointOffset;

	// Use this for initialization
	void Start () {

		NG = new NoiseGenerator ();

		NG.SetLevelLength (this.levelLength);

		//Instantiate arrays
		levelAreas = new AreaType[ levelLength ];
		respawnPoints = new Vector3[levelLength * LevelManager.numPlayers];

		generateAcceptableLevel ();

		//Create noise array to use with level elements
		canyonNoise = NG.GetPerlinNoise1D (4, 7, 0.6f, -halfWidth +1, halfWidth -1);
		bridgeNoise = NG.GetPerlinNoise1D (5, 6, 0.5f, -halfWidth, (halfWidth / 2) );
		bridgeNoise2 = NG.GetPerlinNoise1D (5, 6, 1f, (-halfWidth / 2), halfWidth);
		crystalNoise = NG.GetPerlinNoise1D (8, 10, 0.8f, -1, 1);
		crystalPositionNoise = NG.GetPerlinNoise1D (8, 10, 0.8f, -1, 1);
		lavaSteppingStonesNoise1 = NG.GetPerlinNoise1D (4, 10, 0.8f, -halfWidth -1, halfWidth + 1);
		lavaSteppingStonesNoise2 = NG.GetPerlinNoise1D (4, 10, 0.8f, -halfWidth -1, halfWidth + 1);
		cliffHeightNoise = NG.GetPerlinNoise1D (6, 10, 1f, 300, 600);


		//NG.CreateVisualRepresentationSingle (levelAreaNoise, 20f);

		for (int sample = 0; sample < levelLength; sample++) {

			float y = levelAreaNoise [sample];

			CreateSides (sample);

			//TEMPORARY
			if (levelAreas [sample] == AreaType.start){
				//CreateGround (sample);
				CreateGroundSection(sample);

			} else if (levelAreas [sample] == AreaType.cliff) {

				//CreateCanyonSlice (sample);
				CreateCliffSection(sample);

			} else if (levelAreas [sample] == AreaType.lava) {

				CreateLavaSection (sample);

			} else if (levelAreas [sample] == AreaType.bridge) {

				//CreateBridges (sample);
				CreateBridgeStep (sample);

			} else if (levelAreas [sample] == AreaType.grass) {

				//CreateGround(sample);
				CreateGroundSection(sample);

			} else {
				Debug.Log ("Level area type not defined @" + sample + " | " + levelAreas [sample]);
			}
		}

		//Put in water, where needed
		if(useWater)
			CreateWater ();

		//Copy respawn points to the LevelManager
		LevelManager.respawnPoints = respawnPoints;

		NG.ConvertSamplesToUnits (levelAreaNoise);

	}


	/**
	 * Checks that no areas are just a single line. If prev and following
	 * area is different, and those two are the same, then it modifies the middle one to be 
	 * that of those two.
	 */
	private void RemoveIsolatedAreas(){
	
				for (int sample = startingCell + 1; sample < levelLength - 1; sample++) {

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

			if (GetCountOfAreaType (AreaType.lava) < minLavaPercentage)
				accepted = false;

			if (GetCountOfAreaType (AreaType.cliff) < minCliffPercentage)
				accepted = false;

			if (GetCountOfAreaType (AreaType.bridge) < minBridgePercentage)
				accepted = false;

			if (GetCountOfAreaType (AreaType.grass) < minRegularGroundPercentage)
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



		Debug.Log ("WATER = " + GetCountOfAreaType (AreaType.lava));
		Debug.Log ("CANYON = " + GetCountOfAreaType (AreaType.cliff));
		Debug.Log ("GAP = " + GetCountOfAreaType (AreaType.bridge));
		Debug.Log ("GRASS = " + GetCountOfAreaType (AreaType.grass));
	}


	/**
	 * Returns the percentage-wise represenation of a certain area type
	 * in the levelAreas array
	 */
	private float GetCountOfAreaType(AreaType type){
		
		int count = 0;

				for (int sample = startingCell; sample < levelLength; sample++) {
			if (levelAreas [sample] == type) {
				count++;		
			}
		}

				float percent = count / (float)levelLength * 100; 		  

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
				for (int sample = startingCell; sample < levelLength; sample++) {

			float n = levelAreaNoise [sample];

			if (n > -1f && n < -0.3f) {

				levelAreas[sample] = AreaType.lava;
			}
			else if (n > 0.2f && n < 0.4f) {

				levelAreas[sample] = AreaType.cliff;

			} else if( (n > 0.7f && n < 1f ) /*|| (n > 0.4f && n < 0.6f )*/){
				
				levelAreas[sample] = AreaType.bridge;

			} else {
				
				levelAreas[sample] = AreaType.grass;

			}
		}

		//SET GOAL AREA
		for (int sample = levelLength - 6; sample < levelLength; sample++)
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
		if (z < levelLength - 2) {
			AreaType nextArea = levelAreas [z + 1];
			AreaType nextNextArea = levelAreas [z + 2];
			if ( (nextNextArea == AreaType.lava || nextNextArea == AreaType.bridge ) 
				&& nextArea == AreaType.grass ) {

				CreateGroundEdge (z, false);

			}
		}

		//If previous area type is not grass
		if (z >= 2) {
			AreaType prevArea = levelAreas [z - 1];
			AreaType prevPrevArea = levelAreas [z - 2];
			if ( (prevPrevArea == AreaType.lava || prevPrevArea == AreaType.bridge )
				&& prevArea == AreaType.grass) {

				CreateGroundEdge (z, true);

			}
		}

		//Insert pillars
//		if ( (pillarNoise [z] > 0.5f && pillarNoise[z] < 0.6f)
//			|| (pillarNoise [z] > -0.1f && pillarNoise[z] < 0.0f)
//			|| (pillarNoise[z] > -0.6f && pillarNoise[z] < -0.5f)
//		) {

		if (z > 0 && z < levelLength) {

			if (levelAreas [z - 1] == AreaType.grass && levelAreas [z + 1] == AreaType.grass) {

				if (levelAreaNoise [z] < 0.6f) {

					if (z % 10 == 0) {

						CreateCrystalLarge (z, false);

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
		


	/**
	 * Searches through the levelAreas array and covers lava spots with water
	 */
	private void CreateWater(){

		for (int i = startingCell; i < levelLength; i++) {
		
			if (levelAreas [i - 1] != AreaType.lava && levelAreas [i] == AreaType.lava) {

				for (int g = i; g < levelLength; g++) {
				
					if (levelAreas [g + 1] != AreaType.lava && levelAreas [g] == AreaType.lava
						|| g == levelLength - 1
					) {

						//Create water prefab
						int center = i + (g - i) / 2;
						GameObject water = Instantiate (waterPrefab, new Vector3 (0, -0.2f, center), Quaternion.identity) as GameObject;

						//Stretch on the Z-axis
						int length = g - i;
						water.transform.localScale = new Vector3 (20, 0,length);

						//Create floor

						GameObject floor = Instantiate (lavaCube, new Vector3( 0, -1, center), Quaternion.identity) as GameObject;
						floor.transform.localScale = new Vector3 (20, 1,length);
						break;
					}
				}
				
			}

		}
	
	}


	private void CreateLavaSection(int sample){


		if (!useWater) {
			Vector3 position = new Vector3 (0, -1, sample);

			for (int i = -halfWidth - 10; i < halfWidth + 10; i++) {
				position.x = i + 0.5f;
				GameObject cube = Instantiate (lavaCube, position, Quaternion.identity) as GameObject;
			}
		}


		//Don't check array out of bounds
		if ( sample > 0 && sample < levelLength - 2 ) {

			//Only if prev and next area is water
			if (levelAreas [sample - 1] == AreaType.lava && levelAreas [sample + 1] == AreaType.lava) {

				if (sample % 4 == 0) {

					CreateSteppingStone(lavaSteppingStonesNoise1, sample);

					if (levelAreaNoise [sample] < 0.0f) {

						if (sample % 8 == 0) {

							CreateCrystalLarge (sample, true);

						}
					}
						
					SetRespawnPointOnSteppingStone (sample, lavaSteppingStonesNoise1 [sample]);
					SetRespawnPointOnSteppingStone (sample + 1, lavaSteppingStonesNoise1 [sample]);
				} 
				if ((sample + 2) % 4 == 0) {

					CreateSteppingStone(lavaSteppingStonesNoise2, sample);

					SetRespawnPointOnSteppingStone (sample, lavaSteppingStonesNoise2 [sample]);
					SetRespawnPointOnSteppingStone (sample + 1 , lavaSteppingStonesNoise2 [sample]);
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
					if (levelAreas [z-2] != AreaType.cliff) {

						for (int i = 1; i < Mathf.Abs(difference) - 4 && i < 5; i+=2) {

							position.x += Random.Range (-0.2f, 0.2f);
							position.z = z - i ;

							GameObject cliffy2 = Instantiate (cliff, position, Quaternion.identity) as GameObject;
							cliffy2.name = "extra cliff";
							//float height2 = 1000f;
							cliffy2.transform.localScale = new Vector3 (width, height, width);


							//float rotationZ2 = difference * 1.4f;
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



	private void CreateCrystalLarge(int z, bool createHex){

		float x = crystalPositionNoise [z];

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
		
		Vector3 position1 = new Vector3 ( bridgeNoise [z], 0, z );
		Vector3 position2 = new Vector3 ( bridgeNoise2 [z], 0, z );

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
	 * Finds the rotation for a bridge step, by comparing positions with 
	 * the prev and next bridge step.
	 */
	private float GetBrigdeStepRotation(int sample, float[] noise){

		float angle = 0;

		if ( sample > 0 && sample < levelLength - 2 ) {

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
			if (z + i < levelLength) {
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

			respawnPoints [ GetRespawnIndex (z, i) ] = new Vector3(bridgeNoise[z - i], 0.5f, z - i);
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

	public Vector3[] GetRespawnPoints(){
		return this.respawnPoints;
	}
					
}
