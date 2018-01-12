using UnityEngine;
using System.Collections;

public class StringFunction : MonoBehaviour {

	static public bool isNumber(char i){

		switch(i){
			case '1': case '2': case '3': case '4': case '5': case '6': case '7': case '8': case '9': case '0':
				return true;
			default:
				return false;
		}
	}

	static public string FilterString(string input){
		
		int i = 0;
		string output = input;
		
		while(i < output.Length){
			if(output[i] == '('){ 
				if(output[i+2] == ')')// removes (r)
					output = output.Substring(0, i) + output.Substring (i+3);
				else if(output[i+3] == ')') // removes (tm)
					output = output.Substring(0, i) + output.Substring (i+4);
			}
			
			i ++;
		}
		
		return output;
	}
}
