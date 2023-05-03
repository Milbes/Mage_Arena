#pragma warning disable 0649 

using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    public class SettingsVibrationToggleButton : SettingsButtonBase
    {
        [SerializeField] Image imageRef;

        [Space]
        [SerializeField] Sprite activeSprite;
        [SerializeField] Sprite disableSprite;

        private bool isActive = true;

        private void Start()
        {
            isActive = GameSettingsPrefs.Get<bool>("vibration");

            if (isActive)
                imageRef.sprite = activeSprite;
            else
                imageRef.sprite = disableSprite;
        }

        public override bool IsActive()
        {
            return AudioController.IsVibrationModuleEnabled();
        }

        public override void OnClick()
        {
            isActive = !isActive;

            if (isActive)
            {
                imageRef.sprite = activeSprite;
                GameSettingsPrefs.Set("vibration", true);
            }
            else
            {
                imageRef.sprite = disableSprite;
                GameSettingsPrefs.Set("vibration", false);
            }

            // Play button sound
            AudioController.PlaySound(AudioController.Settings.sounds.buttonSound);
        }
    }
}

// -----------------
// Settings Panel v 0.2.1
// -----------------