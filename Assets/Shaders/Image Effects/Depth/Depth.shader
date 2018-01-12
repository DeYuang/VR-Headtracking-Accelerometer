Shader "Hidden/Depth" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Pass{
			ZTest Off
			ZWrite Off
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma exclude_renderers metal xbox360 xboxone ps3 ps4 psp2
			#pragma target 2.0
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			sampler2D_float _CameraDepthTexture;
			fixed _ScreenHeight;
			fixed _ScreenWidth;
			
			struct v2f {
			    float4 pos 		: SV_POSITION;
			    //float2 depth 	: TEXCOORD0;
			   	float2 uv    	: TEXCOORD0;
			};

			v2f vert (appdata_img v) { // Vertex Shader
			    v2f o;
			    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
			    o.uv =  v.texcoord.xy;
			    return o;
			}
			
			fixed4 frag(v2f i) : SV_Target { // Pixel Shader
				float d = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv.xy));
				float sum = d; // this sample is essentially free
				sum += Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, fixed2(i.uv.x, i.uv.y - 1 / _ScreenHeight)));
				sum += Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, fixed2(i.uv.x, i.uv.y + 1 / _ScreenHeight)));
				sum += Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, fixed2(i.uv.x - 1 / _ScreenHeight, i.uv.y)));
				sum += Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, fixed2(i.uv.x + 1 / _ScreenHeight, i.uv.y)));
				sum += Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, fixed2(i.uv.x - 1 / _ScreenHeight, i.uv.y - 1 / _ScreenHeight)));
				sum += Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, fixed2(i.uv.x - 1 / _ScreenHeight, i.uv.y + 1 / _ScreenHeight)));
				sum += Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, fixed2(i.uv.x + 1 / _ScreenHeight, i.uv.y - 1 / _ScreenHeight)));
				sum += Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, fixed2(i.uv.x + 1 / _ScreenHeight, i.uv.y + 1 / _ScreenHeight)));
				sum /= 9;
				sum += d; sum /= 2;
				
				fixed4 original = tex2D(_MainTex, i.uv);
				
				float ao = (((sum/d)/2)+.5) / (1-(sum-d)*2.5);
				ao = pow(ao, 3);
				return ao*original;
			}
			ENDCG
		}
	}
	// Depth Buffer example
	// Pixel Shader Cg
	// This shader accesses the depth buffer directly
	SubShader {
		Pass{
			ZTest Off
			ZWrite Off
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma exclude_renderers metal xbox360 xboxone ps3 ps4 psp2
			#pragma target 2.0
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			sampler2D_float _CameraDepthTexture;
			
			struct v2f {
			    float4 pos 		: SV_POSITION;
			    //float2 depth 	: TEXCOORD0;
			   	float2 uv    	: TEXCOORD0;
			};

			v2f vert (appdata_img v) { // Vertex Shader
			    v2f o;
			    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
			    o.uv =  v.texcoord.xy;
			    return o;
			}
			
			fixed4 frag(v2f i) : SV_Target { // Pixel Shader
				float d = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv.xy);
				d = Linear01Depth(d);
				fixed4 original = tex2D(_MainTex, i.uv);
				
				return original * (1-d);
			}
			ENDCG
		}
	}
	// This a Empty Image Effect, used as a fallback for where the effect is not supported
	FallBack "Hidden/EmptyImg"
}
