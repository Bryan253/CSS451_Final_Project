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
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
                float3 vertexWC : TEXCOORD3;
			};

			sampler2D _MainTex;
            float4x4 MyTRSMatrix;
            fixed4 MyColor;
			float3 lightPos1;
			float3 lightPos2;
			fixed4 lightCol1;
			fixed4 lightCol2;
			float lightI1;
			float lightI2;
			float lightRange1;
			float lightRange2;
			
			v2f vert (appdata v)
			{
				v2f o;
                o.vertex = mul(MyTRSMatrix, v.vertex);
                o.vertex = mul(UNITY_MATRIX_VP, o.vertex);

                o.uv = v.uv;

				o.vertexWC = mul(MyTRSMatrix, v.vertex); // this is in WC space!
				float3 p = v.vertex + v.normal; // p in object space in normal direction
				p = mul(MyTRSMatrix, float4(p, 1));
				o.normal = normalize(p - o.vertexWC); // NOTE: this is in the world space!!
				return o;
			}

			fixed4 computeDiffuse(v2f i)
			{
				fixed4 result = fixed4(0.2, 0.2, 0.2, 1);           // Lowest light intensity

				// Lamp light calculation
				float3 lightDir1 = normalize(lightPos1 - i.vertexWC);
				float cos1 = clamp(dot(lightDir1, i.normal), 0, 1);
				float distance1 = dot(lightPos1 - i.vertexWC, float3(0, 1, 0));
				lightI1 = lightI1 * clamp((lightRange1 - distance1) / lightRange1, 0, 1);
				result += cos1 * lightI1 * lightCol1;

				// Orb light calculation
				float3 lightDir2 = normalize(lightPos2 - i.vertexWC);
				float cos2 = clamp(dot(lightDir2, i.normal), 0, 1);
				float distance2 = lightPos2 - i.vertexWC;
				lightI2 = lightI2 * clamp((lightRange2 - distance2) / lightRange2, 0, 1);
				result += cos1 * lightI2 * lightCol2;
				return result;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
                col = MyColor * col;
				return col * computeDiffuse(i);
			}
			ENDCG
		}
	}
}
