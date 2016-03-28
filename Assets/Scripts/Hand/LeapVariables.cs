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
	public float GetHandGrabStrength (){
		return hand.GrabStrength;
	}

	//Get hand's grab strength (0-1f)
	public float GetHandPinchStrength (){
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

	//Get palm's normal
	public Vector3 GetPalmVelocity (){
		return GetHand().PalmVelocity.ToUnity();
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
	public bool PalmNormalNear(Vector3 normal, float t)
	{
		return GetVectorsClose (GetPalmNormal (), normal, t);
	}

	//Whether palm's normal is appropriating a certain direction, by (magnitude) threshold 't', when ignoring X, Y and/or Z (true/false)
	public bool PalmNearIgnore(Vector3 pos, float t, bool ignoreX,  bool ignoreY,  bool ignoreZ)
	{
		Vector3 newPos;

		if (ignoreX)
		{	newPos.x = GetPalmPosition().x;		}
		else
		{	newPos.x = pos.x;				}

		if (ignoreY)
		{	newPos.y = GetPalmPosition().y;		}
		else
		{	newPos.y = pos.y;				}

		if (ignoreZ)
		{	newPos.z = GetPalmPosition().z;		}
		else
		{	newPos.z = pos.z;				}

		return	GetVectorsClose (GetPalmPosition (), newPos, t);
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

	//Get whether the fingers meet a certain patthern, in terms of being extended (true/false)
	public bool GetFingerPatternIsExtended (bool f0, bool f1, bool f2, bool f3, bool f4){
		return (GetFinger (0).IsExtended == f0)
			&& (GetFinger (1).IsExtended == f1)
			&& (GetFinger (2).IsExtended == f2)
			&& (GetFinger (3).IsExtended == f3)
			&& (GetFinger (4).IsExtended == f4);
	}

	//Prints normal, position, fingers etc. to console
	public void DebugVariables (){
		Debug.Log ("Pos: " + GetPalmPosition ().ToString () + " | Norm:" + GetPalmNormal ().ToString () + " | FingersExt: {" + GetFingerIsExtended (0).ToString () + "," + GetFingerIsExtended (1).ToString () + "," + GetFingerIsExtended (2).ToString () + "," + GetFingerIsExtended (3).ToString () + "," + GetFingerIsExtended (4).ToString () + "}");
	}
}
