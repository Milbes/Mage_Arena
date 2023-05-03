using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Miss Chance Ability", menuName = "Content/Abilities/Miss Chance Ability")]
    public class MissChanceAbilityData : AbilityData
    {
        public float[] missChances = new float[] { 0.02f, 0.04f, 0.08f };

        public override Ability CreateAbilityInstance()
        {
            return new MissChanceAbility { data = this };
        }
    }
}
