using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
[CustomEditor(typeof(CustomImageEffect), true)]
public class CustomImageEffectEditor : Editor
{
	public override void OnInspectorGUI()
	{
		//DrawDefaultInspector();

		CustomImageEffect s = (CustomImageEffect)target;
		s.ExposeProperties();
		foreach(ShaderProperty p in s.properties) {
			if(p.name != "_MainTex"){
				p.Draw();
				p.Update();
			}
		}
	}
}

abstract public class ShaderProperty
{
	public string name;
	public string description;
	public int id;
	public ShaderUtil.ShaderPropertyType type;
	protected Material material;

	public ShaderProperty(Material _material, int _index, ShaderUtil.ShaderPropertyType _type)
	{
		material = _material;
		name = ShaderUtil.GetPropertyName(material.shader, _index);
		description = ShaderUtil.GetPropertyDescription(material.shader, _index);
		id = Shader.PropertyToID(name);
		type = _type;

	}

	// factory for returning shader property for material at index
	static public ShaderProperty Get(Material material, int index)
	{
		Shader shader = material.shader;
		ShaderUtil.ShaderPropertyType type = ShaderUtil.GetPropertyType(shader, index);
		switch(type) {
			case ShaderUtil.ShaderPropertyType.Float:
				return new ShaderPropertyFloat(material, index);
			case ShaderUtil.ShaderPropertyType.Range:
				return new ShaderPropertyRange(material, index);
			case ShaderUtil.ShaderPropertyType.TexEnv:
				return new ShaderPropertyTexEnv(material, index);
			case ShaderUtil.ShaderPropertyType.Vector:
				return new ShaderPropertyVector(material, index);
			case ShaderUtil.ShaderPropertyType.Color:
				return new ShaderPropertyColor(material, index);
			default:
				throw new UnityException("Shader property type not supported");
		}
	}

	public abstract void Draw();
	public abstract void Update();
}

public class ShaderPropertyFloat : ShaderProperty
{
	private float value;

	public ShaderPropertyFloat(Material _material, int _index) :
		base(_material, _index, ShaderUtil.ShaderPropertyType.Float)
	{
		value = _material.GetFloat(id);
	}

	override public void Draw()
	{
		value = EditorGUILayout.FloatField(description, value);
	}

	override public void Update()
	{
		material.SetFloat(id, value);
	}
}

public class ShaderPropertyRange : ShaderProperty
{
	private float min;
	private float max;
	private float value;

	public ShaderPropertyRange(Material _material, int _index) :
		base(_material, _index, ShaderUtil.ShaderPropertyType.Range)
	{
		value = _material.GetFloat(id);
		min = ShaderUtil.GetRangeLimits(material.shader, _index, 1);
		max = ShaderUtil.GetRangeLimits(material.shader, _index, 2);
	}

	override public void Draw()
	{
		value = EditorGUILayout.Slider(description, value, min, max);
	}

	override public void Update()
	{
		material.SetFloat(id, value);
	}
}
public class ShaderPropertyVector : ShaderProperty
{
	private Vector4 value;

	public ShaderPropertyVector(Material _material, int _index) :
		base(_material, _index, ShaderUtil.ShaderPropertyType.Vector)
	{
		value = _material.GetVector(id);
	}

	override public void Draw()
	{
		value = EditorGUILayout.Vector4Field(description, value);
	}

	override public void Update()
	{
		material.SetVector(id, value);
	}
}

public class ShaderPropertyTexEnv : ShaderProperty
{
	private Texture value;
	private UnityEngine.Rendering.TextureDimension dim;
	private System.Type fieldType;

	public ShaderPropertyTexEnv(Material _material, int _index) :
		base(_material, _index, ShaderUtil.ShaderPropertyType.TexEnv)
	{
		value = material.GetTexture(id);
		dim = ShaderUtil.GetTexDim(material.shader, _index);
		if(dim == UnityEngine.Rendering.TextureDimension.Tex2D) {
			fieldType = typeof(Texture2D);
		} else {
			fieldType = typeof(Texture3D);
		}
	}

	override public void Draw()
	{
		value = (Texture)EditorGUILayout.ObjectField(description, value, fieldType, true);
	}

	override public void Update()
	{
		material.SetTexture(id, value);
	}
}

public class ShaderPropertyColor : ShaderProperty
{
	private Color value;
	private UnityEngine.Rendering.TextureDimension dim;
	private System.Type fieldType;

	public ShaderPropertyColor(Material _material, int _index) :
		base(_material, _index, ShaderUtil.ShaderPropertyType.Color)
	{
		value = material.GetColor(id);
	}

	override public void Draw()
	{
		value = EditorGUILayout.ColorField(description, value);
	}

	override public void Update()
	{
		material.SetColor(id, value);
	}
}
#endif