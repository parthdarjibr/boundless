//
//  Code by: Parth Darji
//  Company: Boundless Reality
//  (c) Boundless Reality, All rights reserved.
//
//  Details:
//
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.NetworkInformation;
using UnityEngine.SceneManagement;

namespace BR.App {
	public class ConnectionManager : MonoBehaviour
	{

		#region INITIALIZATION

		private static ConnectionManager instance;

		private static bool Exists() {
			return instance != null;
		}

		public static ConnectionManager Instance() {
			if (!Exists ()) {
				throw new Exception ("ConnectionManager object not found");
			}
			return instance;
		}

		void Awake() {
			if (!Exists ()) {
				instance = this;
			}

			// DontDestroyOnLoad (this.gameObject);
			NetworkChange.NetworkAvailabilityChanged += NetworkChange_NetworkAvailabilityChanged;

			// Check for internet on start
			if (Application.internetReachability != NetworkReachability.ReachableViaLocalAreaNetwork) {
				if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork) {
					// Reacahable via mobile network
					// But can't use the app
					errorDescription = "We have detected that you are on a mobile network. A Wi-Fi connection is required to use this app.";
				} else {
					errorDescription = "A Wi-Fi connection is required to use the app";
				}

				isNetworkAvailable = false;

				// All not good - show errors
				// ShowNetworkError();
			} else {
//				Debug.Log ("Internet available using wifi");
				isNetworkAvailable = true;
			}
		}

        private void OnDestroy()
        {
            instance = null;
        }
        #endregion

        #region VARIABLES

        public bool isNetworkAvailable = false;
        public float checkingInterval = 10f;
        private float startTime = 0f;
		private string errorTitle = "Wi-Fi connection required";
		private string errorDescription;
        
		#endregion

		#region UNITY MONO METHODS

		void NetworkChange_NetworkAvailabilityChanged (object sender, NetworkAvailabilityEventArgs e)
		{
			Debug.Log ("Wi-Fi network changed");

			if (e.IsAvailable) {
				isNetworkAvailable = true;

				// Use unity api to check wifi availability
				if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork) {
					// All good
				} else {
					errorDescription = "We have detected that you are on a mobile network. A Wi-Fi connection is required to use this app.";
					isNetworkAvailable = false;
				}
			} else {
				errorDescription = "A Wi-Fi connection is required to use the app";
				isNetworkAvailable = false;
			}

			if (!isNetworkAvailable) {
				// Setup the error detail
				ShowNetworkError();
			}
		}

        /*
        private void Update()
        {
            if(Time.time - startTime > checkingInterval)
            {
                // Check for internet
                isNetworkAvailable = (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork);
                
                // Show error

                startTime = Time.time;
            }
        }*/

        private void Start()
        {
            startTime = Time.time;
        }
        #endregion

        #region PRIVATE METHODS

        void CheckForInternetAgain() {
			if (!isNetworkAvailable) {
				ShowNetworkError ();
			}
		}

		public void ShowNetworkError(ErrorDetail ed = null) {
            if (ed == null)
            {
                ed = new ErrorDetail();
                ed.SetErrorTitle(errorTitle);
                ed.SetErrorDescription(errorDescription);

                
                // Associate this error detail with an action
                // Add a reset button to the panel
                ed.AddToDictionary(ErrorDetail.ResponseType.RETRY, new UnityEngine.Events.UnityAction(delegate
                {
                    Debug.Log("Retry");
                    //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }));

                ed.AddToDictionary(ErrorDetail.ResponseType.SETTINGS, new UnityEngine.Events.UnityAction(delegate
                {
                    ViewManagerUtility.Instance().BackButtonPressed();
                    OVRManager.PlatformUIGlobalMenu();
                }));


                ed.AddToDictionary(ErrorDetail.ResponseType.EXIT, new UnityEngine.Events.UnityAction(delegate
                {
                    ViewManagerUtility.Instance().BackButtonPressed();
                    OVRManager.PlatformUIConfirmQuit();
                    // OVRManager.PlatformUIGlobalMenu();
                }));
            }
			ViewManagerUtility.Instance().SetupErrorView(ed);
		}

		#endregion
	}
}
