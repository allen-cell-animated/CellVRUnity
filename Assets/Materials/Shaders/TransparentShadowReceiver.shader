// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Selfmade/TransparentShadowReceiver" 
{ 
 
Properties 
{ 
    // Usual stuffs
    _Color ("Main Color", Color) = (1,1,1,1)
    _SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 0)
    _Shininess ("Shininess", Range (0.01, 1)) = 0.078125
    _MainTex ("Base (RGB) TransGloss (A)", 2D) = "white" {}
 
    // Bump stuffs
    //_Parallax ("Height", Range (0.005, 0.08)) = 0.02
    _BumpMap ("Normalmap", 2D) = "bump" {}
    //_ParallaxMap ("Heightmap (A)", 2D) = "black" {}
 
    // Shadow Stuff
    _ShadowIntensity ("Shadow Intensity", Range (0, 1)) = 0.6
} 
 
 
SubShader 
{ 
    Tags {
    "Queue"="AlphaTest" 
    "IgnoreProjector"="True" 
    "RenderType"="Transparent"
    }
 
    LOD 300

        // Shadow Pass : Adding the shadows (from Directional Light)
        // by blending the light attenuation
        Pass {
            Blend SrcAlpha OneMinusSrcAlpha 
            Name "ShadowPass"
            Tags {"LightMode" = "ForwardBase"}
 
            CGPROGRAM 
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #pragma fragmentoption ARB_fog_exp2
            #pragma fragmentoption ARB_precision_hint_fastest
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
 
            struct v2f { 
                float2 uv_MainTex : TEXCOORD1;
                float4 pos : SV_POSITION;
                LIGHTING_COORDS(3,4)
                float3  lightDir : TEXCOORD2;
            };
 
            float4 _MainTex_ST;
 
            sampler2D _MainTex;
            float4 _Color;
            float _ShadowIntensity;
 
            v2f vert (appdata_full v)
            {
                v2f o;
                o.uv_MainTex = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.pos = UnityObjectToClipPos (v.vertex);
                o.lightDir = ObjSpaceLightDir( v.vertex );
                TRANSFER_VERTEX_TO_FRAGMENT(o);
                return o;
            }
 
            float4 frag (v2f i) : COLOR
            {
                float atten = LIGHT_ATTENUATION(i);
 
                half4 c;
                c.rgb =  0;
                c.a = (1-atten) * _ShadowIntensity * (tex2D(_MainTex, i.uv_MainTex).a); 
                return c;
            }
            ENDCG
        }
 
 
}
 
FallBack "Transparent/Cutout/VertexLit"
}