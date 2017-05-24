//
//  Code by: Parth Darji
//  Company: Boundless Reality
//  (c) Boundless Reality, All rights reserved.
//
//  Details:
//
using UnityEngine;
using System.Collections;

public class ScrollObject
{

	public GameObject scrollGo;
	public bool shouldScroll;

	ScrollObject() {
		scrollGo = null;
		shouldScroll = false;
	}

	ScrollObject(GameObject go, bool _shouldScroll) {
		scrollGo = go;
		shouldScroll = _shouldScroll;
	}

	public bool isEmpty() {
		return scrollGo == null;
	}
}

