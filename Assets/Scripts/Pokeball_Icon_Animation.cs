using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Pokeball_Icon_Animation : MonoBehaviour {

    float multiplier = 1.0f;
    public bool start = false;
    // Use this for initialization
    Renderer image;
	void Start () {
        image = GetComponent<Renderer>();
    }
	
	// Update is called once per frame
	void Update () {
        
        if(start)
        {
            transform.Rotate(Vector3.forward * Time.deltaTime * 500);

            transform.localScale += new Vector3(0.015f * multiplier, 0.015f * multiplier, 0.2f);

            multiplier += Time.deltaTime * 6;

            if (transform.localScale.x > 10)
            {
				if (image.material.color.a > 0) {
					
					Color temp_color = image.material.color;
					temp_color.a -= Time.deltaTime * 5;
					image.material.color = temp_color;

				} else {
					
					gameObject.SetActive (false);

				}
            }
        }
    }
}
