using UnityEngine;
using System.Collections;

public class CameraFacingBillboard : MonoBehaviour {

	public Camera camera;

	void Update()
	{
		transform.LookAt(transform.position + camera.transform.rotation * Vector3.forward,
			Vector3.up);
	}
}
