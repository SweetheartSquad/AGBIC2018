using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
[AddComponentMenu("Image Effects/CustomImageEffect")]
public class CustomImageEffect : MonoBehaviour
{
#if UNITY_EDITOR
	public ShaderProperty[] properties;
#endif

	private Material m_material;
	public Material material {
		get {
			if(m_material == null) {
				// try to load the material
				m_material = Resources.Load("Materials/"+ GetType() + "Material", typeof(Material)) as Material;
#if UNITY_EDITOR
				// if the material doesn't exist, create a new one and save it to the resources folder
				if(!m_material) {
					m_material = new Material(Shader.Find("Custom/" + GetType() + "Shader"));
					AssetDatabase.CreateAsset(m_material, "Assets/Resources/Materials/" + GetType() + "Material.mat");
				}
#endif
			}
			return m_material;
		}
	}



	protected void Awake()
	{
		if(!SystemInfo.supportsImageEffects) {
			enabled = false;
			print("Image Effects not supported");
			return;
		}

		Shader shader = material.shader;
		if(!shader || !shader.isSupported) {
			enabled = false;
			print("Shader not found/supported");
		}
#if UNITY_EDITOR
		ExposeProperties();
#endif
	}

#if UNITY_EDITOR
	private void OnDestroy()
	{
		if(m_material) {
			AssetDatabase.DeleteAsset("Assets/Reousrces/Materials/" + GetType() + "Material.mat");
		}
	}
#endif

#if UNITY_EDITOR
	public void ExposeProperties()
	{
		int l = ShaderUtil.GetPropertyCount(material.shader);
		properties = new ShaderProperty[l];
		for(int i = 0; i < l; ++i) {
			properties[i] = ShaderProperty.Get(material, i);
		}
	}
#endif

	private void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		Graphics.Blit(src, dest, material);
	}
}