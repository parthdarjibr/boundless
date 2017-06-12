using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BR.App {
    public class SliderEventScript : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler {

        public VideoPlayerMenu videoMenu;
        public Image backgroundImage;
        public Sprite hoverSprite, idleSprite;

        public void OnPointerDown(PointerEventData eventData)
        {
            backgroundImage.sprite = hoverSprite;
            AudioController.Instance().PlayOneShot(ApplicationController.Instance().interactionClickAudioClip);
            videoMenu.SeekbarPointerDown();

        }

        public void OnPointerUp(PointerEventData eventData)
        {
            videoMenu.SeekbarPointerUp();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            backgroundImage.sprite = hoverSprite;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            backgroundImage.sprite = idleSprite;
        }
    }
}
