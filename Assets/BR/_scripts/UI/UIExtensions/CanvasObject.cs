using UnityEngine;
using UnityEngine.UI;
using System;
using MA;
using UnityEngine.Events;

namespace BR.BRUtilities.UI {
	public class CanvasObject : MonoBehaviour {
		public enum CanvasType
		{
			BASE,
			INFLUENCER,
			ERROR,
			VIDEOMENU,
			LOADINGWHEEL
		}

		[Tooltip("The amount the alpha of the UI elements changes per second")]
		public CanvasType canvasType;
		public int currentIndex;
		public GameObject parentObject;
		public bool setupOnLoad = false;
		public UnityAction actionOpen, actionClose;

		private UIMenu menu;
		private Canvas canvas;
		//private bool isFading = false;
		//private bool isVisible;
		//public float fadeSpeed;

		public void SetupViewObject(CanvasType t, int i, GameObject p, bool s) {
			menu = GetComponent<UIMenu> ();
			canvas = GetComponent<Canvas> ();
			canvasType = t;
			currentIndex = i;
			parentObject = p;
			setupOnLoad = s;
		}

		void Awake() {
			canvas = GetComponent<Canvas> ();
			menu = GetComponent<UIMenu> ();
		}

		public UIMenu GetMenu() {
			return menu;
		}

		public Canvas GetCanvas() {
			return canvas;
		}
		/*
		public void SetIsFading(bool val) {
			isFading = val;
		}

		public void SetIsVisible(bool val) {
			isVisible = val;
		}

		public bool GetIsFading() {
			return isFading;
		}

		public bool GetIsVisble() {
			return isVisible;
		}*/
	}
}
