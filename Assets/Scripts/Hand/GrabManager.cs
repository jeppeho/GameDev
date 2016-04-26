using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GrabManager : MonoBehaviour {

	private float pinchThreshold = 0.8f;
	private float distanceThreshold = 2f;

	private Vector3 thumbPosition;
	private Vector3 prev_palm_position;

	private bool pinching = false;

	Collider grabbedObject = null;
	//Include only certain layers
	private LayerMask allowedLayers;

	private int _releaseCounter = 5;

	private LeapManager leapManager;
	// Use this for initialization
	void Start ()
	{
		leapManager = GetComponent<LeapManager> ();
		//Construct layermask
		LayerMask mask1 = 1 << 13;
		LayerMask mask2 = 1 << 14;
		allowedLayers = mask1 | mask2;
	}
	
	// Update is called once per frame
	void Update ()
	{
		UpdatePinch ();

		if (grabbedObject != null) {

			//Get relative movement based on palm position
			Vector3 palmPosition = leapManager.GetPalmWorldPosition ();
			Vector3 palmMovement = palmPosition - prev_palm_position;
			prev_palm_position = palmPosition;

			//Find the vector towards the thumb
			Vector3 directionToThumb = thumbPosition - grabbedObject.transform.position;

			//directionToThumb *= 0.002f; //Limit the speed

			//I think gravity should be disabled when trying to make relative movement to palm or thumb
			//Make sure to set it to true on release
			//Maybe make an OnRelease() method

			grabbedObject.attachedRigidbody.useGravity = false;
			grabbedObject.attachedRigidbody.AddForce (palmMovement);
		}
	}

	private void UpdatePinch()
	{
		bool pinchTriggered = false;
		float pinchStrength = 0;

		//Get thumb position
		thumbPosition = leapManager.GetFingertipWorldPosition(0);

		//Get pinch strength
		pinchStrength = leapManager.GetHandPinchStrength();

		//if pinch strength is high enough
		if (pinchStrength >= pinchThreshold)
		{
			ResetReleaseCounter ();
			pinchTriggered = true;
		}
		else
		{
			if (pinching == true)
			{
				//Pinch
				if (_releaseCounter > 0)
				{
					decreaseReleaseCounter ();
				}
				else
				{
					pinching = false;
					grabbedObject.attachedRigidbody.useGravity = true;
					grabbedObject = null;
				}
					//Release object
			}
		}

		if (pinchTriggered && !pinching)
		{
			OnPinch (thumbPosition);
		}

		if (!pinchTriggered && pinching)
		{
			OnRelease ();
		}
	}


	void ResetReleaseCounter()
	{
		_releaseCounter = 5;
	}

	void decreaseReleaseCounter()
	{
		_releaseCounter -= 1;
	}


	/*
		When a pinch has been detected this method will
		run ONCE, and find the closest object to the inputted position;
		pinch_position = the position that it will find the closest object to
	*/
	void OnPinch(Vector3 pinchPosition)
	{
		if (grabbedObject != null) {
			Debug.Log("Pinching "+grabbedObject.ToString());
		}
		else
		if (grabbedObject != null) {
			Debug.Log("Pinching null");
		}


		//Get all objects within PINCH_DISTANCE
		List<Collider> inProximity = new List<Collider>();

		inProximity.AddRange( Physics.OverlapSphere (pinchPosition, distanceThreshold, allowedLayers).ToList() );

		Debug.Log ("Considering " + inProximity.Count.ToString() +  " objects for pinching | " + inProximity[0].ToString ());

		//DEBUG! Because world positions are fucked up..

		if (inProximity[0] != null)
		{
			grabbedObject = inProximity[0];
			pinching = true;
			Debug.Log ("Grabbed " + grabbedObject.ToString());
		}

		//Get the nearest object
		float searchDistance = distanceThreshold;

		for (int i = 0; i < inProximity.Count; i++)
		{
			Vector3 worldPost = inProximity [i].transform.position;

			float newDistance = (pinchPosition - worldPost).magnitude;

			Debug.Log (pinchPosition.ToString() + "->" + worldPost.ToString());

			if (newDistance < searchDistance)
			{
				grabbedObject = inProximity[i];
				searchDistance = newDistance;
			}

			if (grabbedObject != null)
			{
				pinching = true;
			}

			grabbedObject.gameObject.SetActive (false);
		}

	}

	/*
		When a pinch is no longer detected this method will
		run ONCE, and release the object.
	*/
	void OnRelease()
	{
		grabbedObject.attachedRigidbody.useGravity = true;
		grabbedObject = null;
		pinching = false;
	}
}