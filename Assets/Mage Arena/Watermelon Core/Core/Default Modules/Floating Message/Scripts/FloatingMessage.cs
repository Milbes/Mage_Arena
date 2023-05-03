﻿using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Watermelon
{
    public class FloatingMessage : MonoBehaviour
    {
        private static FloatingMessage floatingMessage;

        [SerializeField] RectTransform windowRectTransform;
        [SerializeField] CanvasGroup windowCanvasGroup;
        [SerializeField] Text floatingText;
        [SerializeField] Image panelImage;

        [Space]
        [SerializeField] Vector2 panelPadding = new Vector2(30, 25);

        private TweenCase animationTweenCase;

        public void Init()
        {
            floatingMessage = this;

            // Init clickable panel
            EventTrigger trigger = floatingText.gameObject.AddComponent<EventTrigger>();

            // Create new event entry
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((eventData) => { OnPanelClick(); });

            // Add entry to trigger
            trigger.triggers.Add(entry);
        }

        private void OnPanelClick()
        {
            if (floatingMessage.animationTweenCase != null && !floatingMessage.animationTweenCase.isCompleted)
                floatingMessage.animationTweenCase.Kill();

            floatingMessage.animationTweenCase = floatingMessage.windowCanvasGroup.DOFade(0, 0.3f).SetEasing(Ease.Type.CircOut).OnComplete(delegate
            {
                floatingMessage.windowRectTransform.gameObject.SetActive(false);
            });
        }

        public static void ShowMessage(string message)
        {
            if(floatingMessage != null)
            {
                if (floatingMessage.animationTweenCase != null && !floatingMessage.animationTweenCase.isCompleted)
                    floatingMessage.animationTweenCase.Kill();

                floatingMessage.floatingText.text = message;

                Tween.NextFrame(delegate
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(floatingMessage.floatingText.rectTransform);

                    floatingMessage.panelImage.rectTransform.anchoredPosition = floatingMessage.floatingText.rectTransform.anchoredPosition;
                    floatingMessage.panelImage.rectTransform.sizeDelta = floatingMessage.floatingText.rectTransform.sizeDelta + floatingMessage.panelPadding;
                });

                floatingMessage.windowRectTransform.gameObject.SetActive(true);
                floatingMessage.windowRectTransform.localScale = Vector2.zero;

                floatingMessage.windowCanvasGroup.alpha = 1.0f;

                floatingMessage.animationTweenCase = floatingMessage.windowRectTransform.DOScale(1.1f, 0.2f).SetEasing(Ease.Type.CircOut).OnComplete(delegate
                {
                    floatingMessage.animationTweenCase = floatingMessage.windowRectTransform.DOScale(1f, 0.05f).OnComplete(delegate
                    {
                        floatingMessage.animationTweenCase = Tween.DelayedCall(0.4f, delegate
                        {
                            floatingMessage.animationTweenCase = floatingMessage.windowCanvasGroup.DOFade(0, 0.5f).SetEasing(Ease.Type.CircOut).OnComplete(delegate
                            {
                                floatingMessage.windowRectTransform.gameObject.SetActive(false);
                            });
                        });
                    });
                });
            }
            else
            {
                Debug.Log("[Floating Message]: " + message);
                Debug.LogError("[Floating Message]: ShowMessage() method has called, but module isn't initialized! Add Floating Message module to Project Init Settings.");
            }
        }
    }
}

// -----------------
// Floating Message v 0.1
// -----------------

// Changelog
// v 0.1
// • Added basic version