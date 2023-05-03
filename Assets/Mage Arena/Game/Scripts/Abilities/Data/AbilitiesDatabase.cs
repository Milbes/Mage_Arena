using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Ability Database", menuName = "Content/Abilities/Abilities Database")]
    public class AbilitiesDatabase : ScriptableObject
    {
        public List<AbilityData> abilities;

        public List<OrbData> orbs;
    }
}

