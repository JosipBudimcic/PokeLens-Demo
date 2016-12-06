using UnityEngine;
using System.Collections;

public class stripe_animation : MonoBehaviour {

    float timer;
    public float start_time = 1.5f; //seconds
    public bool start = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (start)
        {
            timer += Time.deltaTime;

            if (timer >= start_time)
            {
                transform.Translate(Vector3.right * -Time.deltaTime * 5);
            }

			if (timer > 3.0f)
				gameObject.SetActive (false);
        }

    }
}
