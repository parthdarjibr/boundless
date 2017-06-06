using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Net.NetworkInformation;

public class NetworkTest : MonoBehaviour {

    public Text pauseState, netState, unityState;
    public Text infoBox;

    private void Awake()
    {
        NetworkChange.NetworkAvailabilityChanged += NetworkChange_NetworkAvailabilityChanged;
    }

    private void Update()
    {
        switch (Application.internetReachability)
        {
            case NetworkReachability.ReachableViaLocalAreaNetwork:
                unityState.text = "Wifi connected";
                break;
            case NetworkReachability.NotReachable:
            case NetworkReachability.ReachableViaCarrierDataNetwork:
                unityState.text = "Not reachable";
                break;
        }
    }

    private void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
    {
        infoBox.text += "Netork availability change: " + e.IsAvailable + "\n";
        netState.text = e.IsAvailable.ToString();
    }

    private void OnApplicationPause(bool pause)
    {
        infoBox.text += "Application Pause: " + pause + "\n";
        pauseState.text = pause.ToString();
    }
}
