using UnityEngine;
using System.Collections;

public class VR_Buttons : MonoBehaviour {

	public 	GameObject	camera			= null;
	public	GameObject	sphere2D		= null;
	public	GameObject	sphere3D		= null;

	public void CameraSphereProjection(){

		if(sphere2D.activeInHierarchy)
			sphere2D.SetActive(false);
		if(sphere3D.activeInHierarchy)
			sphere3D.SetActive(false);

		if(!camera.activeInHierarchy)
			camera.SetActive(true);
	}

	public void SphereProjection2D(){

		if(camera.activeInHierarchy)
			camera.SetActive(false);
		if(sphere3D.activeInHierarchy)
			sphere3D.SetActive(false);

		if(!sphere2D.activeInHierarchy)
			sphere2D.SetActive(true);
	}

	public void SphereProjection3D(){

		if(sphere2D.activeInHierarchy)
			sphere2D.SetActive(false);
		if(camera.activeInHierarchy)
			camera.SetActive(false);

		if(!sphere3D.activeInHierarchy)
			sphere3D.SetActive(true);
	}
}
