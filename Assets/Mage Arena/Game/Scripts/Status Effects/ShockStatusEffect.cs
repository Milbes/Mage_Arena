using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Shock Status Effect", menuName = "Content/Status Effects/Shock")]
    public class ShockStatusEffect : StatusEffect
    {
        public DamageType damageType;

        public GameObject lightningTrail;

        private List<ShockEffectInfo> currentEffects;

        private bool isClockRunning = false;

        private void Awake()
        {
            type = StatusEffectType.Shock;
        }

        public override void Activate(MonoBehaviour coroutineHolder)
        {
            isClockRunning = false;

            currentEffects = new List<ShockEffectInfo>();

            this.coroutineHolder = coroutineHolder;
        }

        public override void Dispose()
        {
            isClockRunning = false;

            currentEffects.Clear();
        }

        public override void Register(StatusEffectInfo info)
        {
            Register(info as ShockEffectInfo);
        }

        public void Register(ShockEffectInfo info)
        {
            IGameplayEntity entity = info.entity;

            bool exists = false;

            for (int i = 0; i < currentEffects.Count; i++)
            {
                ShockEffectInfo effect = currentEffects[i];
                if (effect.entity == entity)
                {

                    if (effect.damagePerHit < info.damagePerHit) effect.damagePerHit = info.damagePerHit;
                    if (effect.durationLeft < info.durationLeft) effect.durationLeft = info.durationLeft;

                    exists = true;
                }
            }

            if (!exists)
            {
                currentEffects.Add(info);
            }

            if (!isClockRunning)
            {
                coroutineHolder.StartCoroutine(Tick());
            }
        }

        public IEnumerator Tick()
        {
            isClockRunning = true;

            while (!currentEffects.IsNullOrEmpty())
            {
                yield return null;

                for (int i = 0; i < currentEffects.Count; i++)
                {
                    ShockEffectInfo info = currentEffects[i];

                    if(!info.entity.IsActiveSelf())
                    {
                        info.cooldowns.Clear();

                        currentEffects.RemoveAt(i);
                        i--;

                        continue;
                    }

                    for(int j = 0; j < LevelObjectSpawner.Enemies.Count; j++)
                    {
                        EnemyBehavior enemy = LevelObjectSpawner.Enemies[j];
                        if (enemy != null && !enemy.IsDying && enemy.gameObject.activeSelf)
                        {
                            info.TryShockEntity(LevelObjectSpawner.Enemies[j], lightningTrail);
                        }
                    }

                    info.durationLeft -= Time.deltaTime;

                    if (info.durationLeft > 0)
                    {
                        currentEffects[i] = info;
                    }
                    else
                    {
                        info.cooldowns.Clear();

                        currentEffects.RemoveAt(i);
                        i--;
                    }
                }
            }

            isClockRunning = false;
        }
    }

    public class ShockEffectInfo : StatusEffectInfo
    {
        public ShockEffectInfo()
        {
            type = StatusEffectType.Shock;

            cooldowns = new Dictionary<IGameplayEntity, float>();
        }

        public float damagePerHit;
        public float durationLeft;
        public float enemyHitCooldown;
        public float effectDistance;

        public Dictionary<IGameplayEntity, float> cooldowns;

        public void TryShockEntity(IGameplayEntity entityToShock, GameObject lightningParticle)
        {
            if (entityToShock != entity)
            {
                Vector3 entityPosition = entity.GetTransform().position;
                Vector3 entityToShockPosition = entityToShock.GetTransform().position;

                if (Vector3.Distance(entityPosition, entityToShockPosition) <= effectDistance)
                {
                    if (cooldowns.ContainsKey(entityToShock))
                    {
                        if (Time.time - cooldowns[entityToShock] >= enemyHitCooldown)
                        {
                            cooldowns[entityToShock] = Time.time;

                            ShockEnemy(entityToShock, lightningParticle, entityPosition, entityToShockPosition);
                        }
                    }
                    else
                    {
                        cooldowns.Add(entityToShock, Time.time);

                        ShockEnemy(entityToShock, lightningParticle, entityPosition, entityToShockPosition);
                    }
                }
            }
        }

        public void ShockEnemy(IGameplayEntity entityToShock, GameObject lightningParticle, Vector3 entityPosition, Vector3 entityToShockPosition)
        {
            entityToShock.GetHit(new Damage
            {
                amount = damagePerHit,
                isCrit = false,
                type = DamageType.Storm
            });

            GameObject particle = Object.Instantiate(lightningParticle);

            particle.transform.position = entityPosition + Vector3.up * 0.5f;
            particle.transform.localScale = Vector3.one * 0.5f;

            Tween.NextFrame(() => {
                particle.transform.position = entityToShockPosition;
            });

            Tween.DelayedCall(0.2f, () =>
            {
                Object.Destroy(particle);
            });
        }
    }

}