using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Blinding Light Ability", menuName = "Content/Abilities/Light/Blinding Light Ability")]
    public class BlindingLightAbilityData : AbilityData
    {

        public BlindingLightInfo[] info = new BlindingLightInfo[] {
            new BlindingLightInfo{ chance = 0.2f, duration = 2f },
            new BlindingLightInfo{ chance = 0.3f, duration = 3f },
            new BlindingLightInfo{ chance = 0.4f, duration = 4f }
        };

        public GameObject particleObject;

        public override Ability CreateAbilityInstance()
        {
            return new BlindingLightAbility{ data = this };
        }
    }

    [System.Serializable]
    public struct BlindingLightInfo
    {
        public float chance;
        public float duration;
    }

}