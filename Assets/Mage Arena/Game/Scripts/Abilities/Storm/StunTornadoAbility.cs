using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    public class StunTornadoAbility : Ability, IOnOwnerAttackAbility
    {
        private static readonly string STUN_TORNADO_PARTICLE_POOL_NAME = "Stun Tornado Ability Particle";

        public StunTornadoAbilityData data;

        private Dictionary<IGameplayEntity, StunObjects> stunTweens = new Dictionary<IGameplayEntity, StunObjects>();

        private static Pool stunTornadoParticle;

        public override void ActivateAbility(IGameplayEntity owner)
        {
            base.ActivateAbility(owner);

            owner.onEntityAttackEvent += OnOwnerAttack;

            if (!PoolManager.ContainsPool(STUN_TORNADO_PARTICLE_POOL_NAME))
            {
                stunTornadoParticle = PoolManager.AddPool(new PoolSettings { 
                    name = STUN_TORNADO_PARTICLE_POOL_NAME,
                    autoSizeIncrement = true,
                    size = 3,
                    type = Pool.PoolType.Single,
                    singlePoolPrefab = data.stunParticle
                });
            }
        }

        public override void DisableAbility()
        {
            base.DisableAbility();

            owner.onEntityAttackEvent -= OnOwnerAttack;
        }

        public void OnOwnerAttack(OnOwnerAttackInfo info)
        {
            var tornadoInfo = data.stunTornadoInfo[stackAmount];

            if (tornadoInfo.chance < Random.value) return;

            //Selecting an enemy to stun. It should be eligible and not to have been stunned already.

            var availableEnemies = new List<EnemyBehavior>();

            for(int i = 0; i < LevelObjectSpawner.Enemies.Count; i++)
            {
                var enemy = LevelObjectSpawner.Enemies[i];

                if(enemy == null || stunTweens.ContainsKey(enemy) || enemy.IsDying || !enemy.gameObject.activeSelf)
                {
                    continue;
                }

                availableEnemies.Add(enemy);
            }

            if (availableEnemies.IsNullOrEmpty()) return;

            var entityToStun = availableEnemies.GetRandomItem();

            entityToStun.onEntityDied += OnStunedEnemyDied;

            var particle = Object.Instantiate(data.stunParticle).GetComponent<ParticleSystem>();
            particle.transform.position = entityToStun.transform.position + Vector3.up * 0.05f;

            particle.Play();

            if (entityToStun.IsBoss)
            {
                StatusEffectsController
                    .GetStatusEffect<SlowStatusEffect>(StatusEffectType.Slow)
                    .Register(new SlowStatusEffectInfo { 
                        entity = entityToStun,
                        particle = particle.transform,
                        duration = tornadoInfo.duration,
                        slowMagnitude = 1 - 0.1f * stackAmount
                    });    
            } else
            {
                StatusEffectsController
                    .GetStatusEffect<StunStatusEffect>(StatusEffectType.Stun)
                    .Register(new StunEffectInfo {
                        entity = entityToStun,
                        duration = tornadoInfo.duration
                    });
            }

            stunTweens.Add(entityToStun, new StunObjects
            {
                tweenCase = Tween.DelayedCall(tornadoInfo.duration, () => DisableStun(entityToStun)),
                particle = particle.gameObject,
                particleSystem = particle
            });
        }

        public void OnStunedEnemyDied(IGameplayEntity entity)
        {
            stunTweens[entity].tweenCase.Kill();

            DisableStun(entity);
        }

        private void DisableStun(IGameplayEntity entity)
        {
            entity.onEntityDied -= OnStunedEnemyDied;

            stunTweens[entity].particleSystem.Stop();
            stunTweens[entity].particle.SetActive(false);

            stunTweens.Remove(entity);
        }
    }

    [System.Serializable]
    public struct StunTornadoInfo
    {
        [Tooltip("Percent of player's damage")]
        public float duration;
        public float chance;
    }

    struct StunObjects
    {
        public TweenCase tweenCase;
        public ParticleSystem particleSystem;
        public GameObject particle;
    }
}