// This a Empty Image Effect, used as a fallback for where the effect is not supported

Shader "Hidden/EmptyImg" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	// Absolute failsafe
	// Pixel Shader Cg
	// This returns the original image back and thus doesnt realy do anything
	SubShader {
		Pass{
			ZTest Off
			Cull Off
			ZWrite Off
			
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma target 2.0
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			
			fixed4 frag(v2f_img i) : SV_Target { // Pixel Shader
				return tex2D(_MainTex, i.uv);
			}
			ENDCG
		}
	} 
	FallBack off
}
