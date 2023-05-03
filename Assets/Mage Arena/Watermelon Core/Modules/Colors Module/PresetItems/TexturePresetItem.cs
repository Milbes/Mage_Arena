#pragma warning disable 649, 414

using UnityEngine;

namespace Watermelon
{
    [System.Serializable]
    public class TexturePresetItem:PresetItem
    {
        
        [SerializeField]
        private Texture2D texture;
        public Texture2D Texture { get => texture; }
    }
}
