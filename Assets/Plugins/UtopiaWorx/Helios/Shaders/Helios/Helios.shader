// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Utopiaworx/Shaders/PostProcessing/Helios"
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

			uniform sampler2D _MergeL;
			uniform sampler2D _MergeR;
			uniform float _Gamma;
			uniform samplerCUBE _Cube;

			uniform float _Streo_Zoom;
			uniform float4x4 _Rotation;
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
			float4 fragFlip(v2f i) : SV_Target 
			{
				//i.uv.x = 1.0 - i.uv.x;
				i.uv.y = 1.0 - i.uv.y;
				return FixAlpha(tex2D(_MainTex,i.uv ));
			}

			float4 fragFlip2(v2f i) : SV_Target 
			{
				//i.uv.x = 1.0 - i.uv.x;
				i.uv.x = 1.0 - i.uv.x;
				return FixAlpha(tex2D(_MainTex,i.uv ));
			}
			float4 fragNull(v2f i) : SV_Target 
			{

				return FixAlpha(tex2D(_MainTex,i.uv ));
			}

			float4 fragWinFix(v2f i) : SV_Target 
			{
				//i.uv.x = 1.0 - i.uv.x;
				i.uv.y = 1.0 - i.uv.y;
				return FixAlpha(tex2D(_MainTex,i.uv ));
			}

			float4 MergeStereo(v2f i) : SV_Target
			{
				if(i.uv.y < 0.5)
				{
					float2 NewUV = float2(i.uv.x , (i.uv.y * 2.0));
					 return FixAlpha(tex2D(_MergeR,NewUV ));
				}
				else
				{
					float2 NewUV = float2(i.uv.x, ( i.uv.y - 0.5 ) * 2.0);
					return FixAlpha(tex2D(_MergeL,NewUV ));
				}
			}

			float4 frag(v2f i) : SV_Target 
			{
   
    

    float2 thetaphi =   ((i.uv * 2.0 ) - float2(1.0,1.0)) * float2(3.1415926535897932384626433832795, 1.5707963267948966192313216916398); 
    float3 rayDirection = float3(cos(thetaphi.y) * cos(thetaphi.x), sin(thetaphi.y), cos(thetaphi.y) * sin(thetaphi.x));
    rayDirection = mul(_Rotation,rayDirection);

		return pow(FixAlpha(texCUBE(_Cube, rayDirection)),_Gamma);


			}

			float4 fragStereo(v2f i) : SV_Target 
			{
				float pi = 3.1415926536;
				float2 p = float2(i.uv.x, i.uv.y);
	float m = (1.0 - _Streo_Zoom) * pi;
	p = p * 2.0 * m - m;
					 float3 dir = float3( 
(2.0*p.x) / (1.0+p.x*p.x+p.y*p.y),
					 (-1.0+p.x*p.x+p.y*p.y) / (1.0+p.x*p.x+p.y*p.y),
					 (2.0*p.y) / (1.0+p.x*p.x+p.y*p.y) 
					 );
		
	return pow(FixAlpha(texCUBE(_Cube, dir)),_Gamma);
			}

			float4 Grain(v2f i):SV_Target
			{
				float4 SampleColor = tex2D(_MainTex,i.uv);
				if(_UseChroma == 1.0 
				&& (SampleColor.r >= _ChromaColor.r - _ChromaBlend && SampleColor.r <= _ChromaColor.r + _ChromaBlend)
				&& (SampleColor.g >= _ChromaColor.g - _ChromaBlend && SampleColor.g <= _ChromaColor.g + _ChromaBlend)
				&& (SampleColor.b >= _ChromaColor.b - _ChromaBlend && SampleColor.b <= _ChromaColor.b + _ChromaBlend)
				)
				{
					float4 AlphaRet = float4(0,0,0,0);
					AlphaRet.r  = SampleColor.r;
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


		ENDCG


	SubShader 
	{


		ZTest Always
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
			#pragma fragment fragFlip
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
			#pragma fragment fragFlip2
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
			#pragma fragment fragNull
			#pragma target 3.0

			ENDCG
		}

								Pass {
			CGPROGRAM
//************************************
//
//Pragmas 4
//
//************************************
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex vert
			#pragma fragment fragWinFix
			#pragma target 3.0

			ENDCG
		}


	Pass {
			CGPROGRAM
//************************************
//
//Pragmas 5
//
//************************************
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex vert
			#pragma fragment fragStereo
			#pragma target 3.0

			ENDCG
		}

	Pass {
			CGPROGRAM
//************************************
//
//Pragmas 6
//
//************************************
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex vert
			#pragma fragment MergeStereo
			#pragma target 3.0

			ENDCG
		}

			Pass {
			CGPROGRAM
//************************************
//
//Pragmas 7
//
//************************************
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex vert
			#pragma fragment Grain
			#pragma target 3.0

			ENDCG
		}
	}
	Fallback Off	
}