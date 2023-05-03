using UnityEngine;

namespace Watermelon
{
    public class CurrencyInitModule : InitModule
    {
        [SerializeField] Sprite gemsSprite;
        [SerializeField] Sprite gemsDisableSprite;

        [Space]
        [SerializeField] Sprite coinsSprite;
        [SerializeField] Sprite coinsDisableSprite;

        public CurrencyInitModule()
        {
            moduleName = "Currency";
        }

        public override void CreateComponent(Initialiser Initialiser)
        {
            Currency.Init(gemsSprite, gemsDisableSprite, coinsSprite, coinsDisableSprite);
        }
    }
}