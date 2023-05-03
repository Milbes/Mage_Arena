using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Ricochet Projectile Ability", menuName = "Content/Abilities/Ricochet Projectile Ability")]
    public class RicochetAbilityData : AbilityData
    {
        public override Ability CreateAbilityInstance()
        {
            return new RicochetAbility { data = this };
        }
    }

}