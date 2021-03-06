﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LevelGenerator : MonoBehaviour {

	public int levelLength = 400;
	public int beginningAreaLength = 30;
	public int goalAreaLength = 20;

	public float numElevationSteps = 10f;
	public float elevationHeight = 5f;

	public int minLowGroundPercentage;
	public int minHighGroundPercentage;
	public int minLavaPercentage;
	public int minCliffPercentage;
	public int minBridgePercentage;
	public bool useWater = true;

	NoiseGenerator NG;

	private int startingCell = 5;
	private int levelWidth = 10;
	private int halfWidth = 6;


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
	public GameObject pillarSmallHex;
	public GameObject[] bridgeSteps;
	public GameObject[] sides;
	public GameObject[] steppingStones;
	public GameObject cliff;
	public GameObject waterPrefab;
	public GameObject boulderPrefab;
	public GameObject pitFog;
	public GameObject goal;


	private int prevLargeCrystalIndex = -1;
	private int prevSmallCrystalIndex = -1;
	private int prevFirstbridgeStepIndex = -1;
	private int prevSecondbridgeStepIndex = -1;
	private int prevGroundEdgeIndex = -1;
	private int prevSidesIndex = -1;
	private int prevSteppingStoneIndex = -1;


	public enum AreaType { lowGround, highGround, lava, cliff, bridge, start, goal }; 
	public AreaType[] levelAreas; 

	private float[] levelAreaNoise;
	private float[] levelAreaHeightNoise;
	private float[] canyonNoise;
	private float[] bridgeNoise;
	private float[] bridgeNoise2;
	private float[] crystalNoise;
	private float[] crystalPositionNoise;
	private float[] smallCrystalPositionNoise;
	private float[] lavaSteppingStonesNoise1;
	private float[] lavaSteppingStonesNoise2;
	private float[] lavaSteppingStonesNoise3;
	private float[] cliffHeightNoise;

	//Respawn
	private Vector3[] respawnPoints;
	private float respawnpointOffset;

	//Camera
//	private float[] cameraPositionRoute;
//	private float[] cameraLookAtRoute;


	//The parent object for all level elements
	private GameObject levelContainer;

	// Use this for initialization
	void Start () {

		levelContainer = GameObject.Find ("LevelElements");

		NG = new NoiseGenerator ();

		NG.SetLevelLength (this.levelLength);

		//Instantiate arrays
		levelAreas = new AreaType[ levelLength ];
		respawnPoints = new Vector3[levelLength * LevelManager.numPlayers];
		levelAreaHeightNoise = new float[ levelLength ];
//		cameraPositionRoute = new float[levelLength];
//		cameraLookAtRoute = new float[levelLength];

		generateAcceptableLevel ();

		//Create noise array to use with level elements
		canyonNoise = NG.GetPerlinNoise1D (4, 7, 0.6f, -halfWidth +1, halfWidth -1);
		bridgeNoise = NG.GetPerlinNoise1D (5, 6, 0.5f, -halfWidth * 1.5f, (halfWidth / 2) );
		bridgeNoise2 = NG.GetPerlinNoise1D (5, 6, 1f, (-halfWidth / 2), halfWidth * 1.5f);
		crystalNoise = NG.GetPerlinNoise1D (8, 10, 0.8f, -1, 1);
		crystalPositionNoise = NG.GetPerlinNoise1D (8, 10, 0.8f, -1, 1);
		smallCrystalPositionNoise = NG.GetPerlinNoise1D (6, 10, 0.5f, -halfWidth, halfWidth);
		lavaSteppingStonesNoise1 = NG.GetPerlinNoise1D (4, 10, 0.8f, -halfWidth -1, halfWidth/3);
		lavaSteppingStonesNoise2 = NG.GetPerlinNoise1D (4, 10, 0.8f, -halfWidth / 3, halfWidth + 1);

		cliffHeightNoise = NG.GetPerlinNoise1D (1, 10, 1f, 50, 600);


		//NG.CreateVisualRepresentationSingle (levelAreaNoise, 20f);

		//CreateStartSection (1);

		for (int sample = 0; sample < levelLength; sample++) {

			float y = levelAreaNoise [sample];

			CreateSides (sample);

			CreateBridgeBackwall (sample);

			//TEMPORARY
			if (levelAreas [sample] == AreaType.start){
				//CreateGround (sample);
				CreateLowGroundSection(sample);

			} else if (levelAreas [sample] == AreaType.cliff) {

				//CreateCanyonSlice (sample);
				CreateCliffSection(sample);

			} else if (levelAreas [sample] == AreaType.lava) {

				CreateLavaSection (sample);

			} else if (levelAreas [sample] == AreaType.bridge) {
				
				if(sample % 2 == 0)
					CreateBridgeStep (sample);

			} else if (levelAreas [sample] == AreaType.lowGround) {

				//CreateGround(sample);
				CreateLowGroundSection(sample);

			}else if (levelAreas [sample] == AreaType.highGround) {

				//CreateGround(sample);
				//CreateLowGroundSection(sample);
				CreateHighGroundSection(sample);

			} else {
				Debug.Log ("Level area type not defined @" + sample + " | " + levelAreas [sample]);
			}
		}


		//Create the last edge
		//CreateGroundEdge (levelLength - 7, false);

		//Create goal
		CreateGoalSection();

		//Put in water, where needed
		if(useWater)
			CreateWater ();

		//CreatePitFog ();

		InsertBoulders ();

//		CreateCameraRoute ();

		//Copy respawn points to the LevelManager
		LevelManager.respawnPoints = respawnPoints;

		SetLevelAreaHeights ();

	}


	//----------------------------------
	// PUBLIC METHODS FOR RETRIEVING 
	// THE DIFFERENT NOISE ARRAYS
	//----------------------------------



	public float[] GetCanyonNoise(){
		return this.canyonNoise;
	}

	public float[] GetBridge1Noise(){
		return this.bridgeNoise;
	}

	public float[] GetSteppingStone1Noise(){
		return this.lavaSteppingStonesNoise1;
	}

	public float[] GetLevelAreaNoise(){
		return this.levelAreaNoise;
	}

	public AreaType[] GetLevelAreas(){
		return this.levelAreas;
	}




	//----------------------------------
	// PRIVATE METHODS USED TO BUILD, 
	// CHANGE AND MODIFY THE LEVEL
	//----------------------------------

	/**
	 * Makes the provided gameObject a child of the levelContainer
	 */
	private void SetContainerAsParent(GameObject g){
		g.transform.SetParent (levelContainer.transform);
	}


	/**
	 * Checks that no areas are just a single line. If prev and following
	 * area is different, and those two are the same, then it modifies the middle one to be 
	 * that of those two.
	 */
	private void RemoveIsolatedAreas(){
	
		for (int sample = startingCell; sample < levelLength; sample++) {

			//Remove areas of length 1
			if (levelAreas[ GetValidLevelIndex( sample - 1) ] != levelAreas[sample] 
				&& levelAreas [ GetValidLevelIndex(sample + 1) ] != levelAreas [sample]
				&& levelAreas[ GetValidLevelIndex( sample - 1) ] == levelAreas [ GetValidLevelIndex( sample + 1 ) ])
			{
				levelAreas [sample] = levelAreas [sample - 1]; 
			}

			//Remove areas of length 2
			if (levelAreas [sample] == levelAreas [ GetValidLevelIndex( sample + 1) ]
				&& levelAreas [ GetValidLevelIndex(sample - 1) ] != levelAreas [sample]
				&& levelAreas [ GetValidLevelIndex(sample - 1) ] == levelAreas [ GetValidLevelIndex(sample + 2) ]) 
			{
				levelAreas[sample] = levelAreas [ GetValidLevelIndex(sample - 1) ]; 
				levelAreas[sample + 1] = levelAreas [ GetValidLevelIndex(sample - 1) ]; 
			}
		}
	}


	private void generateAcceptableLevel(){

		bool accepted = false;
		int numTests = 20000;

		//To keep score of how many tries it took to get an accepted
		int acceptedNum = 0;

		//Debug.Log ("Generating acceptable level with " + numTests + " tries");

		while (!accepted) {

			accepted = true;
		
			levelAreaNoise = NG.GetPerlinNoise1D (3, 6, 0.6f, -1, 1);
			InsertLevelBeginningArea ();
			InsertGoalArea ();
			SetLevelAreaArray ();

			if (GetCountOfAreaType (AreaType.lava) < minLavaPercentage)
				accepted = false;

			else if (GetCountOfAreaType (AreaType.cliff) < minCliffPercentage)
				accepted = false;

			else if (GetCountOfAreaType (AreaType.bridge) < minBridgePercentage)
				accepted = false;

			else if (GetCountOfAreaType (AreaType.lowGround) < minLowGroundPercentage)
				accepted = false;

			else if (GetCountOfAreaType (AreaType.highGround) < minHighGroundPercentage)
				accepted = false;
			
				
			numTests--;
			//Dont let it continue forever
			if (numTests < 0) {
				accepted = true;
				Debug.Log ("No succes for generating acceptable level");
			}


			//if (accepted == true)
				//Debug.Log (acceptedNum + " tries to generate accepted level!");
			//else
            if (accepted != true)
				acceptedNum++;
		}

		//Print percentages of area types for accepted level
		//Debug.Log ("WATER = " + GetCountOfAreaType (AreaType.lava));
		//Debug.Log ("LOW GROUND = " + GetCountOfAreaType (AreaType.lowGround));
		//Debug.Log ("CANYON = " + GetCountOfAreaType (AreaType.cliff));
		//Debug.Log ("HIGH GROUND = " + GetCountOfAreaType (AreaType.highGround));
		//Debug.Log ("GAP = " + GetCountOfAreaType (AreaType.bridge));
	}

	/**
	 * Used to set the first part of the level to a specific area type.
	 * Sets the first levelAreaNoise index to zero and lerps between that 
	 * a number of level sections back, specified by the public variable beginningAreaLength.
	 */
	private void InsertLevelBeginningArea(){

		float a = 0f;
		float b = levelAreaNoise [beginningAreaLength];

		for (int sample = 0; sample <= beginningAreaLength; sample++) {

			float t = (float)sample / (float)beginningAreaLength;
			float v = Mathf.Lerp( a, b, t);

			levelAreaNoise [sample] = v;
		}
	}


	/**
	 * Used to set the last part of the level to a specific area type.
	 * Sets the last levelAreaNoise index to zero and lerps between that 
	 * a number of level sections back, specified by the public variable goalAreaLength.
	 */
	private void InsertGoalArea(){

		float a = levelAreaNoise[ levelLength - goalAreaLength ];
		float b = 0f;


		int firstSample = levelLength - goalAreaLength;

		for(int sample = 0; sample < goalAreaLength; sample++){

			float t = (float)sample / (float)(goalAreaLength - 1f);
			float v = Mathf.Lerp (a, b, t);

			levelAreaNoise [firstSample + sample] = v;

		}
	
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

		int startLength = 20;

		//SET START LINE
		for (int sample = 0; sample < startingCell; sample++)
			levelAreas[sample] = AreaType.start;


		//SET OBSTACLE COURSE
		for (int sample = startingCell; sample < levelLength; sample++) {

			float n = levelAreaNoise [sample];

			if (n >= -1f && n < -0.3f) {

				levelAreas[sample] = AreaType.lava;
			}
			else if(n >= -0.3f && n <= 0.2f){
				
				levelAreas[sample] = AreaType.lowGround;
			
			}
			else if (n > 0.2f && n < 0.4f) {

				levelAreas[sample] = AreaType.cliff;

			} 
			else if(n >= 0.4f && n <= 0.7f){

				levelAreas[sample] = AreaType.highGround;

			}
			else if( (n > 0.7f && n < 1f ) /*|| (n > 0.4f && n < 0.6f )*/){
				
				levelAreas[sample] = AreaType.bridge;

			} 
		}

		//MODIFY AREAS OF LENGTH 1
		RemoveIsolatedAreas ();
	
	}


	private void SetLevelAreaHeights(){

		//float[] levelAreaHeightNoise = GetLevelAreaNoise ();

		for(int z = 0; z < levelLength; z++){
			
			switch (levelAreas[z]) {

			case AreaType.start:
				levelAreaHeightNoise[z] = levelAreaNoise [z];
				break;
			case AreaType.lava:
				levelAreaHeightNoise[z] = 0;
				break;
			case AreaType.lowGround:
				levelAreaHeightNoise[z] = levelAreaNoise [z] + 0.3f;
				break;
			case AreaType.cliff:
				levelAreaHeightNoise[z] = levelAreaNoise [z] + 0.3f;
				break;
			case AreaType.highGround:
				levelAreaHeightNoise[z] = levelAreaNoise [z] + 0.3f;
				break;
			case AreaType.bridge:
				levelAreaHeightNoise[z] = 0.8f;
				break;
			default:
				Debug.Log ("Noise not registered in SetLevelAreaHeights()");
				break;
			}

			levelAreaHeightNoise[z] *= numElevationSteps;

			levelAreaHeightNoise [z] = Mathf.FloorToInt (levelAreaHeightNoise [z]) * elevationHeight / 10f;
		}
	}



	public float[] GetLevelAreaHeights(){
		return this.levelAreaHeightNoise;
	}



	//----------------------------------
	// METHODS FOR CREATING A SECTION OF THE LEVEL
	// A section is one step deep on the Z-axis
	//----------------------------------


	/**
	 * Creates the ground floor for the
	 */
	private void CreateStartSection(int firstZ){

		for (int i = 0; i < 10; i++) {

			int z = firstZ - i - 2;
			Debug.Log ("start z = " + z);

			CreateGround (z);
		}
	}


	/**
	 * Creates the high ground between canyons and bridges, where the smalls crystals grow. 
	 * 
	 */
	private void CreateHighGroundSection(int z){

		CreateGround (z);


//		Vector3 position = new Vector3( 0, 0, z );
//
//		//Instantiate middle part
//		GameObject ground = Instantiate (groundMiddle, position, Quaternion.identity) as GameObject;
//
//		//Set levelContainer as parent
//		SetContainerAsParent (ground);
//
//		//If next area type is not grass
//
//		AreaType nextArea = levelAreas [ GetValidLevelIndex(z + 1) ];
//		AreaType nextNextArea = levelAreas [ GetValidLevelIndex(z + 2) ];
//		if ( (nextNextArea == AreaType.lava || nextNextArea == AreaType.bridge ) 
//			&& nextArea == AreaType.lowGround || nextArea == AreaType.highGround ) {
//
//			CreateGroundEdge (z, false);
//
//		}
//
//
//		//If previous area type is not grass
//		AreaType prevArea = levelAreas [ GetValidLevelIndex(z - 1) ];
//		AreaType prevPrevArea = levelAreas [ GetValidLevelIndex(z - 2) ];
//		if ( (prevPrevArea == AreaType.lava || prevPrevArea == AreaType.bridge )
//			&& prevArea == AreaType.lowGround || prevArea == AreaType.highGround) {
//
//			CreateGroundEdge (z, true);
//
//		}


		//Insert small and large pillars
		if (levelAreas [ GetValidLevelIndex(z - 1) ] == AreaType.highGround && levelAreas [ GetValidLevelIndex(z + 1) ] == AreaType.highGround) {

			if (levelAreaNoise [z] < 0.6f && levelAreaNoise[z] > 0.3f) {
				if (z % 3 != 0 /*&& z % 10 != 0*/) {

					CreateCrystalGroupSmall (z);

				}
			}
		}

		//Set respawn point
		SetRespawnPointOnGrass (z);
	
	}


	/**
	 * Creates two inversed random edges, used for ground
	 */
	private void CreateGround(int z){
		CreateGroundEdge(z, false);
		CreateGroundEdge (z, true);
	}


	private void CreateLowGroundSection(int z){

		CreateGround (z);


		//If not in the beginning og the level
		if (z > 7) {
			
			//If prev and next area is lowGround
			if (levelAreas [GetValidLevelIndex (z - 1)] == AreaType.lowGround && levelAreas [GetValidLevelIndex (z + 1)] == AreaType.lowGround) {

				//if (levelAreaNoise [z] > -0.5f  && levelAreaNoise[z] < 0.3f) {

				//Vary probability on distance to goal - the closer the more crystals
				int probablity = Mathf.FloorToInt (10 - levelLength / (levelLength - z + 30));

				if (z % probablity == 0) {

					//Insert a large crystal
					CreateCrystalLarge (z, false);
				}
				//}
			}
		}
			
		//Set respawn point
		SetRespawnPointOnGrass (z);
	}


	private void CreateLavaSection(int sample){

		//Create floor
		GameObject floor = Instantiate (lavaCube, new Vector3( 0, -1.5f, sample), Quaternion.identity) as GameObject;
		floor.transform.localScale = new Vector3 (levelWidth * 4, 1, 1);

		//Set levelContainer as parent
		SetContainerAsParent (floor);


		int distBetweenSteps = 6;
		int numPaths = 2;

		//Don't check array out of bounds
		//if ( sample > 0 && sample < levelLength - distBetweenSteps ) {

		//Only if prev and next area is water
		if (levelAreas [ GetValidLevelIndex(sample - 1) ] == AreaType.lava && levelAreas [ GetValidLevelIndex(sample + 1) ] == AreaType.lava) {

			if (sample % distBetweenSteps == 0) {

				CreateSteppingStone(lavaSteppingStonesNoise1, sample);

				if (levelAreaNoise [sample] < 0.0f) {

					if (sample % 8 == 0) {

						CreateCrystalLarge (sample, true);

					}
				}
					
				for(int i = 0; i < distBetweenSteps; i++)
					SetRespawnPointOnSteppingStone (sample + i, lavaSteppingStonesNoise1 [sample]);
			} 

			else if ((sample + distBetweenSteps / numPaths) % distBetweenSteps == 0) {

				CreateSteppingStone(lavaSteppingStonesNoise2, sample);

				for(int i = 0; i < distBetweenSteps; i++)
					SetRespawnPointOnSteppingStone (sample + i, lavaSteppingStonesNoise2 [sample]);
			}
		}
	}


	/**
	 * Searches through the levelAreas array and covers lava spots with water
	 */
	private void CreateWater(){

		for (int sample = startingCell; sample < levelLength; sample++) {

			if (levelAreas [sample - 1] != AreaType.lava && levelAreas [sample] == AreaType.lava) {

				for (int g = sample; g < levelLength; g++) {

					if (levelAreas [g + 1] != AreaType.lava && levelAreas [g] == AreaType.lava
						|| g == levelLength - 1
					) {

						//Create water prefab
						float center = sample + (g - sample) / 2f;

						float y = -0.8f;//value without elevation = -0.7f;

						GameObject water = Instantiate (waterPrefab, new Vector3 (0, y, center), Quaternion.identity) as GameObject;
					
						//Set levelContainer as parent
						SetContainerAsParent (water);

						//Stretch on the Z-axis
						float length = (g - sample) / 10f;// * 2 / 3.7f;
						//water.transform.localScale = new Vector3 (100, 0,length);
						water.transform.localScale = new Vector3 (6, 0,length);

						//Create floor
//						GameObject floor = Instantiate (lavaCube, new Vector3( 0, -1.5f, center), Quaternion.identity) as GameObject;
//
//						//Set levelContainer as parent
//						SetContainerAsParent (floor);
//
//						length *= 2; 
//						floor.transform.localScale = new Vector3 (20, 1,length);
						break;
					}
				}	
			}
		}
	}


	private void CreateCliffSection(int z){

		Vector3 position = new Vector3 ( 0, 0, z );

		CreateGround (z);

		//Set levelContainer as parent
		//SetContainerAsParent (ground);

		if (z % 2 == 0) {

			for (int x = -halfWidth - 5; x < halfWidth + 5; x += 2) {

				//Check if to close to road
				float difference = (x + 0.5f) - canyonNoise [z];

				if ( Mathf.Abs ( difference) > 4f) {

					position.x = Random.Range (x - 0.2f, x + 0.2f);

					//Instantiate object
					GameObject cliffy = Instantiate (cliff, position, Quaternion.identity) as GameObject;
					cliffy.name = "normal cliff";
					float height = cliffHeightNoise [z] + Mathf.Abs ( difference ) * 30;

					//Set levelContainer as parent
					SetContainerAsParent (cliffy);

					//Rescale
					float width = Random.Range (110, 150);
					cliffy.transform.localScale = new Vector3 (width, height, width);

					//Rotate
					float rotationZ = difference * 1f;
					cliffy.transform.RotateAround (position, new Vector3 (0, 0, 1), -rotationZ);

					//ADD NARROWING AREA IN FRONT OF CANYON
					if (levelAreas [ GetValidLevelIndex(z - 2) ] != AreaType.cliff) {

						for (int i = 1; i < Mathf.Abs(difference) - 4 && i < 5; i+=2) {

							position.x += Random.Range (-0.2f, 0.2f);
							position.z = z - i ;

							GameObject cliffy2 = Instantiate (cliff, position, Quaternion.identity) as GameObject;

							//Set levelContainer as parent
							SetContainerAsParent (cliffy2);

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


	private void CreateGoalSection(){

		Vector3 position = new Vector3 (0, 1, levelLength + 18);

		GameObject goal_ = Instantiate (goal, position, Quaternion.identity) as GameObject;

		SetContainerAsParent (goal_);

		//Create Sides on left and right side
		for (int i = 0; i <40; i += 10) {
			//float scale = Random.Range(90, 110);
			int min = 130;
			int max = 150;

			Vector3 scalar = new Vector3 (Random.Range (min, max), Random.Range (min, max), Random.Range (min, max));
			int rotationVariator = 30;
			float x = halfWidth + 10;

			Vector3 positionLeft = new Vector3 (x * -1.5f, 0, levelLength + i);
			Vector3 positionRight = new Vector3 (x * 1.5f, 0, levelLength + i);

			//Get index for model
			int index = NG.GetRandomButNotPreviousValue (0, sides.Length, prevSidesIndex);

			GameObject leftSide = Instantiate (sides [index], positionLeft, Quaternion.identity) as GameObject;
			leftSide.transform.RotateAround (positionLeft, new Vector3 (0, 1, 0), Random.Range (-rotationVariator, rotationVariator) + 180);
			leftSide.transform.localScale = scalar;

			//Set levelContainer as parent
			SetContainerAsParent (leftSide);

			//Get new index for model
			index = NG.GetRandomButNotPreviousValue (0, sides.Length, index);

			GameObject rightSide = Instantiate (sides [index], positionRight, Quaternion.identity) as GameObject;
			rightSide.transform.RotateAround (positionRight, new Vector3 (0, 1, 0), Random.Range (-rotationVariator, rotationVariator));
			rightSide.transform.localScale = scalar;

			//Set levelContainer as parent
			SetContainerAsParent (rightSide);

			prevSidesIndex = index;
		}


		//Create Sides in the end
		Vector3 posLeft = new Vector3 (-10, 0, levelLength + 40);
		Vector3 posRight = new Vector3 (10, 0, levelLength + 40);
		Vector3 scale = new Vector3 (150, 150, 150);

		//Get index for model
		int _index = NG.GetRandomButNotPreviousValue (0, sides.Length, prevSidesIndex);
	
		GameObject _leftSide = Instantiate (sides [_index], posLeft, Quaternion.identity) as GameObject;
		_leftSide.transform.RotateAround (posLeft, new Vector3 (0, 1, 0), 90);
		_leftSide.transform.localScale = scale;

		GameObject _rightSide = Instantiate (sides [_index], posRight, Quaternion.identity) as GameObject;
		_rightSide.transform.RotateAround (posRight, new Vector3 (0, 1, 0), 90);
		_rightSide.transform.localScale = scale;

		//Set levelContainer as parent
		SetContainerAsParent (_leftSide);
		SetContainerAsParent (_rightSide);

	}


	/**
	 * Inserts boulders on the whole level, if the minimum distance to prev boulder is reached
	 * or an area with lots of cliffs is approaching
	 */
	private void InsertBoulders(){

		int lastBoulderZ = 0;
		int minBoulderDistance = 70;

		for (int z = 0; z < levelLength; z++) {

			if (levelAreas [z] == AreaType.cliff && levelAreas [ GetValidLevelIndex(z - 1) ] != AreaType.cliff) {

				//Count number of nearby oncoming cliffs
				int numCliffs = 0;
				for (int i = 0; i < 20; i++) {
					if (levelAreas [z] == AreaType.cliff) {
						numCliffs++;
					}
				}

				int lastBoulderDistance = z - lastBoulderZ;

				if (lastBoulderDistance > minBoulderDistance || (numCliffs > 12 && lastBoulderDistance > 10)) {

					float x = canyonNoise [z];

					int rotation = 40 + Random.Range (-50, 50); //Unnecesary if no boulder bottom

					//Put boulders on the sides
//					if (Random.Range (0f, 1f) > 0.5f)
//						x += 7;
//					else
//						x -= 7;

					//x += rotation / 15;

					CreateBoulder (x, z, rotation);

					lastBoulderZ = z;
				}

			}
		}
	}





	//----------------------------------
	// METHODS FOR INSTANTIATING AND PLACING OBJECTS
	// Objects such as pillars, sides and stepping stones
	//----------------------------------


	private void CreateGroundEdge(int z, bool invertOnZ){

		Vector3 position = new Vector3 (0, 0, z);


		int index = NG.GetRandomButNotPreviousValue (0, groundEdges.Length, prevGroundEdgeIndex);
		GameObject edge = Instantiate (groundEdges [index], position, Quaternion.identity) as GameObject;

		//Set levelContainer as parent
		SetContainerAsParent (edge);

		Vector3 scale = edge.transform.localScale;
		scale.y = 30;
		edge.transform.localScale = scale;

		//Invert on z-axis
		if (invertOnZ) {
			edge.transform.RotateAround (position, new Vector3 (0, 1, 0), 180);
		}

		prevGroundEdgeIndex = index;

	}



	private void CreateBridgeBackwallEdge(Vector3 position){
	
		int index = NG.GetRandomButNotPreviousValue (0, groundEdges.Length, prevGroundEdgeIndex);
		GameObject edge = Instantiate (groundEdges [index], position, Quaternion.identity) as GameObject;


		//Stretch down
		Vector3 scale = edge.transform.localScale;
		scale.y = 100;
		scale.z *= 1.5f;
		edge.transform.localScale = scale;

		edge.transform.RotateAround (position, new Vector3 (0, 1, 0), 180);

		SetContainerAsParent (edge);
	
	}

//	private void CreateBridgeSideEdge(int z, bool invertOnZ){
//
//		Vector3 position = new Vector3 (-22, 0, z);
//		int rotation = 90;
//		//Invert on z-axis
//		if (invertOnZ) {
//			rotation += 180;
//			position.x *= -1;
//		}
//
//		int index = NG.GetRandomButNotPreviousValue (0, groundEdges.Length, prevGroundEdgeIndex);
//		GameObject edge = Instantiate (groundEdges [index], position, Quaternion.identity) as GameObject;
//		SetContainerAsParent (edge);
//
//		Vector3 scale = edge.transform.localScale;
//		scale.y = 30;
//		edge.transform.localScale = scale;
//
//		edge.transform.RotateAround (position, new Vector3 (0, 1, 0), rotation);
//
//		prevGroundEdgeIndex = index;
//
//	}



	private void CreateCrystalLarge(int z, bool createHex){

		float x = crystalPositionNoise [z];

		int pillarIndex = NG.GetRandomButNotPreviousValue (0, pillars.Length, prevLargeCrystalIndex);

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

		//Set levelContainer as parent
		SetContainerAsParent (pillar);

		//Set size
		float size = Random.Range (80, 110);
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
		prevLargeCrystalIndex = pillarIndex;

	}


	private void CreateCrystalGroupSmall(int z){

		float x = Random.Range (-halfWidth + 1f, halfWidth - 1f);
		x = smallCrystalPositionNoise [z];
		Vector3 position = new Vector3( x, 0, z );

		//int index = NG.GetRandomButNotPreviousValue (0, pillars.Length, prevSmallCrystalIndex);

		GameObject pillar = Instantiate (pillarSmallHex/*pillars_hex [index]*/, position, Quaternion.identity) as GameObject;

		//Set levelContainer as parent
		SetContainerAsParent (pillar);

		float scale = Random.Range (50, 80);
		pillar.transform.localScale = new Vector3 (scale, scale, scale);
		pillar.transform.RotateAround( position, new Vector3(0,1,0), Random.Range(0, 360));

		//prevSmallCrystalIndex = index;

	}


	private void CreateBoulder(float x, int z, int rotation){

		//Set position
		Vector3 position = new Vector3 (x, 6f, z);

		//Instantiate boulder
		GameObject boulder = Instantiate (boulderPrefab, position, Quaternion.identity) as GameObject; 

		//Set levelContainer as parent
		SetContainerAsParent (boulder);

		//Rotate boulder around Y axis
		boulder.transform.RotateAround( position, new Vector3(0,1,0), rotation);

	}


	private void CreateSteppingStone(float[] noise, int z){
		int index = NG.GetRandomButNotPreviousValue (0, steppingStones.Length, prevSteppingStoneIndex);

		Quaternion randomRotation = Quaternion.Euler (0, Random.Range (0, 360), 0);

		GameObject steppingStone = Instantiate(steppingStones[index], new Vector3(noise[z], 0, z), randomRotation) as GameObject;

		//Set levelContainer as parent
		SetContainerAsParent (steppingStone);

		int scale = Random.Range (50, 65);
		steppingStone.transform.localScale = new Vector3 (scale, scale, scale);

		prevSteppingStoneIndex = index;
	}
		 

	private void CreateBridgeBackwall(int z){

		if (levelAreas [z] == AreaType.bridge && levelAreas [ GetValidLevelIndex(z + 1) ] != AreaType.bridge) {

			int width = 80;

			for (int i = -width; i < width; i += 10) {

				//Avoid putting in sides on the center of the level
				if (i == -20)
					i = 20;

				Vector3 sidePosition = new Vector3 (i, 0, z + 6);

				CreateSideElement (sidePosition, -90f);

				Vector3 edgePosition = new Vector3 (i, 0, z + 2);

				CreateBridgeBackwallEdge (edgePosition);
			}
		}
	}




	private void CreateSides(int z){

		bool toCloseToGap = false;

		//Put in every 10th slice or add an edge
		if (z % 10 == 0) {

			//Check if a gap is nearby
			for (int i = 0; i < 7; i++) {
				if ( levelAreas [ GetValidLevelIndex(z - i) ] == AreaType.bridge ) {
					toCloseToGap = true;
					break;
				}
			}

			if (!toCloseToGap) {
		
				//float scale = Random.Range(90, 110);
				int min = 130;
				int max = 150;

				Vector3 scalar = new Vector3 (Random.Range(min, max), Random.Range(min, max), Random.Range(min, max));
				int rotationVariator = 30;
				float x = halfWidth + 10;



				Vector3 positionLeft = new Vector3 (-x, 0, z);
				Vector3 positionRight = new Vector3 (x, 0, z);

				float rotationLeft = Random.Range (-rotationVariator, rotationVariator) + 180;
				float rotationRight = Random.Range (-rotationVariator, rotationVariator);

				CreateSideElement (positionLeft, rotationLeft);
				CreateSideElement (positionRight, rotationRight);
			}
		}
	}


	private void CreateSideElement(Vector3 position, float rotation){

		int min = 130;
		int max = 150;

		Vector3 scalar = new Vector3 (Random.Range(min, max), Random.Range(min, max), Random.Range(min, max));

		//Vector3 position = new Vector3 (i, 0, z);

		//Get index for model
		int index = NG.GetRandomButNotPreviousValue (0, sides.Length, prevSidesIndex);

		//Instantiate side prefab
		GameObject side = Instantiate (sides [index], position, Quaternion.identity) as GameObject;

		//Rotate the side
		side.transform.RotateAround (position, new Vector3 (0, 1, 0), rotation);

		//Scale the side
		side.transform.localScale = scalar;

		//Set as child of the levelElement object 
		SetContainerAsParent (side);	
	}


	private void CreateBridgeStep(int z){

			Vector3 position1 = new Vector3 (bridgeNoise [z], 0, z);
			Vector3 position2 = new Vector3 (bridgeNoise2 [z], 0, z);

			int firstBridgeIndex = NG.GetRandomButNotPreviousValue (0, bridgeSteps.Length, prevFirstbridgeStepIndex);
			int secondBridgeIndex = NG.GetRandomButNotPreviousValue (0, bridgeSteps.Length, prevSecondbridgeStepIndex);

			//Instantiate steps
			GameObject step1 = Instantiate (bridgeSteps [firstBridgeIndex], position1, Quaternion.identity) as GameObject;
			GameObject step2 = Instantiate (bridgeSteps [secondBridgeIndex], position2, Quaternion.identity) as GameObject;

			//Set levelContainer as parent
			SetContainerAsParent (step1);
			SetContainerAsParent (step2);

			//Rotate around Y-axis
			step1.transform.RotateAround (position1, new Vector3 (0, 1, 0), Random.Range (0, 360));
			step2.transform.RotateAround (position2, new Vector3 (0, 1, 0), Random.Range (0, 360));

			//Set scale
			float size = Random.Range (45, 55);
			step1.transform.localScale = new Vector3 (size, 1500, size);
			size = Random.Range (45, 55);
			step2.transform.localScale = new Vector3 (size, 1500, size);

//		if (z % 20 == 0 || (z > 0 && levelAreas[z - 1] != AreaType.bridge ) || (z < levelLength && levelAreas[z + 1] != AreaType.bridge ) ) {
//				CreateBridgeSideEdge (z, true);
//				CreateBridgeSideEdge (z, false);
//			}

			SetRespawnPointOnBridge (z, position1.x);
			SetRespawnPointOnBridge (z+1, position1.x);
	}


	/**
	 * Finds the rotation for a bridge step, by comparing positions with 
	 * the prev and next bridge step.
	 */
	private float GetBrigdeStepRotation(int sample, float[] noise){

		float angle = 0;

		if ( sample > 0 && sample < levelLength - 2 ) {

			//Set positions for prev, current and next bridge step
			Vector2 prevPos = new Vector2 ( -1f, noise[ GetValidLevelIndex( sample - 1 ) ] );
			Vector2 thisPos = new Vector2 ( 0f, noise[ sample ] );
			Vector2 nextPos = new Vector2 ( 1f, noise[ GetValidLevelIndex( sample + 1 ) ] );

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


	private int GetValidLevelIndex(int z){

		if (z < 0)
			z = 0;
		else if (z >= levelLength)
			z = levelLength - 1;

		return z;
	}


	//----------------------------------
	// RESPAWN METHODS
	//----------------------------------

	public Vector3 GetRespawnPoint(/*int player,*/ int z){

		int offset = 10;
		int index = 0;

		for (int i = offset; i > 0; i--) {
			if ( GetValidLevelIndex(z + i) < levelLength) {
				index = GetValidLevelIndex(z + i);
			}
		
		}

		return this.respawnPoints[index /* * player */];
	}



	private void SetRespawnPointInCanyon(int z){

		for (int i = 0; i < LevelManager.numPlayers; i++) {

			respawnPoints [ GetRespawnIndex (z, i) ] = new Vector3(canyonNoise[ GetValidLevelIndex(z - i) ], 0.5f, z - i);
		}
	}

	private void SetRespawnPointOnBridge(int z, float x){
	
		for (int i = 0; i < LevelManager.numPlayers; i++) {

			respawnPoints [ GetRespawnIndex (z, i) ] = new Vector3(bridgeNoise[ GetValidLevelIndex(z - i) ], 0.5f, z - i);
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
		return GetValidLevelIndex( z * (player + 1) );
	}

	public Vector3[] GetRespawnPoints(){
		return this.respawnPoints;
	}
					
}
