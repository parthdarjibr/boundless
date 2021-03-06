﻿//
//  Code by: Parth Darji
//  Company: Boundless Reality
//  (c) Boundless Reality, All rights reserved.
//
//  Details: This class controls the application flow.
//			For view objects, it creates the views and sends them 
//			to the ViewManager class
//
using UnityEngine;
using System;
using BR.BRUtilities;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Events;

namespace BR.App
{
    public class ApplicationController : MonoBehaviour
    {
        #region INSTANTIATION

        private static ApplicationController _instance = null;

        private static bool Exists()
        {
            return _instance != null;
        }

        public static ApplicationController Instance()
        {
            if (!Exists())
            {
                throw new Exception("ApplicationController object not found");
            }
            return _instance;
        }

        #endregion

        #region VARIABLES

        public bool isDebug = true;
        public TextAsset[] JSONFiles;
        public float RenderScale = 1.6f;
        public int AntiAliasing = 4;
        public GameObject videoPrefabMono, videoPrefabStereo, videoPrefabVol;
        public AudioClip hoverAudioClip, videoClickAudioClip, influencerClickAudioClip, interactionClickAudioClip;
        public AudioClip videoBackClip, influencerBackClip;
        [Tooltip("Time in seconds for recentering the view")]
        public float longClickTime = 1.5f;
        private float clickTime = 0f;
        private bool clicking = false;

        // Internet variables
        // These are used on the event of retrying for connection
        bool internetStatusSet = false;
        bool internetAvailable = false;

        #endregion

        #region UNITY MONO METHODS

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            // DontDestroyOnLoad (this.gameObject);
            //			Debug.Log (UnityEngine.VR.VRSettings.renderScale);
            //			Debug.Log (QualitySettings.antiAliasing);

            UnityEngine.VR.VRSettings.renderScale = RenderScale;
            // QualitySettings.antiAliasing = AntiAliasing;
            QualitySettings.vSyncCount = 0;

            // Create a unique ID for the user upon start
            // if it doesn't exist in PlayerPrefs
            if (!PlayerPrefs.HasKey("UniqueID"))
            {
                // string UUID = System.Guid.NewGuid ().ToString ();
                string UUID = SystemInfo.deviceUniqueIdentifier;
                PlayerPrefs.SetString("UniqueID", UUID);
            }
        }

        void OnDestroy()
        {
            _instance = null;
        }

        void Start()
        {
            // Setup the home page as application Starts
            ViewManagerUtility.Instance().SetupHomeView();
        }

        private float timeSinceLastCalled;

        private float delay = 5f;

        void Update()
        {

            if (Input.GetKeyUp(KeyCode.A))
            {
                // UnityEngine.SceneManagement.SceneManager.UnloadScene(0);
                // UnityEngine.SceneManagement.SceneManager.LoadScene(0);
                //Application.LoadLevel(Application.loadedLevel);
                SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
            }

            if (OVRInput.GetDown(OVRInput.Button.Two))
            {
                // Start click timer
                clickTime = Time.time;
                clicking = true;
            }

            if (OVRInput.Get(OVRInput.Button.Two) && clicking == true)
            {
                // Check for timer
                if (Time.time - clickTime > longClickTime)
                {
                    // This is a long press
                    // Recenter controller
                    // UnityEngine.VR.InputTracking.Recenter();
                    // OVRManager.PlatformUIConfirmQuit();
                    OVRManager.PlatformUIGlobalMenu();

                    // Reset timer variables
                    clicking = false;
                    return;
                }
            }

#if UNITY_EDITOR
                if(Input.GetKeyUp(KeyCode.Escape)) {
#elif UNITY_ANDROID
                if (OVRInput.GetUp(OVRInput.Button.Two))
                {
#endif
                    // Reset timer variables
                    clicking = false;

                // Handle analytics
                AnalyticsManager.Instance().SendButtonClickAnalytics("interaction", "Hardware Back", "Hardware Back Button");

                ViewManagerUtility.Instance().BackButtonPressed();
            }


            #region GC CODE

            timeSinceLastCalled += Time.deltaTime;
            if (timeSinceLastCalled > delay)
            {
                System.GC.Collect();
                timeSinceLastCalled = 0f;
            }
            #endregion
        }

/*
        void OnApplicationPause(bool pauseState)
        {
            if (!pauseState)
            {
                // Check internet every time 
                if (!ConnectionManager.Instance().isNetworkAvailable)
                {
                    ErrorDetail ed = new ErrorDetail();
                    ed.SetErrorTitle("Wi-Fi Unavailable");
                    ed.SetErrorDescription("We could not detect a Wi-Fi " +
                        "connection. To continue, please check your Wi-Fi connection, " +
                        "and try again.");

                    // Associate this error detail with actions
                    ed.AddToDictionary(ErrorDetail.ResponseType.IGNORE, new UnityEngine.Events.UnityAction(delegate
                    {
                        ViewManagerUtility.Instance().RemoveCanvasFromCurrentView();
                        //OVRManager.PlatformUIGlobalMenu();
                    }));

                    ed.AddToDictionary(ErrorDetail.ResponseType.RESTART, new UnityEngine.Events.UnityAction(delegate
                    {
                        StartCoroutine(CheckForInternetAgain(2f, new UnityAction(delegate
                        {
                            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
                        })));
                        
                    }));

                    OpenErrorView(ed);
                }

            }
        }
*/
        #endregion

        #region PUBLIC METHODS

        public void OpenHomeView()
        {
            // Check for internet 
            // if (!(Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork))
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                // Internet unreachable, show error
                ErrorDetail ed = new ErrorDetail();
                ed.SetErrorTitle("No Internet Connection");
                ed.SetErrorDescription("To continue, please check your internet connection," +
                    "and try again.");

                // Associate this error detail with actions
                ed.AddToDictionary(ErrorDetail.ResponseType.SETTINGS, new UnityEngine.Events.UnityAction(delegate
                {
                    OVRManager.PlatformUIGlobalMenu();
                }));

                ed.AddToDictionary(ErrorDetail.ResponseType.RETRY, new UnityEngine.Events.UnityAction(delegate
                {
                    StartCoroutine(CheckForInternetAgain(2f, new UnityAction(delegate
                    {
                        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
                    })));
                }));

                OpenErrorView(ed);
            }
            else
            {
                ViewManagerUtility.Instance().SetupHomeView();
            }
        }

        public void OpenInfluencerView(string userGid)
        {
            // Do a lookup on the featured influencer dictionary
            if (HomePageLoader.featuredInfluencerDict.Count > 0)
            {
                InfluencerDetail influencer;
                HomePageLoader.featuredInfluencerDict.TryGetValue(userGid, out influencer);

                if (influencer != null)
                {
                    OpenInfluencerView(influencer);
                }
                else
                {
                    // No influencer with the given id in the dictionary
                    // Show error
                    Debug.LogError("No featured influencer with id : " + userGid);
                }
            }
            else
            {
                // No featured influencers
                // Show error
                Debug.LogError("Featured Influencer Dictionary empty");
            }
        }

        public void OpenInfluencerView(InfluencerDetail influencer)
        {
            // if (!(Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork))
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                // Setup a new errordetail
                ErrorDetail ed = new ErrorDetail();
                ed.SetErrorTitle("No Internet Connection");
                ed.SetErrorDescription("To continue, please check your internet connection, " +
                    "and try again.");

                // Associate this error detail with an action
                // Add a reset button to the panel
                ed.AddToDictionary(ErrorDetail.ResponseType.RETRY, new UnityEngine.Events.UnityAction(delegate
                {
                    StartCoroutine(CheckForInternetAgain(1f, new UnityAction(delegate
                    {
                        ViewManagerUtility.Instance().BackButtonPressed();
                        OpenInfluencerView(influencer);
                    })));
                }));

                ed.AddToDictionary(ErrorDetail.ResponseType.IGNORE, new UnityEngine.Events.UnityAction(delegate
                {
                    ViewManagerUtility.Instance().BackButtonPressed();
                }));

                OpenErrorView(ed);
                return;
            }
            else
            {
                ViewManagerUtility.Instance().SetupInfluencerView(influencer);
            }
        }

        public void OpenErrorView(ErrorDetail ed)
        {
            // Ask the view controller to show the error
            ViewManagerUtility.Instance().SetupErrorView(ed);
        }

        public void OpenVideoPlayer(VideoDetail video)
        {
            // Check for internet 
            if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
            {
                // Setup a new errordetail
                ErrorDetail ed = new ErrorDetail();
                ed.SetErrorTitle("Wi-Fi Disconnected");
                ed.SetErrorDescription("We could not detect a Wi-Fi " +
                    "connection. Would you like to continue watching videos with " +
                    "mobile data?");

                // Associate this error detail with an action
                // Add a reset button to the panel
                ed.AddToDictionary(ErrorDetail.ResponseType.CANCEL, new UnityEngine.Events.UnityAction(delegate
                {
                    ViewManagerUtility.Instance().BackButtonPressed();
                }));

                ed.AddToDictionary(ErrorDetail.ResponseType.ACCEPT, new UnityEngine.Events.UnityAction(delegate
                {
                    ViewManagerUtility.Instance().BackButtonPressed();
                    GameObject currentVideoPlayerPrefab;
                    // Decide which video player to instantiate
                    switch (video.type)
                    {
                        case VideoDetail.Type.MONO:
                            currentVideoPlayerPrefab = videoPrefabMono;
                            break;
                        case VideoDetail.Type.STEREO:
                            currentVideoPlayerPrefab = videoPrefabStereo;
                            break;
                        case VideoDetail.Type.VOLUMETRIC:
                            currentVideoPlayerPrefab = videoPrefabVol;
                            break;
                        default:
                            goto case VideoDetail.Type.MONO;
                    }

                    // Ask view controller to show the video
                    ViewManagerUtility.Instance().SetupVideoView(video, currentVideoPlayerPrefab);
                }));
                OpenErrorView(ed);
            }
            else if(Application.internetReachability == NetworkReachability.NotReachable)
            {
                // Setup a new errordetail
                ErrorDetail ed = new ErrorDetail();
                ed.SetErrorTitle("No Internet Connection");
                ed.SetErrorDescription("To continue, please check your internet connection, " +
                    "and try again.");

                // Associate this error detail with actions
                ed.AddToDictionary(ErrorDetail.ResponseType.IGNORE, new UnityEngine.Events.UnityAction(delegate
                {
                    ViewManagerUtility.Instance().BackButtonPressed();
                }));

                ed.AddToDictionary(ErrorDetail.ResponseType.RETRY, new UnityEngine.Events.UnityAction(delegate
                {
                    StartCoroutine(CheckForWifiAgain(2f, new UnityAction(delegate
                    {
                        ViewManagerUtility.Instance().BackButtonPressed();
                        GameObject currentVideoPlayerPrefab;
                        // Decide which video player to instantiate
                        switch (video.type)
                        {
                            case VideoDetail.Type.MONO:
                                currentVideoPlayerPrefab = videoPrefabMono;
                                break;
                            case VideoDetail.Type.STEREO:
                                currentVideoPlayerPrefab = videoPrefabStereo;
                                break;
                            case VideoDetail.Type.VOLUMETRIC:
                                currentVideoPlayerPrefab = videoPrefabVol;
                                break;
                            default:
                                // Open mono video by default
                                currentVideoPlayerPrefab = videoPrefabMono;
                                break;
                        }
                    }), new UnityAction(delegate
                    {
                        ViewManagerUtility.Instance().BackButtonPressed();
                        OpenVideoPlayer(video);
                    })));
                    
                }));
                OpenErrorView(ed);
            }
            else
            {
                // Wifi available - setup as usual

                GameObject currentVideoPlayerPrefab;
                // Decide which video player to instantiate
                switch (video.type)
                {
                    case VideoDetail.Type.MONO:
                        currentVideoPlayerPrefab = videoPrefabMono;
                        break;
                    case VideoDetail.Type.STEREO:
                        currentVideoPlayerPrefab = videoPrefabStereo;
                        break;
                    case VideoDetail.Type.VOLUMETRIC:
                        currentVideoPlayerPrefab = videoPrefabVol;
                        break;
                    default:
                        // Open mono video by default
                        // TODO implement video type error
                        currentVideoPlayerPrefab = videoPrefabMono;
                        break;
                }

                // Ask view controller to show the video
                ViewManagerUtility.Instance().SetupVideoView(video, currentVideoPlayerPrefab);
            }
        }
        
        #endregion

        #region PRIVATE METHODS

        public IEnumerator CheckForInternetAgain(float time, UnityAction functionToCall)
        {
            GameObject ErrorCanvas = GameObject.Find("ErrorCanvas(Clone)");
            Animator animator = ErrorCanvas.GetComponent<Animator>();
            animator.SetBool("shouldShowSpinner", true);
            internetStatusSet = true;

            yield return new WaitForSeconds(time);

            // Stop the spinner
            animator.SetBool("shouldShowSpinner", false);
            if (!(Application.internetReachability == NetworkReachability.NotReachable))
            {
                // Internet available either via Wi-Fi or data connection
                internetAvailable = true;
                functionToCall();
            }
            else
            {
                if(ErrorCanvas != null)
                {
                    
                    animator.SetBool("shouldShake", true);
                }
                internetAvailable = false;
            }
            internetStatusSet = true;
        }

        public IEnumerator CheckForWifiAgain(float time, UnityAction fnOnWifiAvailable, UnityAction fnOnDataAvailable)
        {
            GameObject ErrorCanvas = GameObject.Find("ErrorCanvas(Clone)");
            Animator animator = ErrorCanvas.GetComponent<Animator>();
            animator.SetBool("shouldShowSpinner", true);
            internetStatusSet = true;

            yield return new WaitForSeconds(time);

            // Stop the spinner
            animator.SetBool("shouldShowSpinner", false);
            if (!(Application.internetReachability == NetworkReachability.NotReachable))
            {
                if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
                {
                    fnOnWifiAvailable();
                } else
                {
                    fnOnDataAvailable();
                }
            }
            else
            {
                if (ErrorCanvas != null)
                {

                    animator.SetBool("shouldShake", true);
                }
                internetAvailable = false;
            }
            internetStatusSet = true;
        }

        #endregion
    }
}

