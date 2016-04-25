﻿using UnityEngine;
using System.Collections;

public static class LevelManager {

	//CAMERA SPEED
	public static float SPEED = 3f;

	//OLD VALUES
	//public static float MIN_X = 0, MAX_X = 10, MIN_Y = 0, MAX_Y = 30, RELIC_MINZ = -150, MOVE_MINZ = -8, MOVE_MAXZ = 15, MOVE_ZONEWIDTH = 5;

	//NEW VALUES WITH AT ZERO AND START AT ZERO
	//It shouldn't be necessary with both min and max x, as they are centered around 0
	public static float MIN_X = -15, MAX_X = 15, MIN_Y = 0, MAX_Y = 30, RELIC_MINZ = -150, MOVE_MINZ = -9, MOVE_MAXZ = 15, MOVE_ZONEWIDTH = 5;

	public static int numPlayers = 4;

	public static Vector3[] respawnPoints;


	//TODO Should not be made here...
	public static int numSamples = 256;

	public static Vector3 GetRespawnPoint(/*int player,*/ int z){

		int offset = 10;
		int index = 0;

		for (int i = offset; i > 0; i--) {
			if (z + i < numSamples) {
				index = z + i;
				break;
			}

		}
		return respawnPoints[index /* * player */];
	}

}