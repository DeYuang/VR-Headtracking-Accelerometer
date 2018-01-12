using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class VR_TrackingModeUI : MonoBehaviour {

	public void OnClick(){

		AccelerometerTest.trackingMode = (TrackingMode)Mathf.Clamp(GetComponent<Dropdown>().value, 0, 3);

		if(AccelerometerTest.useCompass && 
		   AccelerometerTest.trackingMode == TrackingMode.gyroscope) {
			AccelerometerTest.useCompass = false;
		}
	}
}
