using UnityEngine;
using System.Collections;

public class UI_SensorOverlay : MonoBehaviour {

	public GameObject	Overlay			= null;

	public void onClick(){

		Overlay.SetActive(!Overlay.activeInHierarchy);
	}
}
