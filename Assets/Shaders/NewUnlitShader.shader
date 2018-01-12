Shader "Custom/Foliage" {
     Properties {
         _Color ("Main Color", Color) = (.5, .5, .5, .5)
         _MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
         _Cutoff ("Base Alpha cutoff", Range (0,.9)) = .5
     }
     SubShader {
        Material {
            Diffuse [_Color]
            Ambient [_Color]
        }
	   	Cull off
		ZWrite On
		ZTest On
		Lighting Off
 
        // first pass:
        //   render any pixels that are more than [_Cutoff] opaque
        Pass {
            AlphaTest Greater [_Cutoff]
        	SetTexture [_MainTex] {
            	combine texture * primary//, texture
            }
        }
     	
     	Pass { // Pass 2: 2nd sample (2x multisampling)
             ZWrite off
             ZTest Less
             AlphaTest Less [_Cutoff]
 
             // Set up alpha blending
             Blend SrcAlpha OneMinusSrcAlpha
 
             SetTexture [_MainTex] {
                 combine texture * primary//, texture                
             }
        }
	}
        
    SubShader {
	    Cull Back
		ZWrite Off
		ZTest Off
		Lighting Off
		
		Fog{
			Mode Off
		}
 
        Pass {
            AlphaTest Greater [_Cutoff]
        	SetTexture [_MainTex] {
            	combine texture * primary
            }
        }
    }
 } 