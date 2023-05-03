using UnityEngine;

namespace Watermelon
{
    [SetupTab("Audio", texture = "icon_audio")]
    [CreateAssetMenu(fileName = "Audio Settings", menuName = "Settings/Audio Settings")]
    public class AudioSettings : ScriptableObject
    {
        public AudioClip[] musicAudioClips;

        public Sounds sounds;
        public Vibrations vibrations;

        public bool isMusicEnabled = true;
        public bool isAudioEnabled = true;
        public bool isVibrationEnabled = true;
    }
}

// -----------------
// Audio Controller v 0.3.1
// -----------------