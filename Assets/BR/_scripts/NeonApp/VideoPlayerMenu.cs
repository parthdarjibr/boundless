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

namespace BR.App
{
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
        public Slider videoSeekbar;
        public Text txtBeginTime, txtEndTime, txtCurrentTime;
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

        // Analytics for total time spent in the video
        public float totalTimeSpent = 0f;

        bool pointerOut = false;
        #endregion

        #region DATA VARIABLES

        private VideoPlayerManager currentVideoPlayer;
        private VideoDetail currentVideoDetail;

        #endregion

        #region UNITY MONO METHODS

        void Update()
        {
            // Check for the time and hide the menu after threshold
            if (shouldDisappear && ((Time.time - startTime) > autoHideTimeOut) && viewLoaded)
            {
                shouldDisappear = false;
                PerformForcedFadeOut(0, () =>
                {
                    shouldReposition = true;
                    // Disable the collider on the menu
                    GetComponent<MeshCollider>().enabled = false;
                });
            }

            // Update the progressbar is the video is playing
            if (currentVideoPlayer != null && currentVideoPlayer.mediaPlayer.Control.IsPlaying())
            {
                // Update the total time on the video player
                totalTimeSpent += Time.deltaTime;

                float currentTimeMs = currentVideoPlayer.mediaPlayer.Control.GetCurrentTimeMs();
                float totalTimeMs = currentVideoPlayer.mediaPlayer.Info.GetDurationMs();

                VideoProgressBar.fillAmount = (1 - currentTimeMs / totalTimeMs);

                if (videoSeekbar.interactable)
                {
                    videoSeekbar.value = currentTimeMs / totalTimeMs;
                    txtBeginTime.text = TruncateMsToMin(currentTimeMs);
                    txtEndTime.text = "-" + TruncateMsToMin(totalTimeMs - currentTimeMs);

                }
            }

            // Show menu only if error canvas is not active
            // Handle opening and closing of the menu
            if ((Input.GetMouseButtonUp(0) || OVRInput.GetUp(OVRInput.Button.One) || OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger)) && viewLoaded
                && GameObject.Find("ErrorCanvas(Clone)") == null)
            {
                if (shouldReposition)
                {
                    // Change position of the menu to in front of the camera
                    menuPanelObject.transform.localPosition = Camera.main.transform.forward * CanvasDistance;
                    //menuCanvas.transform.LookAt (Camera.main.transform);
                    menuPanelObject.transform.rotation = Quaternion.LookRotation(menuPanelObject.transform.position - Camera.main.transform.position);

                    // Enable the mesh collider and perform fade in
                    GetComponent<MeshCollider>().enabled = true;
                    PerformForcedFadeIn(1, () =>
                    {
                        shouldReposition = false;
                    });
                }
            }
        }

        #endregion

        #region PUBLIC METHODS

        public void SetupMenu(VideoDetail video)
        {
            ExecuteEvents.Execute(btnNext.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerExitHandler);
            ExecuteEvents.Execute(btnPlay.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerExitHandler);
            ExecuteEvents.Execute(btnBack.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerExitHandler);

            // Remove all listeners in the interaction panel 
            btnPlay.onClick.RemoveAllListeners();
            btnBack.onClick.RemoveAllListeners();
            btnNext.onClick.RemoveAllListeners();
            // videoSeekbar.onValueChanged.RemoveAllListeners();

            // ShowMenuForced (true);
            // OVRGazePointer.instance.ShowLoadingReticle();
            if (video != null)
            {
                // Set the current video
                currentVideoDetail = video;

                // remove user image until download
                influencerPicture.sprite = defaultUserSprite;

                // Get the influencer picture
                DownloadQueue influencerPictureQueue = new DownloadQueue();
                TextureDownloadObject influencerPicObject = new TextureDownloadObject(video.userPicUrl, influencerPicture, infProgressBar, 1, InfluencerPictureLoaded);
                influencerPictureQueue.queue.Enqueue(influencerPicObject);
                influencerPictureQueue.currentStatus = true;
                DownloadManager.Instance().listDownloadQueues.Add(influencerPictureQueue);

                // Set the influencer button click object
                btnInfluencer.onClick.AddListener(delegate
                {
                    AnalyticsManager.Instance().SendButtonClickAnalytics("creator", "creatorInVideo", currentVideoDetail.userHandle);
                    OnInfluencerButtonClicked();
                });

                videoName.text = video.name.Replace(" ", "\n");
                influencerName.text = "@" + video.userHandle;

                // Get the current video player manager
                currentVideoPlayer = GetComponentInParent<VideoPlayerManager>();

                // Setup event handlers
                btnFollow.onClick.AddListener(FollowButtonClicked);
                btnPlay.onClick.AddListener(delegate
                {
                    // ANALYTICS IS HANDLED IN TOGGLEPLAYBACK
                    // Audio playback handled in toggle playback
                    ExecuteEvents.Execute(btnPlay.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerExitHandler);
                    currentVideoPlayer.TogglePlayback();
                });
                btnBack.onClick.AddListener(delegate
                {
                    ExecuteEvents.Execute(btnBack.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerExitHandler);
                    AnalyticsManager.Instance().SendButtonClickAnalytics("interaction", "BackInVideo", "Back button clicked on playback menu");

                    AudioController.Instance().PlayOneShot(ApplicationController.Instance().videoBackClip);
                    currentVideoPlayer.BackButtonHandler();
                });

                btnNext.onClick.AddListener(delegate
                {
                    PointerEventData pEvent = new PointerEventData(EventSystem.current);

                    pEvent = ExecuteEvents.ValidateEventData<PointerEventData>(pEvent);
                    ExecuteEvents.Execute(this.gameObject.gameObject, pEvent, ExecuteEvents.pointerExitHandler);
                    ExecuteEvents.Execute(btnNext.gameObject, pEvent, ExecuteEvents.pointerUpHandler);

                    // Send analytics data
                    AnalyticsManager.Instance().SendButtonClickAnalytics("interaction", "NextInVideo", "Next button clicked on playback menu");

                    // Reset the reticle
                    CUIGazePointer.instance.HideLoadingReticle();

                    // Disable view loaded
                    viewLoaded = false;
                    // AUDIO PLAYBACK IS HANDLED IN PLAY NEXT VIDEO
                    currentVideoPlayer.PlayNextVideo();
                });

                videoSeekbar.onValueChanged.AddListener(OnSeekbarValueChanged);

                // Reset the progress bar
                VideoProgressBar.fillAmount = 1;

                // Set the seekbar position and values
                videoSeekbar.value = 0;
                videoSeekbar.minValue = 0;
                // videoSeekbar.maxValue = currentVideoPlayer.mediaPlayer.Info.GetDurationMs();

                txtBeginTime.text = "-:--";
                txtEndTime.text = "-:--";
                txtCurrentTime.gameObject.SetActive(false);

                // Disable the seekbar until the video starts
                // videoSeekbar.interactable = false;

                // txtEndTime.text = TruncateFloatToString(ConvertMillisecondsToMinutes(currentVideoPlayer.mediaPlayer.Info.GetDurationMs()));

                // reposition the menu
                menuPanelObject.transform.localPosition = Camera.main.transform.forward * CanvasDistance;
                menuPanelObject.transform.rotation = Quaternion.LookRotation(menuPanelObject.transform.position - Camera.main.transform.position);

                // Show the menu
                menuPanelObject.GetComponent<CanvasGroup>().alpha = 1;
                menuPanelObject.GetComponent<CanvasGroup>().interactable = true;
                menuPanelObject.GetComponent<CanvasGroup>().blocksRaycasts = true;

                CUIGazePointer.instance.ShowLoadingReticle();

                if (currentVideoPlayer.OnVideoPlayerLoaded != null)
                {
                    currentVideoPlayer.OnVideoPlayerLoaded();
                }

                // viewLoaded = true;

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
        public void ShowMenuForced(bool _show)
        {
            if (_show)
            {
                // Turn off auto hide
                shouldAutoHide = false;
                // Show the menu
                // Change position of the menu to in front of the camera
                menuPanelObject.transform.localPosition = Camera.main.transform.forward * CanvasDistance;
                //menuCanvas.transform.LookAt (Camera.main.transform);
                menuPanelObject.transform.rotation = Quaternion.LookRotation(menuPanelObject.transform.position - Camera.main.transform.position);

                PerformForcedFadeIn(1, null);
                //isShowing = true;
            }
            else
            {
                shouldAutoHide = true;
                // isShowing = false;
            }
        }

        /// <summary>
        /// Called from the video player to set the max value of the seekbar
        /// </summary>
        /// <param name="val"></param>
        public void SetSeekbarMaxValue(float val)
        {
            // Activate seekbar interactions
            videoSeekbar.interactable = true;
        }
        #endregion

        #region EVENT HANDLERS 

        private void FollowButtonClicked()
        {
            ErrorDetail ed = new ErrorDetail();
            ed.SetErrorTitle("Coming Soon...");
            ed.SetErrorDescription("");

            /*
			List<ErrorDetail.ResponseType> r = new List<ErrorDetail.ResponseType> ();
			r.Add (ErrorDetail.ResponseType.ACCEPT);

			ed.SetResponseTypes (r);
			

            // Add responses to dictionary
            ed.AddToDictionary(ErrorDetail.ResponseType.ACCEPT, new UnityEngine.Events.UnityAction(delegate
            {
                ViewManagerUtility.Instance().RemoveCanvasFromCurrentView();
            }));
            ApplicationController.Instance().OpenErrorView(ed);
            */
        }

        public void OnPointerEnter()
        {
            pointerOut = false;
            ExecuteEvents.Execute(btnNext.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerExitHandler);
            if (shouldAutoHide)
            {
                PerformForcedFadeIn(1, () =>
                {
                    shouldReposition = false;
                    GetComponent<MeshCollider>().enabled = true;
                    // isShowing = true;
                });
            }
        }

        public void OnPointerExit()
        {
            pointerOut = true;
            if (!isScrubbing)
            {
                ExecuteEvents.Execute(btnNext.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerExitHandler);
                if (shouldAutoHide && viewLoaded)
                {
                    PerformForcedFadeOut(0, () =>
                    {
                        shouldReposition = true;
                        // Disable the collider on the menu
                        GetComponent<MeshCollider>().enabled = false;
                        // isShowing = false;
                    });
                }
            }
            /*
			menuPanelObject.GetComponent<CanvasGroupTransition> ().closeAnimation (() => {
				menuPanelObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
				shouldReposition = true;
			}, true);*/
        }

        private void OnInfluencerButtonClicked()
        {
            // Close the video view
            ViewManagerUtility.Instance().CloseVideoView();
            Camera.main.GetComponent<VRScreenFade>().onFadeOutEnd += OpenInfluencerFromVideoView;

            // Close the video view
            // ViewManagerUtility.Instance().CloseVideoView();
        }

        private void OpenInfluencerFromVideoView()
        {
            Camera.main.GetComponent<VRScreenFade>().onFadeOutEnd -= OpenInfluencerFromVideoView;

            ApplicationController.Instance().OpenInfluencerView(currentVideoDetail.userGid);

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


        // Seeking event handlers
        bool isScrubbing = false;
        public void OnSeekbarValueChanged(float val)
        {
            // Function called when seekbar value changed
            txtCurrentTime.text = TruncateMsToMin(val * currentVideoPlayer.mediaPlayer.Info.GetDurationMs());
        }

        public void SeekbarPointerUp()
        {
            // if (_wasPlayingOnScrub)
            // {
            // Seek to the position
            float scrubTo = videoSeekbar.value * currentVideoPlayer.mediaPlayer.Info.GetDurationMs();

            // Send analytics event
            AnalyticsManager.Instance().SendSeekbarAnalytics("seekerDrop",
                scrubTo / 1000,
                currentVideoPlayer.currentVideo.name,
                currentVideoPlayer.currentVideo.gid);

            currentVideoPlayer.mediaPlayer.Control.Seek(scrubTo);
            
            _wasPlayingOnScrub = false;

            // Update the texts
            txtBeginTime.text = txtCurrentTime.text;

            // Deactivate current time text
            txtCurrentTime.gameObject.SetActive(false);

            isScrubbing = false;

            // currentVideoPlayer.mediaPlayer.Control.Play();
            StartCoroutine(PlayVideoAfterScrub());

            // Check if should reposition is still false
            // This means that the pointer up was executed outside of the panel
            // Execute pointerup handler
            //if (!shouldReposition)
            //{
            //    OnPointerExit();
            //}

            // }
        }

        public void SeekbarPointerDown()
        {
            // Send analytics event
            AnalyticsManager.Instance().SendSeekbarAnalytics("seekerGrab",
                currentVideoPlayer.mediaPlayer.Control.GetCurrentTimeMs() / 1000,
                currentVideoPlayer.currentVideo.name,
                currentVideoPlayer.currentVideo.gid);

            // Change the flag
            isScrubbing = true;

            // Enable the current time text
            txtCurrentTime.gameObject.SetActive(true);

            // Disable auto disappearing on beginning
            shouldDisappear = false;

            // Change beginning and end times
            // _wasPlayingOnScrub = currentVideoPlayer.mediaPlayer.Control.IsPlaying();
            // if (_wasPlayingOnScrub)
            // {
            //     currentVideoPlayer.mediaPlayer.Control.Pause();
            // }
            currentVideoPlayer.mediaPlayer.Control.Pause();

            // Change play button
            btnPlay.spriteState = currentVideoPlayer.playState;
            btnPlay.GetComponent<Image>().sprite = currentVideoPlayer.playIdle;
        }

        #endregion

        #region PRIVATE METHODS

        private void PerformForcedFadeIn(float alpha, Action onFinished = null)
        {
            if (alpha > 1)
                alpha = 1;

            maxAlpha = alpha;

            // Disable active fading for the object
            // isFading = false;
            StopAllCoroutines();

            StartCoroutine(FadeIn(onFinished));
        }

        private void PerformForcedFadeOut(float alpha, Action onFinished = null)
        {
            if (alpha < 0)
                alpha = 0;

            minAlpha = alpha;

            // Disable active fading
            // isFading = false;
            StopAllCoroutines();

            StartCoroutine(FadeOut(onFinished));
        }

        IEnumerator FadeIn(Action a = null)
        {
            float currentAlpha;
            // isFading = true;
            CanvasGroup cg = GetComponent<CanvasGroup>();

            do
            {
                currentAlpha = maxAlpha;
                cg.alpha += fadeSpeed * Time.deltaTime;

                // Update the alpha 
                if (cg.alpha < currentAlpha)
                    currentAlpha = cg.alpha;

                // Wait for next frame
                yield return null;
            } while (currentAlpha < maxAlpha);

            if (a != null)
            {
                a();
            }

            cg.blocksRaycasts = true;
            cg.interactable = true;

            // Update state machine
            // isFading = false;
        }

        IEnumerator FadeOut(Action a = null)
        {
            float currentAlpha;
            // isFading = true;
            CanvasGroup cg = GetComponent<CanvasGroup>();


            do
            {
                currentAlpha = minAlpha;
                cg.alpha -= fadeSpeed * Time.deltaTime;

                if (cg.alpha > currentAlpha)
                    currentAlpha = cg.alpha;
                yield return null;

            } while (currentAlpha > minAlpha);

            if (a != null)
            {
                a();
            }

            cg.interactable = false;
            cg.blocksRaycasts = false;

            // Update state machine
            // isFading = false;
        }

        void InfluencerPictureLoaded()
        {
            btnInfluencer.interactable = true;
        }

        private double ConvertMillisecondsToMinutes(double milliseconds)
        {
            return TimeSpan.FromMilliseconds(milliseconds).TotalMinutes;
        }

        /*
        private string TruncateFloatToString(double d)
        {
            string str = d.ToString(@"hh\:mm\:ss\:fff");
            return Math.Round(d, 2).ToString().Replace('.', ':');
            // return string.Format("{0:00", val).Replace('.', ':');
        }
        */

        private string TruncateMsToMin(double ms)
        {
            TimeSpan t = TimeSpan.FromMilliseconds(ms);
            string answer = string.Format("{0:D2}:{1:D2}",
                t.Minutes,
                t.Seconds);
            return answer;
        }


        IEnumerator PlayVideoAfterScrub()
        {
            RenderHeads.Media.AVProVideo.MediaPlayer mp = currentVideoPlayer.mediaPlayer;
            while (mp != null && mp.TextureProducer != null && mp.TextureProducer.GetTextureFrameCount() <= 0
                && mp.Control.IsBuffering())
            {
                yield return null;
            }

            // Check if should reposition is still false
            // This means that the pointer up was executed outside of the panel
            // Execute pointerup handler

            if (pointerOut)
            {
                OnPointerExit();
            }

            currentVideoPlayer.mediaPlayer.Control.Play();

            // Change play button
            btnPlay.spriteState = currentVideoPlayer.pauseState;
            btnPlay.GetComponent<Image>().sprite = currentVideoPlayer.pauseIdle;
        }
        #endregion
    }
}