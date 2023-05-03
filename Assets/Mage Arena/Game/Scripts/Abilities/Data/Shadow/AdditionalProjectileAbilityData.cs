using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Additional Projectile Ability", menuName = "Content/Abilities/Additional Projectile Ability")]
    public class AdditionalProjectileAbilityData : AbilityData
    {
        public float projectilesSpacing = 0.5f;

        public float[] damageReductions = new float[] { 0.33f, 0.66f, 1f };

        public override Ability CreateAbilityInstance()
        {
            return new AdditionalProjectileAbility { data = this };
        }
    }

}