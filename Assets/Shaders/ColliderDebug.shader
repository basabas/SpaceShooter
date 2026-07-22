Shader "Unlit/ColliderDebug"
{
	SubShader
	{
		Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True" }

		Pass
		{
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			//#pragma target 4.5
			#include "UnityCG.cginc"

			struct Entity
			{
				float x;
				float y;
				float rotation;
				float scale;
				float4 color;
				float frameOffset;
			};
			StructuredBuffer<Entity> entityBuffer;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 color : COLOR;
				float scale : float;
			};

			v2f vert(appdata v, uint instanceID : SV_InstanceID)
			{
				Entity entity = entityBuffer[instanceID];
				// Apply rotation around Z using entity.rotation (in radians)
				float s = sin(entity.rotation);
				float c = cos(entity.rotation);
				float2 local = v.vertex.xy * entity.scale;
				float2 rotated;
				rotated.x = local.x * c - local.y * s;
				rotated.y = local.x * s + local.y * c;

				float3 worldPosition = float3(entity.x + rotated.x, entity.y + rotated.y, 0);
				v2f o;
				o.vertex = mul(UNITY_MATRIX_VP, float4(worldPosition, 1.0f));
				o.uv = v.uv;
				o.color = entity.color;
				o.scale = entity.scale;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float mag = length(i.uv.xy-float2(0.5,0.5));
				mag/= i.scale;
				return fixed4(i.color.rgb, 0.7 - step(0.5,mag));
			}
			ENDCG
		}
	}
}
