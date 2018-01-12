using UnityEngine;
using System; // used for Environment members
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
using Microsoft.Win32; // used for reading reg
#elif UNITY_ANDROID
using System.IO; // used for getting cpuinfo and cpufreq
#endif
using System.Collections;
using System.Collections.Generic; // used ofr List
using System.Globalization; // used for TextInfo
using System.Net.NetworkInformation; // used for getting mac adress
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class NetworkAdapter : System.Object{
	
	public 		string 					adapterName			= string.Empty;
	public 		string 					adapterAltName		= string.Empty;
	public		string					IPv4Adress			= null;
	public		string					IPv6Adress			= null;
	public		string					macAdress			= null;
	public		string					gateway				= null;
	public		string					nominalSpeed		= string.Empty;
	public		string					MTU					= string.Empty;
}

//[ExecuteInEditMode]
public class Stats : MonoBehaviour {
	
	static	public 	string				renderer			= null;
	static	public 	string				cpuName				= null;
	static	public 	string				cpuSpeed			= null;
	#if UNITY_ANDROID
	static	public	bool 				supports_neon 		= false;
	static	public	string 				soc		 			= null;
	static	public	int					android_sdk			= 0;
	static	public	string				batteryCurrent		= null;
	static	public	string				board				= null;
	static	public	string				brand				= null;
	static	public	string				device				= null;
	static	public	string				manufacturer		= null;
	static	public	string				model				= null;
	static	public	string				product				= null;
	static	public	string				abi					= null;
	static	public	string				serial				= null;
	#endif
	static	public 	string				gpuName				= null;
	static	public	string				vram				= null;
	static	public	string				dram				= null;
	static	public	float				shaderModel			= 0;
	static	public	int					coreCount			= 0;
	static 	public	string				motherBoardModel	= null;
	static	public	string				platform			= null;
	static 	public	NetworkAdapter[]	netAdapters			= new NetworkAdapter[0];
		
	static	private	TextInfo			textInfo			= new CultureInfo("en-US", false).TextInfo;
	static	private	int					tester				= -1;
	static	private int 				i					= 0;
	static	private string				s					= null;

	void Awake(){

		//String[] arguments = Environment.GetCommandLineArgs();
		//Console.WriteLine("GetCommandLineArgs: {0}", String.Join(", ", arguments));

		//HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Hardware Profiles\\UnitedVideo\\CONTROL\\VIDEO for native refresh rate

		Stats.GetInfo ();
	}

	static public void GetNetworkAdress(){

		IPGlobalProperties computerProperties = IPGlobalProperties.GetIPGlobalProperties();
		NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();

		List<NetworkAdapter> adapterList = new List<NetworkAdapter>();

		int i = 0;

		foreach (NetworkInterface adapter in nics){

			//if(adapter.GetIPv4Statistics().UnicastPacketsReceived + adapter.GetIPv4Statistics().NonUnicastPacketsReceived == 0)
			//	continue;

			NetworkAdapter adaptertje = new NetworkAdapter();
			PhysicalAddress address = adapter.GetPhysicalAddress();

			// name of the adapter
			adaptertje.adapterName = StringFunction.FilterString(adapter.Description);
			adaptertje.adapterAltName = adapter.Name;

			// IPv4 and IPv6 adresses and MTU
			if(adapter.GetIPProperties().GatewayAddresses.Count > 0)
				adaptertje.gateway = adapter.GetIPProperties().GatewayAddresses[0].Address.ToString();

			foreach(UnicastIPAddressInformation ip in adapter.GetIPProperties().UnicastAddresses){
				if(ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork){
					adaptertje.IPv4Adress = ip.Address.ToString();
					adaptertje.MTU = (Mathf.Floor(adapter.GetIPProperties().GetIPv4Properties().Mtu / 100f) / 10f) + " KB";
				}
				else if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6){
					adaptertje.IPv6Adress = ip.Address.ToString();
					adaptertje.MTU = (Mathf.Floor(adapter.GetIPProperties().GetIPv6Properties().Mtu / 100f) / 10f) + " KB";
				}
			}

			// Adapter nominal speed
			if(adapter.Speed == 1000000000)
				adaptertje.nominalSpeed = "Gigabit";
			else if(adapter.Speed > 1000000000)
				adaptertje.nominalSpeed = (Mathf.Ceil((float)adapter.Speed / 100000000)/10f) + " Gigabit";
			else if (adapter.Speed < 1000000)
				adaptertje.nominalSpeed = (Mathf.Floor((float)adapter.Speed / 100f) / 10f) + " Kbps";
			else
				adaptertje.nominalSpeed = (Mathf.Floor((float)adapter.Speed / 100000f) / 10f) + " Mbps";

			// MAC address
			byte[] bytes = address.GetAddressBytes();
			string mac = null;
			for (i = 0; i < bytes.Length; i++){
				mac = string.Concat(mac +(string.Format("{0}", bytes[i].ToString("X2"))));
				if (i != bytes.Length - 1){
					mac = string.Concat(mac + "-");
				}
			}
			adaptertje.macAdress = mac;

			adapterList.Add(adaptertje);
		}

