//
//  Code by: Parth Darji
//  Company: Boundless Reality
//  (c) Boundless Reality, All rights reserved.
//
//  Details:
//
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using BR.App;

namespace BR.BRUtilities.UI {
	public class AudioPlaybackManager : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
	{
		public enum ButtonType
		{
			INFLUENCER,
			VIDEO,
			INTERACTION
		}
		public ButtonType buttonType;

		private AudioSource audioSource;

		void Start() {
			AudioSource aud = AudioController.Instance ().audioSource;
			if (aud != null)
				audioSource = aud;
		}

		public void OnPointerEnter(PointerEventData pointer) {
			if (audioSource != null)
				audioSource.PlayOneShot (ApplicationController.Instance ().hoverAudioClip);
		}
			
		public void OnPointerClick(PointerEventData pointer) {
			if (audioSource != null) {
				switch (buttonType) {
				case ButtonType.INFLUENCER:
					audioSource.PlayOneShot (ApplicationController.Instance ().influencerClickAudioClip);
					break;
				case ButtonType.VIDEO:
					audioSource.PlayOneShot (ApplicationController.Instance ().videoClickAudioClip);
					break;
				}
			}
		}
		
	}
}
