using UnityEngine;
using System.Collections;

public class UI_View_Collision_Check : MonoBehaviour {
	public string Collider_Tag = "UI_UserViewBounds";
	// Use this for initialization
	void Start () {
	
	}
	void OnTriggerEnter(Collider other) {
		if (other.tag == Collider_Tag) {
			GetComponent<Chase_UI_3D> ().Follow_Position = false;
		}
	}

	void OnTriggerExit(Collider other) {
		if (other.tag == Collider_Tag) {
			GetComponent<Chase_UI_3D> ().Follow_Position = true;
		}
	}
	// Update is called once per frame
	void Update () {
	
	}
}
