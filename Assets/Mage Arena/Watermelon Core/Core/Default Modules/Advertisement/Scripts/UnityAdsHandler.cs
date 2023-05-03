using UnityEngine;

#if MODULE_UNITYADS
using UnityEngine.Advertisements;
#endif

namespace Watermelon
{
#if MODULE_UNITYADS
    public class UnityAdsHandler : AdvertisingHandler
    {
        private const int INIT_CHECK_MAX_ATTEMPT_AMOUNT = 5;

        private static string placementBannerID;
        private static string placementInterstitialID;
        private static string placementRewardedVideoID;

        private UnityAdvertismentListner unityAdvertisment;

        public UnityAdsHandler(AdvertisingModules moduleType) : base(moduleType) { }

        public override void Init(AdsData adsSettings)
        {
            this.adsSettings = adsSettings;

            placementBannerID = GetBannerID();
            placementInterstitialID = GetInterstitialID();
            placementRewardedVideoID = GetRewardedVideoID();

            unityAdvertisment = Initialiser.InitialiserGameObject.AddComponent<UnityAdvertismentListner>();
            unityAdvertisment.Init(adsSettings, this);

            Advertisement.AddListener(unityAdvertisment);
            Advertisement.Initialize(GetUnityAdsAppID(), adsSettings.testMode);

            Advertisement.Banner.SetPosition((UnityEngine.Advertisements.BannerPosition)adsSettings.unityAdsContainer.bannerPosition);

            InitGDPR(AdsManager.GetGDPRState());

            if (adsSettings.systemLogs)
            {
                Debug.Log("[AdsManager]: Unity Ads initialized: " + Advertisement.isInitialized);
                Debug.Log("[AdsManager]: Unity Ads is supported: " + Advertisement.isSupported);
                Debug.Log("[AdsManager]: Unity Ads test mode enabled: " + Advertisement.debugMode);
                Debug.Log("[AdsManager]: Unity Ads version: " + Advertisement.version);
            }

            TryToInitialize();
        }

        private void TryToInitialize(int attempt = 1)
        {
            if(attempt < INIT_CHECK_MAX_ATTEMPT_AMOUNT)
            {
                if (Advertisement.isInitialized)
                {
                    isInitialized = true;

                    if (AdsManager.OnAdsModuleInitializedEvent != null)
                        AdsManager.OnAdsModuleInitializedEvent.Invoke(ModuleType);

                    if (adsSettings.systemLogs)
                        Debug.Log("[AdsManager]: Unity Ads is initialized!");
                }
                else
                {
                    Tween.DelayedCall(0.5f * attempt, () => TryToInitialize(attempt + 1));
                }
            }
        }

        public override void DestroyBanner()
        {
            Advertisement.Banner.Hide(true);
        }

        public override void HideBanner()
        {
            Advertisement.Banner.Hide(false);
        }

        public override void RequestInterstitial()
        {
            if (adsSettings.systemLogs)
                Debug.Log("[AdsManager]: Unity Ads has auto interstitial caching");
        }

        public override void RequestRewardedVideo()
        {
            if (adsSettings.systemLogs)
                Debug.Log("[AdsManager]: Unity Ads has auto video caching");
        }

        public override void ShowBanner()
        {
            Advertisement.Banner.Show(placementBannerID);
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

            Advertisement.Show(placementInterstitialID);
        }

        public override void ShowRewardedVideo(RewardedVideoCallback callback)
        {
            rewardedVideoCallback = callback;

            Advertisement.Show(placementRewardedVideoID);
        }

        public override bool IsInterstitialLoaded()
        {
#if UNITY_EDITOR
            // Requires to show Unity Ads dummy
            return true;
#else
            return Advertisement.IsReady(placementInterstitialID);
#endif
        }

        public override bool IsRewardedVideoLoaded()
        {
#if UNITY_EDITOR
            // Requires to show Unity Ads dummy
            return true;
#else
            return Advertisement.IsReady(placementRewardedVideoID);
#endif
        }

        public string GetUnityAdsAppID()
        {
#if UNITY_ANDROID
            return adsSettings.unityAdsContainer.androidAppID;
#elif UNITY_IOS
            return adsSettings.unityAdsContainer.IOSAppID;
#endif
        }

        public override void SetGDPR(bool state)
        {
            InitGDPR(state);
        }

        private void InitGDPR(bool state)
        {
            string gdprState = state ? "true" : "false";

            MetaData gdprMetaData = new MetaData("gdpr");
            gdprMetaData.Set("consent", gdprState);
            Advertisement.SetMetaData(gdprMetaData);

            MetaData privacyMetaData = new MetaData("privacy");
            privacyMetaData.Set("consent", gdprState);
            Advertisement.SetMetaData(privacyMetaData);
        }

        public override bool IsGDPRRequired()
        {
            return true;
        }

        public override string GetBannerID()
        {
#if UNITY_ANDROID
            return adsSettings.unityAdsContainer.androidBannerID;
#elif UNITY_IOS
            return adsSettings.unityAdsContainer.IOSBannerID;
#endif
        }

        public override string GetInterstitialID()
        {
#if UNITY_ANDROID
            return adsSettings.unityAdsContainer.androidInterstitialID;
#elif UNITY_IOS
            return adsSettings.unityAdsContainer.IOSInterstitialID;
#endif
        }

        public override string GetRewardedVideoID()
        {
#if UNITY_ANDROID
            return adsSettings.unityAdsContainer.androidRewardedVideoID;
#elif UNITY_IOS
            return adsSettings.unityAdsContainer.IOSRewardedVideoID;
#endif
        }

        private class UnityAdvertismentListner : MonoBehaviour, IUnityAdsListener
        {
            private UnityAdsHandler adsHandler;
            private AdsData adsSettings;

            public void Init(AdsData adsSettings, UnityAdsHandler adsHandler)
            {
                this.adsSettings = adsSettings;
                this.adsHandler = adsHandler;
            }

            public void OnUnityAdsDidError(string message)
            {
                if (adsSettings.systemLogs)
                    Debug.Log("[AdsManager]: OnUnityAdsDidError - " + message);
            }

            public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
            {
                if (placementId == placementInterstitialID)
                {
                    if (AdsManager.OnInterstitialHiddenEvent != null)
                        AdsManager.OnInterstitialHiddenEvent.Invoke();

                    if (adsHandler.interstitialCallback != null)
                        adsHandler.interstitialCallback.Invoke(showResult == ShowResult.Finished);

                    AdsManager.ResetInterstitialDelayTime();
                }
                else if (placementId == placementRewardedVideoID)
                {
                    bool state = showResult == ShowResult.Finished;

                    // Reward the player
                    if (adsHandler.rewardedVideoCallback != null)
                    {
                        adsHandler.rewardedVideoCallback.Invoke(state);

                        adsHandler.rewardedVideoCallback = null;
                    }

                    if (state && AdsManager.OnRewardedAdReceivedRewardEvent != null)
                        AdsManager.OnRewardedAdReceivedRewardEvent.Invoke();

                    if (AdsManager.OnRewardedAdHiddenEvent != null)
                        AdsManager.OnRewardedAdHiddenEvent.Invoke();

                    AdsManager.ResetInterstitialDelayTime();
                }

                if (adsSettings.systemLogs)
                    Debug.Log("[AdsManager]: OnUnityAdsDidFinish - " + placementId + ". Result - " + showResult);
            }

            public void OnUnityAdsDidStart(string placementId)
            {
                if (placementId == placementInterstitialID)
                {
                    if (AdsManager.OnInterstitialLoadedEvent != null)
                        AdsManager.OnInterstitialLoadedEvent.Invoke();
                }
                else if (placementId == placementRewardedVideoID)
                {
                    if (AdsManager.OnRewardedAdLoadedEvent != null)
                        AdsManager.OnRewardedAdLoadedEvent.Invoke();
                }

                if (adsSettings.systemLogs)
                    Debug.Log("[AdsManager]: OnUnityAdsDidStart - " + placementId);
            }

            public void OnUnityAdsReady(string placementId)
            {
                if (placementId == placementBannerID)
                {
                    if (AdsManager.OnBannerAdLoadedEvent != null)
                        AdsManager.OnBannerAdLoadedEvent.Invoke();
                }
                else if (placementId == placementInterstitialID)
                {
                    if (AdsManager.OnInterstitialLoadedEvent != null)
                        AdsManager.OnInterstitialLoadedEvent.Invoke();
                }
                else if (placementId == placementRewardedVideoID)
                {
                    if (AdsManager.OnRewardedAdLoadedEvent != null)
                        AdsManager.OnRewardedAdLoadedEvent.Invoke();
                }

                if (adsSettings.systemLogs)
                    Debug.Log("[AdsManager]: OnUnityAdsReady - " + placementId);
            }
        }
    }
#endif
}

// -----------------
// Advertisement v 1.1f3
// -----------------