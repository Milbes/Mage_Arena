using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    
    public class FireAttackEffectAbility : Ability, IProjectileEffectAbility, IOnTargetHitByProjectileAbility
    {

        private static readonly string FIRE_ATTACK_HIT_POOL_NAME = "Fire Attack Ability Hit Pool";
        private static readonly string FIRE_ATTACK_EXPLOSION_POOL_NAME = "Fire Attack Ability Explosion Pool";

        public FireAttackEffectAbilityData data;

        private static Pool fireHitPool;
        private static Pool fireExplosionPool;

        public override void ActivateAbility(IGameplayEntity owner)
        {
            base.ActivateAbility(owner);

            if (!PoolManager.ContainsPool(FIRE_ATTACK_HIT_POOL_NAME))
            {
                fireHitPool = PoolManager.AddPool(new PoolSettings
                {
                    name = FIRE_ATTACK_HIT_POOL_NAME,
                    autoSizeIncrement = true,
                    size = 3,
                    type = Pool.PoolType.Single,
                    singlePoolPrefab = data.hitParticle
                });
            }

            if (!PoolManager.ContainsPool(FIRE_ATTACK_EXPLOSION_POOL_NAME))
            {
                fireExplosionPool = PoolManager.AddPool(new PoolSettings
                {
                    name = FIRE_ATTACK_EXPLOSION_POOL_NAME,
                    autoSizeIncrement = true,
                    size = 3,
                    type = Pool.PoolType.Single,
                    singlePoolPrefab = data.hitParticle
                });
            }
        }

        public void OnEnemyDeath(IGameplayEntity enemy)
        {
            enemy.onEntityDied -= OnEnemyDeath;

            var explosionParticle = fireExplosionPool.GetPooledObject().GetComponent<ParticleSystem>();
            explosionParticle.transform.position = enemy.GetTransform().position + Vector3.up * 0.5f;
            explosionParticle.transform.localScale = Vector3.one * 2;
            explosionParticle.Play();

            Tween.DelayedCall(0.5f, () => {
                explosionParticle.Stop();
                explosionParticle.gameObject.SetActive(false);
            });

            Tween.DelayedCall(0.1f, () =>
            {
                GameAudioController.PlaySound(data.explosionSound);
            });

            for(int i = 0; i < LevelObjectSpawner.Enemies.Count; i++)
            {
                EnemyBehavior closeEnemy = LevelObjectSpawner.Enemies[i];

                if(closeEnemy != null && !closeEnemy.IsDying && closeEnemy.gameObject.activeSelf && closeEnemy != enemy && Vector3.Distance(enemy.GetTransform().position, closeEnemy.Position) <= data.explosionRadius)
                {
                    StatusEffectsController.RegisterEffect(closeEnemy, new BurningEffectInfo { damagePerTick = data.affectedEnemiesDamagePerTick * owner.GetBaseDamageAmount(), entity = closeEnemy, ticksLeft = data.explosionEffectTicks });
                }
            }
        }

        public void OnTargetHitByProjectile(ProjectileInfo projectileInfo, IGameplayEntity target)
        {
            int stack = AbilitiesController.GetStackAmount(this) - 1;

            target.GetHit(new Damage
            {
                isCrit = false,
                type = DamageType.Fire,
                amount = projectileInfo.damages[0].amount * data.initialFireDamage[stack]
            });

            var hitParticle = fireHitPool.GetPooledObject().GetComponent<ParticleSystem>();
            hitParticle.transform.position = target.GetTransform().position + Vector3.up * 0.5f;
            hitParticle.Play();

            Tween.DelayedCall(0.5f, () => {
                hitParticle.Stop();
                hitParticle.gameObject.SetActive(false);
            });

            GameAudioController.PlaySound(data.hitSound);

            if (stack == 2)
            {
                target.onEntityDied += OnEnemyDeath;
            } 
        }

        public void AddEffectToProjectile(ProjectileInfo projectileInfo)
        {
            int stackIndex = AbilitiesController.GetStackAmount(this) - 1;

            if(stackIndex >= 1)
            {
                projectileInfo.statusEffects.Add(new BurningEffectInfo {
                    damagePerTick = projectileInfo.damages[0].amount * data.burningDamagePerTick[stackIndex],
                    ticksLeft = data.ticksAmount[stackIndex]
                });
            }
        }
    }
}