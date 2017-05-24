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
using UnityEngine.EventSystems;

namespace BR.BRUtilities.UI {
	public class CategoryButtonAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		#region VARIABLES

		public Image imageCategory;
		public Sprite idleSprite, hoverSprite, pressedSprite;
		public Sprite idleBorder, hoverBorder, pressedBorder;

		private Image imageBorder;

		#endregion

		#region UNITY MONOBEHAVIOUR METHODS

		void Start() {
			imageBorder = GetComponent<Image> ();
		}

		#endregion

		#region UNITY EVENTS

		// Send pointer events to the selectable child
		public void OnPointerEnter(PointerEventData pointer) {
			if (imageCategory != null && GetComponent<Button> ().interactable) {
				imageCategory.sprite = hoverSprite;
				imageBorder.sprite = hoverBorder;
			}
		}

		public void OnPointerExit(PointerEventData pointer) {
			if (imageCategory != null && GetComponent<Button> ().interactable) {
				imageCategory.sprite = idleSprite;
				imageBorder.sprite = idleBorder;
			}
		}

		#endregion

		#region PUBLIC METHODS

		public void SetInteractivity(bool interactable) {
			GetComponent<Button> ().interactable = interactable;
			GetComponent<AudioPlaybackManager> ().enabled = interactable;

			if (imageCategory != null) {
				imageCategory.sprite = interactable ? idleSprite : pressedSprite;
				imageBorder.sprite = interactable ? idleBorder : pressedBorder;
			}
		}

		#endregion
	}
}