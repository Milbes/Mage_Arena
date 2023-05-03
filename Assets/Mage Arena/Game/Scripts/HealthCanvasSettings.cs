using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Health Canvas Preset", menuName = "Content/Settings/Health Canvas Preset")]
    public class HealthCanvasSettings : ScriptableObject
    {
        public Vector2 textSpawnPosition;
        public Vector2 textSpawnOffset;

        [Space]
        public Vector2 textMovePosition;
        public Vector2 textMoveOffset;

        [Space]
        public List<TextPreset> textPresets;

        public TextPreset GetPreset(HealthTextType type)
        {
            for(int i = 0; i < textPresets.Count; i++)
            {
                if (type == textPresets[i].type) return textPresets[i];
            }

            return null;
        }
    }

    [System.Serializable]
    public class TextPreset
    {
        public HealthTextType type;
        [Space]
        public Color textColor;
        public Color textColorCrit;
        public Color outlineColor;
        [Space]
        public int spawnFontSize;
        public int fontSize;
        [Space]
        public int spawnFontSizeCrit;
        public int fontSizeCrit;

        [Space]
        public float fadeInDuration;
        public float fadeOutDuration;
        public float fontSizeIncreaseDuration;
        public float moveDuration;
        public float stayDuration;

        public Ease.Type moveEasing;
        public Ease.Type fontSizeEasing;
    }

    public enum HealthTextType
    {
        Base, Fire, Lightning, Air, Shadow, Heal, Ice
    }
}