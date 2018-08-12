using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthFog : CustomImageEffect {
	private void Start()
	{
		//base.Start();
		Camera cam = GetComponent<Camera>();
		cam.depthTextureMode = DepthTextureMode.Depth;
	}
}
