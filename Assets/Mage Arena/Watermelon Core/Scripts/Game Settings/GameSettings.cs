using UnityEngine;

namespace Watermelon
{
    [SetupTab("Settings", texture = "icon_settings", priority = 0)]
    [CreateAssetMenu(fileName = "Game Settings", menuName = "Settings/Game Settings")]
    public class GameSettings : ScriptableObject
    {
        [Tooltip("Delay in seconds between interstitial appearings.")]
        public float interstitialShowingDelay = 30f;
        [Tooltip("Length of game over count down before skipping revive in seconds.")]
        public float reviveCountDownTime = 10f;

        public string rateUsLinkAndroid;
        public string rateUsLinkIOS;
        public string rateUsLinkDefault;
    }
}
