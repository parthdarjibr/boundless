//
//  Code by: Parth Darji
//  Company: Boundless Reality
//  (c) Boundless Reality, All rights reserved.
//
//  Details:
//
using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

namespace BR.BRUtilities.UI {
	public class CanvasFadeManager : MonoBehaviour
	{
		/*
		#region EVENTS

		public delegate void OnFadeInComplete();
		public delegate void OnFadeOutComplete();

		public event OnFadeInComplete onFadeInComplete;
		public event OnFadeOutComplete onFadeOutComplete;

		#endregion

		#region INSTANTIATION

		private static CanvasFadeManager _instance = null;

		private static bool Exists() {
			return _instance != null;
		}

		public static CanvasFadeManager Instance() {
			if (!Exists ()) {
				throw new Exception ("CanvasFadeManager object not found");
			}
			return _instance;
		}

		#endregion

		#region UNITY MONO METHODS

		void Awake() {
			if (_instance == null) {
				_instance = this;
			}
			DontDestroyOnLoad (this.gameObject);
		}

		void OnDestroy() {
			_instance = null;
		}

		#endregion
	
		#region VARIABLES

		float maxAlpha = 1f;
		float minAlpha = 0f;

		#endregion

		public void PerformFadeIn(float alpha, ViewObject obj, float fadeSpeed = 1) {
			if(alpha > 1) alpha = 1;

			maxAlpha = alpha;
			obj.fadeSpeed = fadeSpeed;
			if(!obj.GetIsFading()) 
				StartCoroutine (FadeIn (obj));
		}

		public void PerformFadeOut(float alpha, ViewObject obj, float fadeSpeed = 1) {
			if (alpha < 0) alpha = 0;

			minAlpha = alpha;
			obj.fadeSpeed = fadeSpeed;

			if (!obj.GetIsFading())
				StartCoroutine (FadeOut (obj));
		}

		public void PerformForcedFadeIn(float alpha, ViewObject obj, float fadeSpeed = 1) {
			if (alpha > 1)
				alpha = 1;

			maxAlpha = alpha;
			obj.fadeSpeed = fadeSpeed;

			// Disable active fading for the object
			obj.SetIsFading(false);
			StopAllCoroutines ();

			StartCoroutine (FadeIn (obj));
		}

		public void PerformForcedFadeOut(float alpha, ViewObject obj, float fadeSpeed = 1) {
			if (alpha < 0)
				alpha = 0;

			minAlpha = alpha;
			obj.fadeSpeed = fadeSpeed;

			// Disable active fading
			obj.SetIsFading(false);
			StopAllCoroutines ();

			StartCoroutine (FadeOut (obj));
		}


		#region PRIVATE METHODS

		IEnumerator FadeIn(ViewObject obj) {
			float currentAlpha;
			obj.SetIsFading(true);
			CanvasGroup cg = obj.GetComponent<CanvasGroup> ();

			do {
				currentAlpha = maxAlpha;
				cg.alpha += obj.fadeSpeed * Time.deltaTime;

				// Update the alpha 
				if(cg.alpha < currentAlpha)
					currentAlpha = cg.alpha;

				// Wait for next frame
				yield return null;
			} while(currentAlpha < maxAlpha);

			// Raise events
			if (obj.actionOpen != null)
				obj.actionOpen ();

			// Update state machine
			obj.SetIsFading(false);
			obj.SetIsVisible(true);
		}

		IEnumerator FadeOut(ViewObject obj) {
			float currentAlpha;
			obj.SetIsFading(true);
			CanvasGroup cg = obj.GetComponent <CanvasGroup> ();

			do {
				currentAlpha = minAlpha;
				cg.alpha -= obj.fadeSpeed * Time.deltaTime;

				if (cg.alpha > currentAlpha)
					currentAlpha = cg.alpha;

				yield return null;

			} while (currentAlpha > minAlpha);

			// Raise events
			if (obj.actionClose != null) 
				obj.actionClose ();

			// Update state machine
			obj.SetIsFading(false);
			obj.SetIsVisible(false);
		}

		public void SetInvisible(ViewObject obj) {
			obj.GetComponent<CanvasGroup> ().alpha = 0;

			// Raise events
			if (obj.actionClose != null)
				obj.actionClose ();

			// Update state machine
			obj.SetIsVisible(false);
		}

		public void SetVisible(ViewObject obj) {
			obj.GetComponent<CanvasGroup> ().alpha = 1;

			// Raise events
			if (obj.actionOpen != null)
				obj.actionOpen ();

			// Update state machine
			obj.SetIsVisible(true);
		}

		#endregion
		*/
	}
}

