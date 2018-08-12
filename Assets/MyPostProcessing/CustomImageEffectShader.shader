Shader "Custom/PSX Image Effect"
{
	Properties
	{
		_Pixelate ("Pixelate", Vector) = (1280, 720, 0, 0)
		_Posterize ("Posterize", Range(0.0, 256.0)) = 32.0
		_PosterizeAmount ("Posterize Amount", Range(0.0, 1.0)) = 1.0
		_GrilleX("Grille X", Range(0.0, 1.0)) = 0.6
		_GrilleY("Grille Y", Range(0.0, 1.0)) = 0.3
		_ChrAb ("Chromatic Aberration", Range(0.0, 64.0)) = 1.0
		_MainTex ("Render Texture", 2D) = "white" {}
		_DitherTex ("Dither Texture", 2D) = "white" {}
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
			
			float2 _Pixelate;
			float _Posterize;
			float _PosterizeAmount;
			float _GrilleX;
			float _GrilleY;
			float _ChrAb;
			sampler2D _MainTex;
			sampler2D _DitherTex;
			float4 _MainTex_TexelSize;
			float4 _DitherTex_TexelSize;


			// END BOILERPLATE
			static const float PI = 3.14159265359f;
			static const float PI_TWO = 6.28318530718f;

			float rand(float2 uv){
				return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
			}

			float4 tex(float2 uv){
				return tex2D(_MainTex, uv);
			}

			float2 getSize(float4 texel){
				return float2(
					abs(texel.z),
					abs(texel.w)
				);
			}
			
			// chromatic abberation
			float4 chrAbb(const float2 uv, const float separation, const float rotation){
				const float2 o = 1.0f/getSize(_MainTex_TexelSize)*separation;
				const float2 uvR = uv + o*float2(sin(1.0f/3.0f*PI_TWO+rotation), cos(1.0f/3.0f*PI_TWO+rotation));
				const float2 uvG = uv + o*float2(sin(2.0f/3.0f*PI_TWO+rotation), cos(2.0f/3.0f*PI_TWO+rotation));
				const float2 uvB = uv + o*float2(sin(3.0f/3.0f*PI_TWO+rotation), cos(3.0f/3.0f*PI_TWO+rotation));
				return float4(
					tex(uvR).r,
					tex(uvG).g,
					tex(uvB).b,
					tex(uv).a
				);
			}
			
			float2 pixelScale;
			float2 pixelSizeScaled;
			float2 pixelate(const float2 uv){
				return float2(floor(uv*pixelSizeScaled)+0.5f)/pixelSizeScaled;
			}
			
			float grille(const float2 uv, const float2 amount){
				float2 g = frac(uv*pixelSizeScaled);
				g *= 2.0;
				g -= 1.0;
				g = abs(g);
				g = 1.0 - g*amount.yx*clamp(pixelScale-2.0, 0,1);
				return 1.0-pow(1.0-g.y*g.x,2.0);
			}


			// returns binary dither mask of c
			float3 getDither(float2 uv, float3 c) {
				float2 order = getSize(_DitherTex_TexelSize);
			
				uv = floor(fmod(uv, order));
 
				float limit = tex2D(_DitherTex, uv/order).r;
				return step(limit, c);
			}

			fixed4 frag (v2f_img i) : SV_Target
			{
				_Pixelate = floor(_Pixelate);
				pixelScale = getSize(_MainTex_TexelSize)/_Pixelate;
				pixelSizeScaled = getSize(_MainTex_TexelSize)/pixelScale;
				float2 uv = i.uv;
				//uv += 0.5f/getSize(_MainTex_TexelSize);

				// pixelization
				float2 uvPixelated = pixelate(uv);
				//uvPixelated = uv;
				//float2 uvCentered = uvPixelated * 2.0 - 1.0;
				//dfloat2 uvScreen = uv * getSize(_MainTex_TexelSize);

				float4 o = tex(uvPixelated);
				
				// c -= tex (uvPixelated.xy + float2(1.0/uvScreen.x,1.0/uvScreen.y));
				// c -= tex (uvPixelated.xy - float2(1.0/uvScreen.x,1.0/uvScreen.y));
				// c -= tex (uvPixelated.xy + float2(-1.0/uvScreen.x,1.0/uvScreen.y));
				// c -= tex (uvPixelated.xy - float2(1.0/uvScreen.x,-1.0/uvScreen.y));
				
				float4 c = lerp(o, chrAbb(uvPixelated, _ChrAb, 0.5), o.a);

				// dither + posterization
				//float3 d = float3(rand(uvPixelated.xy+_Time.x),rand(uvPixelated.xy+_Time.w),rand(uvPixelated.xy+_Time.z));//frac(uvPixelated.x*_Pixelate.x/2.0)-frac(uvPixelated.y*_Pixelate.y/2.0); // this is the cheap one to add before posterize
				float3 posterize = _Posterize;//float3(_Posterize, _Posterize*2.0, _Posterize); // give g an extra bit
				float3 old = c.rgb;
				c.rgb = lerp(c.rgb, floor(c.rgb*posterize)/posterize, _PosterizeAmount);
				
				float3 dither = getDither(uv*_Pixelate, (old-c.rgb)*posterize)/posterize;
				c.rgb += dither;

				//c.rgb = (old-c.rgb)*5;

				// pixel grille
				c.rgb *= grille(uv,float2(_GrilleX,_GrilleY));

				//c.b=0;
				//c.rg = abs(uvPixelated - uv)*99;
				return fixed4(c);
			}
			// START BOILERPLATE
			ENDCG
		}
	}
}