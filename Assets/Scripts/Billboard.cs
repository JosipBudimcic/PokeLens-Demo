using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour {
    public Transform target;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 lookPoint = transform.position - target.position;
        transform.LookAt(lookPoint);
    }
}
