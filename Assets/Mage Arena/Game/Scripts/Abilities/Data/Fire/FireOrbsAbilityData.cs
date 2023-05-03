using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Fire Orbs Ability", menuName = "Content/Abilities/Fire/Fire Orbs Ability")]
    public class FireOrbsAbilityData : AbilityData
    {
        public GameObject orbProjectile;
        public GameObject orbsParticle;
        public float orbDamage = 0.5f;
        public float radius = 3f;
        public float speed = 180f;
        public override Ability CreateAbilityInstance()
        {
            return new FireOrbsAbility { data = this };
        }
    }

}