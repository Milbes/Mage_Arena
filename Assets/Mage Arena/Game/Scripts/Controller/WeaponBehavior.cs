#pragma warning disable 649

using System.Collections.Generic;
using UnityEngine;
using Watermelon;

namespace Watermelon
{
    public class WeaponBehavior : MonoBehaviour
    {

        [Header("Projectile")]
        [SerializeField] Transform projectilesSpawnPoint;
        [SerializeField] Projectile projectile;
        [SerializeField] float fireDelayPercentage;

        private ParticleSystem spawnParticle;

        public float FireDelayPrecentage => fireDelayPercentage;

        private WeaponItem weaponItem;

        public void Init(WeaponItem weaponItem)
        {
            this.weaponItem = weaponItem;

            if(projectile.ParticleSpawnProjectile != null)
            {
                spawnParticle = Instantiate(projectile.ParticleSpawnProjectile).GetComponent<ParticleSystem>();

                spawnParticle.gameObject.SetActive(false);

                spawnParticle.transform.parent = projectilesSpawnPoint;

                spawnParticle.transform.localPosition = Vector3.zero;
                spawnParticle.transform.rotation = Quaternion.identity;
                spawnParticle.transform.localScale = Vector3.one;
            } else
            {
                spawnParticle = null;
            }

            ProjectilesController.CreateProjectilePool(projectile);
        }

        public void Fire(EnemyBehavior target)
        {
            Vector3 direction = (target.Position - transform.position).normalized; 

            direction.y = 0;

            if (weaponItem.HitAudio != null)
            {
                GameAudioController.PlaySound(projectile.SpawnSound);
            }

            ProjectileInfo projectileInfo = new ProjectileInfo
            {
                owner = PlayerController.playerController,

                projectile = projectile,
                damages = PlayerController.GetDamage(),
                statusEffects = PlayerController.GetStatusEffects(),
                direction = direction,
                spawnPosition = projectilesSpawnPoint.transform.position,
                targetsPlayer = false,
                canPassObstacles = false
            };

            AbilitiesController.ProcessProjectile(projectileInfo);

            if (spawnParticle != null)
            {
                spawnParticle.transform.localScale = Vector3.one;
                spawnParticle.gameObject.SetActive(true);

                spawnParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                spawnParticle.Play();
            }
        }
    }
}