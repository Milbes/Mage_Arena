#pragma warning disable 649, 414

using UnityEngine;

namespace Watermelon
{
    [System.Serializable]
    public class UIImagePresetItem : PresetItem
    {
        [SerializeField]
        private Color color;

        public Color Color { get => color;}
    }
}