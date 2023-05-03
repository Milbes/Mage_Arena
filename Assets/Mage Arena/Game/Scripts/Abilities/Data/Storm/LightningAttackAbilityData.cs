using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Lightning Attack Ability", menuName = "Content/Abilities/Storm/Lightning Attack Ability")]
    public class LightningAttackAbilityData : AbilityData
    {
        [Header("Tier 1")]
        public float initialShockDamage = 0.5f;
        public GameObject hitParticle;
        public AudioClip hitSound;

        [Header("Tier 2")]
        public float shockEffectDuration = 5f;
        public float shockEffectDamage = 0.3f;
        public float enemyShockCooldown = 1f;
        public float shockRadius = 2f;

        [Header("Tier 3")]
        public float bounceRadius = 2f;
        public GameObject trailParticle;

        public override Ability CreateAbilityInstance()
        {
            return new LightningAttackAbility { data = this };
        } 
    }
}