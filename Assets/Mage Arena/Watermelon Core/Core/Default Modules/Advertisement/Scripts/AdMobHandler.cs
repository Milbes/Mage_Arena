using UnityEngine;
using System;
using System.Collections.Generic;
#if MODULE_ADMOB
using GoogleMobileAds.Api;
#endif

#if MODULE_ADMOB
namespace Watermelon
{
    public class AdMobHandler : AdvertisingHandler
    {
        private const int RETRY_ATTEMPT_DEFAULT_VALUE = 1;

        private int interstitialRetryAttempt = RETRY_ATTEMPT_DEFAULT_VALUE;
        private int rewardedRetryAttempt = RETRY_ATTEMPT_DEFAULT_VALUE;

        private BannerView bannerView;
        private InterstitialAd interstitial;
        private RewardBasedVideoAd rewardBasedVideo;

        private AdRequest adRequest;

        private readonly List<string> TEST_DEVICES = new List<string>()
        {
            "9ED87174BA40D80E",
            "03025839C6157A0A",
            "D3C91C4185B0B88C"
        };

        public AdMobHandler(AdvertisingModules moduleType) : base(moduleType) { }

        public override void Init(AdsData adsSettings)
        {
            this.adsSettings = adsSettings;

            adRequest = CreateAdRequest();

            if(adsSettings.systemLogs)
                Debug.Log("[AdsManager]: AdMob is trying to initialize!");

            MobileAds.SetiOSAppPauseOnBackground(true);
            MobileAds.SetRequestConfiguration(new RequestConfiguration.Builder().SetTestDeviceIds(TEST_DEVICES).build());

            // Initialize the Google Mobile Ads SDK.
            MobileAds.Initialize(InitCompleteAction);
        }

        private void InitCompleteAction(InitializationStatus initStatus)
        {
            GoogleMobileAds.Common.MobileAdsEventExecutor.ExecuteInUpdate(() =>
            {
                // Get singleton reward based video ad reference.
                rewardBasedVideo = RewardBasedVideoAd.Instance;

                // RewardBasedVideoAd is a singleton, so handlers should only be registered once.
                rewardBasedVideo.OnAdLoaded += HandleRewardBasedVideoLoaded;
                rewardBasedVideo.OnAdFailedToLoad += HandleRewardBasedVideoFailedToLoad;
                rewardBasedVideo.OnAdOpening += HandleRewardBasedVideoOpened;
                rewardBasedVideo.OnAdStarted += HandleRewardBasedVideoStarted;
                rewardBasedVideo.OnAdRewarded += HandleRewardBasedVideoRewarded;
                rewardBasedVideo.OnAdClosed += HandleRewardBasedVideoClosed;
                rewardBasedVideo.OnAdLeavingApplication += HandleRewardBasedVideoLeftApplication;

                isInitialized = true;

                RequestRewardedVideo();
                RequestInterstitial();
                RequestBanner();

                if (AdsManager.OnAdsModuleInitializedEvent != null)
                    AdsManager.OnAdsModuleInitializedEvent.Invoke(ModuleType);

                if (adsSettings.systemLogs)
                    Debug.Log("[AdsManager]: AdMob is initialized!");
            });
        }

        public override void DestroyBanner()
        {
            if (bannerView != null)
                bannerView.Destroy();
        }

        public override void HideBanner()
        {
            if (bannerView != null)
                bannerView.Hide();
        }

        public override void RequestInterstitial()
        {
            if (!isInitialized)
                return;

            // Clean up interstitial ad before creating a new one.
            if (interstitial != null)
            {
                interstitial.Destroy();
            }

            // Create an interstitial.
            interstitial = new InterstitialAd(GetInterstitialID());

            // Register for ad events.
            interstitial.OnAdLoaded += HandleInterstitialLoaded;
            interstitial.OnAdFailedToLoad += HandleInterstitialFailedToLoad;
            interstitial.OnAdOpening += HandleInterstitialOpened;
            interstitial.OnAdClosed += HandleInterstitialClosed;
            interstitial.OnAdLeavingApplication += HandleInterstitialLeftApplication;

            // Load an interstitial ad.
            interstitial.LoadAd(adRequest);
        }

        public override void RequestRewardedVideo()
        {
            if (!isInitialized)
                return;

            rewardBasedVideo.LoadAd(adRequest, GetRewardedVideoID());
        }

        private void RequestBanner()
        {
            // Clean up banner before reusing
            if (bannerView != null)
            {
                bannerView.Destroy();
            }

            AdSize adSize = AdSize.Banner;

            switch (adsSettings.adMobContainer.bannerType)
            {
                case AdsData.AdMobContainer.BannerType.Banner:
                    adSize = AdSize.Banner;
                    break;
                case AdsData.AdMobContainer.BannerType.MediumRectangle:
                    adSize = AdSize.MediumRectangle;
                    break;
                case AdsData.AdMobContainer.BannerType.IABBanner:
                    adSize = AdSize.IABBanner;
                    break;
                case AdsData.AdMobContainer.BannerType.Leaderboard:
                    adSize = AdSize.Leaderboard;
                    break;
                case AdsData.AdMobContainer.BannerType.SmartBanner:
                    adSize = AdSize.SmartBanner;
                    break;
            }

            AdPosition adPosition = AdPosition.Bottom;
            switch (adsSettings.adMobContainer.bannerPosition)
            {
                case BannerPosition.Bottom:
                    adPosition = AdPosition.Bottom;
                    break;
                case BannerPosition.Top:
                    adPosition = AdPosition.Top;
                    break;
            }

            bannerView = new BannerView(GetBannerID(), adSize, adPosition);

            // Register for ad events.
            bannerView.OnAdLoaded += HandleAdLoaded;
            bannerView.OnAdFailedToLoad += HandleAdFailedToLoad;
            bannerView.OnAdOpening += HandleAdOpened;
            bannerView.OnAdClosed += HandleAdClosed;
            bannerView.OnAdLeavingApplication += HandleAdLeftApplication;

            // Load a banner ad.
            bannerView.LoadAd(adRequest);
        }

        public override void ShowBanner()
        {
            if (!isInitialized)
                return;

            bannerView.Show();
        }

        public override void ShowInterstitial(InterstitialCallback callback)
        {
            if (!isInitialized)
            {
                if(callback != null)
                    callback.Invoke(false);

                return;
            }

            interstitialCallback = callback;

            interstitial.Show();
        }

        public override void ShowRewardedVideo(RewardedVideoCallback callback)
        {
            if (!isInitialized)
                return;

            rewardedVideoCallback = callback;

            rewardBasedVideo.Show();
        }

        public override bool IsInterstitialLoaded()
        {
            return interstitial != null && interstitial.IsLoaded();
        }

        public override bool IsRewardedVideoLoaded()
        {
            return rewardBasedVideo != null && rewardBasedVideo.IsLoaded();
        }

        public override void SetGDPR(bool state)
        {
            adRequest = CreateAdRequest();
        }

        public override bool IsGDPRRequired()
        {
            return true;
        }

        public AdRequest CreateAdRequest()
        {
            AdRequest.Builder builder = new AdRequest.Builder();

            if (adsSettings.testMode)
                builder = builder.AddTestDevice("*");

            return builder.Build();
        }

#region Banner Callbacks
        public void HandleAdLoaded(object sender, System.EventArgs args)
        {
            if (adsSettings.systemLogs)
                Debug.Log("[AdsManager]: HandleAdLoaded event received");

            if (AdsManager.OnBannerAdLoadedEvent != null)
                AdsManager.OnBannerAdLoadedEvent.Invoke();
        }

        public void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            if (adsSettings.systemLogs)
                Debug.Log("[AdsManager]: HandleFailedToReceiveAd event received with message: " + args.Message);

            if (AdsManager.OnBannerAdLoadFailedEvent != null)
                AdsManager.OnBannerAdLoadFailedEvent.Invoke();
        }

        public void HandleAdOpened(object sender, System.EventArgs args)
        {
            if (adsSettings.systemLogs)
                Debug.Log("[AdsManager]: HandleAdOpened event received");

#if MODULE_GA
            GameAnalyticsSDK.GameAnalytics.NewAdEvent(GameAnalyticsSDK.GAAdAction.Clicked, GameAnalyticsSDK.GAAdType.Banner, ModuleType.ToString(), GetBannerID());
#endif

            if (AdsManager.OnBannerAdClickedEvent != null)
                AdsManager.OnBannerAdClickedEvent.Invoke();
        }

        public void HandleAdClosed(object sender, System.EventArgs args)
        {
            if (adsSettings.systemLogs)
                Debug.Log("[AdsManager]: HandleAdClosed event received");
        }

        public void HandleAdLeftApplication(object sender, System.EventArgs args)
        {
            if (adsSettings.systemLogs)
                Debug.Log("[AdsManager]: HandleAdLeftApplication event received");
        }
#endregion

#region Interstitial Callback
        public void HandleInterstitialLoaded(object sender, System.EventArgs args)
        {
            if (adsSettings.systemLogs)
                Debug.Log("[AdsManager]: HandleInterstitialLoaded event received");

            interstitialRetryAttempt = RETRY_ATTEMPT_DEFAULT_VALUE;

            if (AdsManager.OnInterstitialLoadedEvent != null)
                AdsManager.OnInterstitialLoadedEvent.Invoke();
        }

        public void HandleInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            if (adsSettings.systemLogs)
                Debug.Log("[AdsManager]: HandleInterstitialFailedToLoad event received with message: " + args.Message);

            if (AdsManager.OnInterstitialLoadFailedEvent != null)
                AdsManager.OnInterstitialLoadFailedEvent.Invoke();

            interstitialRetryAttempt++;
            float retryDelay = Mathf.Pow(2, interstitialRetryAttempt);

            Tween.DelayedCall(interstitialRetryAttempt, () => AdsManager.RequestInterstitial(), true, TweenType.Update);
        }

        public void HandleInterstitialOpened(object sender, System.EventArgs args)
        {
            if (adsSettings.systemLogs)
                Debug.Log("[AdsManager]: HandleInterstitialOpened event received");

#if MODULE_GA
            GameAnalyticsSDK.GameAnalytics.NewAdEvent(GameAnalyticsSDK.GAAdAction.Show, GameAnalyticsSDK.GAAdType.Interstitial, ModuleType.ToString(), GetInterstitialID());
#endif

            if (AdsManager.OnInterstitialDisplayedEvent != null)
                AdsManager.OnInterstitialDisplayedEvent.Invoke();
        }

        public void HandleInterstitialClosed(object sender, System.EventArgs args)
        {
            if (adsSettings.systemLogs)
                Debug.Log("[AdsManager]: HandleInterstitialClosed event received");

            if (AdsManager.OnInterstitialHiddenEvent != null)
                AdsManager.OnInterstitialHiddenEvent.Invoke();

            if (interstitialCallback != null)
                interstitialCallback.Invoke(true);

            AdsManager.ResetInterstitialDelayTime();
            AdsManager.RequestInterstitial();
        }

        public void HandleInterstitialLeftApplication(object sender, System.EventArgs args)
        {
            if (adsSettings.systemLogs)
                Debug.Log("[AdsManager]: HandleInterstitialLeftApplication event received");

#if MODULE_GA
            GameAnalyticsSDK.GameAnalytics.NewAdEvent(GameAnalyticsSDK.GAAdAction.Clicked, GameAnalyticsSDK.GAAdType.Interstitial, ModuleType.ToString(), GetInterstitialID());
#endif

            if (AdsManager.OnInterstitialClickedEvent != null)
                AdsManager.OnInterstitialClickedEvent.Invoke();
        }
#endregion

#region RewardedVideo Callback
        public void HandleRewardBasedVideoLoaded(object sender, System.EventArgs args)
        {
            if (adsSettings.systemLogs)
                Debug.Log("[AdsManager]: HandleRewardBasedVideoLoaded event received");

            rewardedRetryAttempt = RETRY_ATTEMPT_DEFAULT_VALUE;

            if (AdsManager.OnRewardedAdLoadedEvent != null)
                AdsManager.OnRewardedAdLoadedEvent.Invoke();
        }

        public void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            if (rewardedVideoCallback != null)
            {
                rewardedVideoCallback.Invoke(false);

                rewardedVideoCallback = null;
            }

#if MODULE_GA
            GameAnalyticsSDK.GameAnalytics.NewAdEvent(GameAnalyticsSDK.GAAdAction.FailedShow, GameAnalyticsSDK.GAAdType.RewardedVideo, ModuleType.ToString(), GetRewardedVideoID());
