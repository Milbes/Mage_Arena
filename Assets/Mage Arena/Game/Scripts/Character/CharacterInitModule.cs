using UnityEngine;

namespace Watermelon
{
    public class CharacterInitModule : InitModule
    {
        [SerializeField] SkinData skinData;

        [Space]
        [SerializeField] bool showLogs = true;

        public override void CreateComponent(Initialiser initialiser)
        {
            Character.Init(skinData);

            if (showLogs)
                Debug.Log("[Character]: Character is initialised!");
        }

        public CharacterInitModule()
        {
            moduleName = "Character";
        }
    }
}