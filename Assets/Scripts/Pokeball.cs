using UnityEngine;
using System.Collections;

public class Pokeball : MonoBehaviour {

	public float speed = 1;
	public float acceleration = 1;

	private Vector3 target_Position;
	private Vector3 start_Position;

	private Transform original_local_transform;

	private bool launched = false;
	private bool ready = false;
	private float time_Passed = 0;
	private float trajectoryHeight = 0.5f;

	public bool reached_target = false;

	public GameObject Pokemon;

	public GameObject Skin;
	public GameObject Monster_Ball;
	public Material regular_mat;
	public Material ghost_mat;

	public GameObject preview_UI;

	public AudioClip bounce_SFX;
	public AudioClip throw_SFX;
	public AudioClip struggle_SFX;
	public AudioClip caught_SFX;

	public GameObject catch_VFX;

	public GameObject stars_VFX;
	public GameObject rings_VFX;
	public int animation_state = 0;
	public bool disabled = false;

	private bool locked_on = false;

	private bool caught = false;

	public bool ready_to_catch = false;

	public bool start_fall = false;

	public float struggle_timer = 0;

	public bool pokemon_caught = false;

	// Use this for initialization
	void Start () {
		original_local_transform = gameObject.transform;
	}

	public void Play_SFX(AudioClip SFX)
	{
		GetComponent<AudioSource> ().clip = SFX;
		GetComponent<AudioSource> ().Play ();
	}

	public void Play_Bounce_SFX()
	{
		Play_SFX(bounce_SFX);
	}

	public void Play_Struggle_SFX()
	{
		Play_SFX(struggle_SFX);
	}

	public void Pokemon_Caught_VFX()
	{
		Play_SFX (caught_SFX);
		stars_VFX.GetComponent<ParticleSystem> ().Play ();
		rings_VFX.GetComponent<ParticleSystem> ().Play ();
	}

	public void Pokemon_Caught()
	{
		pokemon_caught = true;
	}

	public void Pokeball_Anim_Opened()
	{
		ready_to_catch = true;
		catch_VFX.GetComponent<ParticleSystem> ().Play ();
		catch_VFX.GetComponent<AudioSource> ().Play ();
	}

	public bool Check_Pokemon_Caught()
	{
		return pokemon_caught;
	}

	public bool Check_Pokeball_Anim_Opened()
	{
		return ready_to_catch;
	}

	public void Reset_Transformation()
	{
		gameObject.transform.rotation = Quaternion.identity;
		gameObject.transform.localRotation = Quaternion.identity;
		Monster_Ball.transform.rotation = Quaternion.Euler(0, 180, 0);
	}

	public void Disable()
	{
		disabled = true;
		Prepare ();
	}

	public void Prepare()
	{
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.rotation = Quaternion.identity;
		ready = false;
		locked_on = false;
		launched = false;
		GetComponent<Animator> ().enabled = true;
		if (disabled) {
			Skin.GetComponent<Renderer> ().material = ghost_mat;
		} else {
			Skin.GetComponent<Renderer> ().material = regular_mat;
		}
	}

	public void Select()
	{
		locked_on = true;
	}

	public void Caught()
	{
		caught = true;
	}
		
	public void Fall()
	{
		GetComponent<Rigidbody> ().isKinematic = false;
		GetComponent<SphereCollider> ().enabled = true;
		GetComponent<SphereCollider> ().isTrigger = false;
		GetComponent<Animator> ().enabled = false;
		gameObject.transform.parent.GetComponent<Chase_UI_3D> ().enabled = false;
		start_fall = true;
	}

	public void Selected_Ready()
	{
		if (!disabled) {
			ready = true;
			GetComponent<SphereCollider> ().enabled = false;
		}
	}

	public bool Check_Ready()
	{
		return ready;
	}


	public void LaunchAtTarget(Vector3 target)
	{
		target_Position = target;
		start_Position = gameObject.transform.position;
		time_Passed = 0;
		GetComponent<AudioSource> ().clip = throw_SFX;
		GetComponent<AudioSource> ().Play ();
		launched = true;
		reached_target = false;
		GetComponent<Animator> ().enabled = false;
	}
	void CalculateTrajectory()
	{
		time_Passed += Time.deltaTime * speed * acceleration * 1.25f;

		Vector3 currentPosition = Vector3.Lerp (start_Position, target_Position, time_Passed);

		currentPosition.y += trajectoryHeight * Mathf.Sin (Mathf.Clamp01 (time_Passed) * Mathf.PI);

		transform.position = currentPosition;

		transform.Rotate (Vector3.right * Time.deltaTime * speed * acceleration * 1000);

		if (currentPosition == target_Position) {
			reached_target = true;
			launched = false;

			gameObject.transform.rotation = Quaternion.identity;
			gameObject.transform.localRotation = Quaternion.identity;
			Monster_Ball.transform.rotation = Quaternion.Euler(0, 180, 0);
			GetComponent<SphereCollider> ().enabled = true; 
		}
	}

	void OnCollisionEnter(Collision collision){
		if (start_fall) {
			Play_Bounce_SFX ();
		}
	}

	// Update is called once per frame
	void Update () {
		
		if (launched) {
			CalculateTrajectory ();
		} else {

			if (!ready) {
				if(!disabled)
					animation_state = 1;
				if (locked_on) {
					animation_state = 3;
				}
				
			} else {
				animation_state = 2;
				if(caught)
					animation_state = 4;
			}
		}

		if (start_fall) {
			struggle_timer += Time.deltaTime;

			if (struggle_timer > 2.3f) {
				animation_state = 5;
				GetComponent<Animator> ().enabled = true;
				transform.parent.transform.position = gameObject.transform.position;
				gameObject.transform.localPosition = Vector3.zero;
				Play_Struggle_SFX ();
				start_fall = false;
			}
		}

		if(preview_UI != null && !disabled)
			preview_UI.SetActive (locked_on);
		
		GetComponent<Animator> ().SetInteger ("State", animation_state);

		animation_state = 0;

		locked_on = false;
	}
}
