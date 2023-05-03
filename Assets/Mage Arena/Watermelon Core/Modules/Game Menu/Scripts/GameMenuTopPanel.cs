using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
	public class GameMenuTopPanel : MonoBehaviour
	{
        [SerializeField] CanvasGroup topPanelCanvasGroup;

        [Space]
        [SerializeField] TextMeshProUGUI coinsAmountText;
        [SerializeField] TextMeshProUGUI coinsAmountShadowText;
        [SerializeField] Text coinsChangeAmountText;

        [SerializeField] TextMeshProUGUI gemsAmountText;
        [SerializeField] TextMeshProUGUI gemsAmountShadowText;
        [SerializeField] Text gemsChangeAmountText;

        private GameMenuController gameMenuController;

        private TweenCase fadeTweenCase;

        private TweenCaseCollection coinsTweenCaseCollection;
        private TweenCaseCollection gemsTweenCaseCollection;

        private AudioClip collectAudioClip;

        private bool isOpened = false;

        private void OnEnable()
        {
            Currency.OnCurrencyUpdated += OnCurrencyUpdated;
        }

        private void OnDisable()
        {
            Currency.OnCurrencyUpdated -= OnCurrencyUpdated;
        }

        public void Init(GameMenuController gameMenuController)
        {
            this.gameMenuController = gameMenuController;

            collectAudioClip = AudioController.Settings.sounds.collectSound;

            OnCurrencyUpdated(Currencies.Gold, Currency.Coins, -1, 0);
            OnCurrencyUpdated(Currencies.Gem, Currency.Gems, -1, 0);
        }

        public void Show(bool animation = true)
        {
            if (isOpened)
                return;

            if (fadeTweenCase != null && !fadeTweenCase.isCompleted)
                fadeTweenCase.Kill();

            if (animation)
            {
                fadeTweenCase = topPanelCanvasGroup.DOFade(1.0f, 0.3f);
            }
            else
            {
                topPanelCanvasGroup.alpha = 1.0f;
            }

            isOpened = true;
        }

        public void Hide(bool animation = true)
        {
            if (!isOpened)
                return;

            if (fadeTweenCase != null && !fadeTweenCase.isCompleted)
                fadeTweenCase.Kill();

            if (animation)
            {
                fadeTweenCase = topPanelCanvasGroup.DOFade(0.0f, 0.3f);
            }
            else
            {
                topPanelCanvasGroup.alpha = 0.0f;
            }

            isOpened = false;
        }

        private void OnCurrencyUpdated(Currencies currency, int value, int oldValue, int valueDifference)
        {
            switch (currency)
            {
                case Currencies.Gem:
                    gemsAmountText.text = value.ToString();
                    gemsAmountShadowText.text = gemsAmountText.text;

                    if (valueDifference < 0)
                    {
                        if (gemsTweenCaseCollection != null && !gemsTweenCaseCollection.IsComplete())
                            gemsTweenCaseCollection.Kill();

                        AudioController.PlaySound(collectAudioClip);

                        gemsTweenCaseCollection = Tween.BeginTweenCaseCollection();
                        gemsChangeAmountText.gameObject.SetActive(true);

                        gemsChangeAmountText.text = valueDifference.ToString();
                        gemsChangeAmountText.color = gemsChangeAmountText.color.SetAlpha(1.0f);

                        gemsChangeAmountText.rectTransform.anchoredPosition = Vector2.zero;
                        gemsChangeAmountText.rectTransform.DOAnchoredPosition(new Vector2(0, -50), 0.5f).SetEasing(Ease.Type.SineOut);

                        gemsChangeAmountText.DOFade(0, 0.5f).OnComplete(delegate
                        {
                            gemsChangeAmountText.gameObject.SetActive(false);
                        });

                        Tween.EndTweenCaseCollection();
                    }
                    break;
                case Currencies.Gold:
                    coinsAmountText.text = value.ToString();
                    coinsAmountShadowText.text = coinsAmountText.text;

                    if(valueDifference < 0)
                    {
                        if (coinsTweenCaseCollection != null && !coinsTweenCaseCollection.IsComplete())
                            coinsTweenCaseCollection.Kill();

                        AudioController.PlaySound(collectAudioClip);

                        coinsTweenCaseCollection = Tween.BeginTweenCaseCollection();
                        coinsChangeAmountText.gameObject.SetActive(true);

                        coinsChangeAmountText.text = valueDifference.ToString();
                        coinsChangeAmountText.color = coinsChangeAmountText.color.SetAlpha(1.0f);

                        coinsChangeAmountText.rectTransform.anchoredPosition = Vector2.zero;
                        coinsChangeAmountText.rectTransform.DOAnchoredPosition(new Vector2(0, -50), 0.5f).SetEasing(Ease.Type.SineOut);

                        coinsChangeAmountText.DOFade(0, 0.5f).OnComplete(delegate
                        {
                            coinsChangeAmountText.gameObject.SetActive(false);
                        });

                        Tween.EndTweenCaseCollection();
                    }
                    break;
            }
        }
    }
}