#endif

            if (adsSettings.systemLogs)
                Debug.Log("[AdsManager]: HandleRewardBasedVideoFailedToLoad event received with message: " + args.Message);

            rewardedRetryAttempt++;
            float retryDelay = Mathf.Pow(2, rewardedRetryAttempt);

            Tween.DelayedCall(rewardedRetryAttempt, () => AdsManager.RequestRewardBasedVideo(), true, TweenType.Update);

            if (AdsManager.OnRewardedAdLoadFailedEvent != null)
                AdsManager.OnRewardedAdLoadFailedEvent.Invoke();
        }

        public void HandleRewardBasedVideoOpened(object sender, System.EventArgs args)
        {
            if (adsSettings.systemLogs)
                Debug.Log("[AdsManager]: HandleRewardBasedVideoOpened event received");

#if MODULE_GA
            GameAnalyticsSDK.GameAnalytics.NewAdEvent(GameAnalyticsSDK.GAAdAction.Show, GameAnalyticsSDK.GAAdType.RewardedVideo, ModuleType.ToString(), GetRewardedVideoID());
#endif

            if (AdsManager.OnRewardedAdDisplayedEvent != null)
                AdsManager.OnRewardedAdDisplayedEvent.Invoke();
        }

        public void HandleRewardBasedVideoStarted(object sender, System.EventArgs args)
        {
            if (adsSettings.systemLogs)
                Debug.Log("[AdsManager]: HandleRewardBasedVideoStarted event received");
        }

        public void HandleRewardBasedVideoClosed(object sender, System.EventArgs args)
        {
            if (rewardedVideoCallback != null)
            {
                rewardedVideoCallback.Invoke(false);

                rewardedVideoCallback = null;
            }

            if (adsSettings.systemLogs)
                Debug.Log("[AdsManager]: HandleRewardBasedVideoClosed event received");

            if (AdsManager.OnRewardedAdHiddenEvent != null)
                AdsManager.OnRewardedAdHiddenEvent.Invoke();

            AdsManager.ResetInterstitialDelayTime();
            AdsManager.RequestRewardBasedVideo();
        }

        public void HandleRewardBasedVideoRewarded(object sender, GoogleMobileAds.Api.Reward args)
        {
            if (rewardedVideoCallback != null)
            {
                rewardedVideoCallback.Invoke(true);

                rewardedVideoCallback = null;
            }

            string type = args.Type;
            double amount = args.Amount;

            if (adsSettings.systemLogs)
                Debug.Log("[AdsManager]: HandleRewardBasedVideoRewarded event received for " + amount.ToString() + " " + type);

#if MODULE_GA
            GameAnalyticsSDK.GameAnalytics.NewAdEvent(GameAnalyticsSDK.GAAdAction.RewardReceived, GameAnalyticsSDK.GAAdType.RewardedVideo, ModuleType.ToString(), GetRewardedVideoID());
#endif

            if (AdsManager.OnRewardedAdReceivedRewardEvent != null)
                AdsManager.OnRewardedAdReceivedRewardEvent.Invoke();
        }

        public void HandleRewardBasedVideoLeftApplication(object sender, System.EventArgs args)
        {
            if (adsSettings.systemLogs)
                Debug.Log("[AdsManager]: HandleRewardBasedVideoLeftApplication event received");

#if MODULE_GA
            GameAnalyticsSDK.GameAnalytics.NewAdEvent(GameAnalyticsSDK.GAAdAction.Clicked, GameAnalyticsSDK.GAAdType.RewardedVideo, ModuleType.ToString(), GetRewardedVideoID());
#endif

            if (AdsManager.OnRewardedAdClickedEvent != null)
                AdsManager.OnRewardedAdClickedEvent.Invoke();
        }
#endregion

        public override string GetBannerID()
        {
#if UNITY_EDITOR
            return "unused";
#elif UNITY_ANDROID
            return adsSettings.adMobContainer.androidBannerID;
#elif UNITY_IOS
            return adsSettings.adMobContainer.IOSBannerID;
#else
            return "unexpected_platform";
#endif
        }

        public override string GetInterstitialID()
        {
#if UNITY_EDITOR
            return "unused";
#elif UNITY_ANDROID
            return adsSettings.adMobContainer.androidInterstitialID;
#elif UNITY_IOS
            return adsSettings.adMobContainer.IOSInterstitialID;
#else
            return "unexpected_platform";
#endif
        }

        public override string GetRewardedVideoID()
        {
#if UNITY_EDITOR
            return "unused";
#elif UNITY_ANDROID
            return adsSettings.adMobContainer.androidRewardedVideoID;
#elif UNITY_IOS
            return adsSettings.adMobContainer.IOSRewardedVideoID;
#else
            return "unexpected_platform";
#endif
        }
    }
}
#endif

// -----------------
// Advertisement v 1.1f3
// -----------------