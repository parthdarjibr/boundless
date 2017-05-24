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
using BR.BRUtilities;
using System.Collections.Generic;
using MA;
using System;
using UnityEngine.EventSystems;

namespace BR.App {
	public class VideoPlayerMenu : MonoBehaviour
	{
		#region UI VARIABLES

		[SerializeField] private Text videoName;
		[SerializeField] private Text influencerName;
		[SerializeField] private Image influencerPicture;
		[SerializeField] private Image VideoProgressBar;
		[SerializeField] private Image infProgressBar;
		[SerializeField] private Button btnFollow, btnInfluencer;
		public Button btnPlay, btnBack, btnNext;
		[SerializeField] private GameObject menuPanelObject;
		[SerializeField] private float CanvasDistance;

		public bool shouldReposition = true;
		public float minAlpha = 0, maxAlpha = 1, fadeSpeed = 0.3f;
		public Sprite defaultUserSprite;
		private bool shouldAutoHide = true;
		public bool viewLoaded = false;

		// Time in seconds when menu automatically disappears on first run
		private float autoHideTimeOut = 3f;
		private float startTime = 0f;
		private bool shouldDisappear = false;

		#endregion

		#region DATA VARIABLES

		private VideoPlayerManager currentVideoPlayer;
		private VideoDetail currentVideoDetail;

		#endregion

		#region UNITY MONO METHODS

		void Update() {
			
			// Check for the time and hide the menu after threshold
			if (shouldDisappear && ((Time.time - startTime) > autoHideTimeOut) && viewLoaded) {
				shouldDisappear = false;
				PerformForcedFadeOut (0, () => {

					shouldReposition = true;
					// Disable the collider on the menu
					GetComponent<MeshCollider>().enabled = false;
				});
			}

			// Update the progressbar is the video is playing
			if (currentVideoPlayer != null && currentVideoPlayer.mediaPlayer.Control.IsPlaying ()) {
				VideoProgressBar.fillAmount = (1 - currentVideoPlayer.mediaPlayer.Control.GetCurrentTimeMs() / currentVideoPlayer.mediaPlayer.Info.GetDurationMs());
			}

			// Handle opening and closing of the menu
			if ((Input.GetMouseButtonUp (0) || OVRInput.GetUp(OVRInput.Button.One) || OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger)) && viewLoaded) {
				if (shouldReposition) {
					// Change position of the menu to in front of the camera
					menuPanelObject.transform.localPosition = Camera.main.transform.forward * CanvasDistance;
					//menuCanvas.transform.LookAt (Camera.main.transform);
					menuPanelObject.transform.rotation = Quaternion.LookRotation (menuPanelObject.transform.position - Camera.main.transform.position);

					// Enable the mesh collider and perform fade in
					GetComponent<MeshCollider>().enabled = true;
					PerformForcedFadeIn (1, () => {
						shouldReposition = false;
					});
				}
			}
		}

		#endregion

		#region PUBLIC METHODS

		public void SetupMenu(VideoDetail video) {
			ExecuteEvents.Execute (btnNext.gameObject, new PointerEventData (EventSystem.current), ExecuteEvents.pointerExitHandler);
			ExecuteEvents.Execute (btnPlay.gameObject, new PointerEventData (EventSystem.current), ExecuteEvents.pointerExitHandler);
			ExecuteEvents.Execute (btnBack.gameObject, new PointerEventData (EventSystem.current), ExecuteEvents.pointerExitHandler);

			// Remove all listeners in the interaction panel 
			btnPlay.onClick.RemoveAllListeners();
			btnBack.onClick.RemoveAllListeners ();
			btnNext.onClick.RemoveAllListeners ();

			// ShowMenuForced (true);
			// OVRGazePointer.instance.ShowLoadingReticle();
			if (video != null) {
				// Set the current video
				currentVideoDetail = video;

				// remove user image until download
				influencerPicture.sprite = defaultUserSprite;

				// Get the influencer picture
				DownloadQueue influencerPictureQueue = new DownloadQueue ();
				TextureDownloadObject influencerPicObject = new TextureDownloadObject (video.userPicUrl, influencerPicture, infProgressBar, 1, InfluencerPictureLoaded);
				influencerPictureQueue.queue.Enqueue (influencerPicObject);
				influencerPictureQueue.currentStatus = true;
				DownloadManager.Instance ().listDownloadQueues.Add (influencerPictureQueue);

				// Set the influencer button click object
				btnInfluencer.onClick.AddListener (delegate {
					AnalyticsManager.Instance ().SendButtonClickAnalytics ("creator", "creatorInVideo", currentVideoDetail.userHandle);
					OnInfluencerButtonClicked ();
				});

				videoName.text = video.name.Replace(" ", "\n");
				influencerName.text = "@" + video.userHandle;

				// Get the current video player manager
				currentVideoPlayer = GetComponentInParent<VideoPlayerManager> ();

				// Setup event handlers
				btnFollow.onClick.AddListener (FollowButtonClicked);
				btnPlay.onClick.AddListener (delegate {
					// ANALYTICS IS HANDLED IN TOGGLEPLAYBACK
					// Audio playback handled in toggle playback
					ExecuteEvents.Execute (btnPlay.gameObject, new PointerEventData (EventSystem.current), ExecuteEvents.pointerExitHandler);
					currentVideoPlayer.TogglePlayback ();
				});
				btnBack.onClick.AddListener (delegate {
					ExecuteEvents.Execute (btnBack.gameObject, new PointerEventData (EventSystem.current), ExecuteEvents.pointerExitHandler);
					AnalyticsManager.Instance ().SendButtonClickAnalytics ("interaction", "BackInVideo", "Back button clicked on playback menu");

					AudioController.Instance ().PlayOneShot (ApplicationController.Instance ().videoBackClip);
					currentVideoPlayer.BackButtonHandler ();
				});

				btnNext.onClick.AddListener (delegate {
					PointerEventData pEvent = new PointerEventData(EventSystem.current);

					pEvent = ExecuteEvents.ValidateEventData<PointerEventData>(pEvent);
					ExecuteEvents.Execute (this.gameObject.gameObject, pEvent, ExecuteEvents.pointerExitHandler);
					ExecuteEvents.Execute (btnNext.gameObject, pEvent, ExecuteEvents.pointerUpHandler);

					// Send analytics data
					AnalyticsManager.Instance ().SendButtonClickAnalytics ("interaction", "NextInVideo", "Next button clicked on playback menu");

					// Reset the reticle
					CUIGazePointer.instance.HideLoadingReticle();

					// Disable view loaded
					viewLoaded = false;
					// AUDIO PLAYBACK IS HANDLED IN PLAY NEXT VIDEO
					currentVideoPlayer.PlayNextVideo ();
				});

				// Reset the progress bar
				VideoProgressBar.fillAmount = 1;

				// reposition the menu
				menuPanelObject.transform.localPosition = Camera.main.transform.forward * CanvasDistance;
				menuPanelObject.transform.rotation = Quaternion.LookRotation (menuPanelObject.transform.position - Camera.main.transform.position);

				// Show the menu
				menuPanelObject.GetComponent<CanvasGroup> ().alpha = 1;
				menuPanelObject.GetComponent<CanvasGroup> ().interactable = true;
				menuPanelObject.GetComponent<CanvasGroup> ().blocksRaycasts = true;

				CUIGazePointer.instance.ShowLoadingReticle();

				if (currentVideoPlayer.OnVideoPlayerLoaded != null) {
					currentVideoPlayer.OnVideoPlayerLoaded ();
				}

				viewLoaded = true;

				// Setup the menu for auto hide 
				startTime = Time.time;
				shouldDisappear = true;

				// Mesh Collider
				GetComponent<MeshCollider>().enabled = true;
			}
		}

		/// <summary>
		/// Shows/Hides the menu forcefully for buffering interactions
		/// </summary>
		/// <param name="_show">If set to <c>true</c> show.</param>
		public void ShowMenuForced(bool _show) {
			if (_show) {
				// Turn off auto hide
				shouldAutoHide = false;
				// Show the menu
				// Change position of the menu to in front of the camera
				menuPanelObject.transform.localPosition = Camera.main.transform.forward * CanvasDistance;
				//menuCanvas.transform.LookAt (Camera.main.transform);
				menuPanelObject.transform.rotation = Quaternion.LookRotation (menuPanelObject.transform.position - Camera.main.transform.position);

				PerformForcedFadeIn (1, null);
				//isShowing = true;
			} else {
				shouldAutoHide = true;
				// isShowing = false;
			}
		}

		#endregion

		#region EVENT HANDLERS 