		if(netAdapters.Length != adapterList.Count)
			netAdapters = new NetworkAdapter[adapterList.Count];

		i = 0;

		foreach(NetworkAdapter adapter in adapterList){
			netAdapters[i] = adapter;
			i ++;
		}
	}

	static public void GetInfo () {

		renderer = SystemInfo.graphicsDeviceVersion;
		renderer = renderer.ToLower ();
		if(renderer.Length > 8){
			s = renderer.Substring(0, 8);
			if(s == "direct3d" || s == "directx")
				renderer = "DirectX" + renderer.Substring(8);
			s = s.Substring(0, 6);
			if(s == "opengl")
				renderer = "OpenGL" + renderer.Substring(6);
			s = s.Substring(0, 3);
			if(s == "d3d")
				renderer = "DirectX" + renderer.Substring(3);
			s = s.Substring(0, 2);
			if(s == "dx" || s == "dd")
				renderer = "DirectX" + renderer.Substring(2);
			// check if there's a drivername in the name still (see OpenGL Core)
		}
		if(renderer.Contains("[")){
			if(renderer.Contains("[level ") == false)
				renderer = renderer.Substring(0, renderer.IndexOf('[') - 1);
			else{
				// remove any number before "[level"
				i = renderer.Length - 6;
				while(i > 0){
					i --;
					if (renderer.Substring(i, 6) == "[level"){
						//remove any numbers that come before i
						while(i > 1){
							i--;
							int.TryParse(renderer.Substring(i, 1), out tester);
							if(tester != 0 || renderer[i] == '0'){
								if(renderer[i-1] == '.')
									renderer = renderer.Substring(0, i - 1) + renderer.Substring(i + 1);
								else
									renderer = renderer.Substring(0, i) + renderer.Substring(i + 1);
							}
						}
						break;
					}
				}

				// find "[level " in string
				//count number of '['s
				i = 0;
				foreach(char character in renderer){
					if(character == '[')
						i ++;
				}
				if(i == 1){
					renderer = renderer.Substring(0, renderer.IndexOf('[') - 1) + renderer.Substring(renderer.IndexOf('[') + 6);
					// remove ']' 
					if(renderer.Contains("]")){
						renderer = renderer.Substring(0, renderer.IndexOf(']')) + renderer.Substring(renderer.IndexOf(']') + 1);
					}
				}
				/*else{ // i > 1
					//find the one that has "level " after it
					i = renderer.Length;
					while(i > 1){
						i --;
						if(renderer.Substring(i, 6) == "[level"){
							renderer = 
						}
					}
				}*/
			}
		}
		renderer = renderer.Replace(" ", string.Empty); // removes any spaces
		// now insert a space in front of the first number in the string
		i = 0;
		while(i < renderer.Length){
			int.TryParse(renderer.Substring(i, 1), out tester);
			if(tester != 0 || renderer[i] == '0'){
				renderer = renderer.Substring(0, i) + " " + renderer.Substring(i);
				break;
			}
			i++;
		}
		while(renderer.Substring(renderer.Length - 1, 1) == " "){
			renderer = renderer.Substring(0, renderer.Length - 1);
		}

		cpuName = SystemInfo.processorType;

		// Android Specific CPU name grab
		#if UNITY_ANDROID && !UNITY_EDITOR
		string filePath = "/proc/cpuinfo";
		string[] content;

		string processor = null;

		if(cpuName.Contains ("NEON"))
			supports_neon = true;
		
		if(File.Exists(filePath)){
			content = File.ReadAllLines(filePath);
			foreach(string line in content){
				if(line == "")
					continue;
				
				// I want to harvest "Processor" (the first one that comes up)
				if(line.Length > 9 && processor == null && line.Substring(0, 9) == "Processor")
					processor = line.Substring(12);
				// and "Hardware"
				else if(line.Length > 8 && soc == null && line.Substring(0, 8) == "Hardware"){
					soc = line.Substring(11);
					break;
				}
				
				// and Neon support
				else if(supports_neon == false && line.Contains("neon"))
					supports_neon = true;
			}
		}
		if(soc.Substring(0, 2) == "MT")
			cpuName = "Mediatek " + soc;
		if(cpuName.Substring(0, 4) == "ARMv")
			cpuName = processor;
		#endif

		// CPU info (name and speed)
		//Intel(R) Core(TM) i5(R) CPU M480 @ 2.67Ghz		// switcheroo M and 480 FIXED
		//Intel(R) Core(TM) i7 CPU Q 720 @ 1.60GHz			// switcheroo Q and 720 and remove the space between them FIXED
		//AMD Athlon(tm) 64 X2 Dual Core Processor 4200+ 	// get rid of "Dual Core" FIXED
		//Intel(R) Core(TM)2 CPU 6300 @ 1.86GHz				// missing "duo" keyword FIXED
		//AMD A6-3620 APU with Radeon(tm) HD Graphics		// get rid of 'APU' and get rid of 'with Radeon(tm) HD Graphics' FIXED
		//Intel(R) Celeron(TM) M CPU 430 @ 1.73Ghz
		i = 0;
		// scan if this PC has a core2 with bad naming convention (has the core2 name, but no 'solo', 'duo' ect suffix)
		if(cpuName.Contains("Core(TM)2") && (cpuName.Contains("Solo") == false && cpuName.Contains("Duo") == false && cpuName.Contains("Quad") == false && cpuName.Contains("Extreme") == false)){
			while(i < cpuName.Length - 1){
				print (cpuName.Substring(i,9));
				if(cpuName.Length - i > 8 && cpuName[i] == 'C' && cpuName.Substring(i,9) == "Core(TM)2"){
					cpuName = cpuName.Substring(0, i) + "Core2 Duo" + cpuName.Substring(i + 9);
					break;
				}
				i ++;
			}
			i = 0;
		}
		while(i < cpuName.Length - 1){
			if(cpuName[i] == '('){ 
				if(cpuName[i+2] == ')')// removes (r)
					cpuName = cpuName.Substring(0, i) + cpuName.Substring (i+3);
				else if(cpuName[i+3] == ')') // removes (tm)
					cpuName = cpuName.Substring(0, i) + cpuName.Substring (i+4);
			}
			else if(cpuName.Length - i > 2 && (cpuName[i] == 'C' || cpuName[i] == 'c') && (cpuName.Substring(i, 3) == "CPU" || cpuName.Substring(i, 3) == "cpu"))
				cpuName = cpuName.Substring(0, i) + cpuName.Substring (i+4);
			else if(cpuName.Length - i > 2 && (cpuName[i] == 'A' || cpuName[i] == 'a') && (cpuName.Substring(i, 3) == "APU" || cpuName.Substring(i, 3) == "apu")){
				cpuName = cpuName.Substring(0, i) + cpuName.Substring (i+4);
				i --;
			}
			else if(cpuName.Length - i > 8 && (cpuName[i] == 'P' || cpuName[i] == 'p') && (cpuName.Substring(i, 9) == "Processor"))
				cpuName = cpuName.Substring(0, i) + cpuName.Substring (i+10);
			else if(cpuName.Length - i > 8 && (cpuName[i] == 'A') && (cpuName.Substring(i, 4) == "ARMv")) {
				cpuName = "Generic ARMv" + cpuName.Substring (i+4, 1) + " Core";
			}
			else if(cpuName.Length - i > 8 && cpuName[i] == 'D' && cpuName.Substring(i, 9) == "Dual Core"){
				cpuName = cpuName.Substring(0, i) + cpuName.Substring (i+10);
				i--;
			}
			else if(cpuName.Length - i > 8 && cpuName[i] == 'Q' && cpuName.Substring(i, 9) == "Quad-Core"){
				cpuName = cpuName.Substring(0, i) + cpuName.Substring (i+10);
				i--;
			}
			else if(cpuName.Length - i > 7 && cpuName[i] == 'S' && cpuName.Substring(i, 8) == "Six-Core"){
				cpuName = cpuName.Substring(0, i) + cpuName.Substring (i+9);
				i--;
			}
			else if(cpuName.Length - i > 9 && cpuName[i] == 'E' && cpuName.Substring(i, 10) == "Eight-Core"){
				cpuName = cpuName.Substring(0, i) + cpuName.Substring (i+11);
				i--;
			}
			else if(cpuName.Length - i > 26 && cpuName[i] == 'w' && cpuName.Substring(i,27) == "with Radeon(tm) HD Graphics"){
				cpuName = cpuName.Substring(0, i);
				break;
			}
			else if(cpuName.Contains("FX-9590")){
				cpuName = "AMD FX-9590 Black Edition";
				cpuSpeed = "4.70GHz";
				break;
			}
			else if(cpuName.Contains("FX-9370")){
				cpuName = "AMD FX-9370 Black Edition";
				cpuSpeed = "4.40GHz";
				break;
			}
			else if(cpuName.Contains("FX-8370E")){
				cpuName = "AMD FX-8370E Black Edition";
				cpuSpeed = "3.30GHz";
				break;
			}
			else if(cpuName.Contains("FX-8370 ")){
				cpuName = "AMD FX-8370 Black Edition";
				cpuSpeed = "4.00GHz";
				break;
			}
			else if(cpuName.Contains("FX-8350")){
				cpuName = "AMD FX-8350 Black Edition";
				cpuSpeed = "4.00GHz";
				break;
			}
			else if(cpuName.Contains("FX-8320E")){
				cpuName = "AMD FX-8320E Black Edition";
				cpuSpeed = "3.20GHz";
				break;
			}
			else if(cpuName.Contains("FX-8320 ")){
				cpuName = "AMD FX-8320 Black Edition";
				cpuSpeed = "3.50GHz";
				break;
			}
			else if(cpuName.Contains("FX-8150")){
				cpuName = "AMD FX-8150 Black Edition";
				cpuSpeed = "3.60GHz";
				break;
			}
			else if(cpuName.Contains("FX-8140")){
				cpuName = "AMD FX-8140 Black Edition";
				cpuSpeed = "4.10GHz";
				break;
			}
			else if(cpuName.Contains("FX-8120")){
				cpuName = "AMD FX-8120 Black Edition";
				cpuSpeed = "3.10GHz";
				break;
			}
			else if(cpuName.Contains("FX-8100")){
				cpuName = "AMD FX-8100 Black Edition";
				cpuSpeed = "2.80GHz";
				break;
			}
			else if(cpuName.Contains("FX-6350")){
				cpuName = "AMD FX-6350 Black Edition";
				cpuSpeed = "3.90GHz";
				break;
			}
			else if(cpuName.Contains("FX-6300")){
				cpuName = "AMD FX-6300 Black Edition";
				cpuSpeed = "3.50GHz";
				break;
			}
			else if(cpuName.Contains("FX-4350")){
				cpuName = "AMD FX-4350 Black Edition";
				cpuSpeed = "4.20GHz";
				break;
			}
			else if(cpuName.Contains("FX-4300")){
				cpuName = "AMD FX-4300 Black Edition";
				cpuSpeed = "3.80GHz";
				break;
			}
			else if(cpuName.Contains("FX-4100")){
				cpuName = "AMD FX-4100 Black Edition";
				cpuSpeed = "3.60Ghz";
				break;
			}
			else if(cpuName.Contains ("X2 5000+")){
				cpuName = "AMD Athlon 64 X2 5000+ Black Edition";
				cpuSpeed = "2.60GHz";
				break;
			}
			i++;
		}
		if(cpuName.Contains("@")){
			// start going string until you hit 'Hz'
			// then work back until there are no more numbers
			string[] temp = cpuName.Split('@');
			cpuName = temp[0];
			if(temp[temp.Length -1].Contains("hz") || temp[temp.Length -1].Contains("HZ") || temp[temp.Length -1].Contains("Hz")){
				cpuSpeed = temp[temp.Length -1].Replace(" ", string.Empty); // removes any spaces
			}
		}
		while(cpuName.Substring(cpuName.Length - 1, 1) == " "){
			cpuName = cpuName.Substring(0, cpuName.Length - 1);
		}
		// scan for first gen i7 names
		if(cpuName.Contains ("i7 ") || cpuName.Contains ("i5 ") || cpuName.Contains ("i3 ")){
			i = 0;
			while(i < cpuName.Length){
				if(cpuName[i] == 'i' && (cpuName[i+1] == '7' || cpuName[i+1] == '5' || cpuName[i+1] == '3')){
					cpuName = cpuName.Substring(0, i+2) + "-" + cpuName.Substring(i+3);
					break;
				}
				
				i++;
			}
		}
		// scan for i7 names with letters ordered BEFORE the type name instead of after it
		if(cpuName.Contains ("i7-") || cpuName.Contains ("i5-") || cpuName.Contains ("i3-")){
			i = 0;
			while(i < cpuName.Length){
				// first fix stray spaces/empty characters
				if(cpuName[i] == 'i' && cpuName[i + 2] == '-'){
					if(cpuName[i+3] == ' '){
						//print ("yo");
						cpuName = cpuName.Substring(0, i+3) + cpuName.Substring(i+4);
						i --;
						
					}
					if(cpuName[i+4] == ' ')
						cpuName = cpuName.Substring(0, i+4) + cpuName.Substring(i+5);
					// cool now scan if there's a stray letter, too
					if(StringFunction.isNumber(cpuName[i+3]) == false){
						// cool, stray letter
						//print ("stray letter detected");
						tester = i; // backup value
						//keep on looking until we find another space
						while(i < cpuName.Length){
							if(cpuName[i] == ' '){
								// first part is name until i-7..
								// second part is the type number
								// final part is the letter
								cpuName = cpuName.Substring(0, tester + 2) + cpuName.Substring(tester +4, i - tester) + cpuName[i+3];
								break;
							}
							i ++;
						}
						if(i >= cpuName.Length) // there are no more spaces, so assume the type number until the end of the string
							cpuName = cpuName.Substring(0, tester+3) + cpuName.Substring(tester+4) + cpuName[tester+3];
					}
					break;
				}
				i++;
			}
		}

		// GPU name
		gpuName = SystemInfo.graphicsDeviceName.ToLower();
		if (gpuName.Contains ("emulated")) // emulated GPU
			gpuName = "Software Renderer";
		else {
			// Removes the ()'s
			i = 0;
			while (i < gpuName.Length) {
				if (gpuName [i] == '(') { 
					tester = i;
					while (tester < gpuName.Length) {
						if (gpuName [tester] == ')') {
							gpuName = gpuName.Substring (0, i) + gpuName.Substring (tester + 1);
							break;
						}
						tester ++;
					}
				}
				i++;
			}
			while (gpuName.Substring(gpuName.Length - 1, 1) == " ") { // remove space at the end
				gpuName = gpuName.Substring (0, gpuName.Length - 1);
			}
			// convert to title casing
			gpuName = textInfo.ToTitleCase (gpuName);
			// TODO: restore HD and GTX and R# (R5, R7, R9 et cetera) if missing
		}
		if(gpuName.Substring(gpuName.Length - 3) == " Mp") // catches Mali 400 MP GPU chips
			gpuName = gpuName.Substring(0, gpuName.Length -3) + " MP";

		// grab shader model version
		shaderModel = SystemInfo.graphicsShaderLevel * 1F / 10F;

		//D-ram
		if(SystemInfo.systemMemorySize <= 1024) // if you have less than one GB of DRam show it in MBs
			dram = SystemInfo.systemMemorySize + "mb";
		else // if you have more than one gb, show in GBs instead
			dram = Mathf.Floor(SystemInfo.systemMemorySize/102.4f)/10f + "gb";

		//V-RAM
		if(SystemInfo.graphicsMemorySize <= 1024) 
			vram = SystemInfo.graphicsMemorySize + "mb";
		else 
			vram = Mathf.Floor(SystemInfo.graphicsMemorySize/10.24f)/100f + "gb";

		// grab # of logical cores
		coreCount = Environment.ProcessorCount;
		if(coreCount == 0) // environment grab failed. Use internal fallback
			coreCount = SystemInfo.processorCount;;

		#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN // windows dependant
			if(cpuSpeed == null)  // internal name speed grab failed. Use registery instead. 
				cpuSpeed = ((int)Registry.GetValue("HKEY_LOCAL_MACHINE\\Hardware\\Description\\System\\CentralProcessor\\0", "~MHz", 0) / 1000f) + " GHz";

			motherBoardModel = (string)Registry.GetValue ("HKEY_LOCAL_MACHINE\\Hardware\\Description\\System\\BIOS", "BaseBoardProduct", null);
			// convert to lower case then capitalise the first letter of every word
			motherBoardModel = motherBoardModel.ToLower ();
			motherBoardModel = textInfo.ToTitleCase(motherBoardModel);

			platform = "Windows";
		#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX // osx dependant
			platform = "OSX";
		#elif UNITY_EDITOR_LINUX || UNITY_STANDALONE_LINUX // linux dependant
			platform = "Linux";
		#elif UNITY_ANDROID
			platform = "Android";

		// this grabs the cpu speed in case we don't already have it
		// www.kernel.org/doc/Documentation/cpu-freq/user-guide.txt
		filePath = "/sys/devices/system/cpu/cpu0/cpufreq/cpuinfo_max_freq";
		
		if(cpuSpeed == null && File.Exists(filePath)){
			content = File.ReadAllLines(filePath);
			
			int frequency;
			int.TryParse(content[0], out frequency);
			
			if(frequency > 1000){
				if(frequency < 1100000)
					cpuSpeed = (frequency / 1000f) + " Mhz";
				else
					cpuSpeed = (frequency / 1000000f) + " Ghz";
			}
		}

		// this gives access to the build information
		var clazz = AndroidJNI.FindClass("android.os.Build$VERSION");
		var fieldID = AndroidJNI.GetStaticFieldID(clazz, "SDK_INT", "I");
		android_sdk = AndroidJNI.GetStaticIntField(clazz, fieldID);
		
		clazz = AndroidJNI.FindClass("android.os.BatteryManager");
		fieldID = AndroidJNI.GetStaticFieldID(clazz, "BATTERY_PROPERTY_CURRENT_AVERAGE", "I");
		batteryCurrent = AndroidJNI.GetStaticIntField(clazz, fieldID).ToString();
		if(batteryCurrent != "0")
			batteryCurrent += " Amp/h";
		else
			batteryCurrent = null;
		
		clazz = AndroidJNI.FindClass("android.os.Build");
		fieldID = AndroidJNI.GetStaticFieldID(clazz, "BOARD", "Ljava/lang/String;");
		board = AndroidJNI.GetStaticStringField(clazz, fieldID);
		if(board == "unknown")
			board = null;
		
		fieldID = AndroidJNI.GetStaticFieldID(clazz, "BRAND", "Ljava/lang/String;");
		brand = AndroidJNI.GetStaticStringField(clazz, fieldID);
		
		fieldID = AndroidJNI.GetStaticFieldID(clazz, "DEVICE", "Ljava/lang/String;");
		device = AndroidJNI.GetStaticStringField(clazz, fieldID);
		
		fieldID = AndroidJNI.GetStaticFieldID(clazz, "MANUFACTURER", "Ljava/lang/String;");
		manufacturer = AndroidJNI.GetStaticStringField(clazz, fieldID);
		
		fieldID = AndroidJNI.GetStaticFieldID(clazz, "MODEL", "Ljava/lang/String;");
		model = AndroidJNI.GetStaticStringField(clazz, fieldID);
		
		fieldID = AndroidJNI.GetStaticFieldID(clazz, "PRODUCT", "Ljava/lang/String;");
		product = AndroidJNI.GetStaticStringField(clazz, fieldID);
		
		fieldID = AndroidJNI.GetStaticFieldID(clazz, "CPU_ABI", "Ljava/lang/String;");
		abi = AndroidJNI.GetStaticStringField(clazz, fieldID);
		
		fieldID = AndroidJNI.GetStaticFieldID(clazz, "SERIAL", "Ljava/lang/String;");
		serial = AndroidJNI.GetStaticStringField(clazz, fieldID);
		#endif
	}
}
