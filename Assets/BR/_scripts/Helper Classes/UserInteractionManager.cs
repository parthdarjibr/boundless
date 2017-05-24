//
//  Code by: Parth Darji
//  Company: Boundless Reality
//  (c) Boundless Reality, All rights reserved.
//
//  Details: This script manages the user interaction with the application
//			It manages the stack of views for interaction
//	
//

using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;

namespace BR.BRUtilities {
	public class UserInteractionManager : MonoBehaviour {

		private static UserInteractionManager _instance = null;

		private Stack viewStack = new Stack();

		private
		#region UNITY MONOBEHAVIOUR METHODS

		void Awake() {
			if (_instance == null) {
				_instance = this;
				DontDestroyOnLoad(this.gameObject);
			}
		}

		void OnDestroy() {
			_instance = null;
		}

		#endregion

		#region PUBLIC METHODS

		public static bool Exists() {
			return _instance != null;
		}

		public bool isInteractable {
			get;
			private set;
		}

		public static UserInteractionManager Instance() {
			if (!Exists ()) {
				throw new Exception ("UserInteractionManager could not find the UnityMainThreadDispatcher object. Please ensure you have added the UserInteractionManager Prefab to your scene.");
			}
			return _instance;
		}

		#endregion

		#region PRIVATE METHODS

		public void SetInteractableState(bool state) {
			EventSystem.current.enabled = state;
		}

		public void AddViewToStack(GameObject go) {
			// Check if the gameobject is a view
			if (go.GetComponent<CanvasRenderer> () == null) {
				Debug.LogError ("Error adding to stack: Not a view");
				return;
			} else {
				viewStack.Push (go);
			}
		}

		public GameObject GetFirstView() {
			return (GameObject)viewStack.Peek ();
		}

		public void RemoveViewFromStack(GameObject go) {
			viewStack.Pop ();
		}

		#endregion
	}
}
