using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Increase Max Health Ability", menuName = "Content/Abilities/Light/Increase Max Health Ability")]
    public class IncreaseMaxHealthAbilityData : AbilityData
    {

        public float[] healthMultipliers = new float[] { 1.1f, 1.1f, 1.1f, 1.1f, 1.1f };

        public GameObject healParticle;

        public override Ability CreateAbilityInstance()
        {
            return new IncreaseMaxHealthAbility { data = this };
        }
    }

}