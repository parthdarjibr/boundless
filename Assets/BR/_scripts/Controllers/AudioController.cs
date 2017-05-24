//
//  Code by: Parth Darji
//  Company: Boundless Reality
//  (c) Boundless Reality, All rights reserved.
//
//  Details:
//

using System;
using UnityEngine;
using System.Collections;

namespace BR.App {
	public class AudioController : MonoBehaviour
	{
		#region INITIALIZATION

		private static AudioController instance;

		private static bool Exists() {
			return instance != null;
		}

		public static AudioController Instance() {
			if (!Exists ()) {
				throw new Exception ("AudioController object not found");
			}
			return instance;
		}

		void Awake() {
			if (!Exists ())
				instance = this;

			DontDestroyOnLoad (this.gameObject);
			audioSource = GetComponent<AudioSource> ();
		}

        private void OnDestroy()
        {
            instance = null;
        }
        #endregion

        #region VARIABLES

        public enum ButtonType {
			influencer,
			video,
			interaction
		}

		// public AudioClip hoverAudioClip, videoClickAudioClip, influencerClickAudioClip, interactionClickAudioClip;
		// public AudioClip videoBackClip, influencerBackClip;
		public AudioSource audioSource;

		#endregion

		#region PUBLIC METHODS

		public void PlayOneShot(AudioClip audioClip) {
			if(!audioSource.isPlaying)
				audioSource.PlayOneShot (audioClip);
		}

		#endregion
	}
}

