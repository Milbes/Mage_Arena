#pragma warning disable 649

using UnityEngine;

namespace Watermelon
{
    [System.Serializable]
    public class MaterialPresetItem : PresetItem
    {

        [SerializeField]
        private Color albedoColor;
        [SerializeField]
        private bool isEmisionEnabled;
        [SerializeField]
        private Color emissionColor;


        public Color AlbedoColor { get => albedoColor;}
        public bool IsEmisionEnabled { get => isEmisionEnabled; }
        public Color EmissionColor { get => emissionColor;}
    }
}
