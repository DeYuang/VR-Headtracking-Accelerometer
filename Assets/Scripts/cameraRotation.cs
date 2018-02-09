using UnityEngine;
using System.Collections;

public class cameraRotation : MonoBehaviour {

	private	Vector3		spawnPosition 		= Vector3.zero;
	private	Vector3		upVector			= Vector3.zero;
	public	float		neckLength			= 1.5f;

	void Start(){

		spawnPosition = new Vector3(transform.position.x, transform.position.y - neckLength, transform.position.z);
		upVector = new Vector3 (0.0f, neckLength, 0.0f);
	}

	void Update () {

		TrackingMode trackingMode = AccelerometerTest.trackingModeInternal;

		if(trackingMode == TrackingMode.None)
			return;

		if(trackingMode == TrackingMode.gyroscope)
			transform.eulerAngles = AccelerometerTest.downVector;
		else{
			Vector3 rotation = Vector3.zero;
			if(trackingMode == TrackingMode.DualAccelerometer){
				if(AccelerometerTest.faceUp)
					rotation = AccelerometerTest.downVector;
				else
					rotation = AccelerometerTest.upVector;

			}
			else if(trackingMode == TrackingMode.TrippleAccelerometer)
				rotation = AccelerometerTest.downVector;

			if(AccelerometerTest.useCompass && stats.hasCompass)
				rotation += Vector3.up * Input.compass.trueHeading;

			rotation += new Vector3(0.0f, AccelerometerTest.jerkVector, 0.0f);

			transform.eulerAngles = rotation;
		}

		transform.position = spawnPosition + (transform.rotation * upVector);
	}
}
