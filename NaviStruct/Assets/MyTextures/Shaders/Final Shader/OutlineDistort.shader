﻿Shader "Custom/OutlineDistort"
{
	Properties
	{
		_DistortColor("Distort COLOR", COLOR) = (1,1,1,1)
		_BumpAmt("Distortion", Range(0.0, 128.0)) = 10
		_DistortTex("Distort Texture (RGB)", 2D) = "white"{}
		_BumpMap("Normal Map", 2D) = "bump"{}
		_OutlineWidth("Outline Width", Range(1.0,10.0)) = 1.1
	}

	SubShader
	{
		Tags { "Queue" = "Transparent" }

		GrabPass{}

		Pass
		{
			Name "OUTLINEDISTORT"		
			
			ZWrite Off

			CGPROGRAM 

			#pragma vertex vertexFunc
			#pragma fragment fragFunc

			#include "UnityCG.cginc"

			struct appdata 
			{
				float4 vertex : POSITION;
				float4 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION; 
				float4 uvgrab : TEXCOORD0;
				float2 uvbump : TEXCOORD1;
				float2 uvmain : TEXCOORD2;
			};						
			
			float _BumpAmt;
			float4 _BumpMap_ST;
			float4 _DistortTex_ST;
			float _OutlineWidth;
			fixed4 _DistortColor;
			sampler2D _GrabTexture;
			float4 _GrabTexture_TexelSize;
			sampler2D _BumpMap;
			sampler2D _DistortTex;

			v2f vertexFunc(appdata IN)
			{		
				IN.vertex.xyz *= _OutlineWidth;
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);

				#if UNITY_UV_STARTS_AT_TOP
					float scale = -1.0;
				#else
					float scale = 1.0;
				#endif

				OUT.uvgrab.xy = (float2(OUT.vertex.x, OUT.vertex.y * scale) + OUT.vertex.w) * 0.5;
				OUT.uvgrab.zw = OUT.vertex.zw;				

				OUT.uvbump = TRANSFORM_TEX(IN.texcoord, _BumpMap);
				OUT.uvmain = TRANSFORM_TEX(IN.texcoord, _DistortTex);

				return OUT;				
			}

			half4 fragFunc(v2f IN) : COLOR
			{
				half2 bump = UnpackNormal(tex2D(_BumpMap, IN.uvbump)).rg;
				float2 offset = bump * _BumpAmt * _GrabTexture_TexelSize.xy;
				IN.uvgrab.xy = offset * IN.uvgrab.z + IN.uvgrab.xy;

				half4 col = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(IN.uvgrab));
				half4 tint = tex2D(_DistortTex, IN.uvmain) * _DistortColor;

				return col * tint;
			}

			ENDCG
		}		
	}
}