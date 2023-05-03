using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{

    public class HealingBoostAbility : Ability
    {
        public HealingBoostAbilityData data;

        public override void ActivateAbility(IGameplayEntity owner)
        {
            base.ActivateAbility(owner);

            IncreaseOwnerHealing();
        }

        public override void StackIncreased()
        {
            base.StackIncreased();

            IncreaseOwnerHealing();
        }

        public void IncreaseOwnerHealing()
        {
            owner.IncreaseHealing(data.healingBoost[stackAmount]);
        }
    }

}