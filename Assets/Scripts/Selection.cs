using UnityEngine;
using System.Collections;

public class Selection : MonoBehaviour {
	public GameObject selection_SFX;
	public string nameID;
	public bool OnPressed = false;
	// Use this for initialization
	void Start () {
	}
	public void OnSelect()
	{
		OnPressed = true;
		selection_SFX.GetComponent<AudioSource> ().Play ();
	}

	public void Deselect()
	{
		OnPressed = false;
	}

	public bool Check_OnSelected_And_Deselect()
	{
		bool temp = OnPressed;
		OnPressed = false;
		return temp;
	}
	public bool Check_OnSelected()
	{
		return OnPressed;
	}
	// Update is called once per frame
	void Update () {
		
	}
}
