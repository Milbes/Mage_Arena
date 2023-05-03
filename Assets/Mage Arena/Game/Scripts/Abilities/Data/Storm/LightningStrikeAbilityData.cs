using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{

    [CreateAssetMenu(fileName = "Lightning Strike Ability", menuName = "Content/Abilities/Storm/Lightning Strike Ability")]
    public class LightningStrikeAbilityData : AbilityData
    {
        public LightningStrikeInfo[] lightningStrikeInfos = new LightningStrikeInfo[] {
            new LightningStrikeInfo { damage = 1f, chance = 0.2f},
            new LightningStrikeInfo { damage = 1.5f, chance = 0.4f},
            new LightningStrikeInfo { damage = 2f, chance = 0.6f}};

        [Space]
        public GameObject lightningParticle;
        public AudioClip lightningStrikeSound;
        [Space]
        public float particleDisableDelay = 0.5f;
        public float damageDelay = 0.3f;

        public override Ability CreateAbilityInstance()
        {
            return new LightningStrikeAbility { data = this };
        }
    }

    [System.Serializable]
    public struct LightningStrikeInfo
    {
        [Tooltip("Percent of player's damage")]
        public float damage;
        public float chance;
    }

}