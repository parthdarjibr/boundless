using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using CurvedUI;

public class ParallaxTest : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public void OnPointerEnter(PointerEventData eventData)
    {

        // Set the intial positions on first pointer enter
        if (!initPosSet)
        {
            t = GetComponentInParent<Transform>();
            rt = t as RectTransform;
            initPos = t.position;
            rectPos = rt.sizeDelta;
            initPosSet = true;
        }
        shouldParallax = true;
        shouldSnapBack = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        shouldParallax = false;
        shouldSnapBack = true;
    }

    private bool shouldParallax = false, shouldSnapBack = false, initPosSet = false;
    private Transform t;
    RectTransform rt;
    private Vector3 initPos;
    private Vector2 rectPos;

    public float change = 5;

    private void Start()
    {
        
    }
    void Update () {
	    if(shouldParallax)
        {
            // t.position = t.position.ModifyX(initPos.x + Input.mousePosition.x.Remap(0, Screen.width, -change, change));
            // t.position = t.position.ModifyY(initPos.y + Input.mousePosition.y.Remap(0, Screen.height, -change, change) * (Screen.height / Screen.width));

            t.position = t.position.ModifyX(initPos.x + CUIGazePointer.instance.transform.position.x.Remap(-rectPos.x/2, rectPos.x/2, -change, change));
            t.position = t.position.ModifyY(initPos.y + CUIGazePointer.instance.transform.position.y.Remap(-rectPos.y/2, rectPos.y/2, -change, change));
            //t.position = t.position.ModifyX(initPos.x + CUIGazePointer.instance.transform.position.x.Remap(0, rectPos.x, -change, change));
            //t.position = t.position.ModifyY(initPos.y + CUIGazePointer.instance.transform.position.y.Remap(0, rectPos.y, -change, change));

        }
        else if(shouldSnapBack)
        {
            t.position = initPos;
        }
    }
}
