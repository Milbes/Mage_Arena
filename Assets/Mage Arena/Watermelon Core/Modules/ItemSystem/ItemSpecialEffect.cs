using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    public abstract class ItemSpecialEffect : ScriptableObject
    {
        [SerializeField] string description;
        public string Description => description;

        public abstract void OnLevelStarted();
        public abstract void OnRoomStarted();

        public abstract void OnEnemyHitted(EnemyBehavior enemy, float damage, bool isCritical);
        public abstract void OnEnemyDies(EnemyBehavior enemy);

        public abstract float OnPlayerHitted(float damage);

        public abstract void OnEffectDisabled();
    }
}
