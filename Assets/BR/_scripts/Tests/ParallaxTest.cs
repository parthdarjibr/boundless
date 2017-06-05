using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using CurvedUI;

public class ParallaxTest : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public void OnPointerEnter(PointerEventData eventData)
    {
        shouldParallax = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        shouldParallax = false;
    }

    private bool shouldParallax = false;
    private Transform t;
    RectTransform rt;
    private Vector3 initPos;
    private Vector2 rectPos;

    public float change = 5;

    private void Start()
    {
        t = GetComponentInParent<Transform>();
        rt = t as RectTransform;
        initPos = t.position;
        rectPos = rt.sizeDelta;
    }
    void Update () {
	    if(shouldParallax)
        {
            // t.position = t.position.ModifyX(initPos.x + Input.mousePosition.x.Remap(0, Screen.width, -change, change));
            // t.position = t.position.ModifyY(initPos.y + Input.mousePosition.y.Remap(0, Screen.height, -change, change) * (Screen.height / Screen.width));

            t.position = t.position.ModifyX(initPos.x + CUIGazePointer.instance.transform.position.x.Remap(0, rectPos.x, -change, change));
            t.position = t.position.ModifyY(initPos.y + CUIGazePointer.instance.transform.position.y.Remap(0, rectPos.y, -change, change));
        } else
        {
            t.position = initPos;
        }
    }
}
