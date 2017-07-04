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
using UnityEngine.Analytics;
using System.Collections.Generic;
using BR.App;

namespace BR.BRUtilities {
	public class AnalyticsManager : MonoBehaviour
	{

		#region INITIALIZATION

		private static AnalyticsManager instance;
		private static string deviceID;// = SystemInfo.deviceUniqueIdentifier;

		private static bool Exists() {
			return instance != null;
		}

		public static AnalyticsManager Instance() {
			if (!Exists ()) {
				throw new Exception ("Analytics Manager object not found in scene");
			}
			return instance;
		}

		void Awake() {
			if (!Exists ())
				instance = this;

			// DontDestroyOnLoad (this.gameObject);

			deviceID = SystemInfo.deviceUniqueIdentifier;

            
			// Setup analytics based on platform
			#if UNITY_EDITOR

			registerAnalytics = true;

			#elif UNITY_ANDROID

			registerAnalytics = true;

			#endif
            
		}

        private void OnDestroy()
        {
            instance = null;
        }

        #endregion

        #region VARIABLES

        public bool registerAnalytics = true;

		#endregion

		#region PUBLIC METHODS

		/// <summary>
		/// Sends the video analytics.
		/// </summary>
		/// <param name="videoName">Video name.</param>
		/// <param name="spentTime">Time spent in video in seconds.</param>
		/// <param name="videoCategory">Video category.</param>
		/// <param name="creatorName">Creator name.</param>
		public void SendVideoAnalytics (string videoName, float spentTimePercentage, string videoCategory, string creatorName, string videoUUID, float totalTime ) {
			if (registerAnalytics) {
				// handle null values in case they exist
				if (videoName == null)
					videoName = "NoName";
				if (float.IsNaN (spentTimePercentage))
					spentTimePercentage = -1;
				if (videoCategory == null)
					videoCategory = "NoCategory";
				if (creatorName == null)
					creatorName = "NoCreatorName";
				if (videoUUID == null)
					videoUUID = "NoVideoUUID";
				if (float.IsNaN(totalTime))
					totalTime = -1;
			
				Analytics.CustomEvent ("VideoWatched", new Dictionary<string, object> {
                    //{ "UserUUID", PlayerPrefs.GetString ("UniqueID") },
                    { "EventUUID", GenerateEventUUID() },
					{ "UserUUID", deviceID },
					{ "VideoName", videoName },
					{ "SpentTime", spentTimePercentage },
					{ "VideoCategory", videoCategory },
					{ "CreatorName", creatorName },
					{ "VideoUUID", videoUUID },
					{ "CustomTS", DateTime.Now.ToString () }
				});

				// Debug.Log ("Spent time: " + spentTimePercentage);
				// Debug.Log ("Total time: " + totalTime);
			}
		}

        public void SendVideoBufferAnalytics(string videoName, float bufferTime, float totalVideoTime, string videoCategory, string creatorName, string videoUUID)
        {
            if (registerAnalytics)
            {
                // handle null values in case they exist
                if (videoName == null)
                    videoName = "NoName";
                if (float.IsNaN(bufferTime))
                    bufferTime = -1;
                if (float.IsNaN(totalVideoTime))
                    totalVideoTime = -1;
                if (videoCategory == null)
                    videoCategory = "NoCategory";
                if (creatorName == null)
                    creatorName = "NoCreatorName";
                if (videoUUID == null)
                    videoUUID = "NoVideoUUID";

                Analytics.CustomEvent("VideoWatched", new Dictionary<string, object> {
                    //{ "UserUUID", PlayerPrefs.GetString ("UniqueID") },
                    { "EventUUID", GenerateEventUUID() },
                    { "UserUUID", deviceID },
                    { "VideoName", videoName },
                    { "BufferTime", bufferTime },
                    { "VideoLength", totalVideoTime },
                    { "VideoCategory", videoCategory },
                    { "CreatorName", creatorName },
                    { "VideoUUID", videoUUID },
                    { "CustomTS", DateTime.Now.ToString () }
                });

                //Debug.Log("Buffer time: " + bufferTime);
            }
        }

        public void SendCategoryAnalytics(string categoryName) {
			if (registerAnalytics) {
				Analytics.CustomEvent ("CategoryClicked", new Dictionary<string, object> {
                    { "EventUUID", GenerateEventUUID() },
                    { "CategoryName", categoryName },
					//{ "UserUUID", PlayerPrefs.GetString ("UniqueID") },
					{ "UserUUID", deviceID },
					{ "CustomTS", DateTime.Now.ToString () }
				});
			}
		}

		/// <summary>
		/// Sends the button click analytics.
		/// </summary>
		/// <param name="btnCategory">Button category.</param>
		/// <param name="btnType">Button Details.</param>
		/// <param name="btnValue">Value.</param>
		public void SendButtonClickAnalytics(string btnCategory, string btnType, string btnValue) {
			if (registerAnalytics) {
				Analytics.CustomEvent ("ButtonClicked", new Dictionary<string, object> {
                    { "EventUUID", GenerateEventUUID() },
                    { "ButtonCategory", btnCategory },
					{ "ButtonType", btnType },
					{ "ButtonValue", btnValue },
					{ "UserUUID", deviceID },
					//{ "UserUUID", PlayerPrefs.GetString ("UniqueID") },
					{ "CustomTS", DateTime.Now.ToString () }
				});
			}
		}

		/// <summary>
		/// Sends analytics event about unreachability of wifi
		/// </summary>
		public void SendNoWifiAnalytics() {
			if (registerAnalytics) {
				Analytics.CustomEvent ("No Wi-Fi", new Dictionary<string, object> {
                    { "EventUUID", GenerateEventUUID() },
                    { "UserUUID", deviceID }
					//{ "UserUUID", PlayerPrefs.GetString ("UniqueID") }
				});
			}
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buttonType">buttonDown, buttonUp</param>
        public void SendSeekbarAnalytics(string buttonType, float clickTime, string videoName, string videoUUID)
        {
            if (registerAnalytics)
            {
                // Edge cases
                if (videoUUID == null)
                   videoUUID = "NoVideoUUID";
                if (videoName == null)
                    videoName = "NoName";


                Analytics.CustomEvent("ButtonClicked", new Dictionary<string, object>
                {
                    { "EventUUID", GenerateEventUUID() },
                    {"ButtonCategory", "VideoSeekbar" },
                    { "ButtonType", buttonType },
                    { "ClickTime",  clickTime},
                    { "VideoName", videoName },
                    { "VideoUUID", videoUUID },
                    { "UserUUID", deviceID }
                });
            }
        }

        #endregion

        #region PRIVATE METHODS

        private string GenerateEventUUID()
        {
            return Guid.NewGuid().ToString();
        }

        #endregion
    }
}
