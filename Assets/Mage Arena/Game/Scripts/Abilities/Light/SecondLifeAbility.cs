using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{

    public class SecondLifeAbility : Ability
    {
        public SecondLifeAbilityData data;

        public override void ActivateAbility(IGameplayEntity owner)
        {
            base.ActivateAbility(owner);

            owner.SetSecondLife(true, data.healthRestoreAmount[stackAmount]);
        }

        public override void StackIncreased()
        {
            base.StackIncreased();

            owner.SetSecondLife(true, data.healthRestoreAmount[stackAmount]);
        }

        public void SetSecondLifeInfo()
        {
            owner.SetSecondLife(true, data.healthRestoreAmount[stackAmount]);
        }
    }

}