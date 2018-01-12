using UnityEngine;
using System.Collections;

public class UI_PlayLoop : MonoBehaviour {

	#if !UNITY_ANDROID
	public	MovieTexture 	movieTexture			= null;
	public	bool			loopMovie				= false;

	// Use this for initialization
	void Start () {
	
		movieTexture.loop = false;

		movieTexture.Stop ();
		movieTexture.Play ();
	}
	
	// Update is called once per frame
	void LateUpdate () {

		if(!movieTexture.isPlaying && loopMovie){
			movieTexture.Stop ();
			movieTexture.Play ();
		}
	}
	#endif
}
