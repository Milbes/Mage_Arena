using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

namespace Watermelon
{
    public class AdGemsButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        private const string LAST_ADS_PREFS_NAME = "last_ads_coins";

        [SerializeField] int coinsAmount;
        [SerializeField] float cooldownMinutes;

        [Space]
        [SerializeField] int cloudCoinsAmount = 10;

        [Header("Refferences")]
        [SerializeField] GameObject adButton;
        [SerializeField] GameObject disabledButton;

        [Space]
        [SerializeField] TextMeshProUGUI cooldownText;
        [SerializeField] TextMeshProUGUI cooldownShadowText;

        [Space]
        [SerializeField] TextMeshProUGUI amountText;
        [SerializeField] TextMeshProUGUI amountShadowText;

        private RectTransform rectTransform;

        private double lastAdsCoins;
        private bool adsIsActive = false;

        private TweenCase coinsTweenCase;

        private void Awake()
        {
            rectTransform = (RectTransform)transform;

            lastAdsCoins = GameSettingsPrefs.Get<double>(LAST_ADS_PREFS_NAME);
            InitButton(lastAdsCoins < TimeUtils.GetCurrentUnixTimestamp());
        }

        private void InitButton(bool state)
        {
            if(state)
            {
                adButton.SetActive(true);
                disabledButton.SetActive(false);
            }
            else
            {
                disabledButton.SetActive(true);
                adButton.SetActive(false);

                StartCoroutine(VisualizeCooldown());
            }

            adsIsActive = state;
        }

        private IEnumerator VisualizeCooldown()
        {
            WaitForSeconds waitForSeconds = new WaitForSeconds(1.0f);

            lastAdsCoins = GameSettingsPrefs.Get<double>(LAST_ADS_PREFS_NAME);

            int seconds = (int)(lastAdsCoins - TimeUtils.GetCurrentUnixTimestamp());
            cooldownText.text = string.Format("{0}:{1:00}", seconds / 60, seconds % 60);
            cooldownShadowText.text = cooldownText.text;

            cooldownText.gameObject.SetActive(true);

            while (seconds > 0)
            {
                yield return waitForSeconds;

                seconds--;

                cooldownText.text = string.Format("{0}:{1:00}", seconds / 60, seconds % 60);
                cooldownShadowText.text = cooldownText.text;
            }

            yield return waitForSeconds;

            InitButton(true);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!adsIsActive)
                return;

            rectTransform.DOScaleX(1.05f, 0.15f);
            rectTransform.DOScaleY(1.05f, 0.2f);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if(rectTransform.localScale != Vector3.one)
            {
                rectTransform.DOScaleX(1.0f, 0.15f);
                rectTransform.DOScaleY(1.0f, 0.2f);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            AudioController.PlaySound(AudioController.Settings.sounds.buttonSound);

            if (adsIsActive)
            {
                AdsManager.ShowRewardBasedVideo((reward) =>
                {
                    Tween.NextFrame(delegate
                    {
                        if (reward)
                        {
                            int coinsOldValue = Currency.Coins;

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

                        GameSettingsPrefs.Set(LAST_ADS_PREFS_NAME, TimeUtils.GetCurrentUnixTimestamp() + (cooldownMinutes * 60));

                        InitButton(false);
                    });
                });
            }
        }
    }
}
