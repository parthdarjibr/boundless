// 
// Code by: Parth Darji
// Company: Boundless Reality
// (c) Boundless Reality, All rights reserved.
// 
// Details: This class works as a helper class for various ui operations
// 

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using CurvedUI;

namespace BR.BRUtilities.UI {
	public static class UIHelper
	{
		/// <summary>
		/// Finds the deep child.
		/// </summary>
		/// <returns>Finds a deep child of the transform.</returns>
		/// <param name="parent">Parent transform.</param>
		/// <param name="name">Name of child</param>
		public static Transform FindDeepChild(Transform parent, string name) {
			foreach (Transform child in parent) {
				if (child.name == name)
					return child;
				Transform result = FindDeepChild (child, name);
				if (result != null)
					return result;
			}
			return null;
		}

		/// <summary>
		/// Modifies the z value of a vector.
		/// </summary>
		/// <returns>The z.</returns>
		/// <param name="trans">Trans.</param>
		/// <param name="newVal">New value.</param>
		public static Vector3 ModifyZ(this Vector3 trans, float newVal)
		{
			trans = new Vector3(trans.x, trans.y, newVal);
			return trans;
		}

		/// <summary>
		/// Finds the ancestor.
		/// </summary>
		/// <returns>The ancestor.</returns>
		/// <param name="child">Child transform.</param>
		/// <param name="name">Name of parent.</param>
		public static Transform FindAncestor(Transform child, string name) {
			while (child != null)
			{
				if (child.name == name)
				{
					return child;
				}
				child = child.parent.transform;
			}
			return null; // Could not find a parent with given tag.
		}

		public static List<Camera> GetAllMainCameras()
		{
			List<Camera> mainCameras = new List<Camera> ();
			if (Camera.main != null)
			{

				Camera[] cameras = Camera.allCameras;
				foreach (Camera cam in cameras)
				{
					// If the camera is not the main camera and doesn't have a "MainCamera" tag, remove it from list
					if (cam == Camera.main || cam.CompareTag("MainCamera"))
					{
						mainCameras.Add (cam);
					}
				}
			}
			return mainCameras;
		}

		/// <summary>
		/// For RectTransform, calculate it's normal in world space
		/// </summary>
		public static Vector3 GetRectTransformNormal(RectTransform rectTransform, CurvedUISettings s = null)
		{
			if (s == null) {
				Vector3[] corners = new Vector3[4];
				rectTransform.GetWorldCorners (corners);
				Vector3 BottomEdge = corners [3] - corners [0];
				Vector3 LeftEdge = corners [1] - corners [0];
				rectTransform.GetWorldCorners (corners);
				return Vector3.Cross (BottomEdge, LeftEdge).normalized;
			} else {
				//find the position in canvas space
				Vector3 pos = rectTransform.localPosition;
				return s.CanvasToCurvedCanvasNormal (pos);
			}
		}

		/// <summary>
		/// Determine the signed angle between two vectors, with normal 'n'
		/// as the rotation axis.
		/// </summary>
		public static float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n)
		{
			return Mathf.Atan2(
				Vector3.Dot(n, Vector3.Cross(v1, v2)),
				Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
		}

	}
}

