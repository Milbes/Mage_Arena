using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    // Everything, that attacks and can be attacked in the game
    public interface IGameplayEntity
    {
        /// <summary>
        /// Applies a specific <see cref="Damage"/> to the entity.
        /// </summary>
        void GetHit(Damage damage);

        /// <summary>
        /// Processes hit of the entity by a projectile.
        /// </summary>
        void GetHit(ProjectileInfo projectileInfo);

        void Stun();
        void Unstun();
        void SlowDown(Transform particleTransform, float magnitude);
        void ResetSpeed();
        float GetBaseDamageAmount();
        void IncreaseMaxHealth(float multiplier);
        void Heal(float percent);
        bool IsActiveSelf();
        void IncreaseAttackSpeed(float magnitude);
        void IncreaseHealing(float magnitude);
        bool HasUsedSecondLife();
        void SetSecondLife(bool isAvailable, float healthMagnitude);
        void SetAbilityTyAttack(bool canAttack);

        Transform GetTransform();

        /// <summary>
        /// Fires upon the entity's death
        /// </summary>
        event OnEntityDiedDelegate onEntityDied;

        /// <summary>
        /// Fires upon the entity getting hit
        /// </summary>
        event OnEntityHitDelegate onEntityHitEvent;

        /// <summary>
        /// Fires upon the entity attacking
        /// </summary>
        event OnEntityAttackDelegate onEntityAttackEvent;

        
    }

    public delegate void OnEntityDiedDelegate(IGameplayEntity entity);
    public delegate void OnEntityHitDelegate(OnOwnerGetHitInfo info);
    public delegate void OnEntityAttackDelegate(OnOwnerAttackInfo info);

    public class OnOwnerGetHitInfo
    {
        public ProjectileInfo projectileInfo;
        public Vector3 projectileDirection;
        public Vector3 hitPosition;

        public bool missed;
    }

    public class OnOwnerAttackInfo
    {
        public Damage baseDamage;
        public List<Damage> additionalDamage;
    }
}