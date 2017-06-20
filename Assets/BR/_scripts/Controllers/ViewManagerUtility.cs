//
//  Code by: Parth Darji
//  Company: Boundless Reality
//  (c) Boundless Reality, All rights reserved.
//
//  Details: This script handles management of the views in the app
//
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using MA;
using BR.BRUtilities.UI;
using UnityEngine.UI;
using BR.BRUtilities;
using UnityEngine.EventSystems;
using RenderHeads.Media.AVProVideo;
using UnityEngine.Events;

namespace BR.App
{
    public class ViewManagerUtility : MonoBehaviour
    {
        #region INSTANTIATION

        private static ViewManagerUtility _instance = null;

        private static bool Exists()
        {
            return _instance != null;
        }

        public static ViewManagerUtility Instance()
        {
            if (!Exists())
            {
                throw new Exception("ViewManager object not found");
            }
            return _instance;
        }

        #endregion

        #region VARIABLES

        [SerializeField] private GameObject UICanvas;
        [SerializeField] private VideoPlayerMenu VideoMenuCanvas;
        [SerializeField] private CanvasObject baseObject;
        [SerializeField] private GameObject appBackground;
        [SerializeField] private CanvasObject influencerPrefab, errorPrefab, homePrefab;
        [SerializeField] private Camera eventCamera;
        [SerializeField] private GameObject loadingObject;

        private GameObject currentVideoPlayerGO;
        // Action onUIScreenFadeEndDelegate;
        public Text debugText;

        #endregion

        #region UNITY MONOBEHAVIOUR METHODS

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                // DontDestroyOnLoad (this.gameObject);

