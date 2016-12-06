using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Pokemon_Stats : MonoBehaviour {

	public GameObject HP_Bar_UI;
	public GameObject HP_Bar_UI_quad;
	public GameObject[] Hit_Particles_VFX;

	public GameObject Low_Health_SFX;
	public AudioClip Regular_Damage_SFX;

	public BattleMove[] Battle_moves;
	public BattleMove Current_move;
    public string[] types;
    public int Level;
	public float HP;
    public int Attack;
    public int Defense;
	public int Speed;
	public int Accuracy; //not implemented

	private float Original_HP;

	public float STAB = 1.5f;

	private float New_HP;
	private float Old_HP;
    //public int Move_Base 

	public bool damage_taken = false;

	public bool wild = true;

	public GameObject[] stat_info;

	public bool reveal_extra_info = false;

	// Use this for initialization
	void Start () {
		Current_move = Battle_moves [0];

		Original_HP = HP;
		HP_Bar_UI_quad.GetComponent<Renderer> ().material.color = Color.green;
		Old_HP = HP;
		New_HP = HP;
    }

	public void Reveal_Extra_Info()
	{
		reveal_extra_info = true;
	}

	public void Add_HP(int value)
	{
		HP += value;
		if (HP > Original_HP)
			HP = Original_HP;
		New_HP = HP;

		if ((HP / Original_HP) > 0.5f) {
			HP_Bar_UI_quad.GetComponent<Renderer> ().material.color = Color.green;
		}

		if ((HP / Original_HP) <= 0.5f) {
			HP_Bar_UI_quad.GetComponent<Renderer> ().material.color = Color.yellow;
		}

		if ((HP / Original_HP) <= 0.2f) {
			HP_Bar_UI_quad.GetComponent<Renderer> ().material.color = Color.red;
		}
	}

	public bool Is_Fainted()
	{
		if (HP == 0)
			return true;
		return false;
	}

	public void Ready()
	{
		damage_taken = false;
	}

	public void SetCurrentMove(int index)
	{
		Current_move = Battle_moves [index];
	}

	public void TakeDamage(float damage, int attack_index_type)
	{
		Old_HP = HP;

		HP -= damage;

		if ((HP / Original_HP) > 0.5f) {
			HP_Bar_UI_quad.GetComponent<Renderer> ().material.color = Color.green;
		}

		if ((HP / Original_HP) <= 0.5f) {
			HP_Bar_UI_quad.GetComponent<Renderer> ().material.color = Color.yellow;
		}

		if ((HP / Original_HP) <= 0.2f) {
			HP_Bar_UI_quad.GetComponent<Renderer> ().material.color = Color.red;
		}

		if (HP <= 0) {
			HP = 0;
		}


		Hit_Particles_VFX[attack_index_type-1].GetComponent<ParticleSystem> ().Play ();
		Hit_Particles_VFX[attack_index_type-1].GetComponent<AudioSource> ().Play ();

		New_HP = HP;

		if (attack_index_type < 5) {
			GetComponent<AudioSource> ().clip = Regular_Damage_SFX;
			GetComponent<AudioSource> ().Play ();

			GetComponent<Pokemon> ().Do_Damage_Animation ();
		}
		damage_taken = true;
	}

	public void TakeEffect(string effect, int attack_index_type)
	{

		Hit_Particles_VFX[attack_index_type-1].GetComponent<ParticleSystem> ().Play ();
		Hit_Particles_VFX[attack_index_type-1].GetComponent<AudioSource> ().Play ();


		if (attack_index_type < 5) {
			GetComponent<AudioSource> ().clip = Regular_Damage_SFX;
			GetComponent<AudioSource> ().Play ();

			GetComponent<Pokemon> ().Do_Damage_Animation ();
		}

		if (effect == "attack") {
			if (Attack > 0)
				Attack--;
			Debug.Log (effect);
		}
		if (effect == "speed") {
			if (Speed > 0)
				Speed--;
			Debug.Log (effect);
		}

		if (effect == "defense") {
			if (Defense > 0)
				Defense--;
			Debug.Log (effect);
		}

		if (effect == "accuracy") {
			if (Accuracy > 0)
				Accuracy--;
			Debug.Log (effect);
		}

		damage_taken = true;
	}
	
	// Update is called once per frame
	void Update () {


		if (Old_HP != New_HP) {
			Old_HP = Mathf.Lerp (Old_HP, New_HP, Time.deltaTime * 10);
		}
			 
		if (New_HP == 0 && Old_HP < 0.1f) //fixing the 0 hp lerp bug
			Old_HP = 0;
		
		HP_Bar_UI.transform.localScale = new Vector3((Old_HP/Original_HP),1,1);


		if (!wild && (HP / Original_HP) <= 0.2f && (HP / Original_HP) != 0) {
			if (!Low_Health_SFX.GetComponent<AudioSource> ().isPlaying)
				Low_Health_SFX.GetComponent<AudioSource> ().Play ();
		}

		foreach (GameObject info in stat_info)
			info.SetActive (reveal_extra_info);

	}
}
