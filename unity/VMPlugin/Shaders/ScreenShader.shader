﻿Shader "ScreenShader"
{
	Properties
	{
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		//Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
//		LOD 100
//		Cull Off

		Pass
		{
			ZTest Always
			ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
//			#pragma multi_compile_fog

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
//				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
//				float2 uv : TEXCOORD0;
//				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float4 color : COLOR0;
			};

//			sampler2D _MainTex;
//			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
//				o.vertex = mul(UNITY_MATRIX_P, mul(UNITY_MATRIX_M, v.vertex));
				o.vertex = v.vertex;
				o.color = v.color;
//				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				//o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				//UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = i.color; //tex2D(_MainTex, i.uv);
				// apply fog
//				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
