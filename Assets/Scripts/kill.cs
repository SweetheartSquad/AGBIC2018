using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class kill : MonoBehaviour {
	private animationmanager anim;
	// Use this for initialization
	void Start () {
		anim = this.GetComponent<animationmanager>();
	}
	
	// Update is called once per frame
	float t = 120.0f;
	void Update () {
		t -= 1.0f;
		if(t <= 0.0f && anim){
			Destroy(anim);
			anim = null;
			Rigidbody[] bs = GetComponentsInChildren<Rigidbody>();
			for(int i = 0; i < bs.Length; ++i) {
				Rigidbody b = bs[i];
				b.isKinematic = false;
			}
			ParticleSystem[] bloods = GetComponentsInChildren<ParticleSystem>();
			for(int i = 0; i < bloods.Length; ++i) {
				ParticleSystem blood = bloods[i];
				ParticleSystem.MainModule m = blood.main;
				m.duration = Random.Range(2.5f, 10.0f);
				blood.Play();
			}
		}
	}
}
