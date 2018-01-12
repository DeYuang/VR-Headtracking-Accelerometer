Shader "Hidden/LuminanceAmbientOcclusion" {
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
			#pragma target 3.0
			#include "UnityCG.cginc"
			
			sampler2D 			_MainTex;				// The rendered image
			//sampler2D_float 	_CameraDepthTexture;	// camera depth texture (Z-buffer)
			fixed 				_ScreenHeight;			// Height of the screen in Pixels
			fixed 				_ScreenWidth;			// Width of the screen in Pixels
			
			fixed4 frag(v2f_img i) : SV_Target { // Pixel Shader
				fixed4 original = tex2D(_MainTex, i.uv);
				fixed bw = Luminance(original);
				// 2 pixel radius
				fixed avg2 = bw; // the first sample is basically free
				avg2 += Luminance(tex2D(_MainTex, fixed2(i.uv.x, 					 i.uv.y - 1 / _ScreenHeight)));
				avg2 += Luminance(tex2D(_MainTex, fixed2(i.uv.x, 					 i.uv.y + 1 / _ScreenHeight)));
				avg2 += Luminance(tex2D(_MainTex, fixed2(i.uv.x - 1 / _ScreenHeight, i.uv.y					   )));
				avg2 += Luminance(tex2D(_MainTex, fixed2(i.uv.x + 1 / _ScreenHeight, i.uv.y					   )));
				avg2 += Luminance(tex2D(_MainTex, fixed2(i.uv.x - 1 / _ScreenHeight, i.uv.y - 1 / _ScreenHeight)));
				avg2 += Luminance(tex2D(_MainTex, fixed2(i.uv.x - 1 / _ScreenHeight, i.uv.y + 1 / _ScreenHeight)));
				avg2 += Luminance(tex2D(_MainTex, fixed2(i.uv.x + 1 / _ScreenHeight, i.uv.y - 1 / _ScreenHeight)));
				avg2 += Luminance(tex2D(_MainTex, fixed2(i.uv.x + 1 / _ScreenHeight, i.uv.y + 1 / _ScreenHeight)));
				avg2 /= 9;
				//3 pixel radius
				fixed avg3 = bw; // yay free sample
				avg3 += Luminance(tex2D(_MainTex, fixed2(i.uv.x + 2 / _ScreenHeight, i.uv.y - 2 / _ScreenHeight)));
				avg3 += Luminance(tex2D(_MainTex, fixed2(i.uv.x + 2 / _ScreenHeight, i.uv.y - 1 / _ScreenHeight)));
				avg3 += Luminance(tex2D(_MainTex, fixed2(i.uv.x + 2 / _ScreenHeight, i.uv.y					   )));
				avg3 += Luminance(tex2D(_MainTex, fixed2(i.uv.x + 2 / _ScreenHeight, i.uv.y + 1 / _ScreenHeight)));
				avg3 += Luminance(tex2D(_MainTex, fixed2(i.uv.x + 2 / _ScreenHeight, i.uv.y + 2 / _ScreenHeight)));

				avg3 += Luminance(tex2D(_MainTex, fixed2(i.uv.x - 2 / _ScreenHeight, i.uv.y - 2 / _ScreenHeight)));
				avg3 += Luminance(tex2D(_MainTex, fixed2(i.uv.x - 2 / _ScreenHeight, i.uv.y - 1 / _ScreenHeight)));
				avg3 += Luminance(tex2D(_MainTex, fixed2(i.uv.x - 2 / _ScreenHeight, i.uv.y					   )));
				avg3 += Luminance(tex2D(_MainTex, fixed2(i.uv.x - 2 / _ScreenHeight, i.uv.y + 1 / _ScreenHeight)));
				avg3 += Luminance(tex2D(_MainTex, fixed2(i.uv.x - 2 / _ScreenHeight, i.uv.y + 2 / _ScreenHeight)));
				
				avg3 += Luminance(tex2D(_MainTex, fixed2(i.uv.x + 1 / _ScreenHeight, i.uv.y - 2 / _ScreenHeight)));
				avg3 += Luminance(tex2D(_MainTex, fixed2(i.uv.x , 					 i.uv.y - 2 / _ScreenHeight)));
				avg3 += Luminance(tex2D(_MainTex, fixed2(i.uv.x - 1 / _ScreenHeight, i.uv.y - 2 / _ScreenHeight)));
				
				avg3 += Luminance(tex2D(_MainTex, fixed2(i.uv.x + 1 / _ScreenHeight, i.uv.y + 2 / _ScreenHeight)));
				avg3 += Luminance(tex2D(_MainTex, fixed2(i.uv.x, 					 i.uv.y + 2 / _ScreenHeight)));
				avg3 += Luminance(tex2D(_MainTex, fixed2(i.uv.x - 1 / _ScreenHeight, i.uv.y + 2 / _ScreenHeight)));
				avg3 /= 17;
				//4 pixel radius
				fixed avg4 = bw;
				avg4 += Luminance(tex2D(_MainTex, fixed2(i.uv.x + 3 / _ScreenHeight, i.uv.y - 3 / _ScreenHeight)));
				avg4 += Luminance(tex2D(_MainTex, fixed2(i.uv.x + 2 / _ScreenHeight, i.uv.y - 3 / _ScreenHeight)));
				avg4 += Luminance(tex2D(_MainTex, fixed2(i.uv.x + 1 / _ScreenHeight, i.uv.y - 3 / _ScreenHeight)));
				avg4 += Luminance(tex2D(_MainTex, fixed2(i.uv.x, 					 i.uv.y - 3 / _ScreenHeight)));
				avg4 += Luminance(tex2D(_MainTex, fixed2(i.uv.x + 1 / _ScreenHeight, i.uv.y - 3 / _ScreenHeight)));
				avg4 += Luminance(tex2D(_MainTex, fixed2(i.uv.x + 2 / _ScreenHeight, i.uv.y - 3 / _ScreenHeight)));
				avg4 += Luminance(tex2D(_MainTex, fixed2(i.uv.x + 3 / _ScreenHeight, i.uv.y - 3 / _ScreenHeight)));
				
				avg4 += Luminance(tex2D(_MainTex, fixed2(i.uv.x + 3 / _ScreenHeight, i.uv.y + 3 / _ScreenHeight)));
				avg4 += Luminance(tex2D(_MainTex, fixed2(i.uv.x + 2 / _ScreenHeight, i.uv.y + 3 / _ScreenHeight)));
				avg4 += Luminance(tex2D(_MainTex, fixed2(i.uv.x + 1 / _ScreenHeight, i.uv.y + 3 / _ScreenHeight)));
				avg4 += Luminance(tex2D(_MainTex, fixed2(i.uv.x, 					 i.uv.y + 3 / _ScreenHeight)));
				avg4 += Luminance(tex2D(_MainTex, fixed2(i.uv.x + 1 / _ScreenHeight, i.uv.y + 3 / _ScreenHeight)));
				avg4 += Luminance(tex2D(_MainTex, fixed2(i.uv.x + 2 / _ScreenHeight, i.uv.y + 3 / _ScreenHeight)));
				avg4 += Luminance(tex2D(_MainTex, fixed2(i.uv.x + 3 / _ScreenHeight, i.uv.y + 3 / _ScreenHeight)));
				avg4 /= 15;
				//5 pixel radius
				fixed avg5 = bw;
				avg5 += Luminance(tex2D(_MainTex, fixed2(i.uv.x - 4 / _ScreenHeight, i.uv.y - 4 / _ScreenHeight)));
				avg5 += Luminance(tex2D(_MainTex, fixed2(i.uv.x - 4 / _ScreenHeight, i.uv.y - 3 / _ScreenHeight)));
				avg5 += Luminance(tex2D(_MainTex, fixed2(i.uv.x - 4 / _ScreenHeight, i.uv.y - 2 / _ScreenHeight)));
				avg5 += Luminance(tex2D(_MainTex, fixed2(i.uv.x - 4 / _ScreenHeight, i.uv.y - 1 / _ScreenHeight)));
				avg5 += Luminance(tex2D(_MainTex, fixed2(i.uv.x - 4 / _ScreenHeight, i.uv.y					   )));
				avg5 += Luminance(tex2D(_MainTex, fixed2(i.uv.x - 4 / _ScreenHeight, i.uv.y + 1 / _ScreenHeight)));
				avg5 += Luminance(tex2D(_MainTex, fixed2(i.uv.x - 4 / _ScreenHeight, i.uv.y + 2 / _ScreenHeight)));
				avg5 += Luminance(tex2D(_MainTex, fixed2(i.uv.x - 4 / _ScreenHeight, i.uv.y + 3 / _ScreenHeight)));
				avg5 += Luminance(tex2D(_MainTex, fixed2(i.uv.x - 4 / _ScreenHeight, i.uv.y + 4 / _ScreenHeight)));
				
				avg5 += Luminance(tex2D(_MainTex, fixed2(i.uv.x + 4 / _ScreenHeight, i.uv.y - 4 / _ScreenHeight)));
				avg5 += Luminance(tex2D(_MainTex, fixed2(i.uv.x + 4 / _ScreenHeight, i.uv.y - 3 / _ScreenHeight)));
				avg5 += Luminance(tex2D(_MainTex, fixed2(i.uv.x + 4 / _ScreenHeight, i.uv.y - 2 / _ScreenHeight)));
				avg5 += Luminance(tex2D(_MainTex, fixed2(i.uv.x + 4 / _ScreenHeight, i.uv.y - 1 / _ScreenHeight)));
				avg5 += Luminance(tex2D(_MainTex, fixed2(i.uv.x + 4 / _ScreenHeight, i.uv.y					   )));
				avg5 += Luminance(tex2D(_MainTex, fixed2(i.uv.x + 4 / _ScreenHeight, i.uv.y + 1 / _ScreenHeight)));
				avg5 += Luminance(tex2D(_MainTex, fixed2(i.uv.x + 4 / _ScreenHeight, i.uv.y + 2 / _ScreenHeight)));
				avg5 += Luminance(tex2D(_MainTex, fixed2(i.uv.x + 4 / _ScreenHeight, i.uv.y + 3 / _ScreenHeight)));
				avg5 += Luminance(tex2D(_MainTex, fixed2(i.uv.x + 4 / _ScreenHeight, i.uv.y + 4 / _ScreenHeight)));
				avg5 /= 19;
				
				fixed avg = (bw+avg2+avg3+avg4+avg5)/5;
				
				fixed ao = bw/max(avg, bw);
				//ao = dot(original,ao)*ao;
	
				return original*clamp(ao, 0, 1);
			}
			ENDCG
		}
	}
	SubShader{
		Pass{
			ZTest Off
			ZWrite Off
			
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma exclude_renderers metal xbox360 xboxone ps3 ps4 psp2 openGL
			#pragma target 2.0
			#include "UnityCG.cginc"
			
			sampler2D 			_MainTex;				// The rendered image
			fixed 				_ScreenHeight;			// Height of the screen in Pixels
			fixed 				_ScreenWidth;			// Width of the screen in Pixels
			
			fixed4 frag(v2f_img i) : SV_Target { // Pixel Shader
				fixed4 original = tex2D(_MainTex, i.uv);
				fixed bw = Luminance(original);
				// 2 pixel radius
				fixed avg2 = Luminance(tex2D(_MainTex, fixed2(i.uv.x - 1 / _ScreenHeight, i.uv.y - 1 / _ScreenHeight)));
				avg2 += Luminance(tex2D(_MainTex, fixed2(i.uv.x - 1 / _ScreenHeight, i.uv.y + 1 / _ScreenHeight)));
				avg2 += Luminance(tex2D(_MainTex, fixed2(i.uv.x + 1 / _ScreenHeight, i.uv.y - 1 / _ScreenHeight)));
				avg2 += Luminance(tex2D(_MainTex, fixed2(i.uv.x + 1 / _ScreenHeight, i.uv.y + 1 / _ScreenHeight)));
				avg2 /= 4;
				
				fixed ao = bw/max((bw+avg2)/2, bw);
	
				return original*ao;
			}
			ENDCG
		}
	}
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
			
			sampler2D 			_MainTex;				// The rendered image
			fixed 				_ScreenHeight;			// Height of the screen in Pixels
			fixed 				_ScreenWidth;			// Width of the screen in Pixels
			
			fixed4 frag(v2f_img i) : SV_Target { // Pixel Shader
				fixed4 original = tex2D(_MainTex, i.uv);
				fixed bw = Luminance(original);
				// 2 pixel radius
				fixed avg2 = Luminance(tex2D(_MainTex, fixed2(i.uv.x - 1 / _ScreenHeight, i.uv.y + 1 / _ScreenHeight)));
				//avg2 += Luminance(tex2D(_MainTex, fixed2(i.uv.x + 1 / _ScreenHeight, i.uv.y - 1 / _ScreenHeight)));
				//avg2 /= 2;
				
				fixed ao = bw/max((bw+avg2)/2, bw);
	
				return original*ao;
			}
			ENDCG
		}
	}
	
	// This a Empty Image Effect, used as a fallback for where the effect is not supported
	FallBack "Hidden/EmptyImg"
}
