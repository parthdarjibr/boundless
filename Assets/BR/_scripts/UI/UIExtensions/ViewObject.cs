//
//  Code by: Parth Darji
//  Company: Boundless Reality
//  (c) Boundless Reality, All rights reserved.
//
//  Details: This class handles all the views in a particular scene
// 			It helps in transition between various scenes within the app
//
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace BR.BRUtilities.UI {
	public class ViewObject
	{
		public ViewObject() {
			canvasStack = new Stack ();
			objectsinView = new List<GameObject> ();
		}

		public ViewObject (ObjectType type) {
			objectType = type;
			canvasStack = new Stack ();
			objectsinView = new List<GameObject> ();
		}

		public enum ObjectType
		{
			UI,
			VIDEO
		}

		public Stack canvasStack {
			get;
			private set;
		}

		public List<GameObject> objectsinView {
			get;
			private set;
		}

		public ObjectType objectType {
			get;
			private set;
		}

		public void AddToCanvasStack(CanvasObject obj) {
			canvasStack.Push (obj);
		}

		/// <summary>
		/// Adds gameobject to the list of gameobjects assosciated with this viewobject
		/// </summary>
		/// <param name="go">Go.</param>
		public void AddObjectToView(GameObject go) {
			objectsinView.Add (go);
		}

		public void SetObjectType(ObjectType type) {
			objectType = type;
		}

	}
}

