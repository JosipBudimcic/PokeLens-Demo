using UnityEngine;
using System;

[Serializable]
public class BattleMove {

	public int index; //sorting index
	public string name;
	public string type;
	public string stat_effect; //lower attack, defense, speed, accuracy
	public int animation_index; //1 = regular, 2 = special
	public int speed_priority;
	public int base_power;
	public int PP;
}
