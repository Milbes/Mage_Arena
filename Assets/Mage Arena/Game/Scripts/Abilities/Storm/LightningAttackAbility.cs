using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    public class LightningAttackAbility : Ability, IOnTargetHitByProjectileAbility, IProjectileEffectAbility
    {
        private static readonly string LIGHTNING_IMPACT_PARTICLE_POOL_NAME = "Lightning Impact Ability Particle";
        
        public LightningAttackAbilityData data;

        private static Pool impactparticlePool;

        public override void ActivateAbility(IGameplayEntity owner)
        {
            base.ActivateAbility(owner);

            if (!PoolManager.ContainsPool(LIGHTNING_IMPACT_PARTICLE_POOL_NAME))
            {
                impactparticlePool = PoolManager.AddPool(new PoolSettings { 
                    name = LIGHTNING_IMPACT_PARTICLE_POOL_NAME,
                    autoSizeIncrement = true,
                    singlePoolPrefab = data.hitParticle,
                    size = 5,
                    type = Pool.PoolType.Single
                });
            }
        }

        public void OnTargetHitByProjectile(ProjectileInfo projectileInfo, IGameplayEntity target)
        {

            target.GetHit(new Damage
            {
                isCrit = false,
                type = DamageType.Storm,
                amount = projectileInfo.damages[0].amount * data.initialShockDamage
            }); ;

            var hitParticle = impactparticlePool.GetPooledObject().GetComponent<ParticleSystem>();
            hitParticle.transform.position = target.GetTransform().position + Vector3.up * 0.5f;
            hitParticle.Play();

            GameAudioController.PlaySound(data.hitSound);

            Tween.DelayedCall(0.5f, () => {
                hitParticle.Stop();

                hitParticle.gameObject.SetActive(false);
            });


            if(stackAmount > 1)
            {
                for(int i = 0; i < LevelObjectSpawner.Enemies.Count; i++)
                {
                    EnemyBehavior enemy = LevelObjectSpawner.Enemies[i];

                    if (enemy != null && enemy != target && !enemy.IsDying && enemy.gameObject.activeSelf)
                    {
                        Vector3 targetPosition = target.GetTransform().position;
                        Vector3 enemyPosition = enemy.Position;

                        if(Vector3.Distance(targetPosition, enemyPosition) < data.bounceRadius)
                        {
                            GameObject particle = Object.Instantiate(data.trailParticle);

                            particle.transform.position = targetPosition + Vector3.up * 0.5f;
                            particle.transform.localScale = Vector3.one;

                            Tween.NextFrame(() => {
                                particle.transform.position = enemyPosition;
                            });

                            Tween.DelayedCall(0.2f, () =>
                            {
                                Object.Destroy(particle);
                            });

                            StatusEffectsController.RegisterEffect(enemy, new ShockEffectInfo
                            {
                                entity = enemy,
                                damagePerHit = data.shockEffectDamage,
                                durationLeft = data.shockEffectDuration,
                                effectDistance = data.shockRadius,
                                enemyHitCooldown = data.enemyShockCooldown
                            });
                        }
                    }
                }
            }
        }

        public void AddEffectToProjectile(ProjectileInfo projectileInfo)
        {
            if (stackAmount > 0)
            {
                projectileInfo.statusEffects.Add(new ShockEffectInfo
                {
                    enemyHitCooldown = data.enemyShockCooldown,
                    durationLeft = data.shockEffectDuration,
                    damagePerHit = projectileInfo.damages[0].amount * data.shockEffectDamage,
                    effectDistance = data.shockRadius
                });
            }
        }

    }
}