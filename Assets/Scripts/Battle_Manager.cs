using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Battle_Manager : MonoBehaviour {
    
	public GameObject main_camera;
	public GameObject cursor;
	public GameObject pokeball_symbol;
    public GameObject stripe;
	public GameObject battle_field;

	public GameObject[] Pokeballs;
	public GameObject[] Item_Pokeballs;

	public GameObject current_Item_Pokeball;

	public GameObject[] Pokeball_selection_targets;

    public GameObject Pokemon_1; //trainer's pokemon
	public GameObject Pokemon_2; //wild pokemon

	public GameObject[] wild_pokemon_list;

	private GameObject Pokemon_Battle_1; //the pokemon that goes first
	private GameObject Pokemon_Battle_2; //the pokemon that goes second

	public GameObject Pokemon_1_Spawn;
	public GameObject Pokemon_2_Spawn;

	private GameObject current_pokeball;
	private GameObject current_opponent;

    public float Battle_Timer = 0;
	public float Switch_Timer = 0;
	public float End_Timer = 0;
	private int Phase_Index = 0;
    public bool Battle_Start = false;
	public bool Switch_Start = false;
	public bool Return_Start = false;

	public GameObject pokemon_summon_target;
	public GameObject enemy_pokemon_summon_target;

	public GameObject pokeball_target;

	public GameObject[] Moves_UI;
	public GameObject Home_UI;
	public GameObject Dialogue_UI;
	public GameObject Battle_UI;
	public GameObject user_view_bounds;

	public GameObject Unavailable_Command_SFX;
	public GameObject Pokeball_Selection_SFX;


	private bool Ready_To_Attack = false;
	private float Attack_Delay = 0;
	private int Attack_Index = 0;

	private float move_selected_timer = 0;
	private float end_turn_timer = 0;

	public AudioClip Battle_Music;
	public AudioClip Victory_Music;
	public AudioClip Caught_Fanfare_Music;
	public bool general_tap_event = false;
	public string general_tap_event_tag = "";
	private bool spawn_ready = false;

	// Use this for initialization
	void Start () {

		int wild_RNG = (int)Random.Range (0, wild_pokemon_list.Length); //rng gods

		Pokemon_2 = wild_pokemon_list[wild_RNG];

		Pokemon_2.transform.localPosition = Pokemon_2_Spawn.transform.localPosition + new Vector3(0,Pokemon_2.GetComponent<Pokemon>().height_above_ground,0);
		Pokemon_2.transform.localRotation = Pokemon_2_Spawn.transform.localRotation;

		Home_UI.GetComponent<Home_Button_UI> ().Disable ();

		foreach (GameObject pokeball in Pokeballs) {
			pokeball.transform.GetChild (0).GetComponent<Pokeball> ().Prepare ();
			pokeball.transform.GetChild (0).gameObject.SetActive (false);
		}
	}
		
	public GameObject Get_Current_Opponent()
	{
		return current_opponent;
	}

	public void Voice_Battle_Command(string command_name)
	{
		if (Phase_Index == 2) {
			for (int i = 0; i < 4; i++) {
				if (Moves_UI [i].transform.GetChild (0).GetComponent<TextMesh> ().text == command_name) {
					Moves_UI [i].GetComponent<Selection> ().OnSelect ();
					Moves_UI [i].GetComponent<Button_push_animation> ().Animate ();
				}
			}

			if (command_name == "Come Back") {
				Dialogue_UI.SetActive (true);
				Dialogue_UI.GetComponent<Dialogue_UI> ().Set_Dialogue_Text (Pokemon_1.GetComponent<Pokemon> ().name + "!" + " Come Back!", 1.4f);

				Switch_Start = true;
			}
		}
	}

    void Start_Battle()
    {
        pokeball_symbol.SetActive(true);
        pokeball_symbol.GetComponent<Pokeball_Icon_Animation>().start = true;
        stripe.SetActive(true);
        stripe.GetComponent<stripe_animation>().start = true;

		GetComponent<AudioSource> ().clip = Battle_Music;
		GetComponent<AudioSource>().Play();
        Battle_Start = true;
    }

	void Prepare_Pokemon_For_Battle()
	{
		Pokemon_2.GetComponent<Pokemon_Stats> ().SetCurrentMove ((int)Random.Range(0,4));

		int Speed_1 = Pokemon_1.GetComponent<Pokemon_Stats>().Speed;
		int Speed_2 = Pokemon_2.GetComponent<Pokemon_Stats>().Speed;

		int Speed_Index = 1;

		if (Speed_1 == Speed_2) {
			Speed_Index = (int)Random.Range (1, 2);
		} else if (Speed_1 > Speed_2) {
			Speed_Index = 1;
		} else {
			Speed_Index = 2;
		}

		int Speed_Priority_1 = Pokemon_1.GetComponent<Pokemon_Stats> ().Current_move.speed_priority;
		int Speed_Priority_2 = Pokemon_2.GetComponent<Pokemon_Stats> ().Current_move.speed_priority;

		if (Speed_Priority_1 > 0 || Speed_Priority_2 > 0) {
			if (Speed_Priority_1 == Speed_Priority_2) {
				Speed_Index = (int)Random.Range (1, 2);
			} else if (Speed_Priority_1 > Speed_Priority_2) {
				Speed_Index = 1;
			} else {
				Speed_Index = 2;
			}
		}

		if (Speed_Index == 1) {
			Pokemon_Battle_1 = Pokemon_1;
			Pokemon_Battle_2 = Pokemon_2;
		} else {
			Pokemon_Battle_1 = Pokemon_2;
			Pokemon_Battle_2 = Pokemon_1;
		}

		current_opponent = Pokemon_Battle_2;
	}

	public void Return_Pokemon(bool fainted)
	{
		if (fainted) 
			current_pokeball.GetComponent<Pokeball> ().Disable ();
		

		Switch_Timer = 0;

		current_pokeball.SetActive (true);

		Pokemon_1.GetComponent<Pokemon>().Pokemon_Return();
		current_pokeball.transform.position = pokemon_summon_target.transform.position;
		current_pokeball.GetComponent<Pokeball> ().LaunchAtTarget (current_pokeball.transform.parent.gameObject.GetComponent<Chase_UI_3D>().origin_target.transform.position);
		current_pokeball.GetComponent<Selection> ().Deselect ();

		if (Phase_Index != -99 && Phase_Index != -4) {
			pokemon_summon_target.GetComponent<ParticleSystem> ().Play ();
			Pokemon_2.GetComponent<Pokemon_Stats> ().SetCurrentMove ((int)Random.Range (0, 4));
			Phase_Index = -1;
		} else {
			Phase_Index = -100;
		}

		if(fainted)
			Check_Party ();
	}

	public void Check_Party()
	{
		bool available_pokemon = false;
		foreach (GameObject pokeball in Pokeballs) {
			if (!pokeball.transform.GetChild (0).gameObject.GetComponent<Pokeball> ().Pokemon.GetComponent<Pokemon_Stats> ().Is_Fainted ())
				available_pokemon = true;
		}

		if (!available_pokemon) {
			Phase_Index = -101;
			//Out of Pokemon!
		}
	}

	public void General_Tap_Event(string tag)
	{
		general_tap_event = true;
		general_tap_event_tag = tag;
	}

	public void Set_Dialogue(string text, float duration)
	{
		Dialogue_UI.SetActive (true);
		Dialogue_UI.GetComponent<Dialogue_UI> ().Set_Dialogue_Text (text, duration);
	}

	// Update is called once per frame
	void Update () {

		if (Phase_Index == 0) {

			if ((general_tap_event || Input.GetKeyDown (KeyCode.I)) && !spawn_ready) {
				spawn_ready = true;
				general_tap_event = false;
				enemy_pokemon_summon_target.GetComponent<ParticleSystem> ().Stop ();
				enemy_pokemon_summon_target.transform.GetChild (0).gameObject.SetActive (false);
			}

			if (!Battle_Start && (Input.GetKeyDown (KeyCode.P) || general_tap_event)) {
				Start_Battle ();
				general_tap_event = false;
			}
			
			if (Battle_Start) {
				Battle_Timer += Time.deltaTime;

				if (Battle_Timer > 1.9f) {

					Pokemon_2.GetComponent<Pokemon> ().Spawn (0);
					Phase_Index = -1;

					Dialogue_UI.SetActive (true);
					Dialogue_UI.GetComponent<Dialogue_UI> ().Set_Dialogue_Text ("A wild " + Pokemon_2.GetComponent<Pokemon> ().name + " appeared!", 2.5f);

					foreach (GameObject pokeball in Pokeballs) {
						pokeball.transform.GetChild (0).gameObject.SetActive (true);
					}

					pokemon_summon_target.SetActive (true);

					Pokemon_1.GetComponent<Pokemon_Stats> ().Ready ();
					Pokemon_2.GetComponent<Pokemon_Stats> ().Ready ();
				}
			} else {
				//battlefield = cursor position
				if (!spawn_ready) {
					battle_field.transform.position = cursor.transform.position;
				} else {
					Pokemon_2.SetActive (true);
				}
			}
		}
		if (Phase_Index == -1) { //pokemon selection 

			if (current_pokeball != null && current_pokeball.GetComponent<Pokeball> ().reached_target) {
				current_pokeball.transform.parent.GetComponent<Chase_UI_3D> ().ResetTarget ();
				current_pokeball.GetComponent<Pokeball> ().reached_target = false;
				current_pokeball.GetComponent<Pokeball> ().Prepare ();
				current_pokeball.GetComponent<Selection> ().Deselect ();

				foreach (GameObject _pokeball in Pokeballs) {
					_pokeball.transform.GetChild(0).gameObject.SetActive (true);
					_pokeball.transform.GetChild(0).gameObject.GetComponent<Pokeball>().Reset_Transformation();
				}
				current_pokeball = null;
				//returned
			}
			if (Input.GetKeyDown (KeyCode.C)) {
				Pokeballs[0].transform.GetChild (0).GetComponent<Selection> ().OnSelect ();
			}
			if (Input.GetKeyDown (KeyCode.V)) {
				Pokeballs[1].transform.GetChild (0).GetComponent<Selection> ().OnSelect ();
			}

			foreach (GameObject pokeball in Pokeballs) {


				if (pokeball.transform.GetChild(0).GetComponent<Selection> ().Check_OnSelected ()) {

					if (!pokeball.transform.GetChild (0).GetComponent<Pokeball> ().disabled) {
						//turn off the rest of the pokeballs
						foreach (GameObject _pokeball in Pokeballs) {
							_pokeball.transform.GetChild (0).GetComponent<Pokeball> ().Reset_Transformation ();
							if (!_pokeball.transform.GetChild (0).GetComponent<Selection> ().Check_OnSelected ())
								_pokeball.transform.GetChild (0).gameObject.SetActive (false);
						}

						pokeball.transform.GetChild (0).GetComponent<Pokeball> ().Selected_Ready ();
						pokeball.transform.GetChild (0).GetComponent<Pokeball> ().reached_target = false;

						pokeball.transform.GetChild (0).GetComponent<Button_push_animation> ().Animate ();
						pokeball.GetComponent<Chase_UI_3D> ().target = pokeball_target.transform;
						current_pokeball = pokeball.transform.GetChild (0).gameObject;

						Pokemon_1 = pokeball.transform.GetChild (0).GetComponent<Pokeball> ().Pokemon;

						Pokemon_1.SetActive (false);
						Pokemon_1.transform.localPosition = Pokemon_1_Spawn.transform.localPosition + new Vector3 (0, Pokemon_1.GetComponent<Pokemon> ().height_above_ground, 0);
						Pokemon_1.transform.localRotation = Pokemon_1_Spawn.transform.localRotation;

						for (int i = 0; i < 4; i++) {
							Moves_UI [i].transform.GetChild (0).GetComponent<TextMesh> ().text = Pokemon_1.GetComponent<Pokemon_Stats> ().Battle_moves [i].name;
						}

						Phase_Index = 1;
					} else {
						Unavailable_Command_SFX.GetComponent<AudioSource> ().Play ();
						pokeball.transform.GetChild (0).GetComponent<Selection> ().Deselect ();
						Dialogue_UI.SetActive (true);
						Dialogue_UI.GetComponent<Dialogue_UI> ().Set_Dialogue_Text (pokeball.transform.GetChild (0).GetComponent<Pokeball>().Pokemon.GetComponent<Pokemon>().name + " is too weak to battle!", 1.5f);
					}

				}
			}
		}
		if (Phase_Index == -2) {
			if (Input.GetKeyDown (KeyCode.O)) {
				Vector3 target = new Vector3 (cursor.transform.position.x,cursor.transform.position.y + 0.25f, cursor.transform.position.z);
				current_Item_Pokeball.GetComponent<Pokeball> ().LaunchAtTarget (target);
			}

			if (general_tap_event) {
				if(general_tap_event_tag == "Wild_Pokemon") {
					Vector3 target = new Vector3 (cursor.transform.position.x,cursor.transform.position.y + 0.25f, cursor.transform.position.z);
					current_Item_Pokeball.GetComponent<Pokeball> ().LaunchAtTarget (target);
				}
				general_tap_event = false;
			}
			if (current_Item_Pokeball.GetComponent<Pokeball> ().reached_target) {
				current_Item_Pokeball.transform.parent.transform.position = current_Item_Pokeball.transform.position;

				current_Item_Pokeball.transform.parent.GetComponent<Chase_UI_3D> ().enabled = false;
				current_Item_Pokeball.GetComponent<Animator> ().enabled = true;
				current_Item_Pokeball.GetComponent<Pokeball> ().Selected_Ready ();
				current_Item_Pokeball.GetComponent<Pokeball> ().Caught ();
				current_Item_Pokeball.GetComponent<Pokeball> ().Play_Bounce_SFX ();
				Phase_Index = -3;
			}
		}
		if (Phase_Index == -3) {

			if (current_Item_Pokeball.GetComponent<Pokeball> ().Check_Pokeball_Anim_Opened ()) {
				Pokemon_2.GetComponent<Pokemon> ().Pokemon_Catch ();
				current_Item_Pokeball.GetComponent<Pokeball> ().ready_to_catch = false;
			}
			if (current_Item_Pokeball.GetComponent<Pokeball> ().Check_Pokemon_Caught()) {
				GetComponent<AudioSource> ().Stop ();
				GetComponent<AudioSource> ().clip = Caught_Fanfare_Music;
				GetComponent<AudioSource> ().Play ();
				Phase_Index = -4;

				Dialogue_UI.SetActive (true);
				Dialogue_UI.GetComponent<Dialogue_UI> ().Set_Dialogue_Text ("Congratulations! " + Pokemon_2.GetComponent<Pokemon>().name + " has been caught!", 4.0f);
			}

		}
		if (Phase_Index == -4) {
			End_Timer += Time.deltaTime;

			if (End_Timer > 4.0f) {
				Return_Pokemon (false);
				GetComponent<AudioSource> ().Stop ();
				GetComponent<AudioSource> ().clip = Victory_Music;
				GetComponent<AudioSource> ().Play ();
				GetComponent<AudioSource> ().loop = false;
				Home_UI.transform.GetChild (0).gameObject.SetActive (true);
				//Phase_Index = -999;
			}
		}
		if (Phase_Index == 1) {

			current_pokeball.GetComponent<Pokeball> ().Selected_Ready (); //fixes the pokeball animation bug

			if (Input.GetKeyDown (KeyCode.O)) {
				current_pokeball.GetComponent<Pokeball> ().LaunchAtTarget (pokemon_summon_target.transform.position);
				Dialogue_UI.SetActive (true);
				Dialogue_UI.GetComponent<Dialogue_UI> ().Set_Dialogue_Text ("Go! " + Pokemon_1.GetComponent<Pokemon>().name + "!", 0.75f);
			}


			if(pokemon_summon_target.GetComponent<Selection>().Check_OnSelected_And_Deselect()) {
				current_pokeball.GetComponent<Pokeball> ().LaunchAtTarget (pokemon_summon_target.transform.position);

				Dialogue_UI.SetActive (true);
				Dialogue_UI.GetComponent<Dialogue_UI> ().Set_Dialogue_Text ("Go! " + Pokemon_1.GetComponent<Pokemon>().name + "!", 0.75f);
			}

			if (current_pokeball.GetComponent<Pokeball> ().reached_target) {
				current_pokeball.SetActive (false);
				Pokemon_1.SetActive (true);
				Pokemon_1.GetComponent<Pokemon> ().Spawn (1);

				if (Switch_Start) {
					Switch_Start = false;
					Ready_To_Attack = false;
					end_turn_timer = 2;
					Phase_Index = 4;

					Pokemon_Battle_1 = Pokemon_1;
					Pokemon_Battle_2 = Pokemon_2;

					current_opponent = Pokemon_Battle_2;

					Home_UI.GetComponent<Home_Button_UI> ().show_Menu = false;
					Home_UI.transform.GetChild (0).gameObject.SetActive (false);
					Home_UI.GetComponent<Home_Button_UI> ().Disable ();
				} else {
					Phase_Index = 2;
				}

				Battle_UI.SetActive (true);
				Battle_UI.transform.position = user_view_bounds.transform.position;

				pokemon_summon_target.GetComponent<ParticleSystem> ().Stop ();

			}
		}
		if (Phase_Index == 2) {
			Home_UI.GetComponent<Home_Button_UI> ().Enable ();
			Home_UI.transform.GetChild (0).gameObject.SetActive (true);
			if (Input.GetKeyDown (KeyCode.K)) {
				Moves_UI [4].GetComponent<Selection> ().OnSelect ();
				Moves_UI [4].GetComponent<Button_push_animation> ().Animate ();
			}

			if (Input.GetKeyDown (KeyCode.L)) {
				Moves_UI [5].GetComponent<Selection> ().OnSelect ();
				Moves_UI [5].GetComponent<Button_push_animation> ().Animate ();
			}

			if (Moves_UI [4].GetComponent<Selection> ().Check_OnSelected_And_Deselect ()) {
				
				Dialogue_UI.SetActive (true);
				Dialogue_UI.GetComponent<Dialogue_UI> ().Set_Dialogue_Text (Pokemon_1.GetComponent<Pokemon> ().name + "!" + " Come Back!", 1.4f);

				Switch_Start = true;
			}

			if (Moves_UI [5].GetComponent<Selection> ().Check_OnSelected_And_Deselect ()) {
				Home_UI.GetComponent<Home_Button_UI> ().show_Menu = false;
				Home_UI.transform.GetChild (0).gameObject.SetActive (false);
				Home_UI.GetComponent<Home_Button_UI> ().Disable ();

				Pokeball_Selection_SFX.GetComponent<AudioSource> ().Play ();

				current_Item_Pokeball = Item_Pokeballs [0].transform.GetChild (0).gameObject;

				current_Item_Pokeball.SetActive (true);
				current_Item_Pokeball.GetComponent<Pokeball> ().Selected_Ready ();
				current_Item_Pokeball.GetComponent<Pokeball> ().reached_target = false;

				Phase_Index = -2;
				general_tap_event = false;
			}

			if (Switch_Start) {
				Home_UI.GetComponent<Home_Button_UI> ().show_Menu = false;
				Home_UI.transform.GetChild (0).gameObject.SetActive (false);
				Home_UI.GetComponent<Home_Button_UI> ().Disable ();
				Switch_Timer += Time.deltaTime;
				if (Switch_Timer > 1.5f) {
					Return_Pokemon (false);
				}
			}

			if (Input.GetKeyDown (KeyCode.Space)) {
				Home_UI.GetComponent<Selection> ().OnSelect ();
				Home_UI.GetComponent<Button_push_animation> ().Animate ();
			}
			if (Input.GetKeyDown (KeyCode.H)) {
				Pokemon_1.GetComponent<Pokemon_Stats> ().Add_HP (10);

			}
			if (Input.GetKeyDown (KeyCode.E)) {
				Moves_UI [0].GetComponent<Selection> ().OnSelect ();
				Moves_UI [0].GetComponent<Button_push_animation> ().Animate ();

			}
			if (Input.GetKeyDown (KeyCode.R)) {
				Moves_UI [1].GetComponent<Selection> ().OnSelect ();
				Moves_UI [1].GetComponent<Button_push_animation> ().Animate ();
			}

			if (Input.GetKeyDown (KeyCode.T)) {
				Moves_UI [2].GetComponent<Selection> ().OnSelect ();
				Moves_UI [2].GetComponent<Button_push_animation> ().Animate ();
			}
			if (Input.GetKeyDown (KeyCode.Y)) {
				Moves_UI [3].GetComponent<Selection> ().OnSelect ();
				Moves_UI [3].GetComponent<Button_push_animation> ().Animate ();
			}
			//
			if (!Ready_To_Attack) {
				for (int i = 0; i < 4; i++) {
					if (Moves_UI [i].GetComponent<Selection> ().Check_OnSelected_And_Deselect ()) {

						Moves_UI [i].GetComponent<Button_push_animation> ().Animate ();

						Attack_Index = i;

						Pokemon_1.GetComponent<Pokemon_Stats> ().SetCurrentMove (Attack_Index);

						move_selected_timer = 0.2f;
						Attack_Delay = 2.0f;


						Ready_To_Attack = true;

						Prepare_Pokemon_For_Battle ();

						Dialogue_UI.SetActive (true);
						Dialogue_UI.GetComponent<Dialogue_UI> ().Set_Dialogue_Text (Pokemon_Battle_1.GetComponent<Pokemon> ().name + " used " + Pokemon_Battle_1.GetComponent<Pokemon_Stats> ().Current_move.name + "!", 2.0f);
					}
				}
			} else {
				
				if (move_selected_timer <= 0) {
					Home_UI.GetComponent<Home_Button_UI> ().show_Menu = false;
					Home_UI.transform.GetChild (0).gameObject.SetActive (false);
					Home_UI.GetComponent<Home_Button_UI> ().Disable ();
				}
				if (Attack_Delay <= 0) {
					Pokemon_Battle_1.GetComponent<Pokemon> ().Do_Attack_Animation ();
					Phase_Index = 3;
					end_turn_timer = 2.5f;
				}

				if (move_selected_timer > 0)
					move_selected_timer -= Time.deltaTime;
				if (Attack_Delay > 0)
					Attack_Delay -= Time.deltaTime;


			}
		} else if (Phase_Index == 3) {

			if (Pokemon_Battle_2.GetComponent<Pokemon_Stats> ().damage_taken) {
				Phase_Index = 4;
			}

		} else if (Phase_Index == 4) {

			end_turn_timer -= Time.deltaTime;
			if (end_turn_timer <= 0) {
				
				if (!Return_Start) {
					if (current_opponent.GetComponent<Pokemon_Stats> ().Is_Fainted ()) {

						Dialogue_UI.SetActive (true);
						Dialogue_UI.GetComponent<Dialogue_UI> ().Set_Dialogue_Text (current_opponent.GetComponent<Pokemon> ().name + " fainted!", 1.4f);
						current_opponent.GetComponent<Pokemon> ().Cry (1);

						Return_Start = true;
					} else {
						end_turn_timer = 2.5f;
						current_opponent = Pokemon_Battle_1;
						Ready_To_Attack = false;

						Phase_Index = 5;
					}
				}

				if (Return_Start) {
					Home_UI.GetComponent<Home_Button_UI> ().show_Menu = false;
					Home_UI.transform.GetChild (0).gameObject.SetActive (false);
					Home_UI.GetComponent<Home_Button_UI> ().Disable ();
					Switch_Timer += Time.deltaTime;

					if (Switch_Timer > 1.5f) {
						
						if (!current_opponent.GetComponent<Pokemon_Stats> ().wild) {
							Pokemon_Battle_1.GetComponent<Pokemon_Stats> ().Ready ();
							Pokemon_Battle_2.GetComponent<Pokemon_Stats> ().Ready ();

							end_turn_timer = 2.5f;
							current_opponent = Pokemon_Battle_1;
							Ready_To_Attack = false;
							Return_Start = false;

							Return_Pokemon (true);
						} else {
							current_opponent.GetComponent<Pokemon> ().Pokemon_Return ();
							Return_Start = false;
							End_Timer = 0;
							Phase_Index = -99;

						}
					}
				}


			}
		} else if (Phase_Index == 5) {
			//AI select move
			if (!Ready_To_Attack) {

				Attack_Delay = 2.0f;

				Dialogue_UI.SetActive (true);
				Dialogue_UI.GetComponent<Dialogue_UI> ().Set_Dialogue_Text (Pokemon_Battle_2.GetComponent<Pokemon> ().name + " used " + Pokemon_Battle_2.GetComponent<Pokemon_Stats> ().Current_move.name + "!", 2.0f);

				Ready_To_Attack = true;
			} else {
				if (Attack_Delay <= 0) {
					Pokemon_Battle_2.GetComponent<Pokemon> ().Do_Attack_Animation ();
					Phase_Index = 6;
				}
			
				if (Attack_Delay > 0)
					Attack_Delay -= Time.deltaTime;
			}

		} else if (Phase_Index == 6) {
			Ready_To_Attack = false;

			if (Pokemon_Battle_1.GetComponent<Pokemon_Stats> ().damage_taken) {

				end_turn_timer -= Time.deltaTime;

				if (end_turn_timer <= 0) {


					if (!Return_Start) {
						if (current_opponent.GetComponent<Pokemon_Stats> ().Is_Fainted ()) {
							
							Dialogue_UI.SetActive (true);
							Dialogue_UI.GetComponent<Dialogue_UI> ().Set_Dialogue_Text (current_opponent.GetComponent<Pokemon> ().name + " fainted!", 1.4f);
							current_opponent.GetComponent<Pokemon> ().Cry (1);
							Return_Start = true;
						} else {
							Phase_Index = 2;
							Battle_UI.SetActive (true);
							Home_UI.GetComponent<Home_Button_UI> ().show_Menu = false;
							Home_UI.transform.GetChild (0).gameObject.SetActive (false);
							Pokemon_Battle_1.GetComponent<Pokemon_Stats> ().Ready ();
							Pokemon_Battle_2.GetComponent<Pokemon_Stats> ().Ready ();
						}
					}

					if (Return_Start) {
						Home_UI.GetComponent<Home_Button_UI> ().show_Menu = false;
						Home_UI.transform.GetChild (0).gameObject.SetActive (false);
						Home_UI.GetComponent<Home_Button_UI> ().Disable ();
						Switch_Timer += Time.deltaTime;
						if (Switch_Timer > 1.5f) {
							Pokemon_Battle_1.GetComponent<Pokemon_Stats> ().Ready ();
							Pokemon_Battle_2.GetComponent<Pokemon_Stats> ().Ready ();
							if (!current_opponent.GetComponent<Pokemon_Stats> ().wild) {
								Return_Start = false;
								Return_Pokemon (true);
							} else {
								current_opponent.GetComponent<Pokemon> ().Pokemon_Return ();
								Return_Start = false;
								End_Timer = 0;
								Phase_Index = -99;
							}
								
						}
					}

				}
			}
		} else if (Phase_Index == -98) {
			if (current_pokeball != null && current_pokeball.GetComponent<Pokeball> ().reached_target) {
				
			}
		} else if (Phase_Index == -99) {
			
			End_Timer += Time.deltaTime;

			if (End_Timer > 1.5f) {
				GetComponent<AudioSource> ().Stop ();
				GetComponent<AudioSource> ().clip = Victory_Music;
				GetComponent<AudioSource> ().Play ();
				GetComponent<AudioSource> ().loop = false;
				Dialogue_UI.SetActive (true);
				Dialogue_UI.GetComponent<Dialogue_UI> ().Set_Dialogue_Text ("Victory! " + current_opponent.GetComponent<Pokemon> ().name + " has been defeated.", 3.0f);
				Return_Pokemon (false);
				Home_UI.transform.GetChild (0).gameObject.SetActive (true);
			}
		} else if (Phase_Index == -100) {
			if (current_pokeball != null && current_pokeball.GetComponent<Pokeball> ().reached_target) {
				current_pokeball.SetActive (false);
			}
		} else if (Phase_Index == -101) {

			if (current_pokeball != null && current_pokeball.GetComponent<Pokeball> ().reached_target) {
				current_pokeball.SetActive (false);
			}

			End_Timer += Time.deltaTime;

			if (End_Timer > 1.5f) {
				Dialogue_UI.SetActive (true);
				Dialogue_UI.GetComponent<Dialogue_UI> ().Set_Dialogue_Text ("Out of pokemon! You are unable to battle.", 3.0f);
				Phase_Index = -102;
				End_Timer = 0;
			}

		} else if (Phase_Index == -102) {
			End_Timer += Time.deltaTime;

			if (End_Timer > 4.0f) {
				SceneManager.LoadScene ("Main");
			}
		}
	}
}
