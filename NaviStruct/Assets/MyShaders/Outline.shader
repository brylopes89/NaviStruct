Shader "Custom/Outline"
{
	Properties
	{
		_MainTex("Main Texture (RGB)", 2D) = "white"{}
		_Color("Color", Color) = (1,1,1,1)		
		_OutlineTex("Outline Texture", 2D) = "white"{}
		_OutlineColor("Outline Color", Color) = (1,1,1,1)
		_OutlineWidth("Outline Width", Range(1.0,10.0)) = 1.1
	}

	SubShader
	{
		Tags { "Queue" = "Transparent" }

		Pass
		{
			Name "OUTLINE"

			ZWrite Off

			CGPROGRAM //Allows talk between two languages: shader lab and Nvidia 

			#pragma vertex vertexFunc
			#pragma fragment fragmentFunc

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION; //SV_POSITION allows it to work on other platforms
				float2 uv: TEXCOORD0;
			};

			sampler2D _OutlineTex;			
			float4 _OutlineColor;	
			float _OutlineWidth;

			v2f vertexFunc(appdata IN)
			{
				IN.vertex.xyz *= _OutlineWidth;
				v2f OUT;

				OUT.pos = UnityObjectToClipPos(IN.vertex);
				OUT.uv = IN.uv;

				return OUT;				
			}

			fixed4 fragmentFunc(v2f IN) : SV_Target
			{
				float4 texColor = tex2D(_OutlineTex, IN.uv);
				return texColor * _OutlineColor;
			}

			ENDCG
		}

		Pass
		{
			Name "OBJECT"
			CGPROGRAM 

			#pragma vertex vertexFunc
			#pragma fragment fragmentFunc

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION; //SV_POSITION allows it to work on other platforms
				float2 uv: TEXCOORD0;
			};

			sampler2D _MainTex;			
			float4 _Color;				

			v2f vertexFunc(appdata IN)
			{
				v2f OUT;

				OUT.pos = UnityObjectToClipPos(IN.vertex);
				OUT.uv = IN.uv;

				return OUT;				
			}

			fixed4 fragmentFunc(v2f IN) : SV_Target
			{
				float4 texColor = tex2D(_MainTex, IN.uv);
				return texColor * _Color;
			}

			ENDCG
		}
	}
}