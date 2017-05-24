
//
//  Code by: Parth Darji
//  Company: Boundless Reality
//  (c) Boundless Reality, All rights reserved.
//
//  Details:
//
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using CurvedUI;

public class CUIScroller : MonoBehaviour
{
	int scrollDirection;
	bool scrollEnabled = true;
	ScrollRect scrollRect;

	public float scrollRange;
	public float scrollPower = 60;
	public CurvedUIRaycaster myCanvas;
	public bool isPointerOnObject = false;

	private bool shouldTransition = false;
	Vector2 newPos, oldPos;
	public float scrollSpeed = 3f;

	void Awake ()
	{
		// Start listening to OVR Events
		OVRTouchpad.Create();
		OVRTouchpad.TouchHandler += OVRTouchpad_TouchHandler;

		scrollRect = GetComponent<ScrollRect> ();
	}

	void OVRTouchpad_TouchHandler (object sender, System.EventArgs e)
	{
		OVRTouchpad.TouchArgs ta = (OVRTouchpad.TouchArgs)e;

		switch (ta.TouchType) {
		case OVRTouchpad.TouchEvent.Up:
			oldPos = scrollRect.content.anchoredPosition;
			newPos = new Vector2 (scrollRect.content.anchoredPosition.x, scrollRect.content.anchoredPosition.y + 220);
			shouldTransition = true;
			Debug.Log ("UP");
			break;
		case OVRTouchpad.TouchEvent.Down:
			newPos = new Vector2(scrollRect.content.anchoredPosition.x, scrollRect.content.anchoredPosition.y - 220);
			shouldTransition = true;
			Debug.Log ("DOWN");
			break;
		case OVRTouchpad.TouchEvent.Left:
			break;
		case OVRTouchpad.TouchEvent.Right:
			break;
		}
	}

	public void SetEnabled(bool enabled)
	{
		scrollEnabled = enabled;
	}

	void OnEnable()
	{
		scrollDirection = 0;
	}

	void OnDisable() {
		OVRTouchpad.TouchHandler -= OVRTouchpad_TouchHandler;
	}

	void RefreshContentSize()
	{
		float scrollRectHeight = GetComponent<RectTransform>().rect.height;
		float contentRectHeight = scrollRect.content.GetComponent<RectTransform>().rect.height;
		if (contentRectHeight != 0)
		{
			scrollRange = contentRectHeight - scrollRectHeight;
		}
	}

	public void GoToTop() {
		if (scrollRect != null)
			scrollRect.verticalNormalizedPosition = 1;
	}

	void Update() {
		if (isPointerInsideRect () && shouldTransition) {
			scrollRect.content.anchoredPosition = Vector2.Lerp (scrollRect.content.anchoredPosition, newPos, Time.deltaTime * scrollSpeed);
		}

		if (Vector2.Distance(scrollRect.content.anchoredPosition, newPos) < 0.005f)
		{
			shouldTransition = false;
		}


	}

	public bool isPointerInsideRect() {
		if(myCanvas.GetObjectsUnderPointer().Contains(this.gameObject)) {
			isPointerOnObject = true;
			return true;
		}
		isPointerOnObject = false;
		return false;
	}
}

