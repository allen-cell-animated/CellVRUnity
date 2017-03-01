Shader "Custom/Dots"
{
    Properties
    {
        _Color("Tint", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
        }
 
        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha // Traditional transparency
 
        Pass
        {
            CGPROGRAM
 
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
 
            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };
 
            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };
 
            fixed4 _Color;
            static const float RADIUS = 0.5;
 
            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
                OUT.texcoord = IN.texcoord - fixed2(0.5, 0.5);
                OUT.color = IN.color * _Color;
                return OUT;
            }
 
            float calcAlpha(float distance)
            {
                float alpha = 1.0 - step(RADIUS, distance);
                return alpha;
 
            }
 
            fixed4 frag(v2f IN) : SV_Target
            {
                float distance = sqrt(pow(IN.texcoord.x, 2) + pow(IN.texcoord.y,2));
                return fixed4(IN.color.r, IN.color.g, IN.color.b, IN.color.a * calcAlpha(distance));
            }
 
            ENDCG
        }
    }
}