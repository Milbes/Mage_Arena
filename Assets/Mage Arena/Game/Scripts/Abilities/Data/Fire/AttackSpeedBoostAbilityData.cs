using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Attack Speed Boost Ability", menuName = "Content/Abilities/Fire/Attack Speed Boost Ability")]
    public class AttackSpeedBoostAbilityData : AbilityData
    {

        public float[] attackSpeeedBoost = new float[] { 1.1f, 1.2f, 1.3f };

        public override Ability CreateAbilityInstance()
        {
            return new AttackSpeedBoostAbility { data = this };
        }
    }
}