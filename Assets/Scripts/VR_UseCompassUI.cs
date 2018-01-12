using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class VR_UseCompassUI : MonoBehaviour {
	
	public void OnClick () {
	
		AccelerometerTest.useCompass = !AccelerometerTest.useCompass;
		if(AccelerometerTest.useCompass && AccelerometerTest.trackingMode == TrackingMode.gyroscope)
			AccelerometerTest.trackingMode = TrackingMode.TrippleAccelerometer;
	}

	void Awake(){

		GetComponent<Toggle>().isOn = AccelerometerTest.useCompass;
	}

	void LateUpdate(){

		if(stats.hasCompass == false){
			AccelerometerTest.useCompass = false;
			GetComponent<Toggle>().interactable = false;
			GetComponent<Toggle>().isOn = false;
			this.enabled = false;
		}
	}
}
