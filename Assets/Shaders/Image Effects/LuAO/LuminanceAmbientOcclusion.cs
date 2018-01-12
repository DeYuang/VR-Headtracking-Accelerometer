using System;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
    [ExecuteInEditMode]
	[AddComponentMenu("Image Effects/Photocopy")]
    public class LuminanceAmbientOcclusion : ImageEffectBase {

        // Called by camera to apply image effect
        void OnRenderImage (RenderTexture source, RenderTexture destination) {

			material.SetFloat("_ScreenHeight", Screen.height);
			material.SetFloat("_ScreenWidth", Screen.width);
	        Graphics.Blit (source, destination, material);
        }
    }
}
