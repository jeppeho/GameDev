﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GrabManager : MonoBehaviour {

	private float pinchThreshold = 0.8f;
	private float distanceThreshold = 2.5f;

	private Vector3 grabPosition;
	private Vector3 prevPalmPosition;
	private int grabbedObjectLayer;

	private bool pinching = false;

	Collider grabbedObject = null;
	//Include only certain layers
	private LayerMask allowedLayers;

	private int releaseCounter = 3;
	private List<Vector3> releaseVectors;

	private LeapManager leapManager;
	protected GestureManager gestureManager;

	// Use this for initialization
	void Start ()
	{
		leapManager = gameObject.GetComponent<LeapManager> ();
		gestureManager = gameObject.GetComponent<GestureManager> ();

		releaseVectors = new List<Vector3>();

		//Construct layermask
		LayerMask mask1 = 1 << 13;
		LayerMask mask2 = 1 << 14;
		allowedLayers = mask1 | mask2;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		if (gestureManager.noSpellActive ())
		{
			UpdatePinch ();

			if (pinching && grabbedObject != null) {

				//Get relative movement based on palm position
				Vector3 palmPosition = leapManager.GetPalmWorldPosition ();
				Vector3 palmMovement = palmPosition - prevPalmPosition;
				prevPalmPosition = palmPosition;

				//Find the vector towards the thumb
				Vector3 directionToThumb = grabPosition - grabbedObject.transform.position;

				releaseVectors.Add(palmMovement);
				if (releaseVectors.Count >= 6)
				{
					releaseVectors.RemoveAt (0);
				}

				grabbedObject.attachedRigidbody.useGravity = false;
				grabbedObject.attachedRigidbody.MovePosition (grabPosition);

				//DEBUGGING:
				//grabbedObject.gameObject.GetComponent<Renderer> ().material.color = Color.magenta;
			}
		}
	}

	private void UpdatePinch()
	{
		bool pinchTriggered = false;
		bool releaseTriggered = false;

		float pinchStrength = 0;

		//Get grab position
		Vector3 thumbPos = leapManager.GetFingertipWorldPosition(0);
		Vector3 indexPos = leapManager.GetFingertipWorldPosition(1);
		grabPosition = thumbPos + (indexPos - thumbPos)/2f;

		//Get pinch strength
		pinchStrength = leapManager.GetHandPinchStrength();

		//if pinch strength is high enough
		if (pinchStrength >= pinchThreshold)
		{
			releaseCounter = 3; //Reset counter
			pinchTriggered = true;
		}
		else
		{
			if (releaseCounter > 0)
			{
				releaseCounter--; //Decrease counter
			}
			else
			{
				releaseTriggered = true;
			}
		}

		if (pinchTriggered && !pinching)
		{
			pinching = true;
			OnPinch (grabPosition);
		}

		if (releaseTriggered && pinching)
		{
			pinching = false;
			OnRelease ();
		}
	}

	/*
		When a pinch has been detected this method will
		run ONCE, and find the closest object to the inputted position;
		pinch_position = the position that it will find the closest object to
	*/
	void OnPinch(Vector3 pinchPosition)
	{
		//Debug.Log ("Pinching!");

		//Get all objects within PINCH_DISTANCE
		List<Collider> inProximity = new List<Collider> ();

		inProximity.AddRange (Physics.OverlapSphere (pinchPosition, distanceThreshold, allowedLayers).ToList ());

		//Get the nearest object
		float searchDistance = distanceThreshold;

		for (int i = 0; i < inProximity.Count; i++) {
			Vector3 worldPost = inProximity [i].transform.position;

			float newDistance = (pinchPosition - worldPost).magnitude;

			//Debug.Log (pinchPosition.ToString () + "->" + worldPost.ToString ());

			if (newDistance < searchDistance) {
				grabbedObject = inProximity [i];
				searchDistance = newDistance;
			}
		}

		if (grabbedObject != null)
		{
			//Store layer, so it can be reverted
			grabbedObjectLayer = grabbedObject.gameObject.layer;
			//Change the object to a light object, to avoid shattering of hand
			grabbedObject.gameObject.layer = 16;
		}
	}

	/*
		When a pinch is no longer detected this method will
		run ONCE, and release the object.
	*/
	void OnRelease()
	{
		//Debug.Log ("Releasing!");
		if (grabbedObject != null)
		{
			Vector3 throwDir = getVectorAverage (releaseVectors);
			throwDir = Vector3.Lerp (throwDir * 6f, throwDir.normalized, 0.5f);
				
			grabbedObject.GetComponent<Rigidbody> ().velocity = throwDir;
			grabbedObject.gameObject.layer = grabbedObjectLayer;
			grabbedObject.attachedRigidbody.useGravity = true;
			grabbedObject = null;
		}
	}

	private Vector3 getVectorAverage(List<Vector3> vectors)
	{
		float xx = 0;
		float yy = 0;
		float zz = 0;

		foreach (Vector3 v in vectors)
		{
			xx += v.x;
			yy += v.y;
			zz += v.z;
		}

		xx /= vectors.Count;
		yy /= vectors.Count;
		zz /= vectors.Count;

		return new Vector3 (xx, yy, zz);
	}
}