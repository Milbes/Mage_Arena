using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    
    public class BlindingLightAbility : Ability, IOnOwnerAttackAbility
    {
        private static readonly string BLINDING_LIGHT_PARTICLE_POOL_NAME = "Blinding Light Ability Pool";

        public BlindingLightAbilityData data;

        private Dictionary<IGameplayEntity, StunObjects> stunTweens = new Dictionary<IGameplayEntity, StunObjects>();

        private static Pool explosionPool;

        public override void ActivateAbility(IGameplayEntity owner)
        {
            base.ActivateAbility(owner);

            owner.onEntityAttackEvent += OnOwnerAttack;

            if (!PoolManager.ContainsPool(BLINDING_LIGHT_PARTICLE_POOL_NAME))
            {
                explosionPool = PoolManager.AddPool(new PoolSettings { 
                    name = BLINDING_LIGHT_PARTICLE_POOL_NAME,
                    autoSizeIncrement = true,
                    size = 3,
                    type = Pool.PoolType.Single,
                    singlePoolPrefab = data.particleObject
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
            var tornadoInfo = data.info[stackAmount];

            if (tornadoInfo.chance < Random.value) return;

            //Selecting an enemy to stun. It should be eligible and not to have been stunned already.

            var availableEnemies = new List<EnemyBehavior>();

            for (int i = 0; i < LevelObjectSpawner.Enemies.Count; i++)
            {
                var enemy = LevelObjectSpawner.Enemies[i];

                if (enemy == null || stunTweens.ContainsKey(enemy) || enemy.IsDying || !enemy.gameObject.activeSelf)
                {
                    continue;
                }

                availableEnemies.Add(enemy);
            }

            if (availableEnemies.IsNullOrEmpty()) return;

            var entityToStun = availableEnemies.GetRandomItem();

            entityToStun.onEntityDied += OnStunedEnemyDied;

            var particle = explosionPool.GetPooledObject().GetComponent<ParticleSystem>();
            particle.transform.position = entityToStun.transform.position + Vector3.up * 0.5f;
            particle.transform.localScale = Vector3.one * 0.2f;
            particle.Play();

            entityToStun.SetAbilityTyAttack(false);

            stunTweens.Add(entityToStun, new StunObjects
            {
                tweenCase = Tween.DelayedCall(tornadoInfo.duration, () => DisableAbility(entityToStun)),
            });

            Tween.DelayedCall(0.5f, () => {
                particle.Stop();
                particle.gameObject.SetActive(false);
            });
        }

        public void OnStunedEnemyDied(IGameplayEntity entity)
        {
            stunTweens[entity].tweenCase.Kill();

            DisableAbility(entity);
        }

        private void DisableAbility(IGameplayEntity entity)
        {
            entity.onEntityDied -= OnStunedEnemyDied;

            entity.SetAbilityTyAttack(true);

            stunTweens.Remove(entity);
        }
    }

    struct BlindingLightObjects
    {
        public TweenCase tweenCase;
        public GameObject particle;
    }
}