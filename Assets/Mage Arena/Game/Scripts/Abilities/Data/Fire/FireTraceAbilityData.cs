using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Fire Trace Ability", menuName = "Content/Abilities/Fire/Fire Trace Ability")]
    public class FireTraceAbilityData : AbilityData
    {
        public GameObject particleObject;
        public GameObject projectileObject;
        [Space]
        public float spawnTime = 0.2f;
        public float lifetime = 2f;
        [Space]
        public float[] damagePerTick = new float[] { 0.2f, 0.4f, 0.6f };
        public int[] ticksAmount = new int[] { 4, 5, 6 };
        

        public override Ability CreateAbilityInstance()
        {
            return new FireTraceAbility { data = this };
        }
    }

}