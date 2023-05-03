using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon {

    [CreateAssetMenu(fileName = "Diagonal Projectile Ability", menuName = "Content/Abilities/Diagonal Projectile Ability")]
    public class DiagonalProjectileAbilityData : AbilityData
    {
        public float[] damageReductions = new float[] { 0.33f, 0.66f, 1f };

        public override Ability CreateAbilityInstance()
        {
            return new DiagonalProjectileAbility { data = this };

        }
    }

}
