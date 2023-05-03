using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    public abstract class AbilityData : ScriptableObject
    {
        [TextArea] public new string name;
        public string description;
        public Sprite icon;

        public OrbType orbType;

        public int maxStackAmount;

        [Header("Dev")]
        public bool activateOnPlay;
        [Range(1, 3)] public int devTier;

        public abstract Ability CreateAbilityInstance();

        public virtual bool CanBeShown(IGameplayEntity entity) { return true; }
    }

    public enum AbilityType
    {
        ProjectileAmountAbility, ProjectileEffectAbility, PlayerHitAbility, PlayerAttackAbility, EnemyAbility, TickAbility
    }

    public enum OrbType
    {
        Storm, Shadow, Fire, Green
    }
}

