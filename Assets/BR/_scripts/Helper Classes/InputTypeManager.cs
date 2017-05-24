using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
// using UnityEngine.UI;

public class InputTypeManager : MonoBehaviour
{

    #region INITIALIZATION

    private static InputTypeManager _instance;
    public static InputTypeManager instance
    {
        get
        {
            if (_instance == null)
            {
                throw new Exception("InputTypeManager object not found");
            }
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    #endregion

    #region VARIABLES

    public OVRInput.Controller currentController;
    private OVRInput.Controller newController;
    public Transform leftHandAnchor, rightHandAnchor, cameraAnchor;
    // public Text currentControllerName;

    #endregion

    #region UNITY MONO METHODS

    void Start()
    {
        // Initial setup
        // StartCoroutine(DelayedInit());
        SetupNewController();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.A))
        {
            CurvedUIInputModule.Controller = CurvedUIInputModule.CurvedUIController.GVRCONTROLLER;
            //currentController = OVRInput.Controller.Touchpad;
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            currentController = OVRInput.Controller.RTrackedRemote;
        }

        SetupNewController();
    }

    #endregion

    #region PRIVATE METHODS

    private void SetupNewController()
    {
        newController = OVRInput.GetActiveController();
        if (newController != currentController)
        {
            // Check the controller type
            switch (newController)
            {
#if UNITY_ANDROID
                /// <summary>
                /// GEARVR TOUCHPAD/CONTROLLER
                /// </summary>
                case OVRInput.Controller.LTrackedRemote:
                    // Left hand remote
                    //CurvedUIInputModule.CustomControllerRay = new Ray (leftHandAnchor.position, leftHandAnchor.forward);
                    CurvedUIInputModule.CustomControllerTransform = leftHandAnchor;
                    CurvedUIInputModule.Controller = CurvedUIInputModule.CurvedUIController.GVRCONTROLLER;
                    OVRInput.RecenterController();

                    // Update Gaze pointer variables
                    CUIGazePointer.instance.rayTransform = leftHandAnchor;
                    break;
                case OVRInput.Controller.RTrackedRemote:
                    // Right hand remote
                    //CurvedUIInputModule.CustomControllerRay = new Ray (rightHandAnchor.position, rightHandAnchor.forward);
                    CurvedUIInputModule.CustomControllerTransform = rightHandAnchor;
                    CurvedUIInputModule.Controller = CurvedUIInputModule.CurvedUIController.GVRCONTROLLER;
                    OVRInput.RecenterController();

                    // Update Gaze pointer variables
                    CUIGazePointer.instance.rayTransform = rightHandAnchor;
                    break;
                case OVRInput.Controller.Touchpad:
                    // Touchpad
                    CurvedUIInputModule.Controller = CurvedUIInputModule.CurvedUIController.GAZE;
                    CurvedUIInputModule.CustomControllerRay = new Ray();

                    // Update Gaze pointer variables
                    CUIGazePointer.instance.rayTransform = cameraAnchor;
                    break;
#endif
                /// <summary>
                /// OCULUS TOUCH CONTROLLERS
                /// </summary>
                case OVRInput.Controller.LTouch:
                    Debug.Log("Ltouch");
                    // Setup custom controller properties
                    CurvedUIInputModule.CustomControllerTransform = leftHandAnchor;
                    CurvedUIInputModule.Instance.CustomControllerInteractionButton = OVRInput.Button.PrimaryIndexTrigger;
                    CurvedUIInputModule.Instance.CustomControllerSwipeDown = OVRInput.Button.PrimaryThumbstickDown;
                    CurvedUIInputModule.Instance.CustomControllerSwipeUp = OVRInput.Button.PrimaryThumbstickUp;
                    // CurvedUIInputModule.CustromControllerButtonDown = OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.LTouch);
                    CurvedUIInputModule.CustomControllerRay = new Ray(leftHandAnchor.position, leftHandAnchor.forward);
                    CurvedUIInputModule.Controller = CurvedUIInputModule.CurvedUIController.CUSTOM_RAY;

                    // Update Gaze pointer variables
                    CUIGazePointer.instance.rayTransform = leftHandAnchor;
                    break;
                case OVRInput.Controller.RTouch:
                    Debug.Log("Rtouch");
                    // Setup custom controller properties
                    CurvedUIInputModule.CustomControllerTransform = rightHandAnchor;
                    CurvedUIInputModule.Instance.CustomControllerInteractionButton = OVRInput.Button.PrimaryIndexTrigger;
                    CurvedUIInputModule.Instance.CustomControllerSwipeDown = OVRInput.Button.PrimaryThumbstickDown;
                    CurvedUIInputModule.Instance.CustomControllerSwipeUp = OVRInput.Button.PrimaryThumbstickUp;
                    CurvedUIInputModule.CustomControllerRay = new Ray(rightHandAnchor.position, rightHandAnchor.forward);
                    CurvedUIInputModule.Controller = CurvedUIInputModule.CurvedUIController.CUSTOM_RAY;

                    // Update Gaze pointer variables
                    CUIGazePointer.instance.rayTransform = rightHandAnchor;
                    break;
                /*    
                case OVRInput.Controller.Remote:
                    // Setup custom controller properties
                    CurvedUIInputModule.CustomControllerTransform = cameraAnchor;
                    CurvedUIInputModule.Instance.CustomControllerInteractionButton = OVRInput.Button.One;
                    CurvedUIInputModule.Instance.CustomControllerSwipeDown = OVRInput.Button.DpadDown;
                    CurvedUIInputModule.Instance.CustomControllerSwipeUp = OVRInput.Button.DpadUp;
                    CurvedUIInputModule.CustomControllerRay = new Ray(cameraAnchor.position, cameraAnchor.forward);
                    CurvedUIInputModule.Controller = CurvedUIInputModule.CurvedUIController.CUSTOM_RAY;

                    // Update Gaze pointer variables
                    CUIGazePointer.instance.rayTransform = cameraAnchor;
                    break;
                */
            }
            currentController = newController;
        }
    }

    private IEnumerator DelayedInit()
    {
        // Wait for a frame to update the input module
        yield return null;

        currentController = OVRInput.GetActiveController();
        switch (currentController)
        {
            case OVRInput.Controller.LTrackedRemote:
                // Left hand remote
                CurvedUIInputModule.CustomControllerTransform = leftHandAnchor;
                CurvedUIInputModule.Controller = CurvedUIInputModule.CurvedUIController.GVRCONTROLLER;
                OVRInput.RecenterController();

                // Update Gaze pointer variables
                CUIGazePointer.instance.rayTransform = leftHandAnchor;
                break;
            case OVRInput.Controller.RTrackedRemote:
                // Right hand remote
                CurvedUIInputModule.CustomControllerTransform = rightHandAnchor;
                CurvedUIInputModule.Controller = CurvedUIInputModule.CurvedUIController.GVRCONTROLLER;
                OVRInput.RecenterController();

                // Update Gaze pointer variables
                CUIGazePointer.instance.rayTransform = rightHandAnchor;
                break;
            case OVRInput.Controller.Touchpad:
                // Touchpad
                CurvedUIInputModule.Controller = CurvedUIInputModule.CurvedUIController.GAZE;
                CurvedUIInputModule.CustomControllerRay = new Ray();

                // Update Gaze pointer variables
                CUIGazePointer.instance.rayTransform = cameraAnchor;
                break;
        }
    }

#endregion
}
