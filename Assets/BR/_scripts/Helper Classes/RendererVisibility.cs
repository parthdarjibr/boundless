using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RendererVisibility : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Image img = GetComponent<Image> ();
		img.onCullStateChanged.AddListener (OnCullChanged);
	}
	
	void OnCullChanged(bool newState) {
		Debug.Log ("Culling " + gameObject.name + " : " + newState);
	}
}
