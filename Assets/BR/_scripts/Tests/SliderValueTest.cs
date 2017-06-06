using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using CurvedUI;

public class SliderValueTest : MonoBehaviour, IPointerUpHandler, IPointerEnterHandler {

    #region VARIABLES

    public Text startTime, currentTime, remainingTime;
    float currentTimeF;
    public Camera eventCamera;
    #endregion

    #region PUBLIC METHODS

    public void OnValueChange(float val)
    {
        currentTimeF = (float)System.Math.Round((double)val, 2);
        // Mathf.Round((double)val, 2);
        currentTime.text = currentTimeF.ToString().Replace('.', ':');
        // remainingTime.text = (1 - val).ToString();
    }

    #endregion

    #region EVENT HANDLERS

    public void OnPointerUp(PointerEventData eventData)
    {
        startTime.text = currentTime.text;
        remainingTime.text = "-" + (1 - currentTimeF).ToString().Replace('.', ':');
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        RectTransform sliderPlane = transform as RectTransform;
        Vector3 globalMousePos;
        if(RectTransformUtility.ScreenPointToWorldPointInRectangle(sliderPlane, eventData.position, eventData.pressEventCamera, out globalMousePos))
        {
            Debug.Log(globalMousePos);
            Debug.Log(sliderPlane.rect);
        }
        /*
        if (dragOnSurfaces && data.pointerEnter != null && data.pointerEnter.transform as RectTransform != null)
             m_DraggingPlane = data.pointerEnter.transform as RectTransform;
         var rt = m_DraggingIcon.GetComponent{RectTransform}();
         Vector3 globalMousePos;
         if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_DraggingPlane, data.position, data.pressEventCamera, out globalMousePos))
         {
             rt.position = globalMousePos;
             rt.rotation = m_DraggingPlane.rotation;
         }
         */
    }
    #endregion
}
