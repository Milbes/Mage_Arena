#pragma warning disable 0649

using UnityEngine;

namespace Watermelon
{
    public class AdsDummyController : MonoBehaviour
    {
        [SerializeField]
        private GameObject bannerObject;

        [Space]
        [SerializeField]
        private GameObject interstitialObject;

        [Space]
        [SerializeField]
        private GameObject rewardedVideoObject;

        private RectTransform bannerRectTransform;

        private AdvertisingHandler.RewardedVideoCallback rewardedVideoCallback;
        private AdvertisingHandler.InterstitialCallback interstitialCallback;

        private void Awake()
        {
            bannerRectTransform = (RectTransform)bannerObject.transform;
        }

        public void Init(AdsData settings)
        {
            switch (settings.dummyContainer.bannerPosition)
            {
                case BannerPosition.Bottom:
                    bannerRectTransform.pivot = new Vector2(0.5f, 0.0f);

                    bannerRectTransform.anchorMin = new Vector2(0.0f, 0.0f);
                    bannerRectTransform.anchorMax = new Vector2(1.0f, 0.0f);

                    bannerRectTransform.anchoredPosition = Vector2.zero;
                    break;
                case BannerPosition.Top:
                    bannerRectTransform.pivot = new Vector2(0.5f, 1.0f);

                    bannerRectTransform.anchorMin = new Vector2(0.0f, 1.0f);
                    bannerRectTransform.anchorMax = new Vector2(1.0f, 1.0f);

                    bannerRectTransform.anchoredPosition = Vector2.zero;
                    break;
            }
        }

        public void SetInterstitialCallback(AdvertisingHandler.InterstitialCallback interstitialCallback)
        {
            this.interstitialCallback = interstitialCallback;
        }

        public void SetRewardedVideoCallback(AdvertisingHandler.RewardedVideoCallback rewardedVideoCallback)
        {
            this.rewardedVideoCallback = rewardedVideoCallback;
        }

        public void ShowBanner()
        {
            bannerObject.SetActive(true);
        }

        public void HideBanner()
        {
            bannerObject.SetActive(false);
        }

        public void ShowInterstitial()
        {
            interstitialObject.SetActive(true);
        }

        public void CloseInterstitial()
        {
            interstitialObject.SetActive(false);

            if (AdsManager.OnInterstitialHiddenEvent != null)
                AdsManager.OnInterstitialHiddenEvent.Invoke();
        }

        public void ShowRewardedVideo()
        {
            rewardedVideoObject.SetActive(true);
        }

        public void CloseRewardedVideo()
        {
            rewardedVideoObject.SetActive(false);

            if (AdsManager.OnRewardedAdHiddenEvent != null)
                AdsManager.OnRewardedAdHiddenEvent.Invoke();
        }

        #region Buttons
        public void CloseInterstitialButton()
        {
            if (interstitialCallback != null)
            {
                interstitialCallback.Invoke(true);

                interstitialCallback = null;
            }

            CloseInterstitial();
        }

        public void CloseRewardedVideoButton()
        {
            if (rewardedVideoCallback != null)
            {
                rewardedVideoCallback.Invoke(false);

                rewardedVideoCallback = null;
            }

            CloseRewardedVideo();
        }

        public void GetRewardButton()
        {
            if (rewardedVideoCallback != null)
            {
                rewardedVideoCallback.Invoke(true);

                rewardedVideoCallback = null;
            }

            CloseRewardedVideo();
        }
        #endregion
    }
}

// -----------------
// Advertisement v 1.1f3
// -----------------