using UnityEngine;
using System.Collections;

public enum		TrackingMode : int { UseBestAvailable, DualAccelerometer, TrippleAccelerometer, gyroscope, None};
public enum		Axis : int { None, X, Y, Z};

public class AccelerometerTest : MonoBehaviour {

	private			Vector3			acceleroCurr				= Vector3.zero;
	private			Vector3			acceleroLast				= Vector3.zero;
	private			Vector3			acceleroSecondLast			= Vector3.zero;

	private			Vector3			acceleroTemporalAverage		= Vector3.zero;

	static public	Vector3			accelero					= Vector3.zero;
	static public	Vector3			acceleroFiltered			= Vector3.zero;
	static public	Vector3			acceleroRaw					= Vector3.zero;
	static public	Vector3			acceleroSmooth				= Vector3.zero;

	static public	Vector3			downVector					= Vector3.zero;
	static public	Vector3			upVector					= Vector3.zero;
	static public	float			jerkVector					= 0f;
	public			float			jerkFactor					= 1f;
	public			float			jerkMax						= 45f;

	public			AnimationCurve	verticalAccelerationCurve	= new AnimationCurve();
		
	static public	bool			faceUp						= false;

	static public	TrackingMode 	trackingMode				= TrackingMode.UseBestAvailable;
	static public	TrackingMode 	trackingModeInternal 		= TrackingMode.DualAccelerometer;

	static public	bool			useCompass					= true;

	void Awake () {
	
		acceleroRaw = new Vector3(Input.acceleration.x, Input.acceleration.y, Input.acceleration.z);
		acceleroCurr = new Vector3(Mathf.Clamp(acceleroRaw.x, -1f, 1f), Mathf.Clamp(acceleroRaw.y, -1f, 1f), Mathf.Clamp(acceleroRaw.z, -1f, 1f));
		acceleroLast = acceleroCurr;
		acceleroSecondLast = acceleroCurr;
		acceleroTemporalAverage = acceleroRaw;
	}

