Shader "Custom/Volume" 
{
	Properties 
	{
		_TextureAtlas ("Texture Atlas", 2D) = "_TextureAtlas" {}
		_TextureAtlasMask ("Texture Atlas Mask", 2D) = "_TextureAtlasMask" {}
	}
	SubShader 
	{
		Pass 
		{
		CGPROGRAM

		// The vertex and fragment shader names
		#pragma vertex vert
		#pragma fragment frag

		#include "UnityCG.cginc"

		struct v2f
		{
			float2 uv : TEXCOORD0;
			float4 vertex : SV_POSITION;
			float4 color : COLOR;
		};

		// The vertex shader
		v2f vert (appdata_base v)
		{
			v2f o;
			o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
			o.uv = v.texcoord;
			o.color = float4(0, 0, 0, 1);
			return o;
		}

		float M_PI = 3.14159265358979323846;
		float2 uv;
		sampler2D _TextureAtlasMask;
		sampler2D _TextureAtlas;

		// constant values
		float2 iResolution = float2(1704, 1152); 
		float FOV = 20;
		float3 AABB_CLIP_MIN = float3(-1,-1,-1);
		float3 AABB_CLIP_MAX = float3( 1, 1, 1);
		float3 BOX_SIZE = float3(0.3449883449883451, 0.5384615384615385, 0.22211122211122208);
		int BLOCK_RES_X = 1;
		int BLOCK_RES_Y = 1;
		int BLOCK_RES_Z = 1;
		float CLIP_NEAR = 0;
		float CLIP_FAR = 0;
		int BREAK_STEPS = 72;
		int DITHERING = 1;
		float BRIGHTNESS = 1;
		float DENSITY0 = 0.0820849986238988;
		int TEX_RES_X = 296;
		int TEX_RES_Y = 462;
		int ATLAS_X = 8;
		int ATLAS_Y = 6;
		int SLICES = 46;
		float maskAlpha = 0;
		float GAMMA_MIN = 0;
		float GAMMA_MAX = 1;
		float GAMMA_SCALE = 1;
		int PASS0 = 1; //this was set to 1
		float3 cameraPosition = float3(0,0,5);

		float unpack (float4 colour)
		{
			float4 bitShifts = float4(1.0 / 256.0, 1.0, 256.0, 0.0);
			return dot(colour, bitShifts);
		}

		bool intersectBox (in float4 r_o, in float4 r_d, in float4 boxmin, in float4 boxmax, out float tnear, out float tfar)
		{
			float4 invR = float4(1.0, 1.0, 1.0, 0.0) / r_d;
			float4 tbot = invR * (boxmin - r_o);
			float4 ttop = invR * (boxmax - r_o);
			float4 tmin = min(ttop, tbot);
			float4 tmax = max(ttop, tbot);
			float largest_tmin  = max(max(tmin.x, tmin.y), max(tmin.x, tmin.z));
			float smallest_tmax = min(min(tmax.x, tmax.y), min(tmax.x, tmax.z));
			tnear = largest_tmin;
			tfar = smallest_tmax;
			return (smallest_tmax > largest_tmin);
		}

		float rand (float2 co)
		{
			float threadId = uv.x/(uv.y + 1.0);
			float bigVal = threadId * 1299721.0 / 911.0;
			float smallVal0 = threadId * 7927.0 / 577.0;
			float smallVal1 = threadId * 104743.0 / 1039.0;
			return frac(sin(dot(co.xy, float2(smallVal0, smallVal1))) * bigVal);
		}

		float4 luma2Alpha (float4 color, float vmin, float vmax, float C)
		{
			float iChannels = 1.0/3.0;
			float x = max(color[2], max(color[0],color[1]));
			float xi = (x - vmin) / (vmax - vmin);
			xi = clamp(xi, 0.0, 1.0);
			float y = pow(xi, C);
			y = clamp(y, 0.0, 1.0);
			color[3] = y;
			return color;
		}

		float2 offsetFrontBack(float t, float nx, float ny)
		{
			float2 os = float2((fmod(t / nx, 1.0)) * nx - 1.0, ny - floor(t / nx) - 1.0);
			return os;
		}

		float4 sampleAs3DTexture (sampler2D tex, float4 pos) 
		{
			pos = 0.5 * (1.0 - pos);
			pos[0] = 1.0 - pos[0];
			float bounds = float(pos[0] > 0.005 && pos[0] < 0.995 &&
								 pos[1] > 0.005 && pos[1] < 0.995 &&
								 pos[2] > 0.005 && pos[2] < 0.995 );
			float nx      = float(ATLAS_X);
			float ny      = float(ATLAS_Y);
			float nSlices = float(SLICES);
			float sx      = float(TEX_RES_X);
			float sy      = float(TEX_RES_Y);
			float2 loc = float2(pos.x / nx, pos.y / ny);
			loc[1] = 1.0 / ny - loc[1];
			float2 pix = float2(1.0 / nx, 1.0 / ny);
			float iz = pos.z * nSlices;
			float zs = floor(iz);
			float ty  = zs;
			float typ = (zs + 1.0);
			typ = clamp(typ, 0.0, nSlices);
			float2 o0 = offsetFrontBack(ty, nx, ny) * pix;
			float2 o1 = offsetFrontBack(typ, nx, ny) * pix;
			o0 = clamp(o0, 0.0, 1.0);
			o1 = clamp(o1, 0.0, 1.0);
			float t = fmod(iz, 1.0);
			float4 slice0Color = tex2Dlod(tex, float4(loc + o0,0,0));
			float4 slice1Color = tex2Dlod(tex, float4(loc + o1,0,0));
			float slice0Mask = tex2Dlod(_TextureAtlasMask, float4(loc + o0,0,0)).x;
			float slice1Mask = tex2Dlod(_TextureAtlasMask, float4(loc + o1,0,0)).x;
			float maskVal = lerp(slice0Mask, slice1Mask, t);
			maskVal = lerp(maskVal, 1.0, maskAlpha);
			float4 retval = lerp(slice0Color, slice1Color, t);
			retval.rgb *= maskVal;
			return bounds * retval;
		}

		float4 sampleStack (sampler2D tex, float4 pos) 
		{
			float4 col = sampleAs3DTexture(tex, pos);
			col = luma2Alpha(col, GAMMA_MIN, GAMMA_MAX, GAMMA_SCALE);
			return col;
		}

		float4 integrateVolume (float4 eye_o, float4 eye_d, float tnear, float tfar, float clipNear, float clipFar, 
			float4 boxScale, sampler2D textureAtlas, int4 nBlocks)
        {
        	float4 C = float4(0.0, 0.0, 0.0, 0.0);
        	float tend   = tfar;
        	float tbegin = tnear;

        	//march along ray from front to back, accumulating color
        	//determine slice plane normal.  half between the light and the view direction
        	//estimate step length
        	const int maxSteps = 256;
        	float csteps = float(BREAK_STEPS);
        	csteps = clamp(csteps, 0.0, float(maxSteps));
        	float isteps = 1.0 / csteps;
        	float r = 0.5 - 1.0 * rand(eye_d.xy);
        	float tstep = 0.25 * isteps;
        	float tfarsurf = float(DITHERING) * r * tstep;
        	float overflow = fmod((tfarsurf - tend), tstep);
        	float t = tbegin + overflow;
        	t = clamp(t, clipNear, clipFar);
        	t += float(DITHERING) * r * tstep;
        	float A = 0.0;
        	float tdist = 0.0;
        	const int maxStepsLight = 32;
        	float T = 1.0;
        	int numSteps = 0;

        	//BEGIN INTEGRATION LOOP
        	for (int i = 0; i < maxSteps; i++)
        	{
        		float4 pos = (eye_o + eye_d * t) / boxScale; // map position to [0, 1] coordinates
        		float4 col = sampleStack(textureAtlas, pos);

        		//Finish up by adding brightness/density
        		col.xyz *= BRIGHTNESS;
        		col.w *= DENSITY0;
        		float s = 1.0 * float(128)/ float(BREAK_STEPS);
        		float stepScale = (1.0 - pow((1.0 - col.w), s));
        		col.w = stepScale;
        		col.xyz *= col.w;
        		col = clamp(col, 0.0, 1.0);
        		C = (1.0 - C.w) * col + C;
        		t += tstep;
        		numSteps = i;
        		if (i > BREAK_STEPS || t  > tend || t > clipFar ) { break; }
        		if (C.w > 1.0 ) { break; }
        	}
        	return C;
        }

		// The fragment shader, returns color as a float4
		float4 frag (v2f In): COLOR
		{
			uv = In.uv.xy;
			float2 vUv = uv / iResolution.xy;
			float fovr = FOV * M_PI / 180.0;
			float zDist = 1.0 / tan(fovr * 0.5);
			float4 eyeRay_d = float4(2.0 * vUv - 1.0, -zDist, 0.0);
			eyeRay_d.x *= iResolution.x / iResolution.y;
			eyeRay_d = mul(eyeRay_d, UNITY_MATRIX_V); // view matrix
			eyeRay_d.w = 0.0;
			float4 eyeRay_o = float4(cameraPosition, 0.0); //_WorldSpaceCameraPos

			float4 boxMin = float4(AABB_CLIP_MIN, -1.0);
			float4 boxMax = float4(AABB_CLIP_MAX,  1.0);
			float4 boxTrans = float4(0.0, 0.0, 0.0, 0.0);
			float4 boxScale = float4(BOX_SIZE, 0.0);
			boxMin *= boxScale;
			boxMax *= boxScale;
			boxMin += boxTrans;
			boxMax += boxTrans;

			int4 nBlocks = int4(BLOCK_RES_X, BLOCK_RES_Y, BLOCK_RES_Z, 1);

//			float4 D = tex2Dlod(_BackgroundDepth, float4(vUv,0,0));
			float4 C1 = float4(0,0,0,0);// tex2Dlod(_BackgroundColor, float4(vUv,0,0));
//			float z_b = unpack(D);
			float tnear, tfar;
			bool hit = intersectBox(eyeRay_o, eyeRay_d, boxMin, boxMax, tnear, tfar);
			tnear = clamp(tnear, 0.0, tnear);
			float eyeDist = length(eyeRay_o.xyz);
			float3 eyeNorm = -normalize(eyeRay_o.xyz) / boxScale.xyz * 0.5;
			float dNear = 1.0 - 2.0 * CLIP_NEAR;
			float dFar  = 1.0 - 2.0 * CLIP_FAR;
			float clipNear = -(dot(eyeRay_o.xyz, eyeNorm) + dNear) / dot(eyeRay_d.xyz, eyeNorm);
			float clipFar  = -(dot(eyeRay_o.xyz,-eyeNorm) +  dFar) / dot(eyeRay_d.xyz,-eyeNorm);

			if (!hit) 
			{
				return C1; //vec4(0.0);
			}

			float w = (PASS0 == 1) ? C1.w : 1.0;
			float4 C = integrateVolume(eyeRay_o, eyeRay_d, tnear, tfar, clipNear, clipFar, boxScale, _TextureAtlas, nBlocks);
			C = clamp(C, 0.0, 1.0);
			if (PASS0 == 0) { C = ((1.0 - C.w) * C1.w) * C1 + C; }
			if (PASS0 == 1) { C = C1 + (1.0 - C1.w) * C; }

			return C;
		}
 
		ENDCG
		}
	}
}