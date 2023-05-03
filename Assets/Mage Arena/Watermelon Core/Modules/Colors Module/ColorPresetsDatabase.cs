#pragma warning disable 649, 414

using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Colors Database", menuName = "Content/Color Presets Database", order = 1)]
    public class ColorPresetsDatabase : ScriptableObject
    {
        [SerializeField] ColorsPreset[] presets;
        [SerializeField] ConfigurationElement[] materialGroups;
        [SerializeField] ConfigurationElement[] uiImageGroups;
        [SerializeField] ConfigurationElement[] textureGroups;

        public ConfigurationElement[] MaterialGroups { get => materialGroups; }
        public ConfigurationElement[] UiImageGroups { get => uiImageGroups;}
        public ConfigurationElement[] TextureGroups { get => textureGroups; }
        public ColorsPreset[] Presets { get => presets;}
    }
}