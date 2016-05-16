using UnityEngine;
using System.Collections;

public class RelicManager : MonoBehaviour {

	private Rigidbody rb;
	private LevelGenerator l;
	public float[] relicRoute;

	private bool isFollowingRoute = true;

	// Use this for initialization
	void Start () {
		rb = this.gameObject.GetComponent<Rigidbody> ();
		l = GameObject.Find ("LevelGenerator").GetComponent<LevelGenerator> ();

		relicRoute = new float[l.levelLength];

		CreateRelicRoute ();
	}


	/**
	 * Set a parent to the relic
	 * Minimize the scale
	 * Freeze rotation
	 * And resets rotation to zero.
	 */
	public void SetParent(Transform parent){
		this.transform.SetParent (parent, false);
		this.transform.localPosition = new Vector3 (0, 0, 0);
		UpdateScale (1f);
		//UpdateFreezeRotation (true);
		ResetRotation ();
		SetKinematic (true);
		SetCollider (false);

	}

	/**
	 * Activates or deactivates the collider of the child object
	 */
	public void SetCollider(bool active){
		foreach (Transform t in transform) {
			if (t.name == "RelicSphere") {
				t.GetComponent<SphereCollider> ().enabled = active;
			}
		}
	}


	/**
	 * Sets the parent variable to null,
	 * updates the scale
	 * and unfreezes the rotation
	 */
	public void ReleaseFromParent(){
		this.transform.parent = null;
		UpdateScale (1f);
		//UpdateFreezeRotation (false);
		SetKinematic (false);
		SetCollider (true);
		StartCoroutine( waitBeforeFollowingRoute () );

	}

	/**
	 * If game object has a parent return true
	 * else return false
	 */
	public bool HasParent(){
		return transform.parent;
	}



	/**
	 * Check if the relic is within the bounds of the level
	 */
	public bool IsWithinBounds(){
		//if (GetPosition ().x > maxX || GetPosition ().x < minX || GetPosition ().y < minY || GetPosition().z < minZ)
		//LevelManager lm = new LevelManager();
		if (GetPosition ().x < LevelManager.MIN_X || GetPosition ().x > LevelManager.MAX_X || GetPosition ().y < LevelManager.MIN_Y || GetPosition().z < LevelManager.RELIC_MINZ)
			return false;
		else
			return true;
	}


	/**
	 * Returns true if the relic is below ground level, otherwise false
	 */
	public bool IsBelowGround(){
		if (GetPosition ().y < 0)
			return true;
		else
			return false;
	}


	/**
	 * Updates the freezeRotation parameter, with the inputted bool value
	 */
     /*
	public void UpdateFreezeRotation(bool rotate){
		rb.freezeRotation = rotate;
	}*/
	/**
	 * Set current rotation to zero
	 */
	public void ResetRotation(){
		rb.transform.localRotation = new Quaternion();
	}


	public void UpdateScale(float scale){
		rb.transform.localScale = new Vector3 (scale, scale, scale);
	}


	public float GetScale(){
		return rb.transform.localScale.x;
	}


	public Transform GetParent(){
		return this.transform.parent;
	}

	public void UpdatePosition(Vector3 position){
		rb.transform.position = position;
	}

	public Vector3 GetPosition (){
		return rb.transform.position;
	}

	public void SetKinematic(bool kinematic){
		rb.isKinematic = kinematic;
	}

	public bool GetIsFollowingRoute(){
		return this.isFollowingRoute;
	}

	//A cool down, so relic will wait after release from parent before going to route
	public IEnumerator waitBeforeFollowingRoute(){

		//Let relic not follow the route
		isFollowingRoute = false;

		//Wait
		yield return new WaitForSeconds (2f);

		//Let relic follow the route
		isFollowingRoute = true;
	}


	private void CreateRelicRoute(){

		for (int i = 0; i < l.levelLength; i++) {

			if (l.levelAreas [i] == LevelGenerator.AreaType.cliff) {

				relicRoute [i] = l.GetCanyonNoise () [i];

			} else if (l.levelAreas [i] == LevelGenerator.AreaType.bridge) {
			
				relicRoute [i] = l.GetBridge1Noise () [i];

			} else if (l.levelAreas [i] == LevelGenerator.AreaType.lava) {

				relicRoute [i] = l.GetSteppingStone1Noise () [i];
			
			} else {
				
				relicRoute [i] = 0;
			
			}
		}
	}

	/**
	 * Returns and index that is not out of bounds
	 */
	public int GetAcceptedLevelIndex(int z){

		if (z < 0)
			z = 0;
		else if (z >= l.levelLength)
			z = l.levelLength - 1;

		return z;
	}

	
}