                ViewObjectStack = new Stack();
            }
        }

        void OnDestroy()
        {
            _instance = null;
        }

        #endregion

        #region DATA VARIABLES

        [SerializeField] private float CanvasDistance = 5f;
        [SerializeField] private float BaseDistance = 15f;
        [SerializeField] private TextAsset[] JSONFiles;
        private Stack ViewObjectStack;

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Setups the influencer view. Loads both canvases in the influencer object
        /// </summary>
        /// <param name="influencer">Influencer.</param>
        public void SetupInfluencerView(InfluencerDetail influencer)
        {
            if (influencerPrefab == null)
            {
                return;
            }

            // The view can be instantiated only on the UI canvas
            GoToViewObject(ViewObject.ObjectType.UI);

            // Check if the last canvas on the canvas is an influencer object
            CanvasObject c = GetLastViewObject().canvasStack.Peek() as CanvasObject;

            CanvasObject influencerObject;
            if (c.canvasType == CanvasObject.CanvasType.INFLUENCER)
            {
                // Debug.Log ("influencer canvas is the last canvas");
                influencerObject = c;

                // If the influencer panel is already loaded, delete all videos from the list
                influencerObject.GetComponentInChildren<VideoPanelLoader>().RemoveAllVideos();

                // Remove the playlist
                VideoPlaylistManager.Instance().RemoveLastList();
            }
            else
            {

                // Instantiate the Object
                influencerObject = (CanvasObject)Instantiate(influencerPrefab, UICanvas.transform);

                // Set the position of the object
                (influencerObject.transform as RectTransform).sizeDelta = Vector3.zero;

                // Set scale of the object
                // influencerObject.transform.localScale = Vector3.one;

                DoBasicSetup(influencerObject);

                // Add the object to stack
                AddCanvasToView(influencerObject, ViewObject.ObjectType.UI);
            }

            // influencerObject.GetComponent<InfluencerPageLoader> ().SetupInfluencerPage (influencer);
            StartCoroutine(influencerObject.GetComponent<InfluencerPageLoader>().SetupInfluencerPageOptimized(influencer));

            /*
			// Set the influencer profile view
			// influencerObject.GetComponentInChildren<InfluencerProfileLoader>().SetupInfluencerProfile(influencer);
			// Set the json for videos panel
			*/
        }

        public void SetupErrorView(ErrorDetail ed)
        {
            SetupErrorView(ed, Vector3.zero, Quaternion.identity);
        }

        /// <summary>
        /// Setups the error view.
        /// </summary>
        /// <param name="ed">Error details</param>
        public void SetupErrorView(ErrorDetail ed, Vector3 oldPos, Quaternion oldRot)
        {
            if (ed == null)
                return;

            // Get the last view object
            ViewObject v = GetLastViewObject();

            if (v != null)
            {
                // Check if error canvas is the last canvas
                if (v.canvasStack.Count > 0)
                {
                    CanvasObject lastobject = (CanvasObject)v.canvasStack.Peek();
                    if (lastobject.canvasType == CanvasObject.CanvasType.ERROR)
                        return;
                }

                // Instantiate the object
                CanvasObject errorObject = (CanvasObject)Instantiate(errorPrefab);

                // Get a new pos only if asked for
                if (oldPos == Vector3.zero)
                {
                    // Change position of the menu to in front of the camera
                    errorObject.transform.localPosition = Camera.main.transform.forward * CanvasDistance * 2;
                    errorObject.transform.rotation = Quaternion.LookRotation(errorObject.transform.position - Camera.main.transform.position);
                }
                else
                {
                    // Use the old pos
                    errorObject.transform.localPosition = oldPos;
                    errorObject.transform.rotation = oldRot;
                }

                // Set the camera for this canvas
                errorObject.GetCanvas().worldCamera = eventCamera;

                // Setup the close button
                Transform btnClose = UIHelper.FindDeepChild(errorObject.transform, "BtnClose");

                if (btnClose != null)
                {
                    btnClose.GetComponent<Button>().onClick.AddListener(delegate
                    {
                        AnalyticsManager.Instance().SendButtonClickAnalytics("interaction", "CloseButton", "Close On Error View");
                        this.RemoveCanvasFromCurrentView();
                    });
                }

                // DoBasicSetup (errorObject);

                // Setup the ui for the error panel
                // Get the text elements
                Text errorTitle = UIHelper.FindDeepChild(errorObject.transform, "ErrorTitle").GetComponent<Text>();
                Text errorDescription = UIHelper.FindDeepChild(errorObject.transform, "ErrorDescription").GetComponent<Text>();

                errorTitle.text = ed.errorTitle;
                errorDescription.text = ed.errorDescription;

                foreach (ErrorDetail.ResponseType response in ed.ResponseDict.Keys)
                {
                    Transform responseButton = null;

                    // The dictionary contains all the keys that go the error
                    switch (response)
                    {
                        case ErrorDetail.ResponseType.ACCEPT:
                            responseButton = UIHelper.FindDeepChild(errorObject.transform, "BtnAccept");
                            responseButton.gameObject.SetActive(true);
                            break;
                        case ErrorDetail.ResponseType.CANCEL:
                            responseButton = UIHelper.FindDeepChild(errorObject.transform, "BtnCancel");
                            responseButton.gameObject.SetActive(true);
                            break;
                        case ErrorDetail.ResponseType.DECLINE:
                            responseButton = UIHelper.FindDeepChild(errorObject.transform, "BtnReject");
                            responseButton.gameObject.SetActive(true);
                            break;
                        case ErrorDetail.ResponseType.RETRY:
                            responseButton = UIHelper.FindDeepChild(errorObject.transform, "BtnRetry");
                            responseButton.gameObject.SetActive(true);
                            break;
                        
                        case ErrorDetail.ResponseType.SETTINGS:
                            responseButton = UIHelper.FindDeepChild(errorObject.transform, "BtnSettings");
                            responseButton.gameObject.SetActive(true);
                            break;
                        case ErrorDetail.ResponseType.IGNORE:
                            responseButton = UIHelper.FindDeepChild(errorObject.transform, "BtnIgnore");
                            responseButton.gameObject.SetActive(true);
                            break;
                        case ErrorDetail.ResponseType.RESTART:
                            responseButton = UIHelper.FindDeepChild(errorObject.transform, "BtnRestart");
                            responseButton.gameObject.SetActive(true);
                            break;
                        case ErrorDetail.ResponseType.EXIT:
                            responseButton = UIHelper.FindDeepChild(errorObject.transform, "BtnExit");
                            responseButton.gameObject.SetActive(true);
                            break;
                    }

                    if (responseButton != null)
                    {
                        // Set the action for the button
                        UnityAction currentAction;
                        ed.ResponseDict.TryGetValue(response, out currentAction);
                        responseButton.GetComponent<Button>().onClick.AddListener(delegate
                        {
                            AnalyticsManager.Instance().SendButtonClickAnalytics("interaction", "ErrorButton", responseButton.name);
                            // Remove this view from stack and perform the action
                            // RemoveCanvasFromCurrentView();
                            currentAction();
                        });
                    }
                }
                // Get the last view object
                AddCanvasToView(errorObject, GetLastViewObject().objectType);
                // AddCanvasToView(errorObject, ViewObject.ObjectType.UI);

                // Keep the error canvas seperate from the canvas stack :/
            }
        }

        public void CloseErrorView()
        {
            GameObject errorObject = GameObject.Find("ErrorCanvas(Clone)");
            if(errorObject != null)
            {
                Destroy(errorObject);
            }
        }
        public VideoPlayerMenu GetVideoPlaybackMenu()
        {
            return VideoMenuCanvas;
        }

        /// <summary>
        /// The first task to do when the app starts. Setups the home view.
        /// Adds views to stack and sends data to HomePageLoader for data processing
        /// </summary>
        public void SetupHomeView()
        {

            // // Instantiate the home panel
            // CanvasObject homeObject = (CanvasObject)Instantiate (homePrefab, Vector3.zero, Quaternion.identity, UICanvas.transform);
            // (homeObject.transform as RectTransform).anchoredPosition3D = Vector3.zero;
            // homeObject.transform.localScale = Vector3.one;
            // (homeObject.transform as RectTransform).sizeDelta = Vector2.zero;

            //homeObject.transform.localScale = Vector3.one;

            CanvasObject homeObject = homePrefab.GetComponent<CanvasObject>();
            // Set the background
            GameObject background = (GameObject)Instantiate(appBackground, UICanvas.transform);

            // Create a new viewobject for that stack
            ViewObject v = new ViewObject(ViewObject.ObjectType.UI);

            // Add homeobject to the viewobject canvas stack
            v.AddToCanvasStack(homeObject);

            // Add background to viewobject
            v.AddObjectToView(background);

            // First instantiation, add the ui object to stack
            ViewObjectStack.Push(v);

            // Do entitlement check
            Oculus.Platform.Core.AsyncInitialize("1636213263075199");

            // Check entitlement state
            Oculus.Platform.Entitlements.IsUserEntitledToApplication().OnComplete(EntitlementCheckCallback);
            // StartCoroutine(homePrefab.GetComponent<HomePageLoader>().SetupHomeScreenOptimized());

            // Home page is setup in entitlement check handler          
        }

        /// <summary>
        /// Decides the type of the video, instantiates appropriate video player
        /// Removes previous object in view
        /// Adds videoplayer to the view stack
        /// Sends data to VideoPlayerMenu script for ui setup
        /// </summary>
        /// <param name="video">Video.</param>
        public void SetupVideoView(VideoDetail video, GameObject videoPrefab)
        {
            // Disable interaction with the current canvas
            ViewObject currentViewObject = GetLastViewObject();

            if (currentViewObject != null)
            {
                foreach (CanvasObject canvas in currentViewObject.canvasStack.ToArray())
                {
                    canvas.GetComponent<CanvasGroup>().interactable = false;
                    canvas.GetComponent<CanvasGroup>().blocksRaycasts = false;
                }
            }

            // Setup the camera for the menu
            // currentVideoPlayerGO.GetComponentInChildren<Canvas>().worldCamera = eventCamera;

            // Set the fade out end listener
            Camera.main.GetComponent<VRScreenFade>().onFadeOutEnd += OpenVideoView;

            FadeAllCamerasOut();

            // Instantiate the video player
            currentVideoPlayerGO = Instantiate(videoPrefab);
            // Get the videoplayer controller and setup the player while view is fading
            currentVideoPlayerGO.GetComponent<VideoPlayerManager>().currentVideo = video;
        }

        /// <summary>
        /// Closes the video view.
        /// </summary>
        public void CloseVideoView()
        {
            // Switch the cursor back to non-loading
            CUIGazePointer.instance.HideLoadingReticle();

            // Enable the mesh collider on the base object
            UICanvas.GetComponent<MeshCollider>().enabled = true;

            Camera.main.GetComponent<VRScreenFade>().onFadeOutEnd += ReopenLastView;

            // Clear variables on the player
            // FindObjectOfType<VideoPlayerManager>().mediaPlayer.Stop();
            // FindObjectOfType<VideoPlayerManager>().mediaPlayer.CloseVideo();
            // FindObjectOfType<VideoPlayerManager>().currentVideo = null;

            // Fade out current view and reopen previous view when done
            FadeAllCamerasOut();

            // Stop the video player
            MediaPlayer mp = FindObjectOfType(typeof(MediaPlayer)) as MediaPlayer;
            if (mp != null)
            {
                mp.Control.Stop();
            }
        }

        public void SwitchVideoView(VideoDetail newVideo)
        {
            Camera.main.GetComponent<VRScreenFade>().onFadeOutEnd += SwitchVideoPlayer;

            FadeAllCamerasOut();

            // Stop the video player
            MediaPlayer mp = FindObjectOfType(typeof(MediaPlayer)) as MediaPlayer;
            if (mp != null)
            {
                mp.Control.Stop();
            }
        }

        /// <summary>
        /// Removes the last canvas from current view stack.
        /// </summary>
        public void RemoveCanvasFromCurrentView(UnityAction actionOnClose = null)
        {
            ViewObject v = ViewObjectStack.Peek() as ViewObject;
            // Disable any active selectable
            if (v.canvasStack.Count > 0)
            {
                // Perform Close action
                CanvasObject lastObject = v.canvasStack.Pop() as CanvasObject;
                if (lastObject != null)
                {
                    // Check if the last canvas was the influencer canvas
                    // If it was, remove the playlist
                    if (lastObject.canvasType == CanvasObject.CanvasType.INFLUENCER)
                    {
                        VideoPlaylistManager.Instance().RemoveLastList();
                    }
                    /*
                    // Destroy the canvas if it is not a loading wheel
                    if (lastObject.canvasType != CanvasObject.CanvasType.LOADINGWHEEL)
                    {
                        lastObject.GetMenu().OnClose.AddListener(
                            () =>
                            {
                                // lastObject.gameObject.SetActive(false);
                                Destroy(lastObject.gameObject);
                                // EventSystem.current.SetSelectedGameObject(null);
                            });
                    }
                    else
                    {*/
                    if (v.objectType == ViewObject.ObjectType.VIDEO && lastObject.canvasType == CanvasObject.CanvasType.ERROR)
                    {
                        if(actionOnClose != null)
                        {
                            lastObject.GetMenu().close(delegate
                            {
                                Destroy(lastObject.gameObject);
                                CloseVideoView();
                            });
                        }
                    }
                    else
                    {
                        if (actionOnClose != null)
                        {
                            lastObject.GetMenu().OnClose.AddListener(actionOnClose);
                        }
                        else
                        {
                            // Open the previous menu
                            lastObject.GetMenu().OnClose.AddListener(
                            () =>
                            {
                            // lastObject.gameObject.SetActive(false);
                            Destroy(lastObject.gameObject);
                            // EventSystem.current.SetSelectedGameObject(null);
                        });
                            lastObject.GetMenu().returnToHigherMenuInstanceMethod();
                        }
                    }
                    //}
                }
            }
            else
            {
                Debug.Log("Nothing to pop");
            }
        }

        /// <summary>
        /// This is a viewhandler for a back button pressed on the application
        /// </summary>
        public void BackButtonPressed()
        {
            // Change the reticle to blue regardless
            CUIGazePointer.instance.HideLoadingReticle();

            // Find the last viewobject type
            ViewObject lastViewObject = GetLastViewObject();
            Stack canvasStack = lastViewObject.canvasStack;
            
            switch (lastViewObject.objectType)
            {
                case ViewObject.ObjectType.UI:
                    // The first canvas is the home canvas which cannot be removed from view
                    if (canvasStack.Count > 1)
                    {
                        CanvasObject lastUIObject = canvasStack.Peek() as CanvasObject;
                        HomePageLoader hpl = FindObjectOfType<HomePageLoader>();
                        if (lastUIObject.canvasType == CanvasObject.CanvasType.ERROR &&
                            hpl != null && hpl.homePageLoaded == false)
                        {
                            // Home page not loaded
                            // Promt to exit the app
                            OVRManager.PlatformUIConfirmQuit();
                        }
                        else
                        {
                            // Play the sound
                            AudioController.Instance().PlayOneShot(ApplicationController.Instance().influencerBackClip);
                            RemoveCanvasFromCurrentView();
                        }
                    }
                    else
                    {
                        // Go to oculus ui
                        Debug.Log("Oculus UI");
                        // OVRManager.PlatformUIGlobalMenu();
                        OVRManager.PlatformUIConfirmQuit();
                    }
                    break;
                case ViewObject.ObjectType.VIDEO:
                    CanvasObject lastObject = null;
                    if(canvasStack.Count > 0)
                        lastObject = canvasStack.Peek() as CanvasObject;

                    if (lastObject != null && lastObject.canvasType == CanvasObject.CanvasType.ERROR)
                    {
                        RemoveCanvasFromCurrentView(new UnityAction(
                                () => CloseVideoView()));
                    }
                    else
                    {
                        // Get the video player manager
                        VideoPlayerManager _videoPlayer = currentVideoPlayerGO.GetComponentInChildren<VideoPlayerManager>();
                        // Handle analytics
                        // Get the first video category
                        string cat = (_videoPlayer.currentVideo.categories != null) ? _videoPlayer.currentVideo.categories[0].ToString() : "";

                        if (_videoPlayer != null)
                        {
                            AnalyticsManager.Instance().SendVideoAnalytics(_videoPlayer.currentVideo.name,
                                //(_videoPlayer.mediaPlayer.Control.GetCurrentTimeMs() / _videoPlayer.mediaPlayer.Info.GetDurationMs()) * 100,
                                _videoPlayer.videoPlayerMenu.totalTimeSpent / _videoPlayer.mediaPlayer.Info.GetDurationMs() * 100,
                                cat,
                                _videoPlayer.currentVideo.userHandle,
                                _videoPlayer.currentVideo.gid);
                        }

                        // Play audio
                        AudioController.Instance().PlayOneShot(ApplicationController.Instance().videoBackClip);
                        CloseVideoView();
                    }
                    break;
            }

        }

        /// <summary>
        /// Sets the event handler of the main camera and fades all the cameras
        /// </summary>
        /// <param name="a">The alpha component.</param>
        public void FadeAllCamerasIn()
        {
            // Fade all cameras
            foreach (Camera cam in UIHelper.GetAllMainCameras())
            {
                cam.GetComponent<VRScreenFade>().PerformFadeIn();
            }
        }

        /// <summary>
        /// Sets the event handler of the main camera and fades all the cameras
        /// </summary>
        /// <param name="a">The alpha component.</param>
        public void FadeAllCamerasOut()
        {
            // Fade all cameras
            foreach (Camera cam in UIHelper.GetAllMainCameras())
            {
                cam.GetComponent<VRScreenFade>().PerformFadeOut();
            }
        }

        /// <summary>
        /// Shows/Hides the loading wheel.
        /// </summary>
        /// <param name="_show">If set to <c>true</c> show.</param>
        public void ShowLoadingWheel(bool _show)
        {
            if (loadingObject == null)
                return;

            if (_show)
            {
                /*
				CanvasObject loadingObject = (CanvasObject)Instance (loadingPrefab, eventCamera);

				// Get the last view object
				ViewObject v = GetLastViewObject ();
				float posZ = BaseDistance - ((v.canvasStack.Count + 1) * CanvasDistance);

				// Change position of the menu to in front of the camera
				Vector3 canvasPos = loadingObject.transform.localPosition = Camera.main.transform.forward * posZ;
				loadingObject.transform.rotation = Quaternion.LookRotation (loadingObject.transform.position - Camera.main.transform.position);

				// Set the object to active
				*/
                //loadingObject.gameObject.SetActive (true);

                AddCanvasToView(loadingObject.GetComponent<CanvasObject>(), GetLastViewObject().objectType);

            }
            else
            {
                //loadingObject.gameObject.SetActive (false);
                RemoveCanvasFromCurrentView();
            }
        }

        public void SortHomeViewByCategory(VideoDetail.Category category)
        {
            HomePageLoader homePage = FindObjectOfType<HomePageLoader>();
            if (homePage != null)
            {
                homePage.SortByCategories(category);
            }
        }

        public void SetCanvasCamera(Canvas c)
        {
            c.worldCamera = eventCamera;
        }

        #endregion

        #region PRIVATE METHODS

        /// <summary>
        /// This method sets the position of the canvas, event camera, and Close button behaviour
        /// </summary>
        /// <param name="obj">CanvasObject to be setup</param>
        private void DoBasicSetup(CanvasObject obj)
        {

            // Get the last view object
            ViewObject v = GetLastViewObject();

            float posZ = BaseDistance - ((v.canvasStack.Count + 1) * CanvasDistance);
            Vector3 objPosition = new Vector3(obj.transform.localPosition.x, obj.transform.localPosition.y, posZ);
            obj.transform.localPosition = objPosition;

            // Set the camera for this canvas
            if (obj.GetCanvas() != null)
                obj.GetCanvas().worldCamera = eventCamera;

            // Setup the close button
            Transform btnClose = UIHelper.FindDeepChild(obj.transform, "BtnClose");

            if (btnClose != null)
            {
                btnClose.GetComponent<Button>().onClick.AddListener(delegate
                {
                    AnalyticsManager.Instance().SendButtonClickAnalytics("interaction", "CloseButton", "Close On Creator View");
                    this.RemoveCanvasFromCurrentView();
                });
            }
        }

        /// <summary>
        /// Checks if the last view is the requested view. Adds canvasobject to the view if true
        /// </summary>
        /// <param name="obj">Object.</param>
        /// <param name="type">Type.</param>
        private void AddCanvasToView(CanvasObject obj, ViewObject.ObjectType type)
        {
            ViewObject v = GetLastViewObject();

            if (v.objectType != type)
                return;

            // If there is a previous object,
            // make this object the next menu
            CanvasObject previousObject;
            if (v.canvasStack.Count > 0)
            {
                previousObject = v.canvasStack.Peek() as CanvasObject;
                // Set the previous menu for this menu
                obj.GetMenu().SetHigherMenu(previousObject.GetMenu());

                // Open this menu
                previousObject.GetMenu().openNext(obj.GetMenu());
            }
            else if(v.objectType == ViewObject.ObjectType.UI)
            {
                previousObject = baseObject;
                // Set the previous menu for this menu
                obj.GetMenu().SetHigherMenu(previousObject.GetMenu());

                // Open this menu
                previousObject.GetMenu().openNext(obj.GetMenu());
            } else
            {
                // We are in video view
                // Just set the error canvas active
            }

            /*
            // Set the previous menu for this menu
            obj.GetMenu().SetHigherMenu(previousObject.GetMenu());

            // Open this menu
            previousObject.GetMenu().openNext(obj.GetMenu());
            */
            // Push new object to the end of the stack
            v.canvasStack.Push(obj);
            
        }

        /// <summary>
        /// Pops canvases from view until a view of particular type is found. Returns true if successful
        /// </summary>
        /// <param name="type">View Object type</param>
        private bool GoToViewObject(ViewObject.ObjectType type)
        {
            while (ViewObjectStack.Count > 0)
            {
                ViewObject v = GetLastViewObject();
                if (v.objectType == type)
                    return true;
                else
                    RemovePreviousObjectFromViewStack(true);
            }
            return false;
        }

        /// <summary>
        /// Adds a viewobject to the stack. Whenever a new viewobject is added,
        /// Everything on the previous object is disabled.
        /// </summary>
        /// <param name="v">V.</param>
        private void AddViewObjectToStack(ViewObject v)
        {
            ViewObjectStack.Push(v);
        }

        /// <summary>
        /// Removes the last object from stack. 
        /// After removal, everything on the previous object is brought back to view
        /// </summary>
        /// <param name="delete">If set to <c>true</c> delete the previous object from stack.</param>
        private void RemovePreviousObjectFromViewStack(bool delete = true)
        {
            ViewObject lastViewObjectStack = GetLastViewObject();

            if (delete)
            {
                while (lastViewObjectStack.canvasStack.Count > 0)
                {
                    Destroy(lastViewObjectStack.canvasStack.Pop() as CanvasObject);
                }

                foreach (GameObject go in lastViewObjectStack.objectsinView)
                    Destroy(go);

                // Pop the object
                ViewObjectStack.Pop();
            }
            else
            {
                foreach (CanvasObject c in lastViewObjectStack.canvasStack.ToArray())
                {
                    c.gameObject.SetActive(false);
                }
                foreach (GameObject go in lastViewObjectStack.objectsinView)
                {
                    go.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Checks for the object type and if it matches the last object, removes the pevious object from view stack.
        /// </summary>
        /// <param name="objectType">Object type.</param>
        /// <param name="delete">If set to <c>true</c> delete.</param>
        private void RemovePreviousObjectFromViewStack(ViewObject.ObjectType objectType, bool delete = true)
        {
            ViewObject lastViewObject = GetLastViewObject();

            if (lastViewObject.objectType == objectType)
            {
                RemovePreviousObjectFromViewStack(delete);
            }
            else
            {
                Debug.Log("Last object is not of type: " + objectType.ToString());
            }
        }

        /// <summary>
        /// Reopens UIObject after video close.
        /// This must be called after a view is removed from the stack.
        /// </summary>
        private void ReopenLastView()
        {
            // This method is called from the view currently active
            // Called after the fading has ended

            // Handle the video player 
            // Clear variables on the player
            VideoPlayerManager videoManager = FindObjectOfType<VideoPlayerManager>();
            if (videoManager != null)
            {
                videoManager.mediaPlayer.Stop();
                videoManager.mediaPlayer.CloseVideo();
                videoManager.currentVideo = null;
            }

            // Reset the reticle
            CUIGazePointer.instance.HideLoadingReticle();

            // Reorient the screen
            UnityEngine.VR.InputTracking.Recenter();

            // Recenter the controller
            OVRInput.RecenterController();

            // Remove handler
            Camera.main.GetComponent<VRScreenFade>().onFadeOutEnd -= ReopenLastView;

            // Check if the video view is the last object in the viewobject stack
            // If it is, remove it from view and destroy the object
            RemovePreviousObjectFromViewStack(ViewObject.ObjectType.VIDEO, true);

            ViewObject lastObject = GetLastViewObject();

            CanvasObject cLast = lastObject.canvasStack.Peek() as CanvasObject;
            cLast.GetComponent<CanvasGroup>().interactable = true;
            cLast.GetComponent<CanvasGroup>().blocksRaycasts = true;

            foreach (CanvasObject c in lastObject.canvasStack.ToArray())
            {
                // Make all canvases interactable again
                c.gameObject.SetActive(true);
            }
            foreach (GameObject go in lastObject.objectsinView)
            {
                go.SetActive(true);
            }

            // Start fading in                               
            FadeAllCamerasIn();
        }

        private void SwitchVideoPlayer()
        {
            Camera.main.GetComponent<VRScreenFade>().onFadeOutEnd -= SwitchVideoPlayer;

            // Get a copy of the next video
            VideoDetail nextVid = FindObjectOfType<VideoPlayerManager>().nextVideo;

            // Close the current player
            RemovePreviousObjectFromViewStack(ViewObject.ObjectType.VIDEO, true);

            // Start fading in
            FadeAllCamerasIn();

            ApplicationController.Instance().OpenVideoPlayer(nextVid);
        }

        /// <summary>
        /// Returns the last viewobject in stack. NULL otherwise.
        /// </summary>
        /// <returns>The last view object.</returns>
        private ViewObject GetLastViewObject()
        {
            if (ViewObjectStack.Count > 0)
                return ViewObjectStack.Peek() as ViewObject;
            else
            {
                Debug.Log("No viewobjects in stack.");
                // Returning base ui canvas
                return null;
            }
        }

        /*
		/// <summary>
		/// Event raised when view has faded out from UI
		/// Open the video view here
		/// </summary>
		private void OnFadeOutFromUIView(GameObject videoObject, ViewObject viewObject, VideoDetail video) {
			// Remove the event listener
			Camera.main.GetComponent<VRScreenFade>().onFadeOutEnd -= new VRScreenFade.OnFadeOutEnd(null);

			// Add the menu to the canvas stack
			viewObject.AddToCanvasStack(videoObject.GetComponentInChildren<CanvasObject>());

			// Add the gameobject to the viewobject
			viewObject.AddObjectToView(videoObject);

			// Disable current view
			// RemovePreviousObjectFromViewStack(false);

			// Add viewobject to stack
			AddViewObjectToStack(viewObject);

			// Get the videoplayer controller and setup the player
			VideoPlayerManager videoManager = videoObject.GetComponent<VideoPlayerManager>();
			videoManager.SetupVideoPlayer (video);
		}*/

        /// <summary>
        /// Opens the video view. This method is an event handler for OnFadeOutEnd
        /// </summary>
        private void OpenVideoView()
        {
            // Acivate the video player
            currentVideoPlayerGO.SetActive(true);

            // Remove the mesh on the base canvas
            UICanvas.GetComponent<MeshCollider>().enabled = false;

            // Create a new viewobject for video
            ViewObject v = new ViewObject(ViewObject.ObjectType.VIDEO);

            // Remove the listener
            Camera.main.GetComponent<VRScreenFade>().onFadeOutEnd -= OpenVideoView;

            // Add the menu to the canvas stack
            // v.AddToCanvasStack(currentVideoPlayerGO.GetComponentInChildren<CanvasObject>());

            // Add the gameobject to the viewobject
            v.AddObjectToView(currentVideoPlayerGO);

            // Disable current view
            RemovePreviousObjectFromViewStack(false);

            // Add viewobject to stack
            AddViewObjectToStack(v);

            currentVideoPlayerGO.GetComponent<VideoPlayerManager>().SetupVideoPlayer();
        }

        private void SetupVideoPlayer()
        {

        }

        void EntitlementCheckCallback(Oculus.Platform.Message msg)
        {
            if (!msg.IsError)
            {
                // Entitlement check passed
                // Load home page
                // Check for internet 
                if (!(Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork))
                {
                    // Internet unreachable, show error
                    ErrorDetail ed = new ErrorDetail();
                    ed.SetErrorTitle("Wi-Fi Unavailable");
                    ed.SetErrorDescription("We could not detect a Wi-Fi " +
                        "connection. To continue, please check your Wi-Fi connection, " +
                        "and try again.");

                    // Associate this error detail with actions
                    ed.AddToDictionary(ErrorDetail.ResponseType.SETTINGS, new UnityEngine.Events.UnityAction(delegate
                    {
                        OVRManager.PlatformUIGlobalMenu();
                    }));

                    ed.AddToDictionary(ErrorDetail.ResponseType.RETRY, new UnityEngine.Events.UnityAction(delegate
                    {
                        StartCoroutine(ApplicationController.Instance().CheckForInternetAgain(1f, new UnityAction(delegate
                        {
                            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, UnityEngine.SceneManagement.LoadSceneMode.Single);
                        })));
                    }));

                    ApplicationController.Instance().OpenErrorView(ed);
                }
                else
                {
                    StartCoroutine(homePrefab.GetComponent<HomePageLoader>().SetupHomeScreenOptimized());
                }
            }
            else
            {
                // Entitlement check failed
                ShowEntitlementCheckError();

            }
        }

        void ShowEntitlementCheckError()
        {
            ErrorDetail ed = new ErrorDetail();
            ed.SetErrorTitle("Unauthorized Access");
            ed.SetErrorDescription("We are unable to verify the app. Please log in or check your internet connection and try again. " +
                "If the problem persists, please try re-installing the app from the Oculus store.");

            // Associate this error detail with an action
            // ed.AddToDictionary(ErrorDetail.ResponseType.RETRY, CheckForInternetAgain);
            ed.AddToDictionary(ErrorDetail.ResponseType.EXIT, new UnityEngine.Events.UnityAction(delegate
            {
                BackButtonPressed();
                OVRManager.PlatformUIConfirmQuit();
                // OVRManager.PlatformUIGlobalMenu();
            }));

            SetupErrorView(ed);
        }

        // This function waits for the videoview to close
        IEnumerator CloseErrorAndSetNextObject()
        {
            yield return new WaitForSeconds(1);

        }
        #endregion
    }
}

/*
		private void AddCanvasToUIView(CanvasObject obj) {
			// If there is a previous object,
			// make this object the next menu
			CanvasObject previousObject;
			if (currentUIObject.canvasStack.Count > 0) {
				previousObject = currentUIObject.canvasStack.Peek () as CanvasObject;
			} else {
				previousObject = baseObject;
			}

			// Set the previous menu for this menu
			obj.GetMenu ().SetHigherMenu (previousObject.GetMenu ());

			// Open this menu
			previousObject.GetMenu ().openNext (obj.GetMenu ());

			// Push new object to the end of the stack
			currentUIObject.canvasStack.Push(obj);
		}

		public void RemoveObjectFromStack() {
			// Disable any active selectable

			if (CanvasStack.Count > 0) {
				// Perform Close action
				CanvasObject lastObject = CanvasStack.Pop () as CanvasObject;
				if (lastObject != null) {
					lastObject.GetMenu ().OnClose.AddListener (
						() =>  { 
							Destroy (lastObject.gameObject);
							EventSystem.current.SetSelectedGameObject(null);
						});
					// Open the previous menu
					lastObject.GetMenu ().returnToHigherMenuInstanceMethod ();
				}
			} else {
				Debug.Log ("Nothing to pop");
			}
		}*/

