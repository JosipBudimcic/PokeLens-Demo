using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Pokemon : MonoBehaviour {

	public string name = "MissingN0";

    public AudioClip shout_sfx;
	public AudioClip faint_sfx;
    public GameObject skin;
    public Material[] newMaterials;
    public Transform spawn_VFX;
	public Transform spawn_VFX_sound;
	public Transform return_VFX_sound;
	public Transform faint_drop_VFX_sound;
    public GameObject health_bar;
	public GameObject battle_manager;
	public GameObject[] battle_VFX;
	public Vector3 Original_Scale = new Vector3 (0.7560361f, 0.7560361f, 0.7560361f); //hard coded because bug with accessing transform.localscale
	public float height_above_ground = 0;
	private int animation_state = 0;

	private bool Return = false;

	// Use this for initialization
	void Start () {
    }

	public void Attack_Opponent()
	{
		GameObject opponent = battle_manager.GetComponent<Battle_Manager> ().Get_Current_Opponent ();
		battle_manager.GetComponent<Damage_Manager> ().Perform_Attack (this.gameObject, opponent);
		//Attacked opponent
	}
	public void Affect_Opponent()
	{
		GameObject opponent = battle_manager.GetComponent<Battle_Manager> ().Get_Current_Opponent ();
		battle_manager.GetComponent<Damage_Manager> ().Perform_Affect (this.gameObject, opponent);
		//Affect opponent
	}

	public void Pokemon_Return()
	{
		if (GetComponent<Pokemon_Stats> ().wild && GetComponent<Pokemon_Stats> ().Is_Fainted ()) {
			Instantiate (faint_drop_VFX_sound, skin.transform.position, Quaternion.Euler (-90.0f, 0.0f, 0.0f));
		} else {
			Instantiate (return_VFX_sound, skin.transform.position, Quaternion.Euler (-90.0f, 0.0f, 0.0f));
		}
			
		Return = true;
	}

	public void Pokemon_Catch()
	{
		Return = true;
	}

	public void Reset()
	{
		health_bar.SetActive (false);
	}
		
	public void SetVisible()
	{
		skin.GetComponent<Renderer>().materials = newMaterials;
		health_bar.SetActive (true);

	}

	public void PlayVFX(int index)
	{
		battle_VFX [index].GetComponent<ParticleSystem> ().Play ();
		if (battle_VFX [index].GetComponent<AudioSource> () != null) {
			battle_VFX [index].GetComponent<AudioSource> ().Play ();
		}
	}

    public void Cry(int index)
    {
		if (index == 0) {
			GetComponent<AudioSource> ().clip = shout_sfx;
		} else {
			GetComponent<AudioSource> ().clip = faint_sfx;
		}

        GetComponent<AudioSource>().Play();
    }
	public void Spawn(int index)
    {
		Cry (0);
		SetVisible ();

		Return = false;

		if(index == 0)
			Instantiate(spawn_VFX, skin.transform.position, Quaternion.Euler(-90.0f,0.0f,0.0f));
		else
			Instantiate(spawn_VFX_sound, skin.transform.position, Quaternion.Euler(-90.0f,0.0f,0.0f));
		
		transform.localScale = new Vector3 (0.1f, 0.1f, 0.1f);
		//Spawned
    }

	public void Do_Attack_Animation()
	{
		animation_state = GetComponent<Pokemon_Stats>().Current_move.animation_index;
	}
	public void Do_Damage_Animation()
	{
		animation_state = 5;
	}

	// Update is called once per frame
	void Update () {

		if (!Return) {
			if (transform.localScale != Original_Scale) {
				transform.localScale = Vector3.Lerp (transform.localScale, Original_Scale, Time.deltaTime * 9);
			}
		} else {
			if (transform.localScale != Vector3.zero) {
				transform.localScale = Vector3.Lerp (transform.localScale, Vector3.zero, Time.deltaTime * 9);
			} else {
				gameObject.SetActive (false);
			}
		}

		GetComponent<Animator>().SetInteger("State", animation_state);

		animation_state = 0;

	}
}