	void Update(){

		// push down old readings
		acceleroSecondLast = acceleroLast;
		acceleroLast = acceleroRaw;
		// get new readings
		acceleroRaw = Input.acceleration;

		// Update Temporal Average
		acceleroTemporalAverage = (acceleroTemporalAverage + acceleroRaw) / 2f;

		/*////////////////////////*/
		///	Autoselect best mode ///
		/*////////////////////////*/

		if(trackingMode == TrackingMode.UseBestAvailable){
			if(SystemInfo.supportsGyroscope)
				trackingModeInternal = TrackingMode.gyroscope;
			else{
				if(acceleroRaw.z != acceleroTemporalAverage.z && acceleroRaw.z != acceleroSecondLast.z)
					trackingModeInternal = TrackingMode.TrippleAccelerometer;
				else
					trackingModeInternal = TrackingMode.DualAccelerometer;

				//if(Input.gyro.enabled)
				//	Input.gyro.enabled = false;
			}
		}
		else
			trackingModeInternal = trackingMode;

		/*///////////////////////////*/
		///	Accelerometer filtering	///
		/*///////////////////////////*/

		// zero the z axis if there is no third accelerometer axis
		if(trackingModeInternal == TrackingMode.DualAccelerometer)
			acceleroCurr = new Vector3(acceleroRaw.x, acceleroRaw.y, 0f);
		else
			acceleroCurr = acceleroRaw;

		// TODO: acceleration cancelation
		// The accelerometer reports acceleration - gravity
		// 2-4 = -2, -(-2)+2 = 4
		// therefore gravity = -(accelerometer)+acceleration

		// gravity on earth is 1G downwards (duh)
		// so the magnitude of the gravity is 1 or (-1?)
		// therefore 1 - accelerometer is the magnitude of the acceleration
		// TODO: can we use this to calculate what the third accelerometer axis should be when we only have 2?

		float accelerationMagnitude = Mathf.Abs (acceleroCurr.magnitude) - 1f;
		if(accelerationMagnitude > 0f){
			if(trackingModeInternal == TrackingMode.DualAccelerometer)
				acceleroCurr -= new Vector3(accelerationMagnitude/2f, accelerationMagnitude/2f, 0f);
			else
				acceleroCurr -= new Vector3(accelerationMagnitude/3f, accelerationMagnitude/3f, accelerationMagnitude/3f);
		}
		// this removes acceleration evenly but it's not all that fair
		// we should find any axis that's over 1 and removes from there first

		// This filters out headbob jerk on the headset
		/*if(Mathf.Abs(acceleroRaw.x) > 1.1f)
				acceleroCurr.x = 0f;
		if(Mathf.Abs(acceleroRaw.y) > 1.1f)
				acceleroCurr.y = 0f;
		if(Mathf.Abs(acceleroRaw.z) > 1.1f)
				acceleroCurr.z = 0f;*/

		// better headbob jerk filter
		/*if(Mathf.Abs(Mathf.Abs (acceleroCurr.x) -
		             (Mathf.Abs(acceleroLast.x) + Mathf.Abs(acceleroSecondLast.x)) / 2f) > 0.1f)
			acceleroCurr.x = (acceleroSecondLast.x - acceleroLast.x + acceleroSecondLast.x);
		else if(Mathf.Abs(acceleroRaw.x) > 1.25f)
				acceleroCurr.x = 0f;

		if(Mathf.Abs(Mathf.Abs (acceleroCurr.y) -
		             (Mathf.Abs(acceleroLast.y) + Mathf.Abs(acceleroSecondLast.y)) / 2f) > 0.1f)
			acceleroCurr.y = (acceleroSecondLast.y - acceleroLast.y + acceleroSecondLast.y);
		else if(Mathf.Abs(acceleroRaw.y) > 1.25f)
				acceleroCurr.y = 0f;

		if(trackingModeInternal == TrackingMode.trippleAccelerometer){
			if(Mathf.Abs(Mathf.Abs (acceleroCurr.z) -
			             (Mathf.Abs(acceleroLast.z) + Mathf.Abs(acceleroSecondLast.z)) / 2f) > 0.1f)
				acceleroCurr.z = (acceleroSecondLast.z - acceleroLast.z + acceleroSecondLast.z);
			else if(Mathf.Abs(acceleroRaw.z) > 1.25f)
				acceleroCurr.z = 0f;
		}*/

		// NOTE: this filters out the jerky movement when you put the headset down
		// and autoswitches the down and up vectors
		if(trackingModeInternal == TrackingMode.DualAccelerometer){
			if(Mathf.Abs (acceleroCurr.y) < 0.08f &&
			   Mathf.Abs (acceleroCurr.x) < 0.08f){

				acceleroRaw = new Vector3(0f, 0f, acceleroRaw.z);
				acceleroCurr = Vector3.zero;
				faceUp = true;

				downVector = upVector = new Vector3(90f, 0f, 0f);
				//upVector = new Vector3(270f, 0f, 0f);
				return;
			}
			else{
				if(Mathf.Abs (acceleroCurr.y) > 0.95f)
					faceUp = false;
			//	else if(Mathf.Abs (acceleroCurr.y) < 0.3f)
			//		faceUp = true;
			}
		}

		// This stablizes the tracking when you put the headset down and you have a 3d accelerometer
		else if(trackingModeInternal == TrackingMode.TrippleAccelerometer 
		        && acceleroCurr.z < -0.95f){ // && acceleroCurr.z > -1.05f){

			acceleroCurr = Vector3.forward;
			faceUp = true;

			downVector = upVector = new Vector3(90f, 0f, 0f);
			return;
		}

		// filter out any remaining peaks
		acceleroCurr = new Vector3(Mathf.Clamp(acceleroCurr.x, -1f, 1f), Mathf.Clamp(acceleroCurr.y, -1f, 1f), Mathf.Clamp(acceleroCurr.z, -1f, 1f));

		/*///////////////////////////////*/
		///	Accelerometer interpolation	///
		/*///////////////////////////////*/

		// interpolated readings
		Vector3 interp = (acceleroSecondLast + acceleroCurr) / 2f; // -1 frames

		// extrapolated readings
		Vector3 extrap1 = (acceleroLast - acceleroSecondLast) + acceleroLast; // +0 frames
		Vector3 extrap2 = (acceleroCurr - acceleroLast) + acceleroCurr; // +1 frames

		/*///////////////////////*/
		///	Combined readings	///
		/*///////////////////////*/

		// (+0 frames)+(-1 frames)+(+1 frames)= +0 frames
		accelero = (acceleroCurr + acceleroLast + extrap2) / 3f;

		// (+0 frames)+(-1 frames)+(-2 frames) = -3 frames / 3 = -1 frames
		acceleroSmooth = (acceleroCurr + acceleroLast + acceleroSecondLast) / 3f;

		// (+0 frames)+(-1 frames)+(-2 frames)+(+3 frames) = +0 frames
		//acceleroSmooth = (acceleroCurr + acceleroLast + acceleroSecondLast + (extrap2*3f)) / 6f;

		// (+0 frames)+(-1 frames)+(-2 frames)+(-1frames)+(+0 frames)+(+1 frames) = -3 frames / 6 = -0.5 frames
		acceleroFiltered = (acceleroCurr + acceleroLast + acceleroSecondLast + interp + extrap1 + extrap2) / 6f;

		/*///////////////////////////*/
		///	Downvector calculation	///
		/*///////////////////////////*/

		if(trackingModeInternal == TrackingMode.DualAccelerometer){
			// lower 90 degrees
			downVector = new Vector3(90f - (verticalAccelerationCurve.Evaluate(Mathf.Abs(acceleroSmooth.magnitude)) * -90f), 
			                         0f, 
			                         (Mathf.Atan2(acceleroSmooth.x, acceleroSmooth.y) * Mathf.Rad2Deg) + 180f);

			// upper 90 degrees
			upVector = new Vector3(-90f - (verticalAccelerationCurve.Evaluate(Mathf.Abs(acceleroSmooth.magnitude)) * -90f), 
			                       0f, 
			                       (Mathf.Atan2(acceleroSmooth.x, acceleroSmooth.y) * Mathf.Rad2Deg) + 180f);

			//downVector = new Vector3(0f, 0f, Mathf.Atan2(extrap2.x, extrap2.y) * Mathf.Rad2Deg + 180f);
			//upVector = downVector;
		}
		else if(trackingModeInternal == TrackingMode.TrippleAccelerometer){
			// new style
			downVector = new Vector3(Mathf.Atan2(acceleroSmooth.z, acceleroSmooth.y) * Mathf.Rad2Deg + 180f, 
			                         0f, 
			                         -Mathf.Atan2(acceleroSmooth.x, Mathf.Sqrt(acceleroSmooth.y*acceleroSmooth.y + acceleroSmooth.z*acceleroSmooth.z)) * Mathf.Rad2Deg);

			upVector = downVector;
		}		
		else if(trackingModeInternal == TrackingMode.gyroscope){
			useCompass = false;
			downVector = Input.gyro.attitude.eulerAngles;
			upVector = downVector;
		}
		else{
			useCompass = false;
			downVector = Vector3.zero;
			upVector = Vector3.zero;
		}

		/*///////////////////////////*/
		///	Headset jerk detection	///
		/*///////////////////////////*/

		//compensate the rotation by detecting jerking motions

		if(Mathf.Abs(acceleroRaw.magnitude) > 1f && Mathf.Abs (acceleroRaw.x) > Mathf.Abs (acceleroRaw.y)){
			jerkVector = Mathf.Clamp(jerkVector + (acceleroRaw.x * jerkFactor), -jerkMax, jerkMax);
		}
		else
			jerkVector = Mathf.Lerp(jerkVector, 0f, jerkFactor * Time.deltaTime);

		/*jerkVector = Mathf.Clamp(jerkVector + (acceleroRaw.x -((acceleroLast.x + acceleroSecondLast.x) / 2f) * jerkFactor), -jerkMax, jerkMax);

		if(acceleroRaw.x -
		   ((acceleroLast.x + acceleroSecondLast.x) / 2f) > 0.3f)
			jerkVector = Mathf.Clamp(jerkVector + (acceleroRaw.x - 0.3f) * jerkFactor, -jerkMax, jerkMax);

		else if(acceleroRaw.x -
		   ((acceleroLast.x + acceleroSecondLast.x) / 2f) < -0.3f)
			jerkVector = Mathf.Clamp(jerkVector + (acceleroRaw.x + 0.3f) * jerkFactor, -jerkMax, jerkMax);

		else
			jerkVector = Mathf.Lerp(jerkVector, 0f, jerkFactor * Time.deltaTime);*/
	}
}
