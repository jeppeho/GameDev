using UnityEngine;
using System.Collections;

public class MinionAnimation : MonoBehaviour {

	NewController controller;
	Animator anim;
	Rigidbody rb;

	// Use this for initialization
	void Start () {

		anim = GetComponent<Animator> ();
		controller = this.gameObject.transform.parent.GetComponent<NewController> ();
		rb = transform.parent.GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {

		anim.SetBool ("isDashing", controller.IsDashing());
		anim.SetBool ("isJumping", controller.IsJumping ());
		anim.SetBool ("isRunning", controller.IsRunning());
		anim.SetFloat ("Velocity", rb.velocity.magnitude);
		anim.SetBool ("hasLanded", controller.IsGrounded());

//		Debug.Log("isDashing : " + controller.IsDashing());
//		Debug.Log("isJumping : " + controller.IsJumping());
//		Debug.Log("isRunning : " + controller.IsRunning());

	}
}
