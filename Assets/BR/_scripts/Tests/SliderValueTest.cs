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
        Vector2 raycastPos;
        if (curvedCanvas.RaycastToCanvasSpace(Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2.0f, Screen.height / 2.0f)), out raycastPos)) 
        {
            Debug.Log("Raycast Pos: " + raycastPos);
            Debug.Log((transform as RectTransform).rect);
        }

    }
    #endregion
}
