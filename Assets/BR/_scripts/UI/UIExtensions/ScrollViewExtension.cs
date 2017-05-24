// 
// Code by: Parth Darji
// Company: Boundless Reality
// (c) Boundless Reality, All rights reserved.
// 
// Details: This scrollview extension adds feature of handling hover states for handle bars
//

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using BR.BRUtilities;

namespace BR.BRUtilities.UI {
	[AddComponentMenu("UIExtensions/ScrollRectExtension")]

	public class ScrollViewExtension : ScrollRect, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IEndDragHandler {
		public Sprite idleSprite, hoverSprite, pressedSprite;
		private Image handleImage;

		protected override void Start() {
			Transform handle = UIHelper.FindDeepChild (this.transform, "Handle");
			if (handle != null)
				handleImage = handle.GetComponent<Image> ();
			else 
				Debug.Log ("No handle found");

			base.Start ();
		}

		public void OnPointerEnter(PointerEventData eventData) {
			if(handleImage != null)
				handleImage.sprite = hoverSprite;
		}

		public void OnPointerExit(PointerEventData eventData) {
			if(handleImage != null)
				handleImage.sprite = idleSprite;
		}

		public override void OnBeginDrag(PointerEventData eventData) {
            /*
			if(handleImage != null)
				handleImage.sprite = pressedSprite;

			base.OnBeginDrag (eventData);
            */
		}

		public override void OnEndDrag(PointerEventData eventData) {
            /*
			if(handleImage != null)
				handleImage.sprite = hoverSprite;

			base.OnEndDrag (eventData);
            */
		}

        public override void OnDrag(PointerEventData eventData)
        {
            
        }
    }
}
