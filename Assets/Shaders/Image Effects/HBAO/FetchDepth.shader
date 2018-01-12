Shader "Hidden/HBAO+/FetchDepth"
{
    SubShader
    {   
        Pass
        {
            ZTest Off
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 5.0
            #include "UnityCG.cginc"

			Texture2D _CameraDepthNormalsTexture : register(t100);
            SamplerState sampler_CameraDepthNormalsTexture;

			float4x4 _Camera2World;

            struct v2f{
            
                float4 posPS : SV_Position;
				float4 scrPos: TEXCOORD0;
            };

            v2f vert(appdata_base i) {
                v2f o;
                o.posPS = mul(UNITY_MATRIX_MVP, float4(i.vertex.xyz, 1));
                o.scrPos  = ComputeScreenPos(o.posPS);
                return o;
            }

            float3 frag(v2f i) : SV_Target {   
            
				float3 normalVS = DecodeViewNormalStereo(_CameraDepthNormalsTexture.Sample(sampler_CameraDepthNormalsTexture, i.scrPos.xy));

				return mul(_Camera2World, float4(normalVS, 0)).xyz * 0.5f + 0.5f	;
            }

            ENDCG
        }

		Pass {
            ZTest Off
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 5.0
            #include "UnityCG.cginc"

            Texture2D _CameraDepthTexture : register(t99);
            SamplerState sampler_CameraDepthTexture;

			float4x4 _Camera2World;

            float4 vert(appdata_base i) : SV_Position{
            
                return mul(UNITY_MATRIX_MVP, float4(i.vertex.xyz, 1));
            }

            float frag() : SV_Target{   
            
                return _CameraDepthTexture.Sample(sampler_CameraDepthTexture, 0..xx);
            }

            ENDCG
        }
    }
    FallBack "Hidden/EmptyImg"
}