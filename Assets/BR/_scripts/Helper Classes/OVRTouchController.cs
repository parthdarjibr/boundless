using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using System.Collections;

/// <summary>
/// Simple helper script that conditionally enables rendering of a controller if it is connected.
/// </summary>
public class OVRTouchController : MonoBehaviour
{
    /// <summary>
    /// The root GameObject that should be conditionally enabled depending on controller connection status.
    /// </summary>
    public GameObject m_model;

    /// <summary>
    /// The controller that determines whether or not to enable rendering of the controller model.
    /// </summary>
    public OVRInput.Controller m_controller;

    private bool m_prevControllerConnected = false;
    private bool m_prevControllerConnectedCached = false;

    void Update()
    {
        bool controllerConnected = (OVRInput.GetActiveController() == m_controller);

        if ((controllerConnected != m_prevControllerConnected) || !m_prevControllerConnectedCached)
        {
            m_model.SetActive(controllerConnected);
            m_prevControllerConnected = controllerConnected;
            m_prevControllerConnectedCached = true;
        }

        if (!controllerConnected)
        {
            return;
        }
    }
}