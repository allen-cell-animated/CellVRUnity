#ifndef UTOPIAWORX_BASE_CGINC
#define UTOPIAWORX_BASE_CGINC 


#include "UnityCG.cginc"

//************************************
//
//STRUCTS
//
//************************************
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

    //vertex to fragment with neighbor cells
	struct v2f_DS
	{
		float4 pos : SV_POSITION;
		half4 uv0 : TEXCOORD0;
		half4 uv1 : TEXCOORD1;
		half4 uv2 : TEXCOORD2;
		half4 uv3 : TEXCOORD3;
	};

    
fixed4 BumpRed(fixed4 v)
{
	v.r+=0.004;
	return v;
}

fixed4 BumpGreen(fixed4 v)
{
	v.g+=0.004;
	return v;
}

fixed4 BumpBlue(fixed4 v)
{
	v.b+=0.004;
	return v;
}

fixed4 BumpAlpha(fixed4 v)
{
	v.a+=0.1;
	return v;
}





float simpleNoise(float2 uv, float seed = 6272.3f)
{
	return frac(sin(uv.x + uv.y * 541.17f + seed) * 273351.5f + seed);	
}

fixed4 simpleGrain(float3 clr, float2 uv, float Strength, float Seed)
{
	float grain = simpleNoise(uv, Seed);	
	grain = grain * 2.0f - 1.0f;
	grain = lerp(0.0f, grain, Strength);		
	return fixed4(clr + grain * clr,1.0);	
}

fixed4 Darkness(fixed4 c)
{
	fixed4 r = (c.r + c.b  + c.b) / 3.0;
	return r;

}

//Color Blending
//From Elringus Github page : https://gist.github.com/Elringus/d21c8b0f87616ede9014

//grey scale
fixed GreyMix(fixed4 c) 
{ 
	return .299 * c.r + .587 * c.g + .114 * c.b; 
}
 
fixed4 Darken (fixed4 a, fixed4 b)
{ 
	fixed4 r = min(a, b);
	r.a = b.a;
	return r;
}

fixed4 Multiply (fixed4 a, fixed4 b)
{ 
	fixed4 r = a * b;
	r.a = b.a;
	return r;
}

fixed4 ColorBurn (fixed4 a, fixed4 b) 
{ 
	fixed4 r = 1.0 - (1.0 - a) / b;
	r.a = b.a;
	return r;
}

fixed4 LinearBurn (fixed4 a, fixed4 b)
{ 
	fixed4 r = a + b - 1.0;
	r.a = b.a;
	return r;
}

fixed4 DarkerColor (fixed4 a, fixed4 b) 
{ 
	fixed4 r = GreyMix(a) < GreyMix(b) ? a : b;
	r.a = b.a;
	return r; 
}

fixed4 Lighten (fixed4 a, fixed4 b)
{ 
	fixed4 r = max(a, b);
	r.a = b.a;
	return r;
}

fixed4 Screen (fixed4 a, fixed4 b) 
{ 	
	fixed4 r = 1.0 - (1.0 - a) * (1.0 - b);
	r.a = b.a;
	return r;
}

fixed4 ColorDodge (fixed4 a, fixed4 b) 
{ 
	fixed4 r = a / (1.0 - b);
	r.a = b.a;
	return r;
}

fixed4 LinearDodge (fixed4 a, fixed4 b)
{ 
	fixed4 r = a + b;
	r.a = b.a;
	return r;
} 

fixed4 LighterColor (fixed4 a, fixed4 b) 
{ 
	fixed4 r = GreyMix(a) > GreyMix(b) ? a : b;
	r.a = b.a;
	return r; 
}

fixed4 Overlay (fixed4 a, fixed4 b) 
{
	fixed4 r = a > .5 ? 1.0 - 2.0 * (1.0 - a) * (1.0 - b) : 2.0 * a * b;
	r.a = b.a;
	return r;
}

fixed4 SoftLight (fixed4 a, fixed4 b)
{
	fixed4 r = (1.0 - a) * a * b + a * (1.0 - (1.0 - a) * (1.0 - b));
	r.a = b.a;
	return r;
}

fixed4 HardLight (fixed4 a, fixed4 b)
{
	fixed4 r = b > .5 ? 1.0 - (1.0 - a) * (1.0 - 2.0 * (b - .5)) : a * (2.0 * b);
	r.a = b.a;
	return r;
}

