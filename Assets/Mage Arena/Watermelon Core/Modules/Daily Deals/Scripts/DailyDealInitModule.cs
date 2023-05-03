using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    public class DailyDealInitModule : InitModule
    {
        [SerializeField] DailyDealDatabase dailyDealDatabase;

        public override void CreateComponent(Initialiser Initialiser)
        {
            dailyDealDatabase.Init();
        }

        public DailyDealInitModule()
        {
            moduleName = "Daily Deal";
        }
    }
}
