using UnityEngine;
using UnityStandardAssets.ImageEffects;
using System.Collections;
using System.Runtime.InteropServices;
using System;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class HBAOPlus : MonoBehaviour {

    public enum BlurRadiusMode {
        BLUR_RADIUS_2,
        BLUR_RADIUS_4,
        BLUR_RADIUS_8,
    }

    [SerializeField] private Shader fetchDepthShader; // Use a dummy shader to explicitly binds the _CameraDepthTexture SRV to a register.
    [SerializeField] private Shader renderAoShader;
	[SerializeField] private Shader renderAoShaderAA;

	private static	Material 		fetchDepthMaterial;
	private static	Material 		renderAoMaterial;
	private static	Material 		renderAoMaterialAA;

    private 		bool 			useNormalTexture 		= false; //Not working.
	private 		RenderTexture	output;
	private 		RenderTexture	normals;
	//cached vars
	private			float[] 		projMatrixArr 			= new float[16];
	private 		float[] 		viewMatrixArr 			= new float[16];
	private 		Matrix4x4		projMatrix;
	private 		Matrix4x4 		viewMatrix;
	private 		int 			i 						= 0;

    public 			float 			radius 					= 0.2f;
    public 			float 			bias;
    public 			float 			powerExponent 			= 1.0f;
    public 			bool 			enableBlur 				= true;
    public 			float 			blurSharpness 			= 1.0f;
    public 			BlurRadiusMode 	blurRadiusMode;

	#if UNITY_STANDALONE_WIN32
	private const 	string 			PluginName 				= "HBAO_Plugin.Win32";
	#else
	private const 	string 			PluginName 				= "HBAO_Plugin.x64";
	#endif

	private 		Camera 			_camera;
	private 		Camera 			Camera 
	{
		get 
		{
			if (_camera == null) 
			{
				_camera = GetComponent<Camera> ();
			}
			return _camera;
		}
	}

    // The block of code below is a neat trick to allow for calling into the debug console from C++
	[DllImport(PluginName)]
    private static extern void LinkDebug([MarshalAs(UnmanagedType.FunctionPtr)]IntPtr debugCal);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void DebugLog(string log);

    private static readonly DebugLog debugLog = DebugWrapper;
    private static readonly IntPtr functionPointer = Marshal.GetFunctionPointerForDelegate(debugLog);

    private static void DebugWrapper(string log) { Debug.Log(log); }

	[DllImport(PluginName, CallingConvention = CallingConvention.StdCall)]
    private static extern int GetEventID();

	[DllImport(PluginName, CallingConvention = CallingConvention.StdCall)]
	private static extern void SetAoParameters( float Radius,
	                                            float Bias,
	                                            float PowerExponent,
	                                            bool EnableBlur,
	                                            int BlurRadiusMode,
	                                            float BlurSharpness,
	                                            int BlendMode );

	[DllImport(PluginName, CallingConvention = CallingConvention.StdCall)]
	private static extern void SetInputData(    float MetersToViewSpaceUnits,
                                                float[] pProjectionMatrix,
												float[] pWorldToViewMatrix, 
	                                            float height, 
	                                            float width,
	                                            float topLeftX,
	                                            float topLeftY,
	                                            float minDepth,
	                                            float maxDepth,
												bool useNormals );

	[DllImport(PluginName, CallingConvention = CallingConvention.StdCall)]
	private static extern void SetNormalsData(IntPtr pNormalsTexture);

	[DllImport(PluginName, CallingConvention = CallingConvention.StdCall)]
    private static extern void SetOutputData(IntPtr pOutputTexture);

    private void Start(){
        LinkDebug(functionPointer); // Hook our c++ plugin into Unitys console log.
    }

    private void OnEnable() {

        if (renderAoMaterial == null)   renderAoMaterial    = new Material(renderAoShader);
		if (renderAoMaterialAA == null)   renderAoMaterialAA    = new Material(renderAoShaderAA);
		if (fetchDepthMaterial == null) fetchDepthMaterial = new Material(fetchDepthShader);
	}

    private void OnDisable() {

        if (renderAoMaterial != null)   DestroyImmediate(renderAoMaterial);
		if (renderAoMaterialAA != null)   DestroyImmediate(renderAoMaterialAA);
        if (fetchDepthMaterial != null) DestroyImmediate(fetchDepthMaterial);

		if (output != null)	 DestroyImmediate(output);
		if (normals != null) DestroyImmediate(normals);
    }

    private void Update(){

		if (SystemInfo.graphicsShaderLevel < 50F){ // HBAO requires shader level 5 or higher.
			this.enabled = false;
			return;
		}
		Camera.depthTextureMode |= DepthTextureMode.Depth;

		if (useNormalTexture)
			Camera.depthTextureMode |= DepthTextureMode.DepthNormals;  
    }

    // Perform AO immediately after opaque rendering.
    [ImageEffectOpaque]
    private void OnRenderImage(RenderTexture source, RenderTexture destination){

		if (SystemInfo.graphicsShaderLevel < 50F)
			return;

		if (output == null || output.width != Screen.width || output.height != Screen.height){
	    	output = new RenderTexture(Screen.width, Screen.height, 0);
	        output.Create(); // Must call create or ptr will be null.
	        SetOutputData(output.GetNativeTexturePtr());
	    } 

		// Convert matrices to float arrays.
		projMatrix = GL.GetGPUProjectionMatrix(Camera.projectionMatrix, false);
		viewMatrix = Camera.cameraToWorldMatrix;// transform.worldToLocalMatrix;
		i = 0;
		while (i < 16){
			projMatrixArr[i] = projMatrix[i];
			viewMatrixArr[i] = viewMatrix[i];
			i ++;
		}

		if (useNormalTexture){
			if (normals == null || normals.width != Screen.width || normals.height != Screen.height){
				normals = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBHalf);
				normals.Create(); // Must call create or ptr will be null.
				SetNormalsData(normals.GetNativeTexturePtr());
			}

			fetchDepthMaterial.SetMatrix("_Camera2World", viewMatrix);
			//Graphics.Blit(null, normals, fetchDepthMaterial, 0);
		}

	    SetAoParameters(radius, bias, powerExponent, enableBlur, (int)blurRadiusMode, blurSharpness, 0);

		SetInputData(1f, projMatrixArr, viewMatrixArr, (float)Screen.height, (float)Screen.width, 0, 0, 0f, 1f, useNormalTexture);
	        
		fetchDepthMaterial.SetPass(1); // Here Unity will bind the _CameraDepthTexture SRV used in this shader to the explicit register t99, which we can fetch in our plugin using ID3D11DeviceContext::PSGetShaderResources(...)
	        
	    // Call our render method from the AO plugin.
	    GL.IssuePluginEvent(GetEventID());

		if (QualitySettings.antiAliasing != 0 && _camera.GetComponent<ScreenSpaceAmbientOcclusion> ().enabled){
			renderAoMaterialAA.SetTexture ("_AoResult", output);
			renderAoMaterialAA.SetTexture ("_MainTex", source);
			Graphics.Blit (source, destination, renderAoMaterialAA);
		}
		else {
			renderAoMaterial.SetTexture ("_AoResult", output);
			renderAoMaterial.SetTexture ("_MainTex", source);
			Graphics.Blit (source, destination, renderAoMaterial);
		}
	}
}