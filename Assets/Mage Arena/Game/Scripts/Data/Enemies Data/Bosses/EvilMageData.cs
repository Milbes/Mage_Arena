using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{

    [CreateAssetMenu(menuName = "Data/Enemy/Boss/Evil Mage", fileName = "Evil Mage Data")]
    public class EvilMageData : EnemyData
    {
        [Space]
        [SerializeField] float idleDuration;

        public float IdleDuration => idleDuration;

        [Space]
        [SerializeField] AudioClip simpleAttackAudio;
        [SerializeField] AudioClip meteorAttackAudio;

        [Space]
        [SerializeField] Projectile meteorProjectile;
        [SerializeField] Projectile simpleProjectile;

        [Space]
        [SerializeField] float firstProjectileDamage;
        [SerializeField] float secondProjectileDamage;

        [Header("Clones Data")]
        [SerializeField] float clonesHealth;
        [SerializeField] GameObject teleportationParticleObject;

        public float ClonesHealth => clonesHealth;
        public GameObject TeleportationParticleObject => teleportationParticleObject;

        [Header("Explosion Data")]

        [SerializeField] float durationBeforeExplosion = 2f;
        [SerializeField] float durationExplosion = 1f;
        [SerializeField] List<Damage> explosionDamage;

        public float DurationBeforeExplosion => durationBeforeExplosion;
        public float DurationExplosion => durationBeforeExplosion;

        public List<Damage> GetExplosionDamage()
        {
            List<Damage> result = new List<Damage>();

            for(int i = 0; i < explosionDamage.Count; i++)
            {
                Damage newDamage = explosionDamage[i].Copy();
                newDamage.amount *= GameController.CurrentRoom.DamageMultiplier;

                result.Add(newDamage);
            }

            return result;
        }

        public Projectile MeteorProjectile => meteorProjectile;
        public Projectile SimplePriojectile => simpleProjectile;

        public float FirstProjectileDamage => GameController.CurrentRoom.DamageMultiplier * firstProjectileDamage;
        public float SecondProjectileDamage => GameController.CurrentRoom.DamageMultiplier * secondProjectileDamage;

        public AudioClip SimpleAttackAudio => simpleAttackAudio;
        public AudioClip MeteorAttackAudio => meteorAttackAudio;
    }

}