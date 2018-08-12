Shader "Custom/DepthFogShader"
{
	Properties
	{
		_Amount ("Amount", Range(0.0, 1.0)) = 1
		_Center ("Center", Range(0.0, 1.0)) = 0.5
		_Range ("Range", Range(0.0, 1.0)) = 0.5
		_MainTex ("Render Texture", 2D) = "white" {}
		_FogTex ("Fog Texture", 2D) = "black" {}
		_Texturize ("Texturize", Range(0.0, 1.0)) = 0.0
	}
	SubShader
	{
		Pass
		{
			Cull Off ZWrite Off ZTest Always
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			float _Amount;
			float _Center;
			float _Range;
			float _Texturize;
			sampler2D _MainTex;
			sampler2D _FogTex;
			sampler2D _CameraDepthTexture;
			float4 _CameraDepthTexture_TexelSize;

			float map(float value, float min1, float max1, float min2, float max2){
				float p = (value - min1) / (max1 - min1);
				return p * (max2 - min2) + min2;
			}

			struct v2f {
			   float4 pos:SV_POSITION;
			   half2  uv:TEXCOORD0;
			   float4 screen:TEXCOORD1;
			};

			//Vertex Shader
			v2f vert (appdata_base v){
				v2f o;
				o.uv = v.texcoord.xy;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.screen = ComputeScreenPos(o.pos);
				return o;
			}

			float getDepth(float4 pos){
				float depth = Linear01Depth(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(pos)));
				depth = map(depth, _Center-_Range/2, _Center+_Range/2, 1, 0);
				depth = clamp(depth, 0, 1);
				return depth;
			}

			fixed4 frag (v2f i) : SV_Target 
			{
				float2 uv = i.uv;

				float4 c = tex2D(_MainTex, uv);

				#if 1 // anti-alias depth
				float depth = 0.0;
				float e = _CameraDepthTexture_TexelSize.y*pow(getDepth(i.screen),2.0)*10.0;
				depth += abs(getDepth(i.screen + float4(e, 0.0, 0.0, 0.0)) - getDepth(i.screen + float4(-e, 0.0, 0.0, 0.0)));
				depth += abs(getDepth(i.screen + float4(0.0, e, 0.0, 0.0)) - getDepth(i.screen + float4(0.0, -e, 0.0, 0.0)));
				depth += abs(getDepth(i.screen + float4(0.0, 0.0, e, 0.0)) - getDepth(i.screen + float4(0.0, 0.0, -e, 0.0)));
				depth /= 3.0;
				depth = getDepth(i.screen)*c.rgb + depth*10.0;
				#else
				float depth = getDepth(i.screen);
				#endif

				float3 fog = float3(depth,depth,depth);

				depth = 1.0-depth;

				#if 0 // remove fog from skybox
				depth = step(depth,0.999)*depth;
				#endif

				// fog wall
				fog = lerp(fog, tex2D(_FogTex, half2(1.0-depth,0.5)).rgb, _Texturize);
				fog = lerp(c.rgb, fog, _Amount);
				c.rgb = clamp(fog,0.0,1.0);
				return fixed4(c);
			}
			// START BOILERPLATE
			ENDCG
		}
	}
}
