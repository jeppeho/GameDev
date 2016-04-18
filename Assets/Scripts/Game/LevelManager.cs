using UnityEngine;
using System.Collections;

public static class LevelManager {

	public static float SPEED = 20f;

	public static float MIN_X = 0, MAX_X = 10, MIN_Y = 0, MAX_Y = 30, RELIC_MINZ = -150, MOVE_MINZ = -8, MOVE_MAXZ = 20, MOVE_ZONEWIDTH = 5;

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
			
		Debug.Log ("finding respawn point [" + index + "]");
		return respawnPoints[index /* * player */];
	}

}