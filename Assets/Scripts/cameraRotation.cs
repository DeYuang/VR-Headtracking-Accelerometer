using UnityEngine;
using System.Collections;

public class cameraRotation : MonoBehaviour {

	private	Vector3		spawnPosition 		= Vector3.zero;
	private	Vector3		upVector			= Vector3.zero;
	public	float		neckLength			= 0.2f;

	void Start(){

		spawnPosition = new Vector3(transform.position.x, transform.position.y - neckLength, transform.position.z);
		upVector = new Vector3 (0f, neckLength, 0f);
	}

	void Update () {

		if(AccelerometerTest.trackingModeInternal == TrackingMode.gyroscope)
			transform.eulerAngles = AccelerometerTest.downVector;
		else{
			if(AccelerometerTest.trackingModeInternal == TrackingMode.DualAccelerometer){
				if(AccelerometerTest.faceUp)
					transform.eulerAngles = AccelerometerTest.downVector;
				else
					transform.eulerAngles = AccelerometerTest.upVector;

			}
			else if(AccelerometerTest.trackingModeInternal == TrackingMode.TrippleAccelerometer)
				transform.eulerAngles = AccelerometerTest.downVector;

			if(AccelerometerTest.useCompass && stats.hasCompass)
				transform.eulerAngles += Vector3.up * Input.compass.trueHeading;

			transform.eulerAngles += new Vector3(0f, AccelerometerTest.jerkVector, 0f);
		}

		transform.position = spawnPosition + (transform.rotation * upVector);
	}
}
