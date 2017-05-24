// 
// Code by: Parth Darji
// Company: Boundless Reality
// (c) Boundless Reality, All rights reserved.
// 
// Details: This extension is for the Influencer object on the
//			video object. This executes the pointer exit handler on the 
//			rooth transform whenever a button is clicked
//

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using BR.BRUtilities.UI;
using System.Net;

[AddComponentMenu("UIExtensions/ChildButtonExtension")]
public class ChildButtonExtension : Button  {

	/*
	public override void OnPointerClick (PointerEventData eventData)
	{
		// Send pointer exit to the root object
		// Find the outline object
		Transform outline = UIHelper.FindAncestor(this.transform, "VideoObject(Clone)");
		if (outline != null) {
			Debug.Log ("deselecting : " + outline.name);
			outline.GetComponent<HoverButtonExtension> ().OnDeselect (null);
		}
		//if(outline != null)
		//	ExecuteEvents.Execute (outline.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerExitHandler);

		base.OnPointerClick (eventData);

	}*/
}
