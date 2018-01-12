// This a Empty Image Effect, used as a fallback for where the effect is not supported

Shader "Hidden/Unsharp Mask" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	// Unsharp Mask Fullscreen Shader
	// Pixel Shader Cg
	// this blurs the image with a box blur (8 samples) and then uses that to get a unsharp mask effect
	SubShader {
		Pass{
			ZTest Off
			ZWrite Off
			
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma exclude_renderers metal xbox360 xboxone ps3 ps4 psp2
			#pragma target 2.0
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			fixed _BlurSize;
			fixed _ScreenHeight;
			fixed _ScreenWidth;
			
			fixed4 frag(v2f_img i) : SV_Target { // Pixel Shader
				fixed4 original = tex2D(_MainTex, i.uv);
				float4 sum = float4(0,0,0,0);
				sum += tex2D(_MainTex, fixed2(i.uv.x, i.uv.y - _BlurSize / _ScreenHeight));
				sum += tex2D(_MainTex, fixed2(i.uv.x, i.uv.y + _BlurSize / _ScreenHeight));
				sum += tex2D(_MainTex, fixed2(i.uv.x - _BlurSize / _ScreenWidth, i.uv.y));
				sum += tex2D(_MainTex, fixed2(i.uv.x + _BlurSize / _ScreenWidth, i.uv.y));
				sum += tex2D(_MainTex, fixed2(i.uv.x + _BlurSize / _ScreenWidth, i.uv.y + _BlurSize / _ScreenHeight));
				sum += tex2D(_MainTex, fixed2(i.uv.x - _BlurSize / _ScreenWidth, i.uv.y - _BlurSize / _ScreenHeight));
				sum += tex2D(_MainTex, fixed2(i.uv.x - _BlurSize / _ScreenWidth, i.uv.y + _BlurSize / _ScreenHeight));
				sum += tex2D(_MainTex, fixed2(i.uv.x + _BlurSize / _ScreenWidth, i.uv.y - _BlurSize / _ScreenHeight));
				
				sum /= 8;
				
				return original + (original-sum);
			}
			ENDCG
		}
	}
	// 2 sample variant
	// Pixel shader Cg
	SubShader{
		Pass{
			ZTest Off
			ZWrite Off
			
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma exclude_renderers metal xbox360 xboxone ps3 ps4 psp2
			#pragma target 2.0
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			fixed _BlurSize;
			fixed _ScreenHeight;
			fixed _ScreenWidth;
			
			fixed4 frag(v2f_img i) : SV_Target { // Pixel Shader
				fixed4 original = tex2D(_MainTex, i.uv);
				float4 sum = float4(0,0,0,0);
				sum += tex2D(_MainTex, fixed2(i.uv.x - _BlurSize / _ScreenWidth, i.uv.y));
				sum += tex2D(_MainTex, fixed2(i.uv.x + _BlurSize / _ScreenWidth, i.uv.y));
				
				sum /= 2;
				
				return original + (original-sum);
			}
			ENDCG
		}
	}
	FallBack "Hidden/EmptyImg"
}
