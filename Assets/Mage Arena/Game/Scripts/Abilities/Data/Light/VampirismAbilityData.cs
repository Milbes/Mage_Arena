using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Vampirism Ability", menuName = "Content/Abilities/Light/Vampirism Ability")]
    public class VampirismAbilityData : AbilityData
    {
        public GameObject healParticle;
        public float[] vampirismMultiplier = new float[] { 0.005f, 0.01f, 0.02f };

        public override Ability CreateAbilityInstance()
        {
            return new VampirismAbility { data = this };
        }
    }

}