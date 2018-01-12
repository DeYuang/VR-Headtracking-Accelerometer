using UnityEngine;
using System.Collections;

public class VR_TrackAccelero : MonoBehaviour {

	public Axis		trackingAxis 			= Axis.None;
	public float	scale					= 1f;

	void Update () {

		if(trackingAxis == Axis.X)
			transform.localPosition = new Vector3(AccelerometerTest.acceleroSmooth.x * scale, 0f, 0f);
		else if(trackingAxis == Axis.Y)
			transform.localPosition = new Vector3(0f, AccelerometerTest.acceleroSmooth.y * scale, 0f);
	}
}
