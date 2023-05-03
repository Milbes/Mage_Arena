#pragma warning disable 649, 414

using UnityEngine;

namespace Watermelon
{
    [System.Serializable]
    public class ColorsPreset
    {
        [SerializeField] string name;
        [SerializeField] MaterialPresetItem[] materialPresetItems;
        [SerializeField] UIImagePresetItem[]  uIImagePresetItems;
        [SerializeField] TexturePresetItem[]  texturePresetItems;

        public MaterialPresetItem[] MaterialPresetItems { get => materialPresetItems; }
        public UIImagePresetItem[] UIImagePresetItems { get => uIImagePresetItems;}
        public TexturePresetItem[] TexturePresetItems { get => texturePresetItems;}
    }
}
