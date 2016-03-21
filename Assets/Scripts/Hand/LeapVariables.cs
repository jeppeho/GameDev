using UnityEngine;
using System.Collections;
using Leap;

public class LeapVariables : MonoBehaviour {

	private Controller controller;
	private Frame frame;
	private Hand hand;

	// Use this for initialization
	void Start () {
		controller = new Controller ();
	}
	
	// Update is called once per frame
	void Update () {
		SetFrame();
		SetHand ();
	}

	//For converting points in world space to vectors adjusted to our scene
	private Vector3 ToCustomScale(Vector v)
	{
		return v.ToUnityScaled() *12  + new Vector3(-0.2f, 0.5f, 0f);
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

	public void SetFrame(){
		frame = controller.Frame ();
	}

	//Get hand
	public Hand GetHand(){
		return hand;
	}

	public void SetHand(){
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
		return ToCustomScale(  hand.PalmPosition  );
	}

	//Get palm's normal
	public Vector3 GetPalmNormal (){
		return hand.PalmNormal.ToUnity();
	}

	//Get finger 'f'
	public Finger GetFinger (int f){
		return hand.Fingers[f];
	}

	//Get position of fingertip 'f'
	public Vector3 GetFingertipPosition (int f){
		return ToCustomScale(  GetFinger(f).TipPosition  );
	}

	//Get bone 'b' on finger 'n' (thumb = 0, index = 1 etc.) (metarcarpal = 0, proximal = 1 etc.)
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
		return ToCustomScale (GetBone (f, b).PrevJoint);
	}

	//Get center position of bone 'b' on finger 'f' (thumb = 0, index = 1 etc.) (metarcarpal = 0, proximal = 1 etc.)
	public Vector3 GetBoneCenterPosition (int f, int b)
	{
		return ToCustomScale (GetBone (f, b).Center);
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
