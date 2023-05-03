using UnityEngine;

namespace Watermelon
{
    public class DailyDealProduct : ScriptableObject
    {
        [ReadOnly]
        [SerializeField] int id;
        [SerializeField] string productName;

        [Space]
        [SerializeField] int cost;
        [SerializeField] Currencies productCurrency;

        [Space]
        [SerializeField, ItemPicker] int itemID;

        public int ID => id;
        public string ProductName => productName;

        public int Cost => cost;
        public Currencies ProductCurrency => productCurrency;

        public Item Item => ItemDatabase.GetItem(itemID);

        public virtual void Init() { }

        public virtual void Unlock()
        {
            // Coins
            if (productCurrency == Currencies.Gold)
            {
                Currency.ChangeCoins(-cost);
            }
            // Gems
            else
            {
                Currency.ChangeGems(-cost);
            }

            Inventory.AddItem(ItemDatabase.GetItem(itemID).GetHolder());
        }

        public virtual bool Check()
        {
            // Coins
            if (productCurrency == Currencies.Gold)
                return Currency.Coins >= cost;

            // Gems
            return Currency.Gems >= cost;
        }
    }
}
