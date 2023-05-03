using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Fire Attack Ability", menuName = "Content/Abilities/Fire/Fire Attack Ability")]
    public class FireAttackEffectAbilityData : AbilityData
    {
        [Space]
        public GameObject hitParticle;
        public AudioClip hitSound;
        [Space]
        public GameObject explosionParticle;
        public AudioClip explosionSound;

        public float[] initialFireDamage = new float[] { 1f, 1.5f, 2f };
        public float[] burningDamagePerTick = new float[] { 0.25f, 0.5f, 0.75f };
        public int[] ticksAmount = new int[] { 3, 4, 5 };

        public float explosionRadius = 4f;
        public float affectedEnemiesDamagePerTick = 0.5f;
        public int explosionEffectTicks = 5;

        public override Ability CreateAbilityInstance()
        {
            return new FireAttackEffectAbility { data = this };
        }
    }
}

