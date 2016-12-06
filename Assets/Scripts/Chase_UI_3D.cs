using UnityEngine;
using System.Collections;

public class Chase_UI_3D : MonoBehaviour {

	public Transform origin_target;
	public Transform target;
	public float smoothTime = 3.0f;
	public GameObject camera;
	private Vector3 velocity = Vector3.zero;

	public bool Follow_Position = true;
	public bool Follow_Orientation = false;

	// Use this for initialization
	void Start () {
	
	}

	public void ResetTarget()
	{
		target = origin_target;
	}
	// Update is called once per frame
	void Update () {
		if(Follow_Position)
			transform.position = Vector3.SmoothDamp (transform.position, target.transform.position, ref velocity, smoothTime);
		if(Follow_Orientation)
			transform.LookAt (transform.position + camera.transform.rotation * Vector3.forward,	camera.transform.rotation * Vector3.up);
		else
			transform.LookAt (transform.position + camera.transform.rotation * Vector3.forward, Vector3.up);
	}
}
