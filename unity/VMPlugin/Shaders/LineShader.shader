// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/LineShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "Queue"="Overlay" }
//		Tags{ "RenderType" = "Opaque" }

		Blend SrcAlpha OneMinusSrcAlpha
		LOD 200

		Pass {
			ZTest Always
			ZWrite Off
		}
		Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                    float4 color : COLOR;
            };

            struct v2f
            {
                    float2 uv : TEXCOORD0;
                    UNITY_FOG_COORDS(1)
                    float4 vertex : SV_POSITION;
                    float4 color : COLOR0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.color = v.color;

                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    UNITY_TRANSFER_FOG(o,o.vertex);
                    o.vertex.z = 0.;
                    return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                    // sample the texture
                    fixed4 col = i.color;
//                    col.r = 1.0;
//                    col.g = 0.0;
//                    col.b = 0.0;
//                    fixed4 col = tex2D(_MainTex, i.uv);
                    // apply fog
//                    UNITY_APPLY_FOG(i.fogCoord, col);
                    return col;
            }
            ENDCG
		}

	}
	FallBack "Diffuse"
}
