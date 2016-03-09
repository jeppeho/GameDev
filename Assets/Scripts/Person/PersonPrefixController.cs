using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using DG.Tweening;

public class PersonPrefixController : MonoBehaviour {

	public string prefix;

	public float speed;
	public float jumpPower;
	Vector3 position;
	bool jumping = false;

	public AnimationCurve anim;
	Vector3 highPos;
	//Vector3 lowPos;

	public float regularPushPower;
	public float extraPushPower;


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

		//Set mass to push object
		Push ();

		//Update position
		this.gameObject.transform.position = position;
	}


	void Jump(){

		//Jump
		if (Input.GetAxis (prefix +  "_Jump") == 1) {
			if (!jumping) {
				jumping = true;
				highPos = position;
				highPos.y += 1.5f;
				transform.DOMove (highPos, jumpPower * Time.deltaTime).SetEase (anim);
			}
		}

		//Reset jumping when key released
		if (Input.GetAxis (prefix +  "_Jump") == 0)
			jumping = false;

	}

	void Move(){

		//Move on the X-axis
		if(Input.GetAxis( prefix + "_Horizontal" ) != 0){
			position.x += Input.GetAxis( prefix + "_Horizontal") * speed * Time.deltaTime;
		}

		//Move on the Z-axis
		if(Input.GetAxis( prefix +  "_Vertical" ) != 0){
			position.z += Input.GetAxis(prefix + "_Vertical") * speed * Time.deltaTime;
		}
	}

	void Push(){
		
		if (Input.GetAxis ( prefix + "_Push") == 1) {
			//Debug.Log("Pushing Square");
			setMass (extraPushPower);
		} else {
			setMass (regularPushPower);
		}
	}

	void setMass(float mass){

		//Set mass
		this.gameObject.GetComponent<Rigidbody>().mass = mass;
	}
}
