using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playercam : MonoBehaviour {
	private Camera cam;
	public Transform target;
	public Transform anchor;
	// Use this for initialization
	void Start () {
		cam = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
		cam.transform.LookAt(target);
		cam.transform.position = Vector3.Lerp(cam.transform.position, anchor.position, .1f);
	}
}
