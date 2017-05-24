using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRControllerManager : MonoBehaviour {
	private readonly string handAnchorName = "HandAnchor";
	/// <summary>
	/// Always coincides with the pose of the left hand.
	/// </summary>
	public Transform leftHandAnchor { get; private set; }
	/// <summary>
	/// Always coincides with the pose of the right hand.
	/// </summary>
	public Transform rightHandAnchor { get; private set; }


	void Start () {
		UpdateAnchors ();
	}
	
	void FixedUpdate () {
		UpdateAnchors ();
	}

	void UpdateAnchors() {
		if (leftHandAnchor == null)
			leftHandAnchor = ConfigureHandAnchor(transform, OVRPlugin.Node.HandLeft);

		if (rightHandAnchor == null)
			rightHandAnchor = ConfigureHandAnchor(transform, OVRPlugin.Node.HandRight);
		
		leftHandAnchor.localRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch);
		rightHandAnchor.localRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);

		leftHandAnchor.localPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
		rightHandAnchor.localPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
	}


	private Transform ConfigureHandAnchor(Transform root, OVRPlugin.Node hand)
	{
		string handName = (hand == OVRPlugin.Node.HandLeft) ? "Left" : "Right";
		string name = handName + handAnchorName;
		Transform anchor = transform.Find(root.name + "/" + name);

		if (anchor == null)
		{
			anchor = transform.Find(name);
		}

		if (anchor == null)
		{
			anchor = new GameObject(name).transform;
		}

		anchor.name = name;
		anchor.parent = root;
		anchor.localScale = Vector3.one;
		anchor.localPosition = Vector3.zero;
		anchor.localRotation = Quaternion.identity;

		return anchor;
	}
}
