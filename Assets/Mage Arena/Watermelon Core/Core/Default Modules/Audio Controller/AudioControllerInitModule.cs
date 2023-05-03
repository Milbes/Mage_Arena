#pragma warning disable 0649

using UnityEngine;

namespace Watermelon
{
    [RegisterModule("Core/Audio Controller")]
    public class AudioControllerInitModule : InitModule
    {
        [SerializeField] AudioSettings audioSettings;

        [Space]
        [SerializeField] bool playRandomMusik = true;

        public override void CreateComponent(Initialiser Initialiser)
        {
            AudioController audioController = new AudioController();
            audioController.Init(audioSettings, Initialiser.gameObject);

            if (playRandomMusik)
                AudioController.PlayRandomMusic();
        }

        public AudioControllerInitModule()
        {
            moduleName = "Audio Controller";
        }
    }
}

// -----------------
// Audio Controller v 0.3.1
// -----------------