using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Second Life Ability", menuName = "Content/Abilities/Light/Second Life Ability")]
    public class SecondLifeAbilityData : AbilityData
    {

        public float[] healthRestoreAmount = new float[] { 0.2f, 0.4f, 0.6f };

        public override Ability CreateAbilityInstance()
        {
            return new SecondLifeAbility { data = this };
        }

        public override bool CanBeShown(IGameplayEntity entity)
        {
            return entity.HasUsedSecondLife();
        }
    }

}