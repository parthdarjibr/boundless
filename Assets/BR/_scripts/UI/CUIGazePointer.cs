// /*
//  * Code by: Parth Darji
//  * Company: Boundless Reality
//  * (c) Boundless Reality, All rights reserved.
//  * 
//  * Details:
//  */
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using BR.BRUtilities.UI;

public class CUIGazePointer : MonoBehaviour
{
	[Tooltip("Should the pointer be hidden when not over interactive objects.")]
	public bool hideByDefault = true;

	[Tooltip("Time after leaving interactive object before pointer fades.")]
	public float showTimeoutPeriod = 1;

	[Tooltip("Time after mouse pointer becoming inactive before pointer unfades.")]
	public float hideTimeoutPeriod = 0.1f;

	[Tooltip("Keep a faint version of the pointer visible while using a mouse")]
	public bool dimOnHideRequest = true;

	[Tooltip("Angular scale of pointer")]
	public float depthScaleMultiplier = 0.03f;

	/// <summary>
	/// The gaze ray.
	/// </summary>
	public Transform rayTransform;

	/// <summary>
	/// Is gaze pointer current visible
	/// </summary>
	public bool hidden { get; private set; }

    /// <summary>
    /// Current scale applied to pointer
    /// </summary>
    public float currentScale { get; private set; }

	/// <summary>
	/// Current depth of pointer from camera
	/// </summary>
	private float depth;
	private float hideUntilTime;
	/// <summary>
	/// How many times position has been set this frame. Used to detect when there are no position sets in a frame.
	/// </summary>
	private int positionSetsThisFrame = 0;
	/// <summary>
	/// Position last frame.
	/// </summary>
	private Vector3 lastPosition;
	/// <summary>
	/// Last time code requested the pointer be shown. Usually when pointer passes over interactive elements.
	/// </summary>
	private float lastShowRequestTime;
	/// <summary>
	/// Last time pointer was requested to be hidden. Usually mouse pointer activity.
	/// </summary>
	private float lastHideRequestTime;

	[Tooltip("Radius of the cursor. Used for preventing geometry intersections.")]
	public float cursorRadius = 1f;

	// Variables for loading/transition control
	public GameObject quadObject;
	private bool originalHideByDefault;

	// How much the gaze pointer moved in the last frame
	public Vector3 positionDelta { private set; get; }

	// Loading canvas variables
	// Error and warning colors
	public Color32 warningColor, errorColor;
	private Color32 originalColor;

	// The loading canvas object
	public GameObject loadingCanvas;
	public Image loadingProgressBar;

	private static CUIGazePointer _instance;
	public static CUIGazePointer instance {
		get {
			if (_instance == null) {
				// Debug.Log ("Instantiating CUIPointer");
				_instance = (CUIGazePointer)GameObject.Instantiate ((CUIGazePointer)Resources.Load ("Prefabs/CUIGazePointerRing", typeof(CUIGazePointer)));
			}
			return _instance;
		}
	}

	/// <summary>
	/// Used to determine alpha level of gaze cursor. Could also be used to determine cursor size, for example, as the cursor fades out.
	/// </summary>
	public float visibilityStrength { 
		get {
			// It's possible there are reasons to show the cursor - such as it hovering over some UI - and reasons to hide 
			// the cursor - such as another input method (e.g. mouse) being used. We take both of these in to account.


			float strengthFromShowRequest;
			if (hideByDefault) {
				// fade the cursor out with time
				strengthFromShowRequest = Mathf.Clamp01 (1 - (Time.time - lastShowRequestTime) / showTimeoutPeriod);
			} else {
				// keep it fully visible
				strengthFromShowRequest = 1;
			}

			// Now consider factors requesting pointer to be hidden
			float strengthFromHideRequest;

			strengthFromHideRequest = (lastHideRequestTime + hideTimeoutPeriod > Time.time) ? (dimOnHideRequest ? 0.1f : 0) : 1;


			// Hide requests take priority
			return Mathf.Min (strengthFromShowRequest, strengthFromHideRequest);
		} 
	}

	public void Awake()
	{
		currentScale = 1;
		// Only allow one instance at runtime.
		if (_instance != null && _instance != this)
		{
			enabled = false;
			DestroyImmediate(this);
			return;
		}

		_instance = this;
		// Hide the loading canvas
		loadingCanvas.SetActive(false);

		// Set the original hide request
		originalHideByDefault = hideByDefault;

		// Get the original color
		originalColor = quadObject.GetComponent<Renderer>().material.GetColor("_TintColor");
	}

