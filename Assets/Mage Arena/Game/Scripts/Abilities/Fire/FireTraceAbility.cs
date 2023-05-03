using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{

    public class FireTraceAbility : Ability, IOnTickAbility
    {
        private static readonly string POOL_NAME = "Fire Trace Ability Projectile";

        public FireTraceAbilityData data;

        private Transform ownerTransform;
        private ParticleSystem trail;

        private static Pool fireTraceProjectilesPool;

        private Queue<FireTraceProjectileInfo> projectiles;

        private float lastSpawnTime = 0;

        private bool shouldTick = false;

        public override void ActivateAbility(IGameplayEntity owner)
        {
            base.ActivateAbility(owner);

            if (!PoolManager.ContainsPool(POOL_NAME))
            {
                fireTraceProjectilesPool = PoolManager.AddPool(new PoolSettings
                {
                    autoSizeIncrement = true,
                    name = "Fire Trace Ability Projectile",
                    objectsContainer = null,
                    size = 10,
                    type = Pool.PoolType.Single,
                    singlePoolPrefab = data.projectileObject
                });
            }

            ownerTransform = owner.GetTransform();

            trail = Object.Instantiate(data.particleObject).GetComponent<ParticleSystem>();
            trail.transform.localScale = Vector3.one * 1;

            projectiles = new Queue<FireTraceProjectileInfo>();

            shouldTick = true;
        }

        public override void OnRoomFinished()
        {
            trail.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            shouldTick = false;

            while(projectiles.Count > 0)
            {
                var info = projectiles.Dequeue();

                info.projectile.gameObject.SetActive(false);

                lastSpawnTime = 0;
            }
        }

        public override void OnRoomEntered()
        {
            trail.Play();

            shouldTick = true;

        }

        public void Tick()
        {
            Vector3 ownerPosition = ownerTransform.position.SetY(0.01f);
            trail.transform.position = ownerPosition;

            if (projectiles.Count == 0)
            {
                SpawnProjectile(ownerPosition);
            } else
            {
                if(Time.time - lastSpawnTime > data.spawnTime)
                {
                    SpawnProjectile(ownerPosition);
                }

                var lastInfo = projectiles.Peek();

                if(Time.time - lastInfo.spawnTime > data.lifetime)
                {
                    projectiles.Dequeue();

                    lastInfo.projectile.gameObject.SetActive(false);
                }
            }
        }

        private void SpawnProjectile(Vector3 position)
        {
            ProjectileBehavior projectile = fireTraceProjectilesPool.GetPooledObject().GetComponent<ProjectileBehavior>();

            projectile.Init(new ProjectileInfo
            {
                canPassObstacles = true,
                damages = null,
                statusEffects = new List<StatusEffectInfo>() {
                    new BurningEffectInfo {
                        damagePerTick = data.damagePerTick[stackAmount] * owner.GetBaseDamageAmount(),
                        ticksLeft = data.ticksAmount[stackAmount]
                    }
                },
                direction = Vector3.forward,
                owner = owner,
                spawnPosition = position + Vector3.up * 0.5f,
                targetsPlayer = false
            });

            projectile.IsOnManualControl = true;

            lastSpawnTime = Time.time;

            var info = new FireTraceProjectileInfo
            {
                projectile = projectile,
                spawnTime = lastSpawnTime
            };

            projectiles.Enqueue(info);
        }
    }

    public struct FireTraceProjectileInfo
    {
        public ProjectileBehavior projectile;
        public float spawnTime;
    }

    
}