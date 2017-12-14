// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Utopiaworx/Shaders/PostProcessing/Helios2D"
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
		CGINCLUDE

//************************************
//
//includes
//
//************************************

			#include "UnityCG.cginc" 
			#include "../../cginc/PhotoelectricShaders.cginc" 

//************************************
//
//Variables;

//
//************************************
			uniform sampler2D _MainTex;
			uniform sampler2D _FlipTex;
			uniform float _Gamma;
			uniform samplerCUBE _Cube;
			uniform float _Grain;
			uniform float _Seed;
			uniform float _UseChroma;
			uniform float4 _ChromaColor;
			uniform float _ChromaBlend;

//			sampler2D _CubeFront;
//			sampler2D _CubeRight;
//			sampler2D _CubeLeft;
//			sampler2D _CubeBack;
//			sampler2D _CubeUp;
//			sampler2D _CubeDown;
//
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
			

			float4 frag(v2f i) : SV_Target 
			{
				return pow(FixAlpha(tex2D(_MainTex,i.uv )),_Gamma);
			}

			float4 Grain(v2f i):SV_Target
			{
				float4 SampleColor = tex2D(_MainTex,i.uv);
				if(_UseChroma == 1.0 
				&& (SampleColor.r >= _ChromaColor.r - _ChromaBlend && SampleColor.r <= _ChromaColor.r + _ChromaBlend)
				&& (SampleColor.g >= _ChromaColor.g - _ChromaBlend && SampleColor.g <= _ChromaColor.g + _ChromaBlend)
				&& (SampleColor.b >= _ChromaColor.b - _ChromaBlend && SampleColor.b <= _ChromaColor.b + _ChromaBlend)
				)				{
					float4 AlphaRet = float4(0,0,0,0);
					AlphaRet.r = SampleColor.r;
					AlphaRet.g = SampleColor.g;
					AlphaRet.b = SampleColor.b;
					AlphaRet.a = 0.0;
					return AlphaRet;
				}
				else
				{
					return simpleGrain(tex2D(_MainTex,i.uv), i.uv, _Grain, _Seed);
				}
			}

			float4 fragFlip(v2f i) : SV_Target 
			{
				//i.uv.x = 1.0 - i.uv.x;
				i.uv.y = 1.0 - i.uv.y;
				return FixAlpha(tex2D(_MainTex,i.uv ));
			}

			float4 fragAA(v2f i) : SV_Target 
			{

				return FixAlpha(tex2D(_MainTex,i.uv ));
			}


		ENDCG


	SubShader 
	{
		ZTest Off
	    Cull Off
	    ZWrite Off
	    Blend Off
	    Lighting Off
	    Fog { Mode off }

		Pass {
			CGPROGRAM
//************************************
//
//Pragmas 0
//
//************************************
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			ENDCG
		}

				Pass {
			CGPROGRAM
//************************************
//
//Pragmas 1
//
//************************************
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex vert
			#pragma fragment Grain
			#pragma target 3.0

			ENDCG
		}

   Pass {
			CGPROGRAM
//************************************
//
//Pragmas 2
//
//************************************
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex vert
			#pragma fragment fragFlip
			#pragma target 3.0

			ENDCG
		}

   Pass {
			CGPROGRAM
//************************************
//
//Pragmas 3
//
//************************************
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex vert
			#pragma fragment fragAA
			#pragma target 3.0

			ENDCG
		}

				


	}
	Fallback Off	
}