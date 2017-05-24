using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomRayController : MonoBehaviour {

	public Transform eventCamera;
	
	// Update is called once per frame
	void Update () {
		//pass ray to canvas
		Ray myRay = new Ray(eventCamera.position, eventCamera.forward);

		CurvedUIInputModule.CustomControllerRay = myRay;
		CurvedUIInputModule.CustromControllerButtonDown = Input.GetKey (KeyCode.None);
	}
}
