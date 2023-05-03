using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    public class LightningStrikeAbility : Ability, IOnOwnerAttackAbility
    {
        private static readonly string LIGHTNING_STRIKE_PARTICLE_POOL_NAME = "Lightning Strike Ability Particle";

        public LightningStrikeAbilityData data;

        private static Pool lightningStrikePool;

        public override void ActivateAbility(IGameplayEntity owner)
        {
            base.ActivateAbility(owner);

            owner.onEntityAttackEvent += OnOwnerAttack;

            if (!PoolManager.ContainsPool(LIGHTNING_STRIKE_PARTICLE_POOL_NAME))
            {
                lightningStrikePool = PoolManager.AddPool(new PoolSettings
                {
                    name = LIGHTNING_STRIKE_PARTICLE_POOL_NAME,
                    autoSizeIncrement = true,
                    size = 3,
                    type = Pool.PoolType.Single,
                    singlePoolPrefab = data.lightningParticle
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
            int stackAmount = AbilitiesController.GetStackAmount(this);

            LightningStrikeInfo lightningInfo = data.lightningStrikeInfos[stackAmount - 1];

            if (lightningInfo.chance >= Random.value)
            {
                EnemyBehavior enemy;

                if (LevelObjectSpawner.Enemies.IsNullOrEmpty()) return;

                int counter = 0;
                do
                {
                    enemy = LevelObjectSpawner.Enemies.GetRandomItem();
                    counter++;
                } while ((enemy == null || enemy.IsDying || !enemy.gameObject.activeSelf) && counter < 50);

                var particle = lightningStrikePool.GetPooledObject().GetComponent<ParticleSystem>();

                particle.transform.position = enemy.Position + Vector3.up * 0.5f;

                particle.Play();

                Tween.DelayedCall(data.damageDelay, () =>
                {
                    if (enemy != null && !enemy.IsDying && enemy.gameObject.activeSelf)
                    {
                        enemy.GetHit(new Damage
                        {
                            amount = info.baseDamage.amount * lightningInfo.damage,
                            isCrit = false,
                            type = DamageType.Storm
                        });
                    }
                });

                Tween.DelayedCall(data.particleDisableDelay, () =>
                {
                    particle.Stop();

                    particle.gameObject.SetActive(false);
                });

                Tween.DelayedCall(0.1f, () => GameAudioController.PlaySound(data.lightningStrikeSound));
            }
        }
    }

    

}