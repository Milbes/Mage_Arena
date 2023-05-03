using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    public static class Currency
    {
        private static readonly string COINS_ID = "coins";
        private static readonly string GEMS_ID = "gems";

        private static int coins = 0;
        public static int Coins => coins;

        private static int gems = 0;
        public static int Gems => gems;

        public static OnCurrencyUpdatedCallback OnCurrencyUpdated;

        private static Sprite gemsSprite;
        private static Sprite gemsDisabledSprite;

        private static Sprite coinsSprite;
        private static Sprite coinsDisabledSprite;

        public static void Init(Sprite gemsSprite, Sprite gemsDisabledSprite, Sprite coinsSprite, Sprite coinsDisabledSprite)
        {
            coins = GameSettingsPrefs.Get<int>(COINS_ID);
            gems = GameSettingsPrefs.Get<int>(GEMS_ID);

            Currency.gemsSprite = gemsSprite;
            Currency.gemsDisabledSprite = gemsDisabledSprite;

            Currency.coinsSprite = coinsSprite;
            Currency.coinsDisabledSprite = coinsDisabledSprite;
        }

        public static void SetGems(int value, bool redrawUI = true)
        {
            int oldValue = gems;

            gems = value;

            GameSettingsPrefs.Set(GEMS_ID, value);

            if (redrawUI)
            {
                if (OnCurrencyUpdated != null)
                    OnCurrencyUpdated.Invoke(Currencies.Gem, value, oldValue, value - oldValue);
            }
        }

        public static void ChangeGems(int changeValue, bool redrawUI = true)
        {
            SetGems(gems + changeValue, redrawUI);
        }

        public static void SetCoins(int value, bool redrawUI = true)
        {
            int oldValue = coins;

            coins = value;

            GameSettingsPrefs.Set(COINS_ID, value);

            if (redrawUI)
            {
                if (OnCurrencyUpdated != null)
                    OnCurrencyUpdated.Invoke(Currencies.Gold, value, oldValue, value - oldValue);
            }
        }

        public static void ChangeCoins(int changeValue, bool redrawUI = true)
        {
            SetCoins(coins + changeValue, redrawUI);
        }

        public static void RedrawCurrency(Currencies currency, int value, int oldValue)
        {
            if (OnCurrencyUpdated != null)
            {
                OnCurrencyUpdated.Invoke(currency, value, oldValue, value - oldValue);
            }
        }

        public static bool EnoughMoney(Currencies currency, int value)
        {
            if(currency == Currencies.Gold)
            {
                return coins >= value;
            }

            return gems >= value;
        }

        public static Sprite GetCurrencyIcon(Currencies currency, bool isActive = true)
        {
            if (currency == Currencies.Gold)
                return isActive ? coinsSprite : coinsDisabledSprite;

            return isActive ? gemsSprite : gemsDisabledSprite;
        }

        public delegate void OnCurrencyUpdatedCallback(Currencies currency, int value, int oldValue, int valueDifference = 0);
    }

    [System.Serializable]
    public enum Currencies
    {
        Gold = 0, 
        Gem = 1
    }
}
