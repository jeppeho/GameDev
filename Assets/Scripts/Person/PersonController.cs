using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using DG.Tweening;

public class PersonController : MonoBehaviour {

	public float speed = 5;
	public float jumpPower = 2f;
	Vector3 position;
	bool jumping = false;

	public AnimationCurve anim;
	Vector3 highPos;
	//Vector3 lowPos;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

		//Get current position
		position = this.gameObject.transform.position;
	
		//Jump Person
		Jump ();

		//Move Person on X and Z axis
		Move ();



		//Update position
		this.gameObject.transform.position = position;
	}


	void Jump(){

		//Jump
		if (Input.GetAxis ("Jump") == 1) {
			if (!jumping) {
				jumping = true;
				highPos = position;
				highPos.y += 1.5f;
				transform.DOMove (highPos, jumpPower * Time.deltaTime).SetEase (anim);
			}
		}

		//Reset jumping when key released
		if (Input.GetAxis ("Jump") == 0)
			jumping = false;
	
	}

	void Move(){

		//Move on the X-axis
		if(Input.GetAxis("Horizontal") != 0){
			position.x += Input.GetAxis("Horizontal") * speed * Time.deltaTime;
		}

		//Move on the Z-axis
		if(Input.GetAxis("Vertical") != 0){
			position.z += Input.GetAxis("Vertical") * speed * Time.deltaTime;
		}
	}
}
