using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

struct Anim
{
	public Transform transform;
	public Vector3 value;
	public Anim(Transform transform, Vector3 value) {
		this.transform = transform;
		this.value = value;
	}
}


public class animationmanager : MonoBehaviour
{
	public TextAsset skeletonSource;
	public TextAsset[] framesSource;
	private List<List<Anim>> frames = new List<List<Anim>>();
	private EasingFunction.Function easingFunction;
	// Use this for initialization
	void Start()
	{
		string text = skeletonSource.text;
		List<Anim> bones = parseAnim(text, transform);
		for(int i = 0; i < bones.Count; ++i) {
			Anim a = bones[i];
			a.transform.localPosition = a.value;
		}

		
		for(int i = 0; i < framesSource.Length; ++i) {
			TextAsset a = framesSource[i];
			frames.Add(parseAnim(a.text, transform));
		}

		
		easingFunction = EasingFunction.GetEasingFunction(EasingFunction.Ease.EaseInBack);
	}

	// Update is called once per frame
	void Update()
	{
		float t = Time.time * 4.0f % frames.Count;
		float interpolate = t%1.0f;
		interpolate = easingFunction(0.0f, 1.0f, interpolate);
		int frame = (int)Mathf.Floor(t);
		List<Anim> bones1 = frames[frame];
		List<Anim> bones2 = frames[(frame+1)%frames.Count];
		for(int i = 0; i < bones1.Count; ++i) {
			Anim a = bones1[i];
			Anim b = bones2[i];
			a.transform.localEulerAngles = Vector3.Slerp(a.value,b.value, interpolate);
		}
	}


	static List<Anim> parseAnim(string input, Transform root)
	{
		List<Anim> output = new List<Anim>();
		string[] lines = input.Split('\n');
		int curDepth = -1;

		Transform curTransform = root;

		// working group
		for (int i = 0; i < lines.Length; ++i)
		{
			string line = lines[i];
			// skip empty lines
			if (line.Trim().Length == 0)
			{
				continue;
			}

			Match match = Regex.Match(line, @"(\t*)(.+?)\s(-?[0-9.]+)\s(-?[0-9.]+)\s(-?[0-9.]+)");
			int depth = match.Groups[1].Value.Length;
			string obj = match.Groups[2].Value;
			float x = float.Parse(match.Groups[3].Value);
			float y = float.Parse(match.Groups[4].Value);
			float z = float.Parse(match.Groups[5].Value);
			Debug.Log(depth + " " + obj + " " + x + "," + y + "," + z);
			Debug.Log(curTransform.gameObject.name + " " + curDepth + " -> " + depth);
			for (int j = curDepth; j >= depth; --j)
			{
				curTransform = curTransform.parent;
			}
			curDepth = depth;
			// Debug.Log(curTransform.gameObject.name);
			curTransform = curTransform.Find(obj).transform;
			// Debug.Log(curTransform.gameObject.name);
			output.Add(new Anim(curTransform, new Vector3(x, y, z)));
		}
		return output;
	}
}