	void Start() {
		
	}

	void Update () 
	{
		// Move the gaze cursor to keep it in the middle of the view
		transform.position = rayTransform.position + rayTransform.forward * depth;

		// Should we show or hide the gaze cursor?
		if (visibilityStrength == 0 && !hidden)
		{
			Hide();
		}
		else if (visibilityStrength > 0 && hidden)
		{
			Show();
		}
	}

	/// <summary>
	/// Set position and orientation of pointer
	/// </summary>
	/// <param name="pos"></param>
	/// <param name="normal"></param>
	public void SetPosition(Vector3 pos, Vector3 normal)
	{
		transform.position = pos;

		// Set the rotation to match the normal of the surface it's on. For the other degree of freedom (rotation around its own normal) use
		// the direction of movement so that trail effects etc are easier
		Quaternion newRot = transform.rotation;
		newRot.SetLookRotation(normal, rayTransform.up);
		transform.rotation = newRot;

		// record depth so that distance doesn't pop when pointer leaves an object
		depth = (rayTransform.position - pos).magnitude;

		//set scale based on depth
		currentScale = depth * depthScaleMultiplier;
		transform.localScale = new Vector3(currentScale, currentScale, currentScale);

		positionSetsThisFrame++;
	}

	/// <summary>
	/// SetPosition overload without normal. Just makes cursor face user
	/// </summary>
	/// <param name="pos"></param>
	public void SetPosition(Vector3 pos)
	{
		SetPosition(pos, rayTransform.forward);
	}

	public float GetCurrentRadius()
	{
		return cursorRadius * currentScale;
	}

	void LateUpdate()
	{
		// This happens after all Updates so we know that if positionSetsThisFrame is zero then nothing set the position this frame
		if (positionSetsThisFrame == 0)
		{
			// No geometry intersections, so gazing into space. Make the cursor face directly at the camera
			Quaternion newRot = transform.rotation;
			newRot.SetLookRotation(rayTransform.forward, rayTransform.up);
			transform.rotation = newRot;
		}

		// Keep track of cursor movement direction
		positionDelta = transform.position - lastPosition;
		lastPosition = transform.position;

		positionSetsThisFrame = 0;
	}

	/// <summary>
	/// Request the pointer be hidden
	/// </summary>
	public void RequestHide()
	{
		if (!dimOnHideRequest)
		{
			Hide();
		}
		lastHideRequestTime = Time.time;
	}

	/// <summary>
	/// Request the pointer be shown. Hide requests take priority
	/// </summary>
	public void RequestShow()
	{
		Show();
		lastShowRequestTime = Time.time;
	}

	// Disable/Enable child elements when we show/hide the cursor. For performance reasons.
	void Hide()
	{
		foreach (Transform child in transform)
		{
			child.gameObject.SetActive(false);
		}
		if (GetComponent<Renderer>())
			GetComponent<Renderer>().enabled = false;
		hidden = true;
	}

	void Show()
	{
		foreach (Transform child in transform)
		{
			child.gameObject.SetActive(true);
		}
		if (GetComponent<Renderer>())
			GetComponent<Renderer>().enabled = true;
		hidden = false;
	}

	// Methods to show/hide the loading indicator
	public void ShowLoadingReticle(bool isWarning = false, string loadingText = "Buffering...") {
		// Change the hiding property
		hideByDefault = false;

		// Request show of thereticle
		RequestShow();

		// Get the color of the reticle
		Color32 currColor = isWarning ? warningColor : errorColor;

		// Change color of reticle
		quadObject.GetComponent<Renderer>().material.SetColor("_TintColor", currColor);

		// Get the text on the canvas
		Text lt = loadingCanvas.GetComponentInChildren<Text>();
		// Change color of text
		lt.color = currColor;
		lt.text = loadingText;

		// Change color and show the loading progressbar
		loadingProgressBar.color = currColor;
		// loadingProgressBar.GetComponent<Outline> ().effectColor = currColor;

		// Show the loading canvas
		loadingCanvas.SetActive(true);
	}

	public void HideLoadingReticle() {
		// Hide the loading canvas
		loadingCanvas.SetActive(false);

		// Change color of reticle
		quadObject.GetComponent<Renderer>().material.SetColor("_TintColor", originalColor);

		// Show the loading indicator
		// canvasObject.SetActive(false);

		// Set the hiding back to original
		hideByDefault = originalHideByDefault;
	}
}

