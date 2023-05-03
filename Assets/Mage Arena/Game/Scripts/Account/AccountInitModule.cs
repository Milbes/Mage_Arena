using UnityEngine;

namespace Watermelon
{
    public class AccountInitModule : InitModule
    {
        public override void CreateComponent(Initialiser initialiser)
        {
            Account.Init();
        }

        public AccountInitModule()
        {
            moduleName = "Account";
        }
    }
}