using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Stun Status Effect", menuName = "Content/Status Effects/Stun")]
    public class StunStatusEffect : StatusEffect
    {
        private Dictionary<IGameplayEntity, TweenCase> disableStunTweens;

        private void Awake()
        {
            type = StatusEffectType.Stun;
        }

        public override void Activate(MonoBehaviour coroutineHolder)
        {
            disableStunTweens = new Dictionary<IGameplayEntity, TweenCase>();
        }

        public override void Dispose()
        {
            disableStunTweens.Clear();

            disableStunTweens = null;
        }

        public override void Register(StatusEffectInfo info)
        {
            Register(info as StunEffectInfo);
        }

        public void Register(StunEffectInfo info)
        {
            var entity = info.entity;

            if (disableStunTweens.ContainsKey(entity))
            {
                disableStunTweens[entity].Kill();

                disableStunTweens[entity] = Tween.DelayedCall(info.duration, () => DisableStun(entity));
            } else
            {
                entity.Stun();

                entity.onEntityDied += OnEnemyDied;

                disableStunTweens.Add(entity, Tween.DelayedCall(info.duration, () => DisableStun(entity)));
            }
        }

        private void DisableStun(IGameplayEntity enemy)
        {
            enemy.onEntityDied -= OnEnemyDied;

            disableStunTweens.Remove(enemy);

            enemy.Unstun();
        }

        public void OnEnemyDied(IGameplayEntity enemy)
        {
            disableStunTweens[enemy].Kill();

            DisableStun(enemy);
        }

    }

    public class StunEffectInfo: StatusEffectInfo
    {
        public StunEffectInfo()
        {
            type = StatusEffectType.Stun;
        }

        public float duration;
    }

}