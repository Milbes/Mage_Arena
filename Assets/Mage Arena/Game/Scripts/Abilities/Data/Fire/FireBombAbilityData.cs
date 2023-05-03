using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Fire Bomb Ability", menuName = "Content/Abilities/Fire/Fire Bomb Ability")]
    public class FireBombAbilityData : AbilityData
    {
        public FireBombInfo[] fireBombInfo = new FireBombInfo[] {
            new FireBombInfo { damage = 1f, chance = 0.2f},
            new FireBombInfo { damage = 1.5f, chance = 0.4f},
            new FireBombInfo { damage = 2f, chance = 0.6f}};

        [Space]
        public GameObject meteorParticle;
        public Vector3 meteorOffset = new Vector3(-1, 2, 1);
        public float meteorDuration = 0.3f;
        [Space]
        public GameObject explosionParticle;
        public AudioClip explosionSound;
        public float particleDisableDelay = 0.5f;
        public float explosionRadius = 0.5f;

        public override Ability CreateAbilityInstance()
        {
            return new FireBombAbility { data = this };
        }
    }

    [System.Serializable]
    public struct FireBombInfo
    {
        [Tooltip("Percent of player's damage")]
        public float damage;
        public float chance;
    }

}