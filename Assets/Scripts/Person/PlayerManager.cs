using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {

	private Rigidbody rb;
	private MeshCollider collider;
	private Vector3 lastVel;
	public enum state {active, inactive, dead, invulnerable};
	public state playerState;
	public float impactResistance = 3f;

	public Material[] materials;


	void Awake() {
		//Keep the system manager from destroying when changing scenes
		DontDestroyOnLoad (transform.gameObject);
//		Debug.Log ("GetState = " + GetState()); 
//		if (GetState () == "inactive") {
//			
//			this.gameObject.SetActive (false);
//		} else {
//			this.gameObject.SetActive (true);
//		}
	}

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
		collider = GetComponentInChildren<MeshCollider>();
	
	}

	// Update is called once per frame
	void FixedUpdate () {
		//Track falling velocity, so we have something to compare to, when landing on environment
		lastVel = rb.velocity;

		//Check to see if player has fallen off grid
		if (transform.position.y <= -1f)
		{
			DeathFall ();
		}

		if (transform.position.z < ( GameObject.Find ("LeapControllerBlockHand").GetComponent<CameraController> ().GetZPosition () - 6f )) {
			Death ();
		}
	}


	//Check for impacts with environment
	void OnCollisionEnter(Collision col)
	{
		//Make a new impact velocity, by subtracting own velocity speed
		Vector3 impact = col.relativeVelocity + lastVel;

		if (/*col.gameObject.tag == "Environment"*/ col.gameObject.layer == 13 && impact.magnitude >= impactResistance) {
			DeathSquished ();

			//Remove energy from the relic
			GameObject relic = GameObject.Find ("Relic");
			if (relic) {
				relic.GetComponent<RelicHealth> ().DrainEnergy (impact.magnitude * 10);
			}
		}
	}


	public void SetMaterial(int index){

		Material m = materials [index];
		Debug.Log(index + "] setting matieral to " + m); 

		foreach (Transform t in transform) {

			t.gameObject.GetComponent<Renderer> ().material = m;
		}
	}


	//Death by squishing (i.e. high relative y-velicity)
	private void DeathSquished()
	{
		Death();
	}

	//Death by falling (i.e. falling below ground)
	private void DeathFall()
	{
		Death();
	}

	//Death
	private void Death()
	{
		if (playerState == state.active)
		{
			playerState = state.dead;
			rb.freezeRotation = false;
			rb.AddTorque(new Vector3(Random.Range(0.2f,1),0,Random.Range(0.2f,1)));
			StartCoroutine(CountdownToRespawn(2f));
		}
	}

	//Respawning
	private void RespawnPrototype()
	{
		float z_offset = 4 * LevelManager.SPEED;
		transform.position = new Vector3 (0, 0.5f, GameObject.Find ("Camera").transform.TransformPoint(Vector3.zero).z + z_offset);

		transform.rotation = new Quaternion ();

		GetComponent<Rigidbody> ().freezeRotation = true;
	}

	private void Respawn/*PCG_LEVEL*/(){

		//Get z-position of camera
		int z = Mathf.FloorToInt(GameObject.Find ("Camera").transform.position.z);

		//Get the respawn point for current position
		transform.position = LevelManager.GetRespawnPoint(z);

		//Reset rotation
		transform.rotation = new Quaternion ();

		//Freeze rotation
		GetComponent<Rigidbody> ().freezeRotation = true;

	}

	IEnumerator CountdownToRespawn(float time)
	{
		yield return new WaitForSeconds (time);
		playerState = state.invulnerable;
		StartCoroutine(FlashOnRespawn(1f));
		Respawn();
	}

	IEnumerator FlashOnRespawn(float time)
	{
		bool transparent = false;
		Renderer[] childRends = GetComponentsInChildren<Renderer> ();
		float subTime = 0;

		while (subTime < time) {
			transparent = !transparent;
			for (int i=0; i < childRends.Length; i++)
			{
				Color c = childRends [i].material.color;
				if (transparent)
				{	childRends [i].material.color = new Color(c.r, c.g, c.b, 0.2f);	}
				else
				{	childRends [i].material.color = new Color(c.r, c.g, c.b, 0.6f);	}
			}
			yield return new WaitForSeconds (0.1f);
			subTime += 0.1f;
		}

		//Reset colors
		for (int i=0; i < childRends.Length; i++)
		{
			Color c = childRends [i].material.color;
			childRends [i].material.color = new Color(c.r, c.g, c.b, 1f);
		}

		//And change state back
		playerState = state.active;
	}

	///////////////////////////////////////////////////////////////////// 
	/// --- Public functions ---
	/////////////////////////////////////////////////////////////////////

	//Returns the exact state of the player, as a string
	//States are:
	//	"active" | alive, movable and succeptible to damage
	//  "inactive" | alive, but in a cinematic state or similarly frozen
	//  "dead" | not so alive
	//	"invulnerable" | alive, but invulnerable
	public string GetState()
	{
		return playerState.ToString ();
	}

	//Marks the player as active (true) or inactive (false)
	public void SetActive(bool b)
	{
		if (b)
		{
			if (playerState == state.inactive)
			{	playerState = state.active;		}
		}
		else
		{
			if (playerState == state.active || playerState == state.invulnerable)
			{	playerState = state.inactive;		}
		}
	}

	//Whether the player is to be considered alive (i.e. whether it's in one of a certain active states)
	public bool IsAlive()
	{
		if (playerState != state.dead)
		{	return true;	}
		else
		{	return false;	}
	}

	//Whether the player is controllable (i.e. whether it's in one of a certain active states)
	public bool IsControllable()
	{
		if (playerState == state.active || playerState == state.invulnerable)
		{	return true;	}
		else
		{	return false;	}
	}

	//Whether the player is damageable (i.e. whether it's in one of a certain active states)
	public bool IsDamageable()
	{
		if (playerState == state.active)
		{	return true;	}
		else
		{	return false;	}
	}

	public void Suicide(){
		Death ();
	}

}