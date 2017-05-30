using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BR.App;
using BR.BRUtilities;
using frame8.ScrollRectItemsAdapter.Util.GridView;
using System.Linq;
using System;
using UnityEngine.EventSystems;

public class ScrollManager : MonoBehaviour
{
    public enum PanelType
    {
        VIDEO_ON_HOME,
        CREATOR_ON_HOME,
        VIDEO_ON_CREATOR
    }

    public enum ScrollDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    #region VARIABLES
    public PanelType panelType;

    public VideoDetailParams videoGridParams;
    public CreatorDetailParams creatorGridParams;
    VideoScrollRectItemsAdapter videoGridAdapter;
    CreatorScrollRectItemsAdapter creatorGridAdapter;

    /// <summary>
    /// Scroll trigger for GearVR
    /// </summary>
    public bool shouldScroll = false;

    /// <summary>
    /// Scroll triggers for Oculus Touch Controllers
    /// </summary>
    public bool shouldScrollUp = false, shouldScrollDown = false;

    private int currentIndex = 1;
    private int totalElements = 0;
    public int amountToScroll = 9;

    /// <summary>
    /// The scroll direction.
    /// -1 => UP; 1 => DOWN
    /// </summary>
    private int scrollDirection = 0;
    int scrollTo = -1;


    #endregion

    #region UNITY MONO METHODS

    void OnDestroy()
    {
        if (videoGridAdapter != null)
            videoGridAdapter.Dispose();

        if (creatorGridAdapter != null)
            creatorGridAdapter.Dispose();

        OVRTouchpad.TouchHandler -= OVRTouchpad_TouchHandler;
    }

    void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        // Instantiate OVRTouchpad and listen to events
        OVRTouchpad.Create();
		OVRTouchpad.TouchHandler += OVRTouchpad_TouchHandler;
#endif
    }

    private void Update()
    {
        // If oculus touch controllers are connected
        // Use the primary thumb stick for scrolling
        OVRInput.Controller activeController = OVRInput.GetActiveController();
        if (activeController == OVRInput.Controller.RTouch
            || activeController == OVRInput.Controller.LTouch
            || activeController == OVRInput.Controller.Remote)
        {
            if (shouldScroll)
            {
                if (OVRInput.GetDown(CurvedUIInputModule.Instance.CustomControllerSwipeUp) || Input.GetKeyDown(KeyCode.Q))
                {
                    Debug.Log("up");
                    PerformScroll(ScrollDirection.Up);
                }
                if(OVRInput.GetDown(CurvedUIInputModule.Instance.CustomControllerSwipeDown) || Input.GetKeyDown(KeyCode.W))
                {
                    Debug.Log("down");
                    PerformScroll(ScrollDirection.Down);
                }
                shouldScroll = false;
            }
        }
    }
    #endregion

    #region UTILITIES
    public void InitializeVideoAdapter(List<VideoEdges> videoList)
    {
        // Set the total video count
        totalElements = videoList.Count;

        // Setup the video and creator lists
        videoGridAdapter = new VideoScrollRectItemsAdapter();
        // Initialize video list
        videoGridAdapter.Init(videoGridParams);
        videoGridAdapter.ChangeModels(videoList.ToArray());
    }

    public void InitializeCreatorAdapter(List<InfluencerEdges> influencerList)
    {
        totalElements = influencerList.Count;

        creatorGridAdapter = new CreatorScrollRectItemsAdapter();
        creatorGridAdapter.Init(creatorGridParams);
        creatorGridAdapter.ChangeModels(influencerList.ToArray());
    }

    public void UpdateVideoAdapter(List<VideoEdges> videoList)
    {
        // if (videoGridAdapter == null) {
        // 	videoGridAdapter.Init (videoGridParams);
        // }

        // Reset the scroll
        // videoGridAdapter.Dispose();
        // videoGridAdapter.Init (videoGridParams);
        videoGridAdapter.Clear();
        videoGridAdapter.ChangeModels(videoList.ToArray());
        videoGridParams.ScrollToStart();
        currentIndex = 1;
        totalElements = videoList.Count;
    }

    #endregion

    #region EVENT HANDLERS
    void OVRTouchpad_TouchHandler(object sender, System.EventArgs e)
    {
        if (shouldScroll)
        {
            OVRTouchpad.TouchArgs ta = (OVRTouchpad.TouchArgs)e;

            switch (ta.TouchType)
            {
                case OVRTouchpad.TouchEvent.Up:
                    PerformScroll(ScrollDirection.Up);
                    break;
                case OVRTouchpad.TouchEvent.Down:
                    PerformScroll(ScrollDirection.Down);
                    break;
                    /*
                case OVRTouchpad.TouchEvent.Up:
                    Debug.Log ("UP");
                    if (currentIndex > amountToScroll)
                        scrollDirection = -1;
                    else
                        scrollDirection = 0;
                    break;
                case OVRTouchpad.TouchEvent.Down:
                    Debug.Log ("DOWN");
                    if (currentIndex < totalElements - amountToScroll)
                        scrollDirection = 1;
                    else
                        scrollDirection = 0;
                    break;
                case OVRTouchpad.TouchEvent.Left:
                    break;
                case OVRTouchpad.TouchEvent.Right:
                    break;
                }

                // Scroll the view in a direction
                switch (panelType) {

                case PanelType.CREATOR_ON_HOME:
                    Debug.Log (creatorGridAdapter.GetItemCount ().ToString());
                    scrollTo = scrollDirection > 0 ? currentIndex + amountToScroll : currentIndex - amountToScroll;
                    Debug.Log(scrollTo);
                    if (scrollDirection > 0) {
                        // Scroll down
                        creatorGridAdapter.SmoothScrollTo (creatorGridParams.GetGroupIndex (scrollTo), 0.5f, null);
                        currentIndex += amountToScroll;

                    } else if (scrollDirection < 0) {
                        // Scroll up
                        creatorGridAdapter.SmoothScrollTo (creatorGridParams.GetGroupIndex (scrollTo), 0.5f, null);
                        currentIndex -= amountToScroll;
                    }
                    break;
                case PanelType.VIDEO_ON_CREATOR:
                case PanelType.VIDEO_ON_HOME:
                    scrollTo = scrollDirection > 0 ? currentIndex + amountToScroll : currentIndex - amountToScroll;
                    Debug.Log(scrollTo);
                    if (scrollDirection > 0) {
                        // Scroll down 
                        videoGridAdapter.SmoothScrollTo (videoGridParams.GetGroupIndex (scrollTo), 0.5f, null);
                        currentIndex += amountToScroll;

                    } else if (scrollDirection < 0) {
                        // Scroll up
                        videoGridAdapter.SmoothScrollTo (videoGridParams.GetGroupIndex (scrollTo), 0.5f, null);
                        currentIndex -= amountToScroll;

                    }
                    break;
                default:
                    break;
                }
                */
                    //shouldScroll = false;
            }
            shouldScroll = false;
        }
    }
    #endregion

    #region HELPER METHODS
    void PerformScroll(ScrollDirection d)
    {
        switch (d)
        {
            case ScrollDirection.Up:
                if (currentIndex > amountToScroll)
                    scrollDirection = -1;
                else
                    scrollDirection = 0;
                break;
            case ScrollDirection.Down:
                if (currentIndex < totalElements - amountToScroll)
                    scrollDirection = 1;
                else
                    scrollDirection = 0;
                break;
        }
        // Scroll the view in a direction
        switch (panelType)
        {

            case PanelType.CREATOR_ON_HOME:
                scrollTo = scrollDirection > 0 ? currentIndex + amountToScroll : currentIndex - amountToScroll;
                if (scrollDirection > 0)
                {
                    // Scroll down
                    creatorGridAdapter.SmoothScrollTo(creatorGridParams.GetGroupIndex(scrollTo), 0.5f);
                    currentIndex += amountToScroll;
                }
                else if (scrollDirection < 0)
                {
                    // Scroll up
                    creatorGridAdapter.SmoothScrollTo(creatorGridParams.GetGroupIndex(scrollTo), 0.5f);
                    currentIndex -= amountToScroll;
                }
                break;
            case PanelType.VIDEO_ON_CREATOR:
            case PanelType.VIDEO_ON_HOME:
                scrollTo = scrollDirection > 0 ? currentIndex + amountToScroll : currentIndex - amountToScroll;
                if (scrollDirection > 0)
                {
                    // Scroll down 
                    videoGridAdapter.SmoothScrollTo(videoGridParams.GetGroupIndex(scrollTo), 0.5f);
                    currentIndex += amountToScroll;
                }
                else if (scrollDirection < 0)
                {
                    // Scroll up
                    videoGridAdapter.SmoothScrollTo(videoGridParams.GetGroupIndex(scrollTo), 0.5f);
                    currentIndex -= amountToScroll;
                }
                break;
            default:
                break;
        }
    }
    #endregion
}
