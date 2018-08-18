using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playercam : MonoBehaviour {
	private Camera cam;
	public Transform targetShoulder;
	public Transform anchorShoulder;
	public float fovShoulder = 70.0f;
	public Transform targetSelfie;
	public Transform anchorSelfie;
	public float fovSelfie = 55.0f;
	public bool selfieMode = false;
	void Start () {
		cam = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
		Transform target = selfieMode ? targetSelfie : targetShoulder;
		Transform anchor = selfieMode ? anchorSelfie : anchorShoulder;
		float fov = selfieMode ? fovSelfie : fovShoulder;
		cam.transform.LookAt(target);
		cam.transform.position = Vector3.Lerp(cam.transform.position, anchor.position, .1f);
		cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fov, .1f);
	}
}
