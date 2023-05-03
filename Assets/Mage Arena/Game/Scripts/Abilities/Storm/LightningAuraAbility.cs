using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    
    public class LightningAuraAbility : Ability, IOnTickAbility
    {
        private static readonly string LIGHTNING_AURA_STRIKE_PARTICLE_POOL_NAME = "Lightning Aura Ability Strike Particle";

        [System.NonSerialized] public bool isActive;

        public LightningAuraAbilityData data;

        private Transform auraParticleInstance;

        private Dictionary<EnemyBehavior, float> cooldowns = new Dictionary<EnemyBehavior, float>();

        private static Pool lightningStrikePool;

        public override void ActivateAbility(IGameplayEntity owner)
        {
            base.ActivateAbility(owner);
       
            isActive = true;

            auraParticleInstance = Object.Instantiate(data.auraParticle).transform;

            auraParticleInstance.position = PlayerController.Position + Vector3.up * 0.05f;
            auraParticleInstance.localScale = Vector3.one * data.lightningAuraInfo[AbilitiesController.GetStackAmount(this) - 1].auraParticleScale;

            cooldowns.Clear();

            if (!PoolManager.ContainsPool(LIGHTNING_AURA_STRIKE_PARTICLE_POOL_NAME))
            {
                lightningStrikePool = PoolManager.AddPool(new PoolSettings { 
                    name = LIGHTNING_AURA_STRIKE_PARTICLE_POOL_NAME,
                    autoSizeIncrement = true,
                    size = 3,
                    type = Pool.PoolType.Single,
                    singlePoolPrefab = data.lightningParticle
                });
            }
        }

        public override void StackIncreased()
        {
            base.StackIncreased();

            auraParticleInstance.position = PlayerController.Position + Vector3.up * 0.05f;
            auraParticleInstance.localScale = Vector3.one * data.lightningAuraInfo[AbilitiesController.GetStackAmount(this) - 1].auraParticleScale;
        }

        public override void DisableAbility()
        {
            base.DisableAbility();

            isActive = false;

            Object.Destroy(auraParticleInstance.gameObject);

            cooldowns.Clear();
        }

        public void Tick()
        {

            auraParticleInstance.position = PlayerController.Position + Vector3.up * 0.05f;

            LightningAuraInfo info = data.lightningAuraInfo[AbilitiesController.GetStackAmount(this) - 1];

            for (int i = 0; i < LevelObjectSpawner.Enemies.Count; i++)
            {
                EnemyBehavior enemy = LevelObjectSpawner.Enemies[i];

                if (enemy == null || !enemy.gameObject.activeSelf || enemy.IsDying) continue;

                if (Vector3.Distance(PlayerController.Position, enemy.Position) > info.auraRadius) continue;

                if (cooldowns.ContainsKey(enemy))
                {
                    if(Time.time - cooldowns[enemy] < info.sameEnemyStrikeCooldown)
                    {
                        continue;
                    } else
                    {
                        cooldowns[enemy] = Time.time;
                    }
                } else
                {
                    cooldowns.Add(enemy, Time.time);
                }

                var particle = lightningStrikePool.GetPooledObject().GetComponent<ParticleSystem>();

                particle.transform.position = enemy.Position + Vector3.up * 0.5f;
                particle.transform.localScale = Vector3.one * data.lightningParticleScale;

                particle.Play();

                Tween.DelayedCall(data.damageDelay, () =>
                {
                    if (enemy != null && !enemy.IsDying && enemy.gameObject.activeSelf)
                    {
                        enemy.GetHit(new Damage { 
                            amount = owner.GetBaseDamageAmount() * info.strikeDamage,
                            isCrit = false,
                            type = DamageType.Storm
                        });
                    }
                });

                Tween.DelayedCall(data.lightningParticleDisableDelay, () =>
                {
                    particle.Stop();
                    particle.gameObject.SetActive(false);
                });

                GameAudioController.PlaySound(data.lightningStrikeSound);
            }
        }
    }

    [System.Serializable]
    public struct LightningAuraInfo
    {
        public float sameEnemyStrikeCooldown;
        public float auraParticleScale;
        public float auraRadius;
        public float strikeDamage;

    }
}

