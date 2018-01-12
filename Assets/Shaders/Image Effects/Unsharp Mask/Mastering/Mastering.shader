Shader "Hidden/Mastering" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_RgbTex ("_RgbTex (RGB)", 2D) = "white" {}
	}
	
	SubShader {
		Pass{
			ZTest Off
			ZWrite Off
					
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma exclude_renderers metal xbox360 xboxone ps3 ps4 psp2 //d3d9 //d3d11_9x
			//#pragma target 2.0
			#include "UnityCG.cginc"
	
			sampler2D			_MainTex;
			sampler2D			_RgbTex;
			fixed				_Saturation;
			fixed				_midR;
			fixed				_midG;
			fixed				_midB;
			
			fixed4 frag(v2f_img i) : SV_Target {
			
				fixed4 col = (tex2D(_MainTex, i.uv) + fixed4(_midR, _midG,_midB, 0)) * _Saturation; 
				
				//midtones
				fixed4 midtones = (clamp((col - 0.33) / 0.25 + 0.5, 0.0, 1.0) * clamp ((col + 0.33 - 1.0) / -0.25 + 0.5, 0.0, 1.0) * 0.75);
				
				
				return midtones;		
			}

			ENDCG 	
		}
	}
	
	//FallBack "Hidden/EmptyImg"
}
