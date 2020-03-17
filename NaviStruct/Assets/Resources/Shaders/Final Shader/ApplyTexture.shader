Shader "Custom/ApplyTexture"
{
	Properties
	{
		_MainTex("Main Texture (RGB)", 2D) = "white"{}
		_Color("Color", Color) = (1,1,1,1)		
	}

	SubShader
	{
		Pass
		{
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

			float4 _Color;
			sampler2D _MainTex;

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