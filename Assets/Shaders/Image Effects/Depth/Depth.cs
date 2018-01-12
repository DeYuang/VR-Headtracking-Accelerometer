using System;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
    [ExecuteInEditMode]
    [AddComponentMenu("Image Effects/Depth")]
    public class Depth : ImageEffectBase {

        // Called by camera to apply image effect
        void OnRenderImage (RenderTexture source, RenderTexture destination) {

			material.SetFloat("_ScreenHeight", Screen.height);
			material.SetFloat("_ScreenWidth", Screen.width);
			GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
	        Graphics.Blit (source, destination, material);
        }
    }
}
