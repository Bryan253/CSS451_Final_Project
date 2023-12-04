Shader "Unlit/NodeShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 200

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

				// to support hacked shading
				float4 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;

				// to support hacked shading
				float3 vInEyeSpace : TEXCOORD1;
				float3 normalInEyeSpace : NORMAL;
			};

			sampler2D _MainTex;
            float4x4 MyTRSMatrix;
            fixed4 MyColor;
			
			v2f vert (appdata v)
			{
				v2f o;
                o.vertex = mul(MyTRSMatrix, v.vertex);
                o.vertex = mul(UNITY_MATRIX_VP, o.vertex);

                o.uv = v.uv;

				// to suport hacked shading
				float3 p = v.vertex + v.normal; // p in object space in normal direction
				p = mul(UNITY_MATRIX_V, mul(MyTRSMatrix, float4(p, 1)));  // in eye space
				o.vInEyeSpace = mul(UNITY_MATRIX_V, mul(MyTRSMatrix, v.vertex)); // vertex in eye space
				o.normalInEyeSpace = normalize(p - o.vInEyeSpace);  // normal (normalized) in eye space
				o.vInEyeSpace = normalize(o.vInEyeSpace);  // hacked L
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// Hack for illumination: assume point light at the eye
				float light = clamp(dot(i.normalInEyeSpace, -i.vInEyeSpace), 0.2, 0.9);  // * 0.8; // light strength

				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
                col = MyColor;
				return light * col;
			}
			ENDCG
		}
	}
}
