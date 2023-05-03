using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Shadow Attack Ability", menuName = "Content/Abilities/Shadow/Shadow Attack Ability")]
    public class ShadowAttackAbilityData : AbilityData
    {
        [Header("Tier 1")]
        public GameObject hitParticle;
        public AudioClip hitSound;
        public float[] initialShadowDamage = new float[] { 0.4f, 0.6f, 0.8f };

        [Header("Tier 2")]
        public GameObject explosionParticle;
        public GameObject markParticke;
        public AudioClip explosionSound;
        public float explosionRadius = 3f;
        public float particleScale = 1f;
        public float explosionDamage = 1f;

        public override Ability CreateAbilityInstance()
        {
            return new ShadowAttackAbility { data = this };
        }
    }

}