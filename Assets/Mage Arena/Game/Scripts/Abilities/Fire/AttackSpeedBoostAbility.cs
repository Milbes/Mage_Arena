using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    public class AttackSpeedBoostAbility : Ability
    {
        public AttackSpeedBoostAbilityData data;

        public override void ActivateAbility(IGameplayEntity owner)
        {
            base.ActivateAbility(owner);

            IncreaseOwnerAttackSpeed();
        }

        public override void StackIncreased()
        {
            base.StackIncreased();

            IncreaseOwnerAttackSpeed();
        }

        public void IncreaseOwnerAttackSpeed()
        {
            owner.IncreaseAttackSpeed(data.attackSpeeedBoost[stackAmount]);
        }
    }
}