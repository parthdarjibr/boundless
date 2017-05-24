using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class mouseposition : MonoBehaviour {

	Text debugText;
	public CurvedUIInputModule cuiInputModule;
	public static Vector3 swipeStartPos;

	// Use this for initialization
	void Start () {
		debugText = GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)) {
			swipeStartPos = Input.mousePosition;
		}
	}
}
