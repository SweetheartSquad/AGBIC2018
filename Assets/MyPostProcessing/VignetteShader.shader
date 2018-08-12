Shader "Custom/VignetteShader"
{
	Properties
	{
		_Radius ("Radius", Range(-1.0, 1.0)) = 0.0
		_Amount ("Amount", Range(0.0, 1.0)) = 0.5
		_Soft ("Soft", Range(0.0, 32.0)) = 3.0
		_MainTex ("Render Texture", 2D) = "white" {}
		_Color ("Color", Color) = (0,0,0)
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			float _Radius;
			float _Amount;
			float _Soft;
			float3 _Color;
			sampler2D _MainTex;
			sampler2D _VignetteTex;

			fixed4 frag (v2f_img i) : SV_Target
			{
				float2 uv = i.uv;
				float2 uvCentered = uv * 2.0 - 1.0;
				float4 c = tex2D(_MainTex, uv);

				float v = pow(clamp((distance(uvCentered, 0.0))+_Radius, 0, 1), 33-_Soft);
				c.rgb = lerp(c.rgb, _Color, v*_Amount);

				return fixed4(c);
			}
			// START BOILERPLATE
			ENDCG
		}
	}
}