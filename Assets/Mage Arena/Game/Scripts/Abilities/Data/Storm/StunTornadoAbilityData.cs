using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Stun Tornado Ability", menuName = "Content/Abilities/Storm/Stun Tornado Ability")]
    public class StunTornadoAbilityData : AbilityData
    {

        public StunTornadoInfo[] stunTornadoInfo = new StunTornadoInfo[] {
            new StunTornadoInfo{ duration = 2f, chance = 0.1f },
            new StunTornadoInfo{ duration = 3f, chance = 0.15f },
            new StunTornadoInfo{ duration = 4f, chance = 0.2f },
        };

        public GameObject stunParticle;

        public override Ability CreateAbilityInstance()
        {
            return new StunTornadoAbility { data = this };
        }
    }
}
