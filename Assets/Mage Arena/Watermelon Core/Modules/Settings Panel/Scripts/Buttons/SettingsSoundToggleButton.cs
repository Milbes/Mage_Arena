#pragma warning disable 649

using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    public class SettingsSoundToggleButton : SettingsButtonBase
    {
        [SerializeField] Image imageRef;

        [Space]
        [SerializeField] Sprite activeSprite;
        [SerializeField] Sprite disableSprite;

        [Space]
        [SerializeField] bool both = false;
        [SerializeField] AudioController.AudioType audioType = AudioController.AudioType.Sound;

        private bool isActive = true;

        private void Start()
        {
            if (both)
            {
                isActive = AudioController.GetVolume(AudioController.AudioType.Sound) != 0 || AudioController.GetVolume(AudioController.AudioType.Music) != 0;
            } else
            {
                isActive = AudioController.GetVolume(audioType) != 0;
            }
            
            if (isActive)
                imageRef.sprite = activeSprite;
            else
                imageRef.sprite = disableSprite;
        }

        public override bool IsActive()
        {
            return AudioController.IsAudioModuleEnabled();
        }

        public override void OnClick()
        {
            isActive = !isActive;

            if (isActive)
            {
                imageRef.sprite = activeSprite;

                if (both)
                {
                    AudioController.SetVolume(AudioController.AudioType.Sound, 1f);
                    AudioController.SetVolume(AudioController.AudioType.Music, 1f);
                } else
                {
                    AudioController.SetVolume(audioType, 1f);
                }
            }
            else
            {
                imageRef.sprite = disableSprite;

                if (both)
                {
                    AudioController.SetVolume(AudioController.AudioType.Sound, 0f);
                    AudioController.SetVolume(AudioController.AudioType.Music, 0f);
                } else
                {
                    AudioController.SetVolume(audioType, 0f);
                }
            }

            // Play button sound
            AudioController.PlaySound(AudioController.Settings.sounds.buttonSound);
        }
    }
}

// -----------------
// Settings Panel v 0.2.1
// -----------------