using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    
    public class SideProjectileAbility : Ability, IProjectileAmountAbility
    {
        public SideProjectileAbilityData data;

        private static Quaternion rightRotation = Quaternion.Euler(0, 90, 0);
        private static Quaternion leftRotation = Quaternion.Euler(0, -90, 0);

        public override void ActivateAbility(IGameplayEntity owner)
        {
            base.ActivateAbility(owner);
        }

        public List<ProjectileInfo> ApplyAbilityToProjectile(ProjectileInfo projectileInfo)
        {
            List<ProjectileInfo> processedProjectiles = new List<ProjectileInfo>();

            ProjectileInfo leftProjectile = ProjectileInfo.Clone(projectileInfo);
            ProjectileInfo rightProjectile = ProjectileInfo.Clone(projectileInfo);

            leftProjectile.direction = leftRotation * projectileInfo.direction;
            rightProjectile.direction = rightRotation * projectileInfo.direction;

            leftProjectile.ReduceDamage(data.damageReductions[stackAmount]);
            rightProjectile.ReduceDamage(data.damageReductions[stackAmount]);

            processedProjectiles.Add(leftProjectile);
            processedProjectiles.Add(rightProjectile);

            return processedProjectiles;

            //int stackLevel = AbilitiesController.GetStackAmount(this);
        }
    }

}