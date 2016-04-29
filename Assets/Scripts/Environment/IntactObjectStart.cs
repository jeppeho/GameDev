using UnityEngine;
using System.Collections;

public class IntactObjectStart : MonoBehaviour {

	private bool untouched = true;
	private int breakForce = 20;
	private Vector3 hitVector;
	public int cooldown = 0;

	void OnCollisionEnter(Collision col){

		//If object is not currently cooling down
		if (cooldown <= 0)
		{
			//If collider layer is not Hand
			if (col.gameObject.layer != 10) {

				//If threshold force is used
				if (col.relativeVelocity.magnitude > breakForce) {
					//Set untouched to false
					untouched = false;
					hitVector = col.relativeVelocity;
					float fadeTime = GameObject.Find("FadingScenes").GetComponent<FadingScenes>().BeginFade (1);
					Invoke( "ChangeLevel", 3.0f );
				}
			}
		}
	}

	public bool IsUntouched(){
		return untouched;
	}

	public Vector3 GetHitVector(){
		return this.hitVector;
	}

	public void SetBreakForce(float force){
		this.breakForce = Mathf.FloorToInt(force);
	}

	public void UpdateCooldown()
	{
		if (cooldown > 0)
		{	cooldown--;		}
	}

	public void ResetCooldown()
	{
		cooldown = 10;
	}

	void ChangeLevel() {
		Application.LoadLevel ("LevelGenerator");  
	}
}
