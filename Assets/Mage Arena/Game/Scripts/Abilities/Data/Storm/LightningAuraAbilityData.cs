using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Lightning Aura Ability", menuName = "Content/Abilities/Lightning Aura Ability")]
    public class LightningAuraAbilityData : AbilityData
    {
        public GameObject auraParticle;
        public GameObject lightningParticle;

        public AudioClip lightningStrikeSound;

        [Space]
        public float lightningParticleScale = 1;
        public float lightningParticleDisableDelay = 0.5f;
        public float damageDelay = 0.3f;


        [Space]
        public LightningAuraInfo[] lightningAuraInfo = new LightningAuraInfo[] {
            new LightningAuraInfo{ auraParticleScale = 1, auraRadius = 1, strikeDamage = 0.5f, sameEnemyStrikeCooldown = 2f },
            new LightningAuraInfo{ auraParticleScale = 1.5f, auraRadius = 1.5f, strikeDamage = 1f, sameEnemyStrikeCooldown = 2f },
            new LightningAuraInfo{ auraParticleScale = 2, auraRadius = 2, strikeDamage = 1.5f, sameEnemyStrikeCooldown = 2f },
        };

        public override Ability CreateAbilityInstance()
        {
            return new LightningAuraAbility { data = this };
        }
    }
}