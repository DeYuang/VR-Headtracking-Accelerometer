using UnityEngine;
using System.Collections;

public class VectorMath : MonoBehaviour {
	
	static public Vector3 FastForward (float angle) {
	
		float sin = Mathf.Pow(Mathf.Sin(angle * Mathf.Deg2Rad), 2);
		float cos = Mathf.Pow(Mathf.Cos(angle * Mathf.Deg2Rad), 2);

		if(angle <= 90f)
			return new Vector3(sin, 0f, cos);
		else if(angle >= 270f)
			return new Vector3(-sin, 0f, cos);
		else if(angle <= 180f)
			return new Vector3(sin, 0f, -cos);
		else
			return new Vector3(-sin, 0f, -cos);
	}

	static public Vector3 Inverse (Vector3 regular){

		return new Vector3(-regular.x, -regular.y, -regular.z);
	}
}
