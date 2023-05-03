using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{

    public abstract class StatusEffect : ScriptableObject
    {
        public new string name;
        public string description;

        [ReadOnly] public StatusEffectType type;

        protected MonoBehaviour coroutineHolder;

        public abstract void Activate(MonoBehaviour coroutineHolder);
        public abstract void Dispose();

        public abstract void Register(StatusEffectInfo info);
    }

    public abstract class StatusEffectInfo
    {
        protected StatusEffectType type;
        public IGameplayEntity entity;

        public StatusEffectType Type => type;
    }

    public enum StatusEffectType
    {
        Burning, Chill, Poison, Stun, Slow, Shock
    }
}