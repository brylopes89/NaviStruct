﻿Shader "Custom/Blur"
{
	Properties
	{
		_BlurRadius("Blur Radius", Range(0.0,20.0)) = 1 
		_Intensity("Blur Intensity", Range(0.0,1.0)) = 0.01		
	}

	SubShader
	{
		Tags { "Queue" = "Transparent" }

		GrabPass{}

		Pass
		{
			Name "HORIZONTALBLUR"			

			CGPROGRAM //Allows talk between two languages: shader lab and Nvidia 

			#pragma vertex vertexFunc
			#pragma fragment fragFunc

			#include "UnityCG.cginc"

			struct v2f
			{
				float4 vertex : SV_POSITION; //SV_POSITION allows it to work on other platforms
				float4 uvgrab: TEXCOORD0;
			};						
			
			float _BlurRadius;
			float _Intensity;	
			sampler2D _GrabTexture;
			float4 _GrabTexture_TexelSize;

			v2f vertexFunc(appdata_base IN)
			{				
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);

				#if UNITY_UV_STARTS_AT_TOP
					float scale = -1.0;
				#else
					float scale = 1.0;
				#endif

				OUT.uvgrab.xy = (float2(OUT.vertex.x, OUT.vertex.y * scale) + OUT.vertex.w) * 0.5;
				OUT.uvgrab.zw = OUT.vertex.zw;				

				return OUT;				
			}

			half4 fragFunc(v2f IN) : COLOR
			{
				half4 texcol = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(IN.uvgrab));
				half4 texsum = half4(0,0,0,0);

				#define GRABPIXEL(weight, kernalx) tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(float4(IN.uvgrab.x + _GrabTexture_TexelSize.x * kernalx * _BlurRadius, IN.uvgrab.y, IN.uvgrab.z, IN.uvgrab.w))) * weight

				texsum += GRABPIXEL(0.05, -4.0);
				texsum += GRABPIXEL(0.09, -3.0);
				texsum += GRABPIXEL(0.12, -2.0);
				texsum += GRABPIXEL(0.15, -1.0);
				texsum += GRABPIXEL(0.18, 0.0);
				texsum += GRABPIXEL(0.15, 1.0);
				texsum += GRABPIXEL(0.12, 2.0);
				texsum += GRABPIXEL(0.09, 3.0);
				texsum += GRABPIXEL(0.05, 3.0);

				texcol = lerp(texcol, texsum, _Intensity);
				return texcol;
			}

			ENDCG
		}

		GrabPass{}

		Pass
		{
			Name "VERTICALBLUR"
			CGPROGRAM //Allows talk between two languages: shader lab and Nvidia 

			#pragma vertex vertexFunc
			#pragma fragment fragFunc

			#include "UnityCG.cginc"

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 uvgrab: TEXCOORD0;
			};

			float _BlurRadius;
			float _Intensity;	
			sampler2D _GrabTexture;
			float4 _GrabTexture_TexelSize;		

			v2f vertexFunc(appdata_base IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);

				#if UNITY_UV_STARTS_AT_TOP
					float scale = -1.0;
				#else
					float scale = 1.0;
				#endif

				OUT.uvgrab.xy = (float2(OUT.vertex.x, OUT.vertex.y * scale) + OUT.vertex.w) * 0.5;
				OUT.uvgrab.zw = OUT.vertex.zw;				

				return OUT;					
			}

			half4 fragFunc(v2f IN) : COLOR
			{
				half4 texcol = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(IN.uvgrab));
				half4 texsum = half4(0,0,0,0);

				#define GRABPIXEL(weight, kernaly) tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(float4(IN.uvgrab.x, IN.uvgrab.y + _GrabTexture_TexelSize.y * kernaly * _BlurRadius, IN.uvgrab.z, IN.uvgrab.w))) * weight

				texsum += GRABPIXEL(0.05, -4.0);
				texsum += GRABPIXEL(0.09, -3.0);
				texsum += GRABPIXEL(0.12, -2.0);
				texsum += GRABPIXEL(0.15, -1.0);
				texsum += GRABPIXEL(0.18, 0.0);
				texsum += GRABPIXEL(0.15, 1.0);
				texsum += GRABPIXEL(0.12, 2.0);
				texsum += GRABPIXEL(0.09, 3.0);
				texsum += GRABPIXEL(0.05, 3.0);

				texcol = lerp(texcol, texsum, _Intensity);
				return texcol;
			}

			ENDCG
		}
	}
}