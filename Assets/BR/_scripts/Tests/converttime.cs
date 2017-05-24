using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class converttime : MonoBehaviour {

	// Use this for initialization
	void Start () {
		var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Math.Round(1492701282424 / 1000d)).ToLocalTime();
		Debug.Log (dt.ToString ());

		var dt2 = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Math.Round(1492614124889 / 1000d)).ToLocalTime();
		Debug.Log (dt2.ToString ());


	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
