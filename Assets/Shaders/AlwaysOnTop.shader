﻿Shader "Unlit/Always On top"
{
    Properties
    {
		_Colour("Colour", Color) = (1,1,1,1)
    }
    SubShader
    {
		ZWrite Off
		ZTest Always

		Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

			float4 _Colour;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				fixed4 col = _Colour;
                return col;
            }
            ENDCG
        }
    }
}
