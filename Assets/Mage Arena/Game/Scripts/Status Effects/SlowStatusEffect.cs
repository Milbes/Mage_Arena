using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Slow Status Effect", menuName = "Content/Status Effects/Slow")]
    public class SlowStatusEffect : StatusEffect
    {
        private Dictionary<IGameplayEntity, TweenCase> disableSlowTweens;

        private void Awake()
        {
            type = StatusEffectType.Slow;
        }

        public override void Activate(MonoBehaviour coroutineHolder)
        {
            disableSlowTweens = new Dictionary<IGameplayEntity, TweenCase>();
        }

        public override void Dispose()
        {
            disableSlowTweens.Clear();

            disableSlowTweens = null;
        }

        public override void Register(StatusEffectInfo info)
        {
            Register(info as SlowStatusEffectInfo);
        }

        public void Register(SlowStatusEffectInfo info)
        {
            var entity = info.entity;

            if (disableSlowTweens.ContainsKey(entity))
            {
                disableSlowTweens[entity].Kill();

                disableSlowTweens[entity] = Tween.DelayedCall(info.duration, () => DisableSlow(entity));
            }
            else
            {
                entity.SlowDown(info.particle, info.slowMagnitude);

                entity.onEntityDied += OnEnemyDied;

                disableSlowTweens.Add(entity, Tween.DelayedCall(info.duration, () => DisableSlow(entity)));
            }
        }

        private void DisableSlow(IGameplayEntity entity)
        {
            entity.onEntityDied -= OnEnemyDied;

            disableSlowTweens.Remove(entity);

            entity.ResetSpeed();
        }

        public void OnEnemyDied(IGameplayEntity entity)
        {
            disableSlowTweens[entity].Kill();

            DisableSlow(entity);
        }
    }

    public class SlowStatusEffectInfo: StatusEffectInfo
    {
        public SlowStatusEffectInfo()
        {
            type = StatusEffectType.Slow;
        }

        public Transform particle;
        public float slowMagnitude;
        public float duration;
    }
}