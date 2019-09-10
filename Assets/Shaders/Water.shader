Shader "Custom/Water"
{
	Properties
	{
		_Colour("Colour", Color) = (1,1,1,1)
		_MainTex("Main Texture", 2D) = "defaulttexture" {}
		_WaveColour("Wave Colour", Color) = (1,1,1,1)
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_WaveSpeed("Wave Speed", float) = 1
		_CellDensity("Cell Density", float) = 2
		_WaveThickness("Wave Thickness", Range(0, 1)) = 0.5
		_WaveHeight("Wave Height", Range(0, 1)) = 0.5
    }

    SubShader
    {
    	//Set up shader for transparency.
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent"}
        LOD 200

        CGPROGRAM

		#pragma surface surf Standard fullforwardshadows alpha vertex:vert
        #pragma target 3.0

		struct Input
        {
			float2 uv_MainTex;
			float3 worldPos;
			float3 worldNormal;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Colour;
		sampler2D _MainTex;
		fixed4 _WaveColour;
		float _CellDensity;
		float _WaveSpeed;
		float _WaveThickness;
		float _WaveHeight;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
		// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)
		
		//Gets a random float2.
		inline float2 randomVector(float2 UV, float Offset)
		{
			float2x2 m = float2x2(15.27, 47.63, 99.41, 89.98);
			UV = frac(sin(mul(UV, m)) * 46839.32);
			return float2(sin(UV.y*+Offset)*0.5 + 0.5, cos(UV.x*Offset)*0.5 + 0.5);
		}
		
		//Applies a radial shear to the UV coordinates.
		float2 radialShear(float2 UV, float2 Center, float Strength, float2 Offset)
		{
			float2 delta = UV - Center;
			float delta2 = dot(delta.xy, delta.xy);
			float2 delta_offset = delta2 * Strength;
			return UV + float2(delta.y, -delta.x) * delta_offset + Offset;
		}
		
		//Generates a voronoi noise texture.
		float voronoiNoise(float2 UV, float AngleOffset, float CellDensity)
		{
			float2 g = floor(UV * CellDensity);
			float2 f = frac(UV * CellDensity);
			float t = 8.0;
			float3 res = float3(8.0, 0.0, 0.0);

			float Out;

			for (int y = -1; y <= 1; y++)
			{
				for (int x = -1; x <= 1; x++)
				{
					float2 lattice = float2(x, y);
					float2 offset = randomVector(lattice + g, AngleOffset);
					float d = distance(lattice + offset, f);
					if (d < res.x)
					{
						res = float3(d, offset.x, offset.y);
						Out = res.x;
					}
				}
			}

			return Out;
		}
		
		//Get a random direction for the gradient noise.
		float2 gradientNoiseDir(float2 p)
		{
			p = p % 289;
			float x = (34 * p.x + 1) * p.x % 289 + p.y;
			x = (34 * x + 1) * x % 289;
			x = frac(x / 41) * 2 - 1;
			return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
		}
		
		//Create a gradient noise texture.
		float gradientNoise(float2 UV, float Scale)
		{
			float2 p = UV * Scale;

			float2 ip = floor(p);
			float2 fp = frac(p);
			float d00 = dot(gradientNoiseDir(ip), fp);
			float d01 = dot(gradientNoiseDir(ip + float2(0, 1)), fp - float2(0, 1));
			float d10 = dot(gradientNoiseDir(ip + float2(1, 0)), fp - float2(1, 0));
			float d11 = dot(gradientNoiseDir(ip + float2(1, 1)), fp - float2(1, 1));
			fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);

			return lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
		}
		
		//Apply a height offset to each vertex.
		void vert(inout appdata_full v)
		{
			float2 offsetUV = v.texcoord + (_Time.y * _WaveSpeed * 1/_CellDensity);
			
			v.vertex.xyz += v.normal * (gradientNoise(offsetUV, _CellDensity) - 0.5) * _WaveHeight;
		}
		
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			//Create a voronoi texture for the waves.
			float2 radialShearUV = radialShear(IN.uv_MainTex, float2(0.5, 0.5), 1, float2(0, 0));
			
			float noise = voronoiNoise(radialShearUV, _WaveSpeed * _Time.y, _CellDensity);
			noise = pow(noise, 1/_WaveThickness);
			
            fixed4 waves = float4(noise, noise, noise, 1);

			o.Albedo = waves * _WaveColour + _Colour * tex2D(_MainTex, IN.uv_MainTex);
			o.Metallic = 0;
            o.Smoothness = _Glossiness;
            o.Alpha = _Colour.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
