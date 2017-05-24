using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeRotation : MonoBehaviour {

	public GameObject cube;

	// Use this for initialization
	void Start () {
		
	}
	
	void Update() {
		if(cube!=null)
			// rotate cube to see if main thread has been blocked;
			cube.transform.Rotate(Vector3.up, Time.deltaTime * 180);
	}
}
