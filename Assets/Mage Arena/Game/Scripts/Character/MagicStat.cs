using UnityEngine;

namespace Watermelon
{
    [System.Serializable]
    public struct MagicStat
    {
        [SerializeField] DamageType magicType;
        public DamageType MagicType => magicType;

        [SerializeField] float value;
        public float Value => value;

        public MagicStat(DamageType magicType)
        {
            this.magicType = magicType;
            this.value = 0;
        }

        public void SetValue(float value)
        {
            this.value = value;
        }

        public void AddValue(float value)
        {
            this.value += value;
        }
    }
}