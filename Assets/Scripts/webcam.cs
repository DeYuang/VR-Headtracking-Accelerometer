using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class webcam : MonoBehaviour {
	
	private	WebCamTexture 	mCamera 			= null;
	public	Material		webcamMat			= null;
	public	Transform		cameraRig			= null;
	public	Vector3			rotationOffset		= Vector3.zero;

	#if !UNITY_EDITOR

	void Start () {
	
		if(WebCamTexture.devices.Length != 0 && WebCamTexture.devices[0].isFrontFacing == false){
			rotationOffset = transform.eulerAngles;
			mCamera = new WebCamTexture (1024, 1024, 30);
			mCamera.wrapMode = TextureWrapMode.Repeat;
			mCamera.filterMode = FilterMode.Bilinear;
			mCamera.anisoLevel = 0;
			mCamera.mipMapBias = 0;
			webcamMat.mainTexture = mCamera;
			mCamera.Play();
		}
	}
	
	void OnDisable() {

		if(WebCamTexture.devices.Length != 0)
			mCamera.Stop();
	}

	void OnEnable(){

		if(WebCamTexture.devices.Length != 0)
			mCamera.Play();
	}

	void LateUpdate(){

		if(mCamera.didUpdateThisFrame && 
		   WebCamTexture.devices.Length != 0){
			transform.eulerAngles = cameraRig.eulerAngles;
			transform.Rotate(rotationOffset);
		}
	}
	#endif
}