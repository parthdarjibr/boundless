//
//  Code by: Parth Darji
//  Company: Boundless Reality
//  (c) Boundless Reality, All rights reserved.
//
//  Details: This script loads details into individual elements of the video object
//			 A parent object instantiates this object and everything is handeld on start and on various events
//
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using BR.BRUtilities;
using BR.BRUtilities.UI;
using System;
using UnityEngine.EventSystems;

namespace BR.App {
	public class VideoDetailsLoader : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler// , IPointerEnterHandler
	{
		#region EVENTS

		public delegate void VideoLoadingComplete();
		public event VideoLoadingComplete onVideoLoaded;

		#endregion

		#region UI VARIABLES

		public Text tvInfluencerName, tvVideoName;
		public Image imgThumb, imgInfluencerPicture, thumbProgressBar, infProgressBar;
		public Button btnPlay, btnInfluencer;
		// public TextMeshPro tvInfluencerName, tvVideoName;
		// public TextMeshProUGUI tvInfluencerName, tvVideoName;

		#endregion

		#region DATA VARIABLES

		[HideInInspector] public VideoDetail currentVideo;
		public bool LoadInfluencerDetails = true;
		DownloadUtility downloadUtility;

		#endregion

		#region UNITY MONOBEHAVIOUR METHODS

		public void OnPointerDown(PointerEventData pointer) {
			btnPlay.GetComponent<Image>().sprite = btnPlay.spriteState.pressedSprite;
		}

		public void OnPointerUp(PointerEventData pointer) {
			btnPlay.GetComponent<Image>().sprite = btnPlay.spriteState.highlightedSprite;
		}

		public void OnPointerExit(PointerEventData ed) {
		}
		#endregion



		#region PUBLIC METHODS

		/// <summary>
		/// Setups the video details.
		/// </summary>
		/// <param name="video">Video Detail class object.</param>
		public void SetupVideoDetails(VideoDetail video) {
			// TODO Check for internet connection
			if (video == null) {
				return;
			} else {
				currentVideo = video;

				// Set the video text views
 				tvVideoName.text = video.name.Replace(" ", "\n");

				GetComponent<Button> ().onClick.AddListener ( this.VideoButtonClicked );
				btnPlay.onClick.AddListener (this.VideoButtonClicked);

				if (LoadInfluencerDetails) {
					tvInfluencerName.text = "@" + video.userHandle;
					// Add the listener
					//btnInfluencer.onClick.AddListener(InfluencerButtonClicked);
					btnInfluencer.onClick.AddListener(InfluencerButtonClicked);
				}
			}
		}

		#endregion

		#region EVENT HANDLERS

		/// <summary>
		/// The video thumbnail is loaded. This method is called from the
		/// download utility when the thumbnail for this object is downloaded and set
		/// </summary>
		public void VideoThumbnailLoaded() {

			// Deactivate the progress bar
			thumbProgressBar.gameObject.SetActive(false);

			// Make the button interactable
			GetComponent<Button>().interactable = true;
			btnPlay.interactable = true;
		}

		/// <summary>
		/// The influencer picture is loaded.This method is called from the
		/// download utility when the influencer picture is downloaded and set
		/// </summary>
		public void InfluencerPictureLoaded() {
			// Deactivate the progress bar
			infProgressBar.gameObject.SetActive(false);

			// Make the button interactable
			btnInfluencer.interactable = true;
		}

		/// <summary>
		/// Click event handler for video buttons
		/// </summary>
		void VideoButtonClicked() {
			// AUDIO IS PLAYED FROM AUDIOPLAYBACKMANAGER
			// Play the audio
			// AudioController.Instance().PlayOneShot(AudioController.Instance().videoClickAudioClip);

			// Also send the event to the play button
			ApplicationController.Instance ().OpenVideoPlayer (currentVideo);

			// Handle analytics
			AnalyticsManager.Instance().SendButtonClickAnalytics("video", LoadInfluencerDetails ? "videoOnHome" : "videoOnInfluencer", currentVideo.name);

			// Send pointer exit message to the video button
			ExecuteEvents.Execute (this.gameObject, new PointerEventData (EventSystem.current), ExecuteEvents.pointerExitHandler);
		}

		/// <summary>
		/// Click event handler for influencer button
		/// </summary>
		void InfluencerButtonClicked() {
			// Send pointer exit messages to both the video object and the influencer object
			ExecuteEvents.Execute (this.gameObject, new PointerEventData (EventSystem.current), ExecuteEvents.pointerExitHandler);
			ExecuteEvents.Execute (btnInfluencer.gameObject, new PointerEventData (EventSystem.current), ExecuteEvents.pointerExitHandler);

			// Handle analytics
			AnalyticsManager.Instance ().SendButtonClickAnalytics ("creator", "creatorOnVideoThumbnail", currentVideo.userHandle);

			ApplicationController.Instance().OpenInfluencerView(currentVideo.userGid);

			// AUDIO IS PLAYED FROM AUDIOPLAYBACKMANAGER
			// Play the audio
			// AudioController.Instance().PlayOneShot(AudioController.Instance().influencerClickAudioClip);

			/*
			// Open the influencer View
			// Define an InfluencerDetail object and send it to ApplicationManager for instantiation
			InfluencerDetail influencer = new InfluencerDetail();
			influencer.SetGid (currentVideo.userGid);
			influencer.SetDisplayName (currentVideo.userDisplayName);
			influencer.SetPicUrl (currentVideo.userPicUrl);
			//influencer.SetInfluencerBio (currentVideo.influencerBio);

			ApplicationController.Instance().OpenInfluencerView(influencer);
			*/
		}

		#endregion
	}
}

