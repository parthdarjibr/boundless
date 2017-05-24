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

public class UserProfileController : MonoBehaviour
{

	#region INSTANTIATION

	private static UserProfileController _instance;

	private static bool Exists() {
		return _instance != null;
	}

	public static UserProfileController Instance() {
		if (!Exists ()) {
			throw new Exception ("UserPanelController object not found");
		}
		return _instance;
	}

	void Awake() {
		if (_instance == null)
			_instance = this;
		
		DontDestroyOnLoad (this.gameObject);
	}

	#endregion

	#region VARIABLES

	public static bool isLoggedIn = false;
	public UserPanelManager userPanalManager;
	public bool isProfilePopulated = false;
	public bool shouldUpdatePanel = true;

	#endregion

	#region UNITY MONO METHODS



	#endregion

	#region PUBLIC METHODS


	#endregion
}

