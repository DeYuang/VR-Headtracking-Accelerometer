using UnityEngine;
using System.Collections;

public class VR_OptionToggle : MonoBehaviour {

	public			bool		active				= false;
	static public	GameObject	menu				= null;
	public			float		uiSpeed				= 1f;
	public			float		startPos			= 0f;
	public			float		endPos				= 0f;
	public			float		currPos				= 0f;

	void Awake () {

		if(startPos == endPos){
			startPos = transform.localPosition.y;
			endPos = 0f;
		}

		currPos = startPos;
		transform.localPosition = new Vector3(transform.localPosition.x, startPos, transform.localPosition.z);

		menu = gameObject;
		menu.SetActive(false);
	}

	void LateUpdate () {
	
		if(active){
			currPos = transform.localPosition.y;
			if(currPos != endPos){
				currPos = Mathf.Clamp(currPos + (uiSpeed * Time.unscaledDeltaTime), startPos, endPos);
				transform.localPosition = new Vector3(transform.localPosition.x, currPos, transform.localPosition.z);
			}
		}
		else{
			currPos = transform.localPosition.y;
			if(currPos != startPos){
				currPos = Mathf.Clamp(currPos - (uiSpeed * Time.unscaledDeltaTime), startPos, endPos);
				transform.localPosition = new Vector3(transform.localPosition.x, currPos, transform.localPosition.z);
			}
			else
				menu.SetActive(false);
		}
	}

	public void OnClick(){
		
		active = !active;
		if(active && menu.activeSelf == false)
			menu.SetActive(true);
	}
}
