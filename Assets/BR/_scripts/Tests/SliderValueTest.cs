using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using CurvedUI;

public class SliderValueTest : MonoBehaviour, IPointerUpHandler, IPointerEnterHandler {

    #region VARIABLES

    public Text startTime, currentTime, remainingTime;
    float currentTimeF;
    public CurvedUISettings curvedCanvas;

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
        RectTransform rt = transform as RectTransform;
        Debug.Log(rt.rect.width / 2 * rt.lossyScale.x);
        Debug.Log(CUIGazePointer.instance.transform.position.x);

    }
    #endregion
}
