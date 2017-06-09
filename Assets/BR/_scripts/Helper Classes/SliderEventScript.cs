using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace BR.App {
    public class SliderEventScript : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

        public VideoPlayerMenu videoMenu;

        public void OnPointerDown(PointerEventData eventData)
        {
            videoMenu.SeekbarPointerDown();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            videoMenu.SeekbarPointerUp();
        }
    }
}
