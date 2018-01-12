using System;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
    [ExecuteInEditMode]
    [AddComponentMenu("Image Effects/Unsharp Mask")]
    public class UnsharpMask : ImageEffectBase {
        public float  blurSize = 1f;

        // Called by camera to apply image effect
        void OnRenderImage (RenderTexture source, RenderTexture destination) {

	        material.SetFloat("_BlurSize", blurSize);
			material.SetFloat("_ScreenHeight", Screen.height);
			material.SetFloat("_ScreenWidth", Screen.width);
	        Graphics.Blit (source, destination, material);
        }
    }
}
