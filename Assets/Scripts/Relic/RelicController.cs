using UnityEngine;
using System.Collections;

public class RelicController : MonoBehaviour {

	//private Vector3 targetPosition;
	private Rigidbody rb;
	private Transform parent;

	private bool movingToTarget;


	//Bounds - should be put in a LevelManager script
	float minX, maxX, minY, minZ;


	// Use this for initialization
	void Start () {
		rb = this.gameObject.GetComponent<Rigidbody> ();
		UpdateFreezeRotation (false);
		movingToTarget = false;

		minX = 0; maxX = 10.5f; minY = 0; minZ = -150; //Bounds - should be put in a LevelManager script
	}
	
	// Update is called once per frame
	void Update () {

		if (GetParent() == null) {

			if (!IsWithinBounds ()) {

				if (GetPosition ().y < 1f) {
				
					rb.AddForce (new Vector3 (0, 40f, 0));

				} else {
					
					if (movingToTarget == false) {
						//movingToTarget = true;
						StartCoroutine (MoveTowardsCenter ());
					}
				}
			}
		} else {
			//Follow parent
			FlyAboveParent ();
		}
	}

	/**
	 * Check if the relic is within the bounds of the level
	 */
	public bool IsWithinBounds(){
		if (GetPosition ().x > maxX || GetPosition ().x < minX || GetPosition ().y < minY || GetPosition().z < minZ)
			return false;
		else
			return true;
	}

	/*
	 * Returns true if the relic is below ground level, otherwise false
	 */
	public bool IsBelowGround(){
		if (GetPosition ().y < 0)
			return true;
		else
			return false;
	}

	//Updates the position to be above the parents head
	private void FlyAboveParent(){
		Vector3 slotPosition = GetParent().position;
		slotPosition.y += 2f;
		UpdatePosition( slotPosition );
	}


	/**
	 * Moves the relic to the center of the level and a bit forward
	 */
	IEnumerator MoveTowardsCenter(){

		//Get target position for dropped carried object
		Vector3 target = new Vector3 (5, GetPosition ().y, GetPosition ().z + 3f);

		Vector3 vectorToTarget = target - GetPosition (); 
		vectorToTarget.Normalize ();
		
		//Move carried object towards target position
		while(!IsWithinBounds()){
			rb.AddForce( vectorToTarget );
		
			yield return new WaitForSeconds (0f);
		}

		if (IsWithinBounds()) {
			movingToTarget = false;
		}
	}
		

	/**
	 * If distance to parent is over some maximum
	 */
	public void ReleaseFromParent(){
		
	}

	/**
	 * Updates the freezeRotation parameter, with the inputted bool value
	 */
	public void UpdateFreezeRotation(bool rotate){
		rb.freezeRotation = rotate;
	}

	/**
	 * Set current rotation to zero
	 */
	public void ResetRotation(){
		rb.transform.localRotation = new Quaternion();
	}

	/**
	 * Set a parent to the relic
	 * Minimize the scale
	 * Freeze rotation
	 * And resets rotation to zero.
	 */
	public void SetParent(Transform parent){
		this.transform.SetParent (parent, false);
		UpdateScale (0.8f);
		UpdateFreezeRotation (true);
		ResetRotation ();
	}

	public void UpdateScale(float scale){
		rb.transform.localScale = new Vector3 (scale, scale, scale);
	}


	public float GetScale(){
		return rb.transform.localScale.x;
	}


	/**
	 * Sets the parent variable to null,
	 * updates the scale
	 * And unfreezes the rotation
	 */
	public void RemoveParent(){
		this.transform.parent = null;
		UpdateScale (1f);
		UpdateFreezeRotation (false);
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
}
