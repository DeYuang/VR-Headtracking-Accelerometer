using UnityEngine;
using System.Collections;

public class MathFunction : MonoBehaviour {

	static public bool isEven(int number){

		if((Mathf.Round(number / 2F) * 2F) == number)
			return true;
		return false;
	}

	static public bool HalfChance(){

		if(Random.Range(0f, 1f) < 0.5F)
			return true;
		return false;
	}

	static public bool RandomChance(float chance){

		if(Random.Range(0f, 100f) < chance)
			return true;
		return false;
	}
}