fixed4 VividLight (fixed4 a, fixed4 b)
{
	fixed4 r = b > .5 ? a / (1.0 - (b - .5) * 2.0) : 1.0 - (1.0 - a) / (b * 2.0);
	r.a = b.a;
	return r;
}

fixed4 LinearLight (fixed4 a, fixed4 b)
{
	fixed4 r = b > .5 ? a + 2.0 * (b - .5) : a + 2.0 * b - 1.0;
	r.a = b.a;
	return r;
}

fixed4 PinLight (fixed4 a, fixed4 b)
{
	fixed4 r = b > .5 ? max(a, 2.0 * (b - .5)) : min(a, 2.0 * b);
	r.a = b.a;
	return r;
}

fixed4 HardMix (fixed4 a, fixed4 b)
{
	fixed4 r = (b > 1.0 - a) ? 1.0 : .0;
	r.a = b.a;
	return r;
}

fixed4 Difference (fixed4 a, fixed4 b) 
{ 
	fixed4 r = abs(a - b);
	r.a = b.a;
	return r; 
}

fixed4 Exclusion (fixed4 a, fixed4 b)
{ 
	fixed4 r = a + b - 2.0 * a * b;
	r.a = b.a;
	return r; 
}

fixed4 Subtract (fixed4 a, fixed4 b)
{ 
	fixed4 r = a - b;
	r.a = b.a;
	return r; 
}

fixed4 Divide (fixed4 a, fixed4 b)
{ 
	fixed4 r = a / b;
	r.a = b.a;
	return r; 
}

fixed4 Invert(fixed4 col)
{
 return 1.0 - col;
}


fixed4 QuickBlur(half2 pms, v2f i, sampler2D MT)
{
	half p = pms.x / _ScreenParams.y;
	half4 c = half4(0.0, 0.0, 0.0, 0.0);
	float r = sin(dot(i.uv, half2(1233.224, 1743.335)));
	half2 rv = half2(0.0, 0.0);

	for(int k = 0; k < int(pms.y); k++)
	{
		r = frac(3712.65 * r + 0.61432);
		rv.x = (r - 0.5) * 2.0;
		r = frac(3712.65 * r + 0.61432);
		rv.y = (r - 0.5) * 2.0;
		c += tex2Dlod(MT, half4(i.uv + rv * p, 0.0, 0.0));
	}

	return c;

}
fixed luminance(fixed4 color)
{
	return dot(color,fixed4(0.222, 0.707, 0.071,1.0));
}
fixed4 luminanceCol(fixed4 color)
{
	fixed v = dot(color,fixed4(0.222, 0.707, 0.071,1.0));
	return fixed4(v,v,v,1.0);
}

fixed4 FixAlpha(fixed4 source)
{
	return clamp(fixed4(source.r,source.g,source.b,1.0),0.0,1.0);
}

fixed4 DominantColor(fixed4 source)
{
	fixed4 retVal = source;
	if(source.r == source.g && source.r == source.b)
	{
		retVal = source;
	}
	else
	{
		if(source.r > source.g)
		{
			if(source.r > source.b)
			{
				//red
				retVal = BumpRed(source);
			}
			else
			{
				//blue
				retVal = BumpBlue(source);
			}
		}
		else
		{
			if(source.g> source.b)
			{
				//green
				retVal = BumpGreen(source);
			}
			else
			{
			 	//blue
				retVal = BumpBlue(source);
			}
		}
	}
	return retVal;

}

float3 HSVtoRGB(float3 c)
{
	float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
	float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
	return c.z * lerp(K.xxx, saturate(p - K.xxx), c.y);
}

float3 RGBtoHSV(float3 c)
{
	float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
	float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
	float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));

	float d = q.x - min(q.w, q.y);
	float e = 1.0e-10;
	return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

//************************************
//
//Static Datas
//
//************************************
		static const half4 curve4[4] = { 
										 half4(0.0205,0.0205,0.0205,0), 
										 half4(0.0855,0.0855,0.0855,0), 
										 half4(0.232,0.232,0.232,0),
										 half4(0.324,0.324,0.324,1)
										  };

#endif 