Shader "Hidden/BoxBlur" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Pass{
			ZTest Off
			ZWrite Off
			
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma exclude_renderers metal xbox360 xboxone ps3 ps4 psp2 opengl //d3d9 //d3d11_9x
			#pragma target 2.0
			#include "UnityCG.cginc"
			
			sampler2D 			_MainTex;				// The rendered image
			//sampler2D_float 	_CameraDepthTexture;	// camera depth texture (Z-buffer)
			fixed 				_ScreenHeight;			// Height of the screen in Pixels
			fixed 				_ScreenWidth;			// Width of the screen in Pixels
			
			fixed4 frag(v2f_img i) : SV_Target { // Pixel Shader
				fixed4 original = tex2D(_MainTex, i.uv);
				// 2 pixel radius
				fixed4 avg2 = 0; // the first sample is basically free
				avg2 += tex2D(_MainTex, fixed2(i.uv.x, 					   i.uv.y - 1 / _ScreenHeight));
				avg2 += tex2D(_MainTex, fixed2(i.uv.x, 					   i.uv.y + 1 / _ScreenHeight));
				avg2 += tex2D(_MainTex, fixed2(i.uv.x - 1 / _ScreenHeight, i.uv.y				     ));
				avg2 += tex2D(_MainTex, fixed2(i.uv.x + 1 / _ScreenHeight, i.uv.y				     ));
				avg2 += tex2D(_MainTex, fixed2(i.uv.x - 1 / _ScreenHeight, i.uv.y - 1 / _ScreenHeight));
				avg2 += tex2D(_MainTex, fixed2(i.uv.x - 1 / _ScreenHeight, i.uv.y + 1 / _ScreenHeight));
				avg2 += tex2D(_MainTex, fixed2(i.uv.x + 1 / _ScreenHeight, i.uv.y - 1 / _ScreenHeight));
				avg2 += tex2D(_MainTex, fixed2(i.uv.x + 1 / _ScreenHeight, i.uv.y + 1 / _ScreenHeight));
				avg2 /= 8;
	
				return avg2;
			}
			ENDCG
		}
	}
}
