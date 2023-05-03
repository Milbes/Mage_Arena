using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{

    public class AdditionalProjectileAbility : Ability, IProjectileAmountAbility
    {
        public AdditionalProjectileAbilityData data;

        public override void ActivateAbility(IGameplayEntity owner)
        {
            base.ActivateAbility(owner);
        }

        public List<ProjectileInfo> ApplyAbilityToProjectile(ProjectileInfo projectileInfo)
        {
            List<ProjectileInfo> processedProjectiles = new List<ProjectileInfo>();

            Vector3 right = new Vector3(projectileInfo.direction.z, projectileInfo.direction.y, -projectileInfo.direction.x).normalized * data.projectilesSpacing / 2f;

            ProjectileInfo additionalProjectileInfo = ProjectileInfo.Clone(projectileInfo);

            additionalProjectileInfo.spawnPosition = projectileInfo.spawnPosition - right;
            projectileInfo.spawnPosition += right;

            additionalProjectileInfo.ReduceDamage(data.damageReductions[stackAmount]);

            processedProjectiles.Add(additionalProjectileInfo);

            return processedProjectiles;
        }
    }
}