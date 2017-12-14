// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Utopiaworx/Shaders/PostProcessing/FadeBlack"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Amount ("Amount",float) =0.0
		_Color ("Color", Color) = (1,1,1,1)
	}
	CGINCLUDE
	//************************************
//
//includes
//
//************************************

			#include "UnityCG.cginc" 

//
//Variables
//
//************************************
			uniform sampler2D _MainTex;
			uniform float _Amount;
			 fixed4 _Color;



				struct appdata
    {
        float4 vertex : SV_POSITION;
        float2 uv : TEXCOORD0;

    };

    struct v2f
    {
        float2 uv : TEXCOORD0;
        float4 vertex : SV_POSITION;
        float4 scrPos : TEXCOORD1;

    };

//************************************
//
//Vertex Function
//
//************************************
            v2f vert(appdata_img v)
			{

				//declare a return value
				v2f o;

 				//set the Vertex position
				o.vertex = UnityObjectToClipPos(v.vertex);

				//set the screen position for the depth texture
				o.scrPos = ComputeScreenPos(o.vertex);

				//set the UV coordinate
				o.uv = v.texcoord.xy;

				//return the object
				return o;
			} 
			

//************************************
//
//Fragment Function
//
//************************************
			fixed4 frag (v2f i) : SV_Target
			{
			return lerp(_Color, tex2D(_MainTex,i.uv),_Amount);
				//return tex2D(_MainTex,i.uv) * _Amount;
			}

	ENDCG
	SubShader
	{
		// No culling or depth
		ZTest Off
	    Cull Off
	    ZWrite Off
	    Blend Off
	    Lighting Off
	    Fog { Mode off }

		Pass
		{
			CGPROGRAM
//************************************
//
//Pragmas
//
//************************************
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			ENDCG
		}




	}
}
