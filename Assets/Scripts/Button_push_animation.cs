using UnityEngine;
using System.Collections;

public class Button_push_animation : MonoBehaviour {

	// Use this for initialization
	public float timer = 0;
	public float scaleFactor = 1.1f;
	public float speed = 25;
	private Vector3 original_scale;
	void Start () {
		original_scale = this.transform.localScale;
	}
	public void Animate()
	{
		if(timer <= 0)
			timer = 1;
	}
	
	// Update is called once per frame
	void Update () {
		if (timer > 0) 
		{
			timer -= Time.deltaTime * speed;
			this.transform.localScale *= (scaleFactor);
		} 
		else if(this.transform.localScale.x > original_scale.x)
		{
			this.transform.localScale *= (0.8f);
		}
		else if (this.transform.localScale.x < original_scale.x) 
		{
			this.transform.localScale = original_scale;
		}
	}
}
