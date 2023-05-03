using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

namespace Watermelon
{
    public class GemsButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] int gemsPrice;
        [SerializeField] int coinsAmount;

        [Space]
        [SerializeField] int cloudCoinsAmount = 10;

        [Header("Refferences")]
        [SerializeField] GameObject coinButton;
        [SerializeField] GameObject disabledButton;

        [Space]
        [SerializeField] TextMeshProUGUI costText;
        [SerializeField] TextMeshProUGUI costShadowText;

        [Space]
        [SerializeField] TextMeshProUGUI costDisabledText;
        [SerializeField] TextMeshProUGUI costDisabledShadowText;

        [Space]
        [SerializeField] TextMeshProUGUI amountText;
        [SerializeField] TextMeshProUGUI amountShadowText;

        private TweenCase coinsTweenCase;

        private RectTransform rectTransform;

        private bool isButtonActive = true;

        private void OnEnable()
        {
            Currency.OnCurrencyUpdated += OnCurrencyUpdated;
        }

        private void OnDisable()
        {
            Currency.OnCurrencyUpdated -= OnCurrencyUpdated;
        }

        private void Awake()
        {
            rectTransform = (RectTransform)transform;

            InitButton();
        }

        private void OnCurrencyUpdated(Currencies currency, int value, int oldValue, int valueDifference)
        {
            if(currency == Currencies.Gem)
            {
                InitButton();
            }
        }

        private void InitButton()
        {
            if (Currency.Gems >= gemsPrice)
            {
                isButtonActive = true;

                coinButton.SetActive(true);
                disabledButton.SetActive(false);
            }
            else
            {
                isButtonActive = false;

                coinButton.SetActive(false);
                disabledButton.SetActive(true);
            }

            costText.text = gemsPrice.ToString();
            costShadowText.text = costText.text;

            costDisabledText.text = costText.text;
            costDisabledShadowText.text = costDisabledText.text;

            amountText.text = coinsAmount.ToString();
            amountShadowText.text = amountText.text;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if(!isButtonActive)
                return;

            rectTransform.DOScaleX(1.05f, 0.15f);
            rectTransform.DOScaleY(1.05f, 0.2f);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!isButtonActive)
                return;

            rectTransform.DOScaleX(1.0f, 0.15f);
            rectTransform.DOScaleY(1.0f, 0.2f);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!isButtonActive)
                return;

            // Show window

            AudioController.PlaySound(AudioController.Settings.sounds.buttonSound);

            if (Currency.Gems >= gemsPrice)
            {
                int coinsOldValue = Currency.Coins;

                Currency.ChangeGems(-gemsPrice, true);
                Currency.ChangeCoins(coinsAmount, false);

                CurrencyCloudController.SpawnCurrency(Currencies.Gold, rectTransform, cloudCoinsAmount, string.Format("+{0}", coinsAmount), delegate
                {
                    if (coinsTweenCase != null && !coinsTweenCase.isCompleted)
                        coinsTweenCase.Kill();

                    coinsTweenCase = Tween.DoFloat(coinsOldValue, coinsOldValue + coinsAmount, GameMenuConsts.MENU_CURRENCY_CLOUD_TEXT_DURATION, (float tweenValue) =>
                    {
                        Currency.RedrawCurrency(Currencies.Gold, (int)tweenValue, coinsOldValue);
                    });
                });
            }
        }
    }
}
