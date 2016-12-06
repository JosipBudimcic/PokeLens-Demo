using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Dialogue_UI : MonoBehaviour {

	public GameObject text_object;

	int current_dialogue_char_position = 0;
	float Delay = 0.02f;
	string Text = "";
	string[] additionalLines;
	float Timer = 0.0f;
	float Popup_delay = 0.0f;
	// Use this for initialization
	void Start () {
	}


	 //Display_Dialogue

	IEnumerator Play_Dialogue_Text()
	{
		while (current_dialogue_char_position < Text.Length) {
			text_object.GetComponent<TextMesh> ().text += Text [current_dialogue_char_position++];
			yield return new WaitForSeconds (Delay);

		}
	}

	public void Set_Dialogue_Text(string _text, float _timer)
	{
		text_object.GetComponent<TextMesh> ().text = "";
		current_dialogue_char_position = 0;
		Text = _text;
		Timer = _timer;

		StartCoroutine ("Play_Dialogue_Text");

	}

	// Update is called once per frame
	void Update () {
		if (GetComponent<Selection> ().OnPressed)
			gameObject.SetActive (false);

		Timer -= Time.deltaTime;

		if (Timer <= 0)
			gameObject.SetActive (false);
	}
}
