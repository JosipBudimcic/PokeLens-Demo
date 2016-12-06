using UnityEngine;
using System.Collections;

public class Health_Bar : MonoBehaviour {
    float elapsed_time = 0;
    Vector3 original_position;
	// Use this for initialization
	void Start () {
        original_position = transform.position;

    }
	
	// Update is called once per frame
	void Update () {
        elapsed_time += Time.deltaTime * 1.5f;

        transform.position = original_position + new Vector3(0,Mathf.Sin(elapsed_time) * 0.02f,0);

    }
}
