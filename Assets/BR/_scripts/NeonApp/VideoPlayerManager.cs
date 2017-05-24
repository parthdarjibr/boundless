//
//  Code by: Parth Darji
//  Company: Boundless Reality
//  (c) Boundless Reality, All rights reserved.
//
//  Details: This script loads the video player and sends data
//			to the video player menu script for instantiation.
//			This script raises events for various states of the playback
//
using UnityEngine;
using System.Collections;
using BR.BRUtilities.UI;
using BR.BRUtilities;
using RenderHeads.Media.AVProVideo;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BR.App {
	public class VideoPlayerManager : MonoBehaviour
	{
		#region EVENTS

		public delegate void VideoPlayerLoaded ();
		public VideoPlayerLoaded OnVideoPlayerLoaded;

		#endregion

		#region VARIABLES

		public VideoDetail currentVideo, nextVideo;
		public VideoPlayerMenu videoPlayerMenu;
		public MediaPlayer mediaPlayer;
		[Tooltip("Play sprite state")]
		public SpriteState playState;
		[Tooltip("Pause sprite state")]
		public SpriteState pauseState;
		public Sprite playIdle, pauseIdle;
		public MediaPlayer.FileLocation fileLocation;
		private int numberOfTries = 0;

		#endregion

		#region UNITY MONO METHODS

		void Update() {
			if (mediaPlayer.Control != null) {
				// Check for buffering
				if (mediaPlayer.Control.IsBuffering ()) {
					//videoPlayerMenu.ShowMenuForced (true);
					CUIGazePointer.instance.ShowLoadingReticle();
				} else if(mediaPlayer.Control.IsPlaying()) {
					//videoPlayerMenu.ShowMenuForced (false);
					CUIGazePointer.instance.HideLoadingReticle();
				}
			}
		}

		#endregion

		#region PUBLIC METHODS

		/// <summary>
		/// helper method for loading from variables
		/// </summary>
		public void SetupVideoPlayer() {
			if (currentVideo != null) {
				SetupVideoPlayer (currentVideo);
			}
		}

		/// <summary>
		/// Initializes the playlist, sets the video object active, calls menu manager to setup menu
		/// </summary>
		/// <param name="video">Video Object</param>
		public void SetupVideoPlayer(VideoDetail video) {
			currentVideo = video;

			// Listen to the fade back in event for control
			Camera.main.GetComponent<VRScreenFade>().onFadeInEnd += VideoViewFadeInComplete;
			// Fade back in to view
			ViewManagerUtility.Instance().FadeAllCamerasIn();

			// Activate the gameobject
			gameObject.SetActive (true);

			// Listen to video player events
			mediaPlayer.Events.AddListener(OnVideoEvent);

			// Setup the video on the video player
			mediaPlayer.m_AutoStart = false;
			mediaPlayer.m_Loop = false;

			// Setup the file location based on the url
			if (video.streamUrl.Contains ("://")) {
				fileLocation = MediaPlayer.FileLocation.AbsolutePathOrURL;
			} else {
				fileLocation = MediaPlayer.FileLocation.RelativeToStreamingAssetsFolder;
			}

			mediaPlayer.OpenVideoFromFile (fileLocation, video.streamUrl, true);

			// Increment the tries count
			numberOfTries++;

			// Change the reticle to red
			CUIGazePointer.instance.ShowLoadingReticle(true, "Loading...");
		}

		public void TogglePlayback() {
			if (mediaPlayer.Control.IsPlaying ()) {
				AnalyticsManager.Instance().SendButtonClickAnalytics("interaction", "PauseInVideo", "Pause button clicked on playback menu");
				AudioController.Instance ().PlayOneShot (ApplicationController.Instance ().interactionClickAudioClip);
				PauseVideo ();
			} else if (mediaPlayer.Control.IsPaused ()) {
				AnalyticsManager.Instance().SendButtonClickAnalytics("interaction", "PlayInVideo", "Play button clicked on playback menu");
				PlayVideo ();
			}
		}

		public void PlayVideo() {
			
			mediaPlayer.Play ();

			// change sprite to pause state
			videoPlayerMenu.btnPlay.spriteState = pauseState;
			videoPlayerMenu.btnPlay.GetComponent<Image> ().sprite = pauseIdle;
		}

		public void PauseVideo() {
			mediaPlayer.Pause ();

			// change sprite to play state
			videoPlayerMenu.btnPlay.spriteState = playState;
			videoPlayerMenu.btnPlay.GetComponent<Image> ().sprite = playIdle;
		}

		public void PlayNextVideo() {
			// Get the next video in the list
			VideoEdges edge =VideoPlaylistManager.Instance().GetNextVideoInPlaylist(currentVideo.idInPlaylist); 
			// nextVideo = VideoPlaylistManager.Instance().GetNextVideoInPlaylist(currentVideo.idInPlaylist).node;
			if (edge == null) {
				BackButtonHandler ();
			}
			else {
				nextVideo = edge.node;
				// Get the first video category
				string cat = (currentVideo.categories != null) ? currentVideo.categories[0].ToString() : "";
				// Log the previous video before going out
				AnalyticsManager.Instance().SendVideoAnalytics(currentVideo.name, 
					mediaPlayer.Control.GetCurrentTimeMs() / mediaPlayer.Info.GetDurationMs() * 100,
					cat,
					currentVideo.userHandle,
					currentVideo.uniqueId);

				// Also send data about the video being clicked but from "Next" Button
				AnalyticsManager.Instance().SendButtonClickAnalytics("video", "videoOnNext", nextVideo.name);

				AudioController.Instance().PlayOneShot(ApplicationController.Instance().videoClickAudioClip);
				// see if the next video is of the same type as the current video
				if (nextVideo.type == currentVideo.type) {
					Camera.main.GetComponent<VRScreenFade> ().onFadeOutEnd += RemoveCurrentVideo;
					ViewManagerUtility.Instance ().FadeAllCamerasOut ();
				} else {
					// If not, close this view and open another video view
					ViewManagerUtility.Instance().SwitchVideoView(nextVideo);
				}

			}
		}

		public void BackButtonHandler() {
			videoPlayerMenu.GetComponent<CanvasGroup>().interactable = false;
			videoPlayerMenu.GetComponent<CanvasGroup>().blocksRaycasts = false;
			videoPlayerMenu.GetComponent<CanvasGroup> ().alpha = 0;

			// Go back to previous view
			ViewManagerUtility.Instance().BackButtonPressed();
		}

		#endregion

		#region EVENT HANDLERS

		// Callback function to handle events
		private void OnVideoEvent(MediaPlayer mp, MediaPlayerEvent.EventType et, ErrorCode errorCode)
		{
			switch (et)
			{
			case MediaPlayerEvent.EventType.ReadyToPlay:
				// Debug.Log ("Ready To Play");
				//mediaPlayer.Play ();
				break;
			case MediaPlayerEvent.EventType.Started:
				// Change the pause button
				// change sprite to pause state
				videoPlayerMenu.btnPlay.spriteState = pauseState;
				videoPlayerMenu.btnPlay.GetComponent<Image> ().sprite = pauseIdle;

				//videoPlayerMenu.ShowMenuForced (false);
				break;
			case MediaPlayerEvent.EventType.FirstFrameReady:
				// Debug.Log ("First Frame ready");
				CUIGazePointer.instance.HideLoadingReticle ();
				CUIGazePointer.instance.RequestHide ();
				break;
			case MediaPlayerEvent.EventType.FinishedPlaying:
				// Send analytics data
				// TODO Change video category on analytics
				// Debug.Log ("Finished Playing");
				PlayNextVideo ();
				break;
			case MediaPlayerEvent.EventType.Unstalled:
				// Debug.Log ("Unstalled");
				CUIGazePointer.instance.HideLoadingReticle();
				//videoPlayerMenu.ShowMenuForced (false);
				break;
			case MediaPlayerEvent.EventType.Stalled:
				// Debug.Log ("Stalled");
				CUIGazePointer.instance.ShowLoadingReticle();
				//videoPlayerMenu.ShowMenuForced (true);
				break;
			case MediaPlayerEvent.EventType.Error:
				if (numberOfTries < 3) {
					SetupVideoPlayer ();
					// Debug.Log("Loading again");
				} else {
					// Debug.Log("Showing Error");

					// Show error
					ErrorDetail ed = new ErrorDetail ();
					ed.SetErrorTitle ("Problem loading video");
					ed.SetErrorDescription ("There was a problem loading the video. We apologize for the inconvenince.");

					// Associate this error detail with an action
					ed.AddToDictionary(ErrorDetail.ResponseType.CANCEL, new UnityEngine.Events.UnityAction( delegate {
						ViewManagerUtility.Instance ().CloseVideoView ();

					}));

					ApplicationController.Instance ().OpenErrorView (ed);
				}
				break;
			}
		}

		/// <summary>
		/// Event raised when the ui screen fade has completed
		/// </summary>
		private void VideoViewFadeInComplete() {
			// Open the menu after video view has faded in
			// Set the event camera for the menu canvas
			ViewManagerUtility.Instance().SetCanvasCamera(videoPlayerMenu.GetComponent<Canvas> ());

			// Stop listening to events
			Camera.main.GetComponent<VRScreenFade>().onFadeInEnd -= VideoViewFadeInComplete;

			// Setup the menu
			videoPlayerMenu.SetupMenu(currentVideo);
		}

		/// <summary>
		/// Removes the current video and opens next video
		/// Also fades in the view
		/// </summary>
		private void RemoveCurrentVideo() {
			// Unsubscribe fadeout event
			Camera.main.GetComponent<VRScreenFade>().onFadeOutEnd -= RemoveCurrentVideo;

			// Subscribe to fade in event
			// ViewManagerUtility.Instance ().FadeAllCamerasIn ();

			// Disable the current menu to avoid  multiple clicks
			videoPlayerMenu.GetComponent<CanvasGroup> ().alpha = 0;
			videoPlayerMenu.GetComponent<CanvasGroup>().interactable = false;
			videoPlayerMenu.GetComponent<CanvasGroup>().blocksRaycasts = false;

			mediaPlayer.Stop ();
			// Setup the video player
			SetupVideoPlayer (nextVideo);
		}
			
		#endregion
	}
}
