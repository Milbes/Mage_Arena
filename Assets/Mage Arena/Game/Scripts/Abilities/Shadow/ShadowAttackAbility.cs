using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    public class ShadowAttackAbility : Ability, IOnTargetHitByProjectileAbility, IOnTickAbility
    {
        private static readonly string SHADOW_ATTACK_HIT_POOL_NAME = "Shadow Attack Hit Pool";
        private static readonly string SHADOW_ATTACK_EXPLOSION_POOL_NAME = "Shadow Attack Explosion Pool";
        private static readonly string SHADOW_ATTACK_MARK_POOL_NAME = "Shadow Attack Mark Pool";

        private static Pool shadowHitPool;
        private static Pool shadowExplosionPool;
        private static Pool shadowMarkPool;

        public ShadowAttackAbilityData data;

        private Dictionary<IGameplayEntity, ParticleSystem> markParticles;

        private float baseDamage;

        public override void ActivateAbility(IGameplayEntity owner)
        {
            base.ActivateAbility(owner);

            markParticles = new Dictionary<IGameplayEntity, ParticleSystem>();

            if (!PoolManager.ContainsPool(SHADOW_ATTACK_HIT_POOL_NAME))
            {
                shadowHitPool = PoolManager.AddPool(new PoolSettings { 
                    name = SHADOW_ATTACK_HIT_POOL_NAME,
                    autoSizeIncrement = true,
                    size = 3,
                    type = Pool.PoolType.Single,
                    singlePoolPrefab = data.hitParticle
                });
            }

            if (!PoolManager.ContainsPool(SHADOW_ATTACK_EXPLOSION_POOL_NAME))
            {
                shadowExplosionPool = PoolManager.AddPool(new PoolSettings
                {
                    name = SHADOW_ATTACK_EXPLOSION_POOL_NAME,
                    autoSizeIncrement = true,
                    size = 3,
                    type = Pool.PoolType.Single,
                    singlePoolPrefab = data.explosionParticle
                });
            }

            if (!PoolManager.ContainsPool(SHADOW_ATTACK_MARK_POOL_NAME))
            {
                shadowMarkPool = PoolManager.AddPool(new PoolSettings
                {
                    name = SHADOW_ATTACK_MARK_POOL_NAME,
                    autoSizeIncrement = true,
                    size = 3, 
                    type = Pool.PoolType.Single,
                    singlePoolPrefab = data.markParticke
                });
            }
        }

        public void Tick()
        {
            foreach(IGameplayEntity enemy in markParticles.Keys)
            {
                markParticles[enemy].transform.position = enemy.GetTransform().position;
            }
        }

        public void OnTargetHitByProjectile(ProjectileInfo projectileInfo, IGameplayEntity target)
        {
            baseDamage = projectileInfo.damages[0].amount;

            target.GetHit(new Damage
            {
                isCrit = false,
                type = DamageType.Shadow,
                amount = baseDamage * data.initialShadowDamage[stackAmount]
            });

            var hitParticle = shadowHitPool.GetPooledObject().GetComponent<ParticleSystem>();
            hitParticle.transform.position = target.GetTransform().position + Vector3.up * 0.5f;
            hitParticle.Play();

            Tween.DelayedCall(0.5f, () => {
                hitParticle.Stop();

                hitParticle.gameObject.SetActive(false);
            });

            GameAudioController.PlaySound(data.hitSound);

            if (stackAmount > 0 && !markParticles.ContainsKey(target))
            {
                target.onEntityDied += OnEnemyDeath;

                var mark = shadowMarkPool.GetPooledObject().GetComponent<ParticleSystem>();
                mark.transform.position = target.GetTransform().position;
                mark.Play();

                markParticles.Add(target, mark);
            }
        }

        public void OnEnemyDeath(IGameplayEntity enemy)
        {
            enemy.onEntityDied -= OnEnemyDeath;

            var explosionParticle = shadowExplosionPool.GetPooledObject().GetComponent<ParticleSystem>();
            explosionParticle.transform.position = enemy.GetTransform().position + Vector3.up * 0.5f;
            explosionParticle.transform.localScale = Vector3.one * data.particleScale;

            explosionParticle.Play();

            markParticles[enemy].Stop();
            markParticles[enemy].gameObject.SetActive(false);

            markParticles.Remove(enemy);

            Tween.DelayedCall(0.5f, () => {
                explosionParticle.Stop();
                explosionParticle.gameObject.SetActive(false);
            });

            GameAudioController.PlaySound(data.explosionSound);

            for (int i = 0; i < LevelObjectSpawner.Enemies.Count; i++)
            {
                EnemyBehavior closeEnemy = LevelObjectSpawner.Enemies[i];

                if (closeEnemy != null && !closeEnemy.IsDying && closeEnemy.gameObject.activeSelf && closeEnemy != enemy && Vector3.Distance(enemy.GetTransform().position, closeEnemy.Position) <= data.explosionRadius)
                {
                    closeEnemy.GetHit(new Damage {
                        amount = baseDamage * data.explosionDamage,
                        type = DamageType.Shadow,
                        isCrit = false
                    });

                    if(stackAmount > 1 && !markParticles.ContainsKey(closeEnemy))
                    {
                        closeEnemy.onEntityDied += OnEnemyDeath;

                        var mark = shadowMarkPool.GetPooledObject().GetComponent<ParticleSystem>();
                        mark.transform.position = closeEnemy.GetTransform().position;
                        mark.Play();

                        markParticles.Add(closeEnemy, mark);
                    }
                }
            }
        }
    }

}
