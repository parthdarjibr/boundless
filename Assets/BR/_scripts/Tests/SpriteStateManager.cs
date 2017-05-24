//
//  Code by: Parth Darji
//  Company: Boundless Reality
//  (c) Boundless Reality, All rights reserved.
//
//  Details:
//
using UnityEngine;
using UnityEngine.EventSystems;

public class SpriteStateManager : MonoBehaviour
{
	void OnEnable() {
// 		Debug.Log ("OnEnable");
//		Debug.Log (EventSystem.current.name);
		ExecuteEvents.Execute (gameObject, new PointerEventData (EventSystem.current), ExecuteEvents.pointerExitHandler);
	}

	void OnDisable() {
		// Debug.Log ("OnDisable");
		// ExecuteEvents.Execute (gameObject, new PointerEventData (EventSystem.current), ExecuteEvents.pointerExitHandler);
	}
}

