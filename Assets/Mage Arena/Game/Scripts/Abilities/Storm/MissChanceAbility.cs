using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    public class MissChanceAbility : Ability, IOnOwnerGetHitAbility
    {

        public MissChanceAbilityData data;

        public override void ActivateAbility(IGameplayEntity owner)
        {
            base.ActivateAbility(owner);

            PlayerController.playerController.onEntityHitEvent += OnOwnerGetHit;
        }

        public override void DisableAbility()
        {
            base.DisableAbility();
        }

        public void OnOwnerGetHit(OnOwnerGetHitInfo info)
        {
            int stackAmount = AbilitiesController.GetStackAmount(this);
            info.missed = data.missChances[stackAmount - 1] >= Random.value;
        }
    }
}

