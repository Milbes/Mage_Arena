using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Purchasing;
using System;

namespace Watermelon
{
    public class DonateGemsButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] ProductKeyType productType;
        [SerializeField] int gemsAmount;

        [Space]
        [SerializeField] int cloudGemsAmount = 10;

        [Header("Refferences")]
        [SerializeField] GameObject iapButton;
        [SerializeField] GameObject disabledButton;

        [Space]
        [SerializeField] TextMeshProUGUI amountText;
        [SerializeField] TextMeshProUGUI amountShadowText;

        [Space]
        [SerializeField] TextMeshProUGUI priceText;
        [SerializeField] TextMeshProUGUI priceShadowText;

        private TweenCase gemsTweenCase;

        private RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = (RectTransform)transform;
        }

        private void Start()
        {
            InitProduct(IAPManager.GetProduct(productType));
        }

        private void InitProduct(Product product)
        {
            iapButton.gameObject.SetActive(false);
            disabledButton.gameObject.SetActive(false);

            if (product != null)
            {
                priceText.text = string.Format("<size='35'>{0}</size> {1}", product.metadata.isoCurrencyCode, product.metadata.localizedPrice);
                priceShadowText.text = priceText.text;

                iapButton.gameObject.SetActive(true);
            }
            else
            {
                disabledButton.gameObject.SetActive(true);
            }

            amountText.text = gemsAmount.ToString();
            amountShadowText.text = amountText.text;

            Tween.NextFrame(delegate
            {
                amountShadowText.rectTransform.sizeDelta = amountShadowText.rectTransform.sizeDelta;
                amountShadowText.rectTransform.anchoredPosition = amountShadowText.rectTransform.anchoredPosition.SetY(-4);
            });
        }

        private void OnEnable()
        {
            IAPManager.OnPurchaseModuleInitted += OnPurchaseInitted;
            IAPManager.OnPurchaseComplete += OnPurchaseComplete;
        }

        private void OnDisable()
        {
            IAPManager.OnPurchaseModuleInitted -= OnPurchaseInitted;
            IAPManager.OnPurchaseComplete -= OnPurchaseComplete;
        }

        private void OnPurchaseInitted()
        {
            InitProduct(IAPManager.GetProduct(productType));
        }

        private void OnPurchaseComplete(ProductKeyType productKeyType)
        {
            if(productType == productKeyType)
            {
                int gemsOldValue = Currency.Gems;

                Currency.ChangeGems(gemsAmount, false);

                CurrencyCloudController.SpawnCurrency(Currencies.Gem, rectTransform, cloudGemsAmount, string.Format("+{0}", gemsAmount), delegate
                {
                    if (gemsTweenCase != null && !gemsTweenCase.isCompleted)
                        gemsTweenCase.Kill();

                    gemsTweenCase = Tween.DoFloat(gemsOldValue, gemsOldValue + gemsAmount, GameMenuConsts.MENU_CURRENCY_CLOUD_TEXT_DURATION, (float tweenValue) =>
                    {
                        Currency.RedrawCurrency(Currencies.Gem, (int)tweenValue, gemsOldValue);
                    });
                });
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            AudioController.PlaySound(AudioController.Settings.sounds.buttonSound);

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
            IAPManager.BuyProduct(productType);
        }
    }
}
