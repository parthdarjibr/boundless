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
using MA;

public class UserPanelManager : MonoBehaviour
{
	public GameObject loggedOutPanel, loggedInPanel;
	public UserProfileController profileController;
	public UIMenu LoginWindow;

	void Start() {
		profileController = UserProfileController.Instance ();
	}

	public void SetupUserPanel(bool loggedState) {
		if (loggedState == true) {
			// User is logged in
			loggedOutPanel.SetActive(false);
			loggedInPanel.SetActive (true);

			// setup the panel if it is not set already
			if (!profileController.isProfilePopulated || profileController.shouldUpdatePanel) {
				PopulateProfilePanel ();
			}
		} else {
			// Show unlogged state
			loggedInPanel.SetActive (false);
			loggedOutPanel.SetActive(true);
		}
	}

	/// <summary>
	/// Opens login view as an overlay when the signin button is clicked
	/// </summary>
	public void OpenLoginView() {
		 
	}

	/// <summary>
	/// Populates the user profile panel.
	/// </summary>
	public void PopulateProfilePanel() {
		// Populate the details in the profile panel

	}
}

