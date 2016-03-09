using UnityEngine;
using System.Collections;
using Leap;

public class LeapVariables : MonoBehaviour {

	private Controller controller;
	private Frame frame;

	// Use this for initialization
	void Start () {
		
		controller = new Controller ();

	}
	
	// Update is called once per frame
	void Update () {
		setFrame ();
	}

	public Frame getFrame(){
		return frame;
	}

	public void setFrame(){
		frame = controller.Frame ();
	}

	public Controller getController(){
		return controller;
	}

}
