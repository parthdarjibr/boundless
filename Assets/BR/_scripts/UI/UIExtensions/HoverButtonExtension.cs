// 
// Code by: Parth Darji
// Company: Boundless Reality
// (c) Boundless Reality, All rights reserved.
// 
// Details: An extension for the buttons to handle overlays on hover
// It is important that the object has a child called 'Overlay;
//

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using BR.BRUtilities.UI;
using System.Net;

[AddComponentMenu("UIExtensions/HoverButtonExtension")]
public class HoverButtonExtension : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler  {
	#region Hover Animation Variables

	public float restPosZ = 0;
	public float hoverPosZ = -50;
	public float restScale = 1;
	public float hoverScale = 1.1f;

	bool Zoomed = false;
	public float AnimTime = 6f;
	float finalPosZ = 0f, finalScale = 0f;
	public RectTransform rt;

	#endregion

	#region UI Variables

	GameObject overlayObject;

	#endregion

	void Start() {
		try {
			overlayObject = UIHelper.FindDeepChild (this.transform, "Overlay").gameObject;
			if (overlayObject != null)
				overlayObject.SetActive (false);
		} catch {
		}

	}

	void Update() {
		if (Zoomed) {
			finalPosZ = rt.anchoredPosition3D.z + (hoverPosZ - restPosZ) * Time.deltaTime * AnimTime;
			finalScale = rt.localScale.z + (hoverScale - restScale) * Time.deltaTime * AnimTime;
		} else {
			finalPosZ = rt.anchoredPosition3D.z - (hoverPosZ - restPosZ) * Time.deltaTime * AnimTime;
			finalScale = rt.localScale.z - (hoverScale - restScale) * Time.deltaTime * AnimTime;
		}

		rt.anchoredPosition3D = rt.anchoredPosition3D.ModifyZ(Mathf.Clamp (finalPosZ, hoverPosZ, restPosZ));

		float scale = Mathf.Clamp (finalScale, restScale, hoverScale);
		rt.localScale = new Vector3 (scale, scale, scale);
	}

	public void OnPointerEnter(PointerEventData eventData) {
		if (overlayObject != null) {
			Zoomed = true;
			overlayObject.SetActive (true);
		}
	}

	public void OnPointerExit(PointerEventData eventData) {
		// Set the eventsystem gameobject to null
		//EventSystem.current.SetSelectedGameObject(null);

		///Debug.Log (EventSystem.current.currentSelectedGameObject.name);

		if (overlayObject != null) {
			Zoomed = false;
			overlayObject.SetActive (false);
		}
	}

	public void OnPointerClick (PointerEventData eventData)
	{
		OnPointerExit (eventData);
	}
}
