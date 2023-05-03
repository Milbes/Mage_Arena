#pragma warning disable 649, 414

using System;
using UnityEngine;

namespace Watermelon
{
    // Colors Module v0.0.1
    [ExecuteInEditMode]
    public class ColorsModuleController : MonoBehaviour
    {
        private static ColorsModuleController instance;

        public static ColorsModuleController Instance { get => instance; }

        [SerializeField]
        private ColorPresetsDatabase colorPresetsDatabase;

        [SerializeField]
        private int selectedPreset;

        [SerializeField]
        private MaterialReference[] materialReferences;

        [SerializeField]
        private UIImageReference[] uIImageReferences;

        [SerializeField]
        private TextureReference[] textureReferences;

        public delegate void OnPresetLoaded(ColorsPreset preset);

        public static event OnPresetLoaded OnPresetLoadedEvent = delegate { };


        private void OnEnable()
        {
            instance = this;
        }


        public void SelectPreset(int newSelectedPreset)
        {
            selectedPreset = Mathf.Clamp(newSelectedPreset, 0, colorPresetsDatabase.Presets.Length - 1);

            for (int i = 0; i < materialReferences.Length; i++)
            {
                materialReferences[i].MeshRenderer.sharedMaterial.color = colorPresetsDatabase.Presets[selectedPreset].MaterialPresetItems[i].AlbedoColor;
                
                if (colorPresetsDatabase.Presets[newSelectedPreset].MaterialPresetItems[i].IsEmisionEnabled)
                {
                    materialReferences[i].MeshRenderer.sharedMaterial.SetColor("_EMISSION", colorPresetsDatabase.Presets[selectedPreset].MaterialPresetItems[i].EmissionColor);                
                    materialReferences[i].MeshRenderer.sharedMaterial.EnableKeyword("_EMISSION");
                }
            }

            for (int i = 0; i < textureReferences.Length; i++)
            {
                textureReferences[i].Material.mainTexture = colorPresetsDatabase.Presets[selectedPreset].TexturePresetItems[i].Texture;
            }

            //onPresetLoadedEvent(colorDatabase.Presets[selectedPreset]);
            OnPresetLoadedEvent?.Invoke(colorPresetsDatabase.Presets[selectedPreset]);
        }

        public static void LoadPresetByLevel(int levelNumber)
        {
            instance.SelectPreset(levelNumber % instance.colorPresetsDatabase.Presets.Length);
        }

        public static void LoadRandomPreset()
        {
            instance.SelectPreset(UnityEngine.Random.Range(0, instance.colorPresetsDatabase.Presets.Length));
        }
    }
}
