using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Healing Boost Ability", menuName = "Content/Abilities/Light/Healing Boost Ability")]
    public class HealingBoostAbilityData : AbilityData
    {
        public float[] healingBoost = new float[] { 1.25f, 1.5f, 1.75f };

        public override Ability CreateAbilityInstance()
        {
            return new HealingBoostAbility { data = this };
        }
    }

}