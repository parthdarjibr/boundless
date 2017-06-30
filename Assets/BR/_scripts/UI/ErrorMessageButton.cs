using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ErrorMessageButton : MonoBehaviour, IPointerClickHandler {

	public void OnPointerClick(PointerEventData ed)
    {
        OVRManager.PlatformUIGlobalMenu();
    }
}
