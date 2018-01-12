using UnityEngine;
using System.Collections;

public class Landscape : MonoBehaviour {
	
	void Awake () {
	
		QualitySettings.vSyncCount = 0;
		QualitySettings.maxQueuedFrames = 0;
		QualitySettings.antiAliasing = 0;
		Application.targetFrameRate = -1;

		Screen.autorotateToLandscapeLeft = true;
		Screen.autorotateToLandscapeRight = true;
		Screen.autorotateToPortrait = false;
		Screen.autorotateToPortraitUpsideDown = false;
		Screen.orientation = ScreenOrientation.LandscapeLeft;
	}
}
