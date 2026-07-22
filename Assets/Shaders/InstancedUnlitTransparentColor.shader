Shader "Unlit/InstancedUnlitTransparentColor"
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
			
			struct Transform
			{
				float x;
				float y;
				float rotation;
				float scale;
			};

			struct DrawInformation
			{
				int frameIndex;
				float4 color;
			};
			
			struct FrameInformation
			{
				float fps;
				int frameStart;
				int frameCount;
				float width;
				float height;
			};
			
			StructuredBuffer<int> entityBuffer;
			StructuredBuffer<Transform> transformBuffer;
			StructuredBuffer<DrawInformation> drawInfoBuffer;
			StructuredBuffer<FrameInformation> frameInfoBuffer;			
			sampler2D atlas;
			uniform float4 frames[1024];

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
			};

			v2f vert(appdata v, uint instanceID : SV_InstanceID)
			{
				int entityID = entityBuffer[instanceID];
				Transform transform = transformBuffer[entityID];
				DrawInformation drawInfo = drawInfoBuffer[entityID];
				FrameInformation frameInfo = frameInfoBuffer[drawInfo.frameIndex];

				float s = sin(transform.rotation);
				float c = cos(transform.rotation);
				float2 local = v.vertex.xy * transform.scale * float2(frameInfo.width, frameInfo.height);
				float2 rotated;
				rotated.x = local.x * c - local.y * s;
				rotated.y = local.x * s + local.y * c;

				float3 worldPosition = float3(transform.x + rotated.x, transform.y + rotated.y, 0);
				v2f o;
				o.vertex = mul(UNITY_MATRIX_VP, float4(worldPosition, 1.0f));
				o.uv = v.uv;

				int frame = floor(_Time.y * frameInfo.fps * frameInfo.frameCount + entityID) % frameInfo.frameCount;
				fixed4 rect = frames[frameInfo.frameStart + frame];
				o.uv = float2(lerp(rect.x,rect.z,v.uv.x),lerp(rect.y,rect.w,v.uv.y));
				
				o.color = drawInfo.color;

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				return tex2D(atlas, i.uv) * i.color;
			}
			ENDCG
		}
	}
}
