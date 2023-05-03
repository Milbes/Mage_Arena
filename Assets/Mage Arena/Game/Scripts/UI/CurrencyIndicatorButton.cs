using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Watermelon
{
    public class CurrencyIndicatorButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] GameMenuPage.Type pageType;
        [SerializeField] float scrollTo;

        [Space]
        [SerializeField] GameMenuController menuController;

        private RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = (RectTransform)transform;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            rectTransform.DOScaleX(1.05f, 0.15f);
            rectTransform.DOScaleY(1.05f, 0.2f);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            rectTransform.DOScaleX(1.0f, 0.15f);
            rectTransform.DOScaleY(1.0f, 0.2f);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            AudioController.PlaySound(AudioController.Settings.sounds.buttonSound);

            menuController.SelectPage(pageType, scrollTo);
        }
    }
}
