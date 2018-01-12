using UnityEngine;
using System.Collections;
using System.IO;

public class stats : MonoBehaviour {
			
	private 		float 	fps 				= 0f;
	private			float 	ms 					= 0f;
	public 			bool	overlayStats		= false;

	static public	bool	hasCompass			= false;

	//private			string	browsePath			= "/";
	
	void  Awake () {
	
		Stats.GetInfo();
		Input.compass.enabled = true;
		Input.location.Start ();
	}

	void Update () {
	
		ms = Mathf.Ceil(Time.unscaledDeltaTime * 100f) / 100f;
		fps = Mathf.Floor(1f / Time.smoothDeltaTime * Time.timeScale * 10f)/10f;
	}

	public void ToggleOverlay(){

		overlayStats = !overlayStats;
	}


	void OnGUI(){

		if(overlayStats){

			string filePath;
			string[] content;

			/*filePath = "/dev/mali0";
			if(File.Exists(filePath)){
				content = File.ReadAllLines(filePath);
				GUILayout.Label(filePath);
				foreach(string line in content){
					GUILayout.Label(line);
				}
			}*/

			/*filePath = "/dev/graphics/fb0";
			if(File.Exists(filePath)){
				content = File.ReadAllLines(filePath);
				GUILayout.Label(filePath);
				foreach(string line in content){
					GUILayout.Label(line);
				}
			}*/

			filePath = "/proc/0/cpuset";
			if(File.Exists(filePath)){
				content = File.ReadAllLines(filePath);
				GUILayout.Label(filePath);
				foreach(string line in content){
					GUILayout.Label(line);
				}
			}

			/*
			GUILayout.BeginHorizontal();
			if(GUILayout.Button(" < ")) 
				browsePath = Directory.GetParent(browsePath).FullName;
			if(GUILayout.Button(" <<< ")) 
				browsePath = "/";
			GUILayout.EndHorizontal();

			GUILayout.Label(browsePath);

			if(Directory.Exists(browsePath)){
				DirectoryInfo d = new DirectoryInfo(browsePath);//Assuming Test is your Folder

				int screenRealestateLeft = 10;
				DirectoryInfo[] subdirs = d.GetDirectories();
				GUILayout.BeginHorizontal();
				foreach(DirectoryInfo subdir in subdirs){
					screenRealestateLeft --;
					if(screenRealestateLeft < 0){
						GUILayout.EndHorizontal();
						GUILayout.BeginHorizontal();
						screenRealestateLeft = 10;
					}

					if(GUILayout.Button("/" + subdir.Name + "/")) {
						browsePath = subdir.FullName;
					}
				}
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				screenRealestateLeft = 10;
				FileInfo[] Files = d.GetFiles(); //Getting Text files
				foreach(FileInfo file in Files){
					screenRealestateLeft --;
					if(screenRealestateLeft < 0){
						GUILayout.EndHorizontal();
						GUILayout.BeginHorizontal();
						screenRealestateLeft = 10;
					}

					GUILayout.Label(file.Name);
				}
				GUILayout.EndHorizontal();
			}*/

			GUILayout.Label("FPS: " + fps + "/" + ms + "ms");

			GUILayout.Label("Platform: " + Stats.platform);

			GUILayout.Label("Screen size: " + Screen.width + "x" + Screen.height);

			if(Stats.cpuSpeed != null)
				GUILayout.Label("CPU: " + Stats.cpuName + " " + Stats.cpuSpeed + "/" + Stats.coreCount + " threads");
			else
				GUILayout.Label("CPU: " + Stats.cpuName + " " + Stats.coreCount + " threads");
			if(SystemInfo.processorType != Stats.cpuName)
				GUILayout.Label("CPU (raw): " + SystemInfo.processorType);

			#if UNITY_ANDROID && !UNITY_EDITOR
			if(Stats.soc != null)
				GUILayout.Label("SoC (raw): " + Stats.soc);

			GUILayout.Label("Supports Neon: " + Stats.supports_neon);

			if(Stats.abi != null)
				GUILayout.Label("Abi: " + Stats.abi);
			#endif

			GUILayout.Label("GPU: " + Stats.gpuName + " " + Stats.vram);
			if(SystemInfo.graphicsDeviceName != Stats.gpuName)
				GUILayout.Label("GPU (raw): " + SystemInfo.graphicsDeviceName);

			GUILayout.Label("Renderer: " + Stats.renderer);
			//GUILayout.Label ("Renderer (Alternate): " + SystemInfo.graphicsDeviceVersion);
			//GUILayout.Label ("Renderer (Alternate 2): " + SystemInfo.graphicsDeviceType);
			GUILayout.Label("Pixel Shader: " + Stats.shaderModel);

			if(Stats.motherBoardModel != null)
				GUILayout.Label("Motherboard: " + Stats.motherBoardModel);

			#if UNITY_ANDROID && !UNITY_EDITOR
			GUILayout.Label("Device Model: " + SystemInfo.deviceModel);
			#endif

			GUILayout.Label("RAM: " + Stats.dram);

			#if UNITY_ANDROID && !UNITY_EDITOR
			if(Stats.android_sdk != 0)
				GUILayout.Label("SDK Version: " + Stats.android_sdk);

			if(Stats.batteryCurrent != null)
				GUILayout.Label("Battery Current: " + Stats.batteryCurrent);

			if(Stats.board != null)
				GUILayout.Label("Board name: " + Stats.board);
			if(Stats.brand != null)
				GUILayout.Label("Brand: " + Stats.brand);
			if(Stats.device != null)
				GUILayout.Label("Device name: " + Stats.device);
			if(Stats.manufacturer != null)
				GUILayout.Label("Manufacturer: " + Stats.manufacturer);
			if(Stats.model != null)
				GUILayout.Label("Phone model: " + Stats.model);
			if(Stats.product != null)
				GUILayout.Label("Product name: " + Stats.product);
			if(Stats.serial != null)
				GUILayout.Label("Serial: " + Stats.serial);

			GUILayout.Label("Tracking mode: " + AccelerometerTest.trackingMode);
			GUILayout.Label("Tracking mode (internal): " + AccelerometerTest.trackingModeInternal);

			GUILayout.Label ("Accelerometer: " + SystemInfo.supportsAccelerometer);//+ " @ " + PlayerSettings.accelerometerFrequency + " hz");
			if(SystemInfo.supportsAccelerometer)
				GUILayout.Label("Accelerometer X: " + AccelerometerTest.acceleroSmooth.x + " Accelerometer Y: " + AccelerometerTest.acceleroSmooth.y + " Accelerometer Z: " + AccelerometerTest.acceleroSmooth.z);

			GUILayout.Label ("Gyroscope: " + SystemInfo.supportsGyroscope);
			if(SystemInfo.supportsGyroscope){
				Input.gyro.enabled = true;
				Vector3 gyro = Input.gyro.attitude.eulerAngles;
				GUILayout.Label("Gyroscope X: " + gyro.x + " Gyroscope Y: " + gyro.y + " Gyroscope Z: " + gyro.z);
			}

			hasCompass = (Input.compass.trueHeading != 0f);
			GUILayout.Label("Compass : " + hasCompass);
			if(Input.compass.trueHeading != 0f)
				GUILayout.Label("Compass Heading: " + Input.compass.trueHeading);
			#endif
		}
	}
}