		private void FollowButtonClicked() {
			ErrorDetail ed = new ErrorDetail ();
			ed.SetErrorTitle ("Coming Soon...");
			ed.SetErrorDescription ("");

			/*
			List<ErrorDetail.ResponseType> r = new List<ErrorDetail.ResponseType> ();
			r.Add (ErrorDetail.ResponseType.ACCEPT);

			ed.SetResponseTypes (r);
			*/

			// Add responses to dictionary
			ed.AddToDictionary (ErrorDetail.ResponseType.ACCEPT, new UnityEngine.Events.UnityAction (delegate {
				ViewManagerUtility.Instance ().RemoveCanvasFromCurrentView ();
			}));
			ApplicationController.Instance ().OpenErrorView (ed);
		}

		public void OnPointerEnter() {
			ExecuteEvents.Execute (btnNext.gameObject, new PointerEventData (EventSystem.current), ExecuteEvents.pointerExitHandler);
			if (shouldAutoHide) {
				PerformForcedFadeIn (1, () => {
					shouldReposition = false;
					GetComponent<MeshCollider>().enabled = true;
					// isShowing = true;
				});
			}
		}
			
		public void OnPointerExit() {
			ExecuteEvents.Execute (btnNext.gameObject, new PointerEventData (EventSystem.current), ExecuteEvents.pointerExitHandler);
			if (shouldAutoHide) {
				PerformForcedFadeOut (0, () => {
					shouldReposition = true;
					// Disable the collider on the menu
					GetComponent<MeshCollider>().enabled = false;
					// isShowing = false;
				});
			}
			/*
			menuPanelObject.GetComponent<CanvasGroupTransition> ().closeAnimation (() => {
				menuPanelObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
				shouldReposition = true;
			}, true);*/
		}

		private void OnInfluencerButtonClicked() {
			// Close the video view
			ViewManagerUtility.Instance().CloseVideoView();
			Camera.main.GetComponent<VRScreenFade>().onFadeOutEnd += OpenInfluencerFromVideoView;

			// Close the video view
			// ViewManagerUtility.Instance().CloseVideoView();
		}

		private void OpenInfluencerFromVideoView() {
			Camera.main.GetComponent<VRScreenFade>().onFadeOutEnd -= OpenInfluencerFromVideoView;

			ApplicationController.Instance ().OpenInfluencerView (currentVideoDetail.userGid);

			/*
			// Open influencer view
			InfluencerDetail influencer = new InfluencerDetail();
			influencer.SetGid (currentVideoDetail.userGid);
			influencer.SetDisplayName (currentVideoDetail.userDisplayName);
			influencer.SetPicUrl (currentVideoDetail.userPicUrl);
			//influencer.SetInfluencerBio (currentVideoDetail.influencerBio);
			ApplicationController.Instance().OpenInfluencerView(influencer);
			*/
		}

		#endregion

		#region PRIVATE METHODS

		private void PerformForcedFadeIn(float alpha, Action onFinished = null) {
			if (alpha > 1)
				alpha = 1;

			maxAlpha = alpha;

			// Disable active fading for the object
			// isFading = false;
			StopAllCoroutines ();

			StartCoroutine (FadeIn (onFinished));
		}

		private void PerformForcedFadeOut(float alpha, Action onFinished = null) {
			if (alpha < 0)
				alpha = 0;

			minAlpha = alpha;

			// Disable active fading
			// isFading = false;
			StopAllCoroutines ();

			StartCoroutine (FadeOut (onFinished));
		}
			
		IEnumerator FadeIn(Action a = null) {
			float currentAlpha;
			// isFading = true;
			CanvasGroup cg = GetComponent<CanvasGroup> ();

			do {
				currentAlpha = maxAlpha;
				cg.alpha += fadeSpeed * Time.deltaTime;

				// Update the alpha 
				if(cg.alpha < currentAlpha)
					currentAlpha = cg.alpha;

				// Wait for next frame
				yield return null;
			} while(currentAlpha < maxAlpha);

			if (a != null) {
				a ();
			}

			cg.blocksRaycasts = true;
			cg.interactable = true;

			// Update state machine
			// isFading = false;
		}

		IEnumerator FadeOut(Action a = null) {
			float currentAlpha;
			// isFading = true;
			CanvasGroup cg = GetComponent <CanvasGroup> ();


			do {
				currentAlpha = minAlpha;
				cg.alpha -= fadeSpeed * Time.deltaTime;

				if (cg.alpha > currentAlpha)
					currentAlpha = cg.alpha;
				yield return null;

			} while (currentAlpha > minAlpha);
		
			if (a != null) {
				a ();
			}

			cg.interactable = false;
			cg.blocksRaycasts = false;

			// Update state machine
			// isFading = false;
		}

		void InfluencerPictureLoaded() {
			btnInfluencer.interactable = true;
		}
		#endregion
	}
}

