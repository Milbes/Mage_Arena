using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    public class GameAudioController : MonoBehaviour
    {
        private static GameAudioController instance;

        [SerializeField] float minSoundInterval;

        private static float MinSoundInterval => instance.minSoundInterval;

        private static float lastSoundTime;

        private void Awake()
        {
            instance = this;

            Tween.DoFloat(1, 0, 1, (volume) => {
                AudioController.SetVolume(AudioController.AudioType.Music, volume);
            }, true).OnComplete(() => {
                AudioController.ReleaseMusic();
            });

            //AudioController.SetVolume(AudioController.AudioType.Music, 0);
            
        }

        public static void PlaySound(AudioClip sound, float volumeMultiplier = 1f)
        {
            if (sound == null) return;

            float volume = AudioController.GetVolume(AudioController.AudioType.Sound);
            if (volume == 0) return;

            if (Time.time - lastSoundTime <= MinSoundInterval) return;

            lastSoundTime = Time.time;

            AudioController.PlaySound(sound, volume * volumeMultiplier, 1);
        }

        public void OnDestroy()
        {
            Tween.DoFloat(0, 1, 1, (volume) => {
                AudioController.SetVolume(AudioController.AudioType.Music, volume);
            }, true);
            
            AudioController.PlayRandomMusic();
        }
    }
}