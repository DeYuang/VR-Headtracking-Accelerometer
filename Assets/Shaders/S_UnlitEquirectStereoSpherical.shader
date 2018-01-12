Shader "RWD/S_UnlitEquirectStereoSpherical" {
    Properties {
        _Yaw ("Yaw", Float ) = 0
        [MaterialToggle] _FlipY ("FlipY", Float ) = 0
        [MaterialToggle] _StereoScopic ("StereoScopic", Float ) = 0
        _MainTex ("MainTex", 2D) = "white" {}
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
           
            
            uniform float _Yaw;
            uniform fixed _FlipY;
            uniform fixed _Eye;
            uniform fixed _StereoScopic;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.posWorld = mul(_Object2World, v.vertex); // In 5.3 or lower may need to change unity_ObjectToWorld to Object2World. Will autofix when we get a 5.4 directive from Unity.
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                return o;
            }

			float2 Function_node_3480(float3 dir, float yaw, float flipY, float Stereoscopic)
			{
				float gC = (acos(dir.g) / 3.141592654);
				float2 N = float2((((atan2(dir.r, dir.b) / 6.28318530718) + 0.5) + yaw), frac(lerp(gC, (1.0 - gC), flipY)));
				N.y *= -1;
				float2 offset = Stereoscopic ? float2(0.0, lerp(0.0, 0.5, _Eye)) : float2(0, 0);
				float2 M = frac((offset + lerp(N, (N*float2(1, 0.5)), Stereoscopic)) );
				return M;
			}

            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float2 node_3480 = Function_node_3480( normalize(i.posWorld.rgb) , _Yaw + 0.5 , _FlipY , _StereoScopic );
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_3480, _MainTex));
                float3 emissive = _MainTex_var.rgb;
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
