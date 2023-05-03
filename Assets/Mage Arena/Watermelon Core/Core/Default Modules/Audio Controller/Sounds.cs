using UnityEngine;

namespace Watermelon
{
    [System.Serializable]
    public class Sounds
    {
        [Header("UI")]
        public AudioClip buttonSound;

        [Space]
        public AudioClip cloudSound;
        public AudioClip collectSound;

        [Space]
        public AudioClip pageSwipeSound;

        [Space]
        public AudioClip windowOpenSound;
        public AudioClip windowCloseSound;

        [Space]
        public AudioClip chestPlaceSound;
        public AudioClip chestOpenSound;
        public AudioClip chestShiningSound;
    }
}

