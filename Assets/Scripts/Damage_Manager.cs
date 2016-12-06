using UnityEngine;
using System.Collections;

public class Damage_Manager : MonoBehaviour {

    float[,] Type_chart = new float[,] { 
        { 1, 1, 1, 1, 1, 0.5f, 1, 0, 0.5f, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
        { 2, 1, 0.5f, 0.5f, 1, 2, 0.5f, 0, 2, 1, 1, 1, 1, 0.5f, 2, 1, 2, 0.5f},
        { 1, 2, 1, 1, 1, 0.5f, 2, 1, 0.5f, 1, 1, 2, 0.5f, 1, 1, 1, 1, 1 },
        { 1, 1, 1, 0.5f, 0.5f, 0.5f, 1, 0.5f, 0, 1, 1, 2, 1, 1, 1, 1, 1, 2 },
        { 1, 1, 0, 2, 1, 2, 0.5f, 1, 2, 2, 1, 0.5f, 2, 1, 1, 1, 1, 1 },
        { 1, 0.5f, 2, 1, 0.5f, 1, 2, 1, 0.5f, 2, 1, 1, 1, 1, 2, 1, 1, 1 },
        { 1, 0.5f, 0.5f, 0.5f, 1, 1, 1, 0.5f, 0.5f, 0.5f, 1, 2, 1, 2, 1, 1, 2, 0.5f },
        { 0, 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 2, 1, 1, 0.5f, 1 },
        { 1, 1, 1, 1, 1, 2, 1, 1, 0.5f, 0.5f, 0.5f, 1, 0.5f, 1, 2, 1, 1, 2 },
        { 1, 1, 1, 1, 1, 0.5f, 2, 1, 2, 0.5f, 0.5f, 2, 1, 1, 2, 0.5f, 1, 1 },
        { 1, 1, 1, 1, 2, 2, 1, 1, 1, 2, 0.5f, 0.5f, 1, 1, 1, 0.5f, 1, 1 },
        { 1, 1, 0.5f, 0.5f, 2, 2, 0.5f, 1, 0.5f, 0.5f, 2, 0.5f, 1, 1, 1, 0.5f, 1, 1 },
        { 1, 1, 2, 1, 0, 1, 1, 1, 1, 1, 2, 0.5f, 0.5f, 1, 1, 0.5f, 1, 1 },
        { 1, 2, 1, 2, 1, 1, 1, 1, 0.5f, 1, 1, 1, 1, 0.5f, 1, 1, 0, 1 },
        { 1, 1, 2, 1, 2, 1, 1, 1, 0.5f, 0.5f, 0.5f, 2, 1, 1, 0.5f, 2, 1, 1 },
        { 1, 1, 1, 1, 1, 1, 1, 1, 0.5f, 1, 1, 1, 1, 1, 1, 2, 1, 0 },
        { 1, 0.5f, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 2, 1, 1, 0.5f, 0.5f },
        { 1, 2, 1, 0.5f, 1, 1, 1, 1, 0.5f, 0.5f, 1, 1, 1, 1, 1, 2, 2, 1 }
    };

    string[] Type = new string[] { "Normal", "Fight", "Flying", "Poison", "Ground", "Rock", "Bug", "Ghost", "Steel", "Fire", "Water", "Grass", "Electric", "Psychic", "Ice", "Dragon", "Dark", "Fairy" };

    // Use this for initialization
    void Start() {

    }

    int FindTypeIndex(string type)
    {
        for(int i = 0; i < Type.Length; i++)
            if(Type[i] == type)
            {
                return i;
            }

        return -1;
    }

    public float TypeDamageMultiplier(string attacker_type, string defender_type)
    {

        int type1_index = FindTypeIndex(attacker_type);
        int type2_index = FindTypeIndex(defender_type);

        return Type_chart[type1_index, type2_index];
    }

    public float Damage(GameObject attacker, GameObject defender)
    {
        float damage = 0;

		float STAB = attacker.GetComponent<Pokemon_Stats>().STAB; //fix this

        float Critical = 1;

        float Type = 0;

        float Other = 1;

		float level = attacker.GetComponent<Pokemon_Stats>().Level;

		float attack = attacker.GetComponent<Pokemon_Stats>().Attack;

		float defense = defender.GetComponent<Pokemon_Stats>().Defense;

		float base_power = attacker.GetComponent<Pokemon_Stats>().Current_move.base_power;

		if (base_power > 0) {
			
			for (int i = 0; i < defender.GetComponent<Pokemon_Stats> ().types.Length; i++) {

				if (i < attacker.GetComponent<Pokemon_Stats> ().types.Length) {
					if (attacker.GetComponent<Pokemon_Stats> ().Current_move.type == attacker.GetComponent<Pokemon_Stats> ().types [i])
						STAB = 1.5f;
				}

				float DamageMultiplier = TypeDamageMultiplier (attacker.GetComponent<Pokemon_Stats> ().Current_move.type, defender.GetComponent<Pokemon_Stats> ().types [i]);
				Type += DamageMultiplier;

				if (DamageMultiplier >= 2)
					GetComponent<Battle_Manager> ().Set_Dialogue ("It's super effective!", 1.5f);
				else if (DamageMultiplier <= 0.5)
					GetComponent<Battle_Manager> ().Set_Dialogue ("It's not very effective.", 1.5f);
			}

			float modifier = STAB * Type * Critical * Other * Random.Range (0.85f, 1.0f);

			damage = (int)((((2 * level + 10) / 250) * (attack / defense) * base_power + 2) * modifier);
		}
        return damage;
    }

	public void Perform_Attack(GameObject attacker_Pokemon, GameObject defender_Pokemon)
	{
		float damage = Damage (attacker_Pokemon, defender_Pokemon);
		defender_Pokemon.GetComponent<Pokemon_Stats> ().TakeDamage (damage, attacker_Pokemon.GetComponent<Pokemon_Stats>().Current_move.index);
	}

	public void Perform_Affect(GameObject attacker_Pokemon, GameObject defender_Pokemon)
	{
		float damage = Damage (attacker_Pokemon, defender_Pokemon);
		defender_Pokemon.GetComponent<Pokemon_Stats> ().TakeEffect (attacker_Pokemon.GetComponent<Pokemon_Stats>().Current_move.stat_effect, attacker_Pokemon.GetComponent<Pokemon_Stats>().Current_move.index);
		GetComponent<Battle_Manager> ().Set_Dialogue (defender_Pokemon.GetComponent<Pokemon> ().name + "'s " +  attacker_Pokemon.GetComponent<Pokemon_Stats>().Current_move.stat_effect + " fell!", 2.0f);

	}

	// Update is called once per frame
	void Update () {
		
    }
}
