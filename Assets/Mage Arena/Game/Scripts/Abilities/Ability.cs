using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    public abstract class Ability
    {
        protected IGameplayEntity owner;
        public IGameplayEntity Owner => owner;

        public int stackAmount;

        public virtual void ActivateAbility(IGameplayEntity owner) {
            this.owner = owner;
            stackAmount = 0;
        }
        public virtual void DisableAbility() { 
        
        }

        public virtual void StackIncreased() {
            stackAmount++;
        }

        public virtual IGameplayEntity GetOwner()
        {
            return owner;
        }

        public virtual void OnRoomFinished(){}

        public virtual void OnRoomEntered() { }
    }

    public interface IOwnable
    {
        IGameplayEntity GetOwner();
    }

    public interface IOnOwnerAttackAbility : IOwnable
    {
        void OnOwnerAttack(OnOwnerAttackInfo info);
    }

    public interface IOnOwnerGetHitAbility : IOwnable
    {
        void OnOwnerGetHit(OnOwnerGetHitInfo info);
    }

    public interface IOnTickAbility : IOwnable
    {
        void Tick();
    }

    public interface IProjectileAmountAbility : IOwnable
    {
        List<ProjectileInfo> ApplyAbilityToProjectile(ProjectileInfo projectileInfo);
    }

    public interface IProjectileEffectAbility : IOwnable
    {
        void AddEffectToProjectile(ProjectileInfo projectileInfo);
    }

    public interface IProjectileHitBorderAbility: IOwnable
    {
        bool OnProjectileHitBorder(ProjectileBehavior projectile, Collider collider);
    }

    public interface IOnTargetHitByProjectileAbility : IOwnable
    {
        void OnTargetHitByProjectile(ProjectileInfo projectileInfo, IGameplayEntity target);
    }

    public interface IOnEnemyDiedAbility : IOwnable
    {
        void OnEnemyDied(IGameplayEntity enemy);
    }


}
