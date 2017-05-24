// 
// Code by: Parth Darji
// Company: Boundless Reality
// (c) Boundless Reality, All rights reserved.
// 
// Details: An extension for the panels to handle overlays on hover
// It is important that the object has a child called 'Overlay;
//

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using BR.BRUtilities;
using System.Net;

namespace BR.BRUtilities.UI {
	[AddComponentMenu("UIExtensions/PanelExtension")]
	public class VideoObjectImageExtension : Image, IPointerEnterHandler, IPointerExitHandler  {
		#region Hover Animation Variables

		public float restPosZ = 0;
		public float hoverPosZ = -50;
		public float restScale = 1;
		public float hoverScale = 1.1f;

		bool Zoomed = false;
		public float AnimTime = 6f;

		#endregion

		#region UI Variables

		GameObject overlayObject;

		#endregion

		protected override void Start() {
			overlayObject = UIHelper.FindDeepChild (this.transform, "Overlay").gameObject;
			if (overlayObject != null)
				overlayObject.SetActive (false);

			base.Start ();
		}

		void Update() {
			float finalPosZ = 0f, finalScale = 0f;
			RectTransform rt = transform as RectTransform;
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

			/*
			if (currentState == State.DOWNLOADED) {
				tex.LoadImage (bytes);
				Sprite spr = Sprite.Create (tex, new Rect (0, 0, tex.width, tex.height), new Vector2 (0.5f, 0.5f));
				sprite = spr;

				currentState = State.IDLE;
			}*/
		}

		public void OnPointerEnter(PointerEventData eventData) {
			if (overlayObject != null) {
				Zoomed = true;
				overlayObject.SetActive (true);
			}
		}

		public void OnPointerExit(PointerEventData eventData) {
			if (overlayObject != null) {
				Zoomed = false;
				overlayObject.SetActive (false);
			}
		}
	}
}
