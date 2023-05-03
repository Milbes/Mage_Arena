using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{

    public class RicochetAbility : Ability, IProjectileHitBorderAbility
    {
        public RicochetAbilityData data;

        public override void ActivateAbility(IGameplayEntity owner)
        {
            base.ActivateAbility(owner);
        }

        public bool OnProjectileHitBorder(ProjectileBehavior projectile, Collider collider)
        {
            if (projectile.recochetAmount > stackAmount) return false;

            Vector3 rayOrigin = projectile.Position - projectile.projectileInfo.direction;

            if(collider.Raycast(new Ray(rayOrigin, projectile.projectileInfo.direction), out RaycastHit hit, 2)){
                Vector3 newDirection = Vector3.Reflect(projectile.projectileInfo.direction, hit.normal);

                ProjectilesController.ChangeVelocity(projectile, newDirection);

                projectile.recochetAmount++;

                return true;
            }

            return false;
        }
    }

}