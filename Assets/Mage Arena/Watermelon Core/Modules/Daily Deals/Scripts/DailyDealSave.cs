using UnityEngine;

namespace Watermelon
{
    public static class DailyDealSave
    {
        private const string FILE_NAME = "DailyDeals";

        public static SaveData Load()
        {
            if (Serializer.FileExistsAtPDP(FILE_NAME))
            {
                return Serializer.DeserializeFromPDP<SaveData>(FILE_NAME);
            }

            return new SaveData();
        }

        public static void Save(SaveData saveData)
        {
            Serializer.SerializeToPDP(saveData, FILE_NAME);
        }

        [System.Serializable]
        public class SaveData
        {
            [SerializeField] Deal[] deals;
            public Deal[] Deals => deals;

            [SerializeField] double refreshTime;
            public double RefreshTime => refreshTime;

            public SaveData()
            {
                deals = null;
                refreshTime = 0;
            }

            public SaveData(Deal[] deals, double refreshTime)
            {
                this.deals = deals;
                this.refreshTime = refreshTime;
            }
        }

        [System.Serializable]
        public class Deal
        {
            [SerializeField] int itemID;
            public int ItemID => itemID;

            [SerializeField] int itemLevel;
            public int ItemLevel => itemLevel;

            [SerializeField] ItemRarity itemRarity = ItemRarity.Common;
            public ItemRarity ItemRarity => itemRarity;

            public Deal(int itemID)
            {
                this.itemID = itemID;

                this.itemLevel = 1;
                this.itemRarity = ItemRarity.Common;
            }

            public Deal(ItemHolder itemHolder)
            {
                itemLevel = itemHolder.ItemLevel;
                itemRarity = itemHolder.ItemRarity;

                Item item = itemHolder.Item;
                itemID = item.ID;
            }

            public void SetItemLevel(int itemLevel)
            {
                this.itemLevel = itemLevel;
            }

            public void SetItemRarity(ItemRarity itemRarity)
            {
                this.itemRarity = itemRarity;
            }
        }
    }
}
