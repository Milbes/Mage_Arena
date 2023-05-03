using System.Collections;
using UnityEngine;

namespace Watermelon
{
    [RegisterModule("Services/Ads Manager")]
    public class AdsManagerInitModule : InitModule
    {
        private const int INIT_ATTEMPTS_AMOUNT = 30;

        public AdsData settings;
        public GameObject dummyCanvasPrefab;

        [Space]
        public bool loadAdOnStart = true;

        private bool isFirstAdLoaded = false;
        private int initAttemps = 0;

        public AdsManagerInitModule()
        {
            moduleName = "Ads Manager";
        }

        public override void CreateComponent(Initialiser Initialiser)
        {
            isFirstAdLoaded = false;
            initAttemps = 0;

            AdsManager.Init(this);

            if(loadAdOnStart)
            {
                Tween.InvokeCoroutine(TryToLoadAdsCoroutine());
            }
        }

        private IEnumerator TryToLoadAdsCoroutine()
        {
            yield return new WaitForSeconds(1.0f);

            while (!isFirstAdLoaded || initAttemps > INIT_ATTEMPTS_AMOUNT)
            {
                if (LoadAds())
                    break;

                yield return new WaitForSeconds(1.0f * (initAttemps + 1));

                initAttemps++;
            }

            if(settings.systemLogs)
                Debug.Log("[AdsManager]: First ads have loaded!");
        }

        private bool LoadAds()
        {
            if (isFirstAdLoaded)
                return true;

            if (settings.gdprContainer.enableGDPR && !AdsManager.IsGDPRStateExist())
                return false;

            bool isRewardedVideoModuleInititalized = AdsManager.IsModuleInititalized(AdsManager.Settings.rewardedVideoType);
            bool isInterstitialModuleInitialized = AdsManager.IsModuleInititalized(AdsManager.Settings.interstitialType);
            bool isBannerModuleInitialized = AdsManager.IsModuleInititalized(AdsManager.Settings.bannerType);

            bool isRewardedVideoActive = AdsManager.Settings.rewardedVideoType != AdvertisingModules.Disable;
            bool isInterstitialActive = AdsManager.Settings.interstitialType != AdvertisingModules.Disable;
            bool isBannerActive = AdsManager.Settings.bannerType != AdvertisingModules.Disable;

            if ((!isRewardedVideoActive || isRewardedVideoModuleInititalized) && (!isInterstitialActive || isInterstitialModuleInitialized) && (!isBannerActive || isBannerModuleInitialized))
            {
                if (isRewardedVideoActive)
                    AdsManager.RequestRewardBasedVideo();

                bool isForcedAdEnabled = AdsManager.IsForcedAdEnabled(false);
                if (isInterstitialActive && isForcedAdEnabled)
                    AdsManager.RequestInterstitial();

                if (isBannerActive && isForcedAdEnabled)
                    AdsManager.ShowBanner();

                isFirstAdLoaded = true;

                return true;
            }

            return false;
        }
    }
}

// -----------------
// Advertisement v 1.1f3
// -----------------