Shader "Custom/FinalOutline"
{
	Properties
	{
		 _MainTex ("Texture", 2D) = "white" {}
        _TintColor ("Tint Color", Color) = (1,1,1,1)
        _Transparency ("Transparency", Range(0.0,0.5)) = 0.25
        _CutoutThresh ("Cutout Threshold", Range(0.0, 1.0)) = 0.2
        _Distance ("Distance", Float) = 1
        _Amplitude ("Amplitude", Float) = 1
        _Speed ("Speed", Float) = 1
        _Amount ("Amount", Range(0.0,1.0)) = 1
	}

	SubShader
	{
		Tags { "Queue" = "Transparent" }

		GrabPass{}
		UsePass "Custom/OutlineDistort/OUTLINEDISTORT"	
		GrabPass{}
		UsePass "Custom/OutlineBlur/OUTLINEHORIZONTALBLUR"
		GrabPass{}
		UsePass "Custom/OutlineBlur/OUTLINEVERTICALBLUR"				

		UsePass "Custom/Outline/OBJECT"
	}
}