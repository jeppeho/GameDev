using UnityEngine;
using System.Collections;
using Leap;

public class LeapVariables : MonoBehaviour {

	private Controller controller;
	private Frame frame;
	private Hand hand;

	public Vector3 handPositionOffset;
	public Vector3 handPositionScale;
	//public Vector3 handSizeScale;
	
	private Vector3 handPositionScaleShift;
	private Vector3 handPositionLastFrame;
	// Use this for initialization
	void Start () {
		controller = new Controller ();
	}
	
	// Update is called once per frame
	void Update () {
		UpdateFrame();
		UpdateHand ();
		UpdateScaleShift ();
	}

	//Updating the scale-shift: The Vector that should be added to all positions, shifting them exponentially away from origo.
	private void UpdateScaleShift()
	{
		handPositionScaleShift = AddMovementScaleOffset (ToCustomScale (hand.PalmPosition)) - ToCustomScale (hand.PalmPosition);
//		Debug.Log (handPositionScaleShift);
	}

	private Vector3 AddScaleShift(Vector3 v)
	{
		return v + handPositionScaleShift;
	}
	//For adding the custom (movement range-) scale and offset to whole hand
	private Vector3 AddMovementScaleOffset(Vector3 v)
	{
		return new Vector3(v.x * handPositionScale.x, v.y * handPositionScale.y, v.z * handPositionScale.z) + handPositionOffset;
	}
/*
	//For adding the custom (size-) scale to the hand as an object
	private Vector3 AddSizeScale(Vector3 v)
	{
		return new Vector3(v.x * handSizeScale.x, v.y * handSizeScale.y, v.z * handSizeScale.z);
	}
*/	
	//For converting points in world space to vectors adjusted to our scene
	private Vector3 ToCustomScale(Vector v)
	{
		return v.ToUnityScaled() *12  /*+ new Vector3(-0.2f, 0.5f, 0f)*/;
	}

	//Whether two vectors are closer than distance 'd'
	private bool GetVectorsClose(Vector3 v1, Vector3 v2, float d)
	{
		float dis = (v1 - v2).magnitude;
		return (dis <= d);
	}

	///////////////////////////////////////////////////////////////////// 
	/// --- Public functions ---
	/////////////////////////////////////////////////////////////////////

	public Controller GetController(){
		return controller;
	}

	public Frame GetFrame(){
		return frame;
	}

	public void UpdateFrame(){
		frame = controller.Frame ();
	}

	//Get hand
	public Hand GetHand(){
		return hand;
	}

	public bool HandIsValid()
	{
		return hand.IsValid;
	}

	public void UpdateHand(){
		hand = frame.Hands [0];
	}

	//Get hand's grab strength (0-1f)
	public float GetHandGrabStrength (int n){
		return hand.GrabStrength;
	}

	//Get hand's grab strength (0-1f)
	public float GetHandPinchStrength (int n){
		return hand.PinchStrength;
	}

	//Get palm's position
	public Vector3 GetPalmPosition (){
		return AddScaleShift(  ToCustomScale(hand.PalmPosition)  );
	}

	//Get palm's normal
	public Vector3 GetPalmNormal (){
		return GetHand().PalmNormal.ToUnity();
	}

	//Get finger 'f'
	public Finger GetFinger (int f){
		return GetHand().Fingers[f];
	}

	//Get position of fingertip 'f'
	public Vector3 GetFingertipPosition (int f){
		return AddScaleShift(  ToCustomScale(GetFinger(f).TipPosition)  );
	}

	//Get bone 'b' on finger 'f' (thumb = 0, index = 1 etc.) (metarcarpal = 0, proximal = 1 etc.)
	public Bone GetBone (int f, int b)
	{
		switch (b)
		{
		default:
			{	return GetFinger(f).Bone (Bone.BoneType.TYPE_METACARPAL);	}
		case 1:
			{	return GetFinger(f).Bone (Bone.BoneType.TYPE_PROXIMAL);	}
		case 2:
			{	return GetFinger(f).Bone (Bone.BoneType.TYPE_INTERMEDIATE);	}
		case 3:
			{	return GetFinger(f).Bone (Bone.BoneType.TYPE_DISTAL);	}
		}
	}

	//Get base position of bone 'b' on finger 'f' (thumb = 0, index = 1 etc.) (metarcarpal = 0, proximal = 1 etc.)
	public Vector3 GetBoneBasePosition (int f, int b)
	{
		return AddScaleShift( ToCustomScale (GetBone (f, b).PrevJoint));
	}

	//Get center position of bone 'b' on finger 'f' (thumb = 0, index = 1 etc.) (metarcarpal = 0, proximal = 1 etc.)
	public Vector3 GetBoneCenterPosition (int f, int b)
	{
			return AddScaleShift( ToCustomScale (GetBone (f, b).Center));
	}

	///////////////////////////////////////////////////////////////////// 
	// Mostly for gesture recognition

	//Whether palm is near a certain position, by (magnitude) threshold 't'
	public bool PalmNear(Vector3 pos, float t)
	{
		return GetVectorsClose (GetPalmPosition (), pos, t);
	}

	//Whether palm's normal is appropriating a certain direction, by (magnitude) threshold 't'
	public bool PalmNormalNear(Vector3 pos, float t)
	{
		return GetVectorsClose (GetPalmNormal (), pos, t);
	}
		
	//Whether palm is in between two certain y-positions (boths positions exclusive)
	public bool PalmBetweenY(float y1, float y2)
	{
		return (GetPalmPosition ().y > Mathf.Min (y1, y2) && GetPalmPosition ().y < Mathf.Max (y1, y2));
	}

	//Whether fingertip 'f' is near a certain position, by (magnitude) threshold 't'
	public bool FigertipNear(int f, Vector3 pos, float t)
	{
		return GetVectorsClose (GetFingertipPosition(f), pos, t);
	}
		
	//Get whether finger 'f' is extended (true/false)
	public bool GetFingerIsExtended (int f){
		return GetFinger(f).IsExtended;
	}
}
