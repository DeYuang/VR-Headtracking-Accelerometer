using System;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
    [ExecuteInEditMode]
	[AddComponentMenu("Image Effects/Color Mastering")]
    public class Mastering: ImageEffectBase {

		public	float		saturation			= 1f;
		public	float		midR				= 0f;
		public	float		midG				= 0f;
		public	float		midB				= 0f;

		private Texture2D	rgbChannelTex		= null;
		private	Material	mat					= null;

        // Called by camera to apply image effect
        void OnRenderImage (RenderTexture source, RenderTexture destination) {

			if (mat == null)   mat = new Material(shader);
			rgbChannelTex = new Texture2D (256, 4, TextureFormat.ARGB32, false, true);
			rgbChannelTex.hideFlags = HideFlags.DontSave;
			rgbChannelTex.wrapMode = TextureWrapMode.Clamp;

			mat.SetTexture ("_RgbTex", rgbChannelTex);
			mat.SetFloat ("_Saturation", saturation);
			mat.SetFloat ("_midR", midR);
			mat.SetFloat ("_midG", midG);
			mat.SetFloat ("_midB", midB);

	        Graphics.Blit (source, destination, mat);
        }
    }
}
