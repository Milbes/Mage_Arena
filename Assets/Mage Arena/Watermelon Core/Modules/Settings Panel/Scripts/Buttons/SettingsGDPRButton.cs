#pragma warning disable 0649 

using UnityEngine;

namespace Watermelon
{
    public class SettingsGDPRButton : SettingsButtonBase
    {
        public override bool IsActive()
        {
            AdsData adsSettings;
#if UNITY_EDITOR
            adsSettings = RuntimeEditorUtils.GetAssetByName<AdsData>("Ads Settings");
#else
            adsSettings = AdsManager.Settings;
#endif

            return adsSettings.gdprContainer.enableGDPR;
        }

        public override void OnClick()
        {
            GDPRPanel.OpenWindow();

            // Play button sound
            AudioController.PlaySound(AudioController.Settings.sounds.buttonSound);
        }
    }
}

// -----------------
// Settings Panel v 0.2.1
// -----------------