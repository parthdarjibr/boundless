//
//  Code by: Parth Darji
//  Company: Boundless Reality
//  (c) Boundless Reality, All rights reserved.
//
//  Details:
//
using UnityEngine;
using System.Collections;

public class PageScroller : MonoBehaviour
{
	public ScrollObject currentScrollObject;

	void Start ()
	{
		// Instantiate OVRTouchpad and listen to events
		OVRTouchpad.Create();
		OVRTouchpad.TouchHandler += OVRTouchpad_TouchHandler; 
	}

	void OVRTouchpad_TouchHandler (object sender, System.EventArgs e)
	{
		if (currentScrollObject != null && !currentScrollObject.isEmpty ()) {
			OVRTouchpad.TouchArgs ta = (OVRTouchpad.TouchArgs)e;

			switch (ta.TouchType) {
			case OVRTouchpad.TouchEvent.Up:
				
				Debug.Log ("UP");
				break;
			case OVRTouchpad.TouchEvent.Down:
				Debug.Log ("DOWN");
				break;
			case OVRTouchpad.TouchEvent.Left:
				break;
			case OVRTouchpad.TouchEvent.Right:
				break;
			}
		}
	}

	void OnDisable() {
		OVRTouchpad.TouchHandler -= OVRTouchpad_TouchHandler;
	}
}

