using UnityEngine;
using System.Collections;

public class VR_ToggleActiveState : MonoBehaviour {

	public	GameObject			target			= null;
	private	bool				active			= false;

	public void OnClick(){

		active = !active;
		target.SetActive(active);
	}

	void Awake () {
	
		active = target.activeInHierarchy;
	}
}
