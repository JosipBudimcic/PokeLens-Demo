using UnityEngine;
using System.Collections;

public class Home_Button_UI : MonoBehaviour {

	public GameObject[] UI_Items;
	public bool show_Menu = false;
	float time_passed = 0;
	// Use this for initialization
	void Start () {
		Disable ();
	}

	public void Disable()
	{
		GetComponent<BoxCollider> ().enabled = false;
		GetComponent<MeshRenderer> ().enabled = false;
	}

	public void Enable()
	{
		GetComponent<BoxCollider> ().enabled = true;
		GetComponent<MeshRenderer> ().enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (GetComponent<Selection> ().OnPressed) {
			show_Menu = !show_Menu;
			GetComponent<Selection> ().OnPressed = false;
			GetComponent<Button_push_animation> ().Animate ();
			foreach (GameObject ui_item in UI_Items)
				ui_item.GetComponent<Button_push_animation> ().Animate ();
		}
		
		foreach (GameObject ui_item in UI_Items)
			ui_item.SetActive (show_Menu);

		time_passed += Time.deltaTime;
		//
		Color color = GetComponent<Renderer>().material.color;
		color.a = (Mathf.Sin (time_passed * 5)/2) + 0.6f;
		GetComponent<Renderer> ().material.color = color;
	}
}
