using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Side Projectile Ability", menuName = "Content/Abilities/Side Projectile Ability")]
    public class SideProjectileAbilityData : AbilityData
    {

        public float[] damageReductions = new float[] { 0.33f, 0.66f, 1f };

        public override Ability CreateAbilityInstance()
        {
            return new SideProjectileAbility { data = this };
        }
    }

}