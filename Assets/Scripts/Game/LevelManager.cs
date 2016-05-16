using UnityEngine;
using System.Collections;

public static class LevelManager {

	//CAMERA SPEED
	public static float SPEED = 2.5f;

	//OLD VALUES
	//public static float MIN_X = 0, MAX_X = 10, MIN_Y = 0, MAX_Y = 30, RELIC_MINZ = -150, MOVE_MINZ = -8, MOVE_MAXZ = 15, MOVE_ZONEWIDTH = 5;

	//NEW VALUES WITH AT ZERO AND START AT ZERO
	//It shouldn't be necessary with both min and max x, as they are centered around 0
	public static float MIN_X = -15, MAX_X = 15, MIN_Y = 0, MAX_Y = 30, RELIC_MINZ = -150, MOVE_MINZ = -9, MOVE_MAXZ = 10, MOVE_ZONEWIDTH = 10;

	public static int numPlayers = 4;

	public static Vector3[] respawnPoints;

	//Should be set by the levelGenerator somehow...
	public static int levelLength = 1000;


	/**
	 * Returns a respawn point, based on the current z-position
	 */
	public static Vector3 GetRespawnPoint(/*int player,*/ int z){

		LevelGenerator lg = GameObject.Find ("LevelGenerator").GetComponent<LevelGenerator>();

		int offset = 5;
		int index = 0;

		//Try at z + offset, otherwise move closer to camera
		for (int i = offset; i > 0; i--) {
			if (z + i < lg.levelLength) {
				index = z + i;
				break;
			}

		}
//		Debug.Log ("levelLength = " + lg.levelLength);
//		Debug.Log ("index = " + index);
//		Debug.Log ("respawnPoints[index] = " + lg.GetRespawnPoints() [index]);

		Vector3 respawn = lg.GetRespawnPoints () [ Mathf.Clamp(index, 0, lg.levelLength) /* * player */];
		respawn.y += lg.GetLevelAreaHeights () [ Mathf.Clamp(index, 0, lg.levelLength) ];

		//return lg.GetRespawnPoints()[index /* * player */];
		return respawn;
	}

}