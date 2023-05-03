using System;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
	public class Test : MonoBehaviour
	{
        [SerializeField] ItemDatabase itemDatabase;

        [ItemPicker]
        public int itemID;

        public Loot loot;

        private ItemsContainer itemsContainer;

        private void Start()
        {
            //if (!PlayerPrefs.HasKey("test"))
            //{
            //    for (int i = 0; i < itemDatabase.Items.Length; i++)
            //    {
            //        Inventory.AddItem(itemDatabase.Items[i].GetHolder());
            //    }

            //    PlayerPrefs.SetFloat("test", 0);
            //}

            //for (int i = 0; i < itemDatabase.Items.Length; i++)
            //{
            //    Inventory.AddItem(itemDatabase.Items[i].GetHolder());
            //}

            //MiscItemHolder scroll = (MiscItemHolder)ItemDatabase.GetItem(ItemSettings.GetChracterUpgradeItem()).GetHolder();
            //scroll.SetAmount(200);

            //Inventory.AddItem(scroll);

            //Currency.SetCoins(99999);
            //Currency.SetGems(99999);
        }

        [Button("Take Loot")]
        public void TakeLoot()
        {
            List<ItemHolder> itemHolders = new List<ItemHolder>();

            var lootResult = loot.result;

            foreach(var item in lootResult)
            {
                LootObject lootObject = (LootObject)item;
                Debug.Log("Loot result: " + lootObject.ItemID);

                ItemHolder itemHolder = ItemDatabase.GetItem(lootObject.ItemID).GetHolder();
                itemHolders.Add(itemHolder);

                Inventory.AddItem(itemHolder);
            }

            RewardWindow.DisplayItems(itemHolders.ToArray());
        }

        [Button("Open Chest")]
        public void OpenChest()
        {
            ItemRarity allowedItemRarities = ItemRarity.Common | ItemRarity.Uncommon;
            Item[] allowedItems = ItemDatabase.GetEquipableItemsByRarity(allowedItemRarities);

            ItemHolder itemHolder = allowedItems.GetRandomItem().GetDefaultHolder();
            ItemRarity itemRarity = ItemSettings.GetRandomRarity(allowedItemRarities);
            itemHolder.SetItemRarity(itemRarity);

            Inventory.AddItem(itemHolder);

            RewardWindow.DisplayItems(itemHolder);
        }

        [Button("Open Rare Chest")]
        public void OpenRareChest()
        {
            ItemRarity allowedItemRarities = ItemRarity.Rare | ItemRarity.Epic | ItemRarity.Legendary;
            Item[] allowedItems = ItemDatabase.GetEquipableItemsByRarity(allowedItemRarities);

            ItemHolder itemHolder = allowedItems.GetRandomItem().GetDefaultHolder();
            ItemRarity itemRarity = ItemSettings.GetRandomRarity(allowedItemRarities);
            itemHolder.SetItemRarity(itemRarity);

            Inventory.AddItem(itemHolder);

            RewardWindow.DisplayItems(itemHolder);
        }

        [Button("Get Key Amount")]
        public void GetKeysAmount()
        {
            Debug.Log(ItemSettings.GetInventoryKeyAmount());
        }

        [Button("Get Royal Key Amount")]
        public void GetRoyalKeysAmount()
        {
            Debug.Log(ItemSettings.GetInventoryRoyalKeyAmount());
        }

        [Button("Add Item")]
        public void AddItem()
        {
            Inventory.AddItem(itemDatabase.GetItemById(itemID).GetHolder());
        }

        [Button("Test Rarity")]
        public void TestRarity()
        {
            ItemRarity itemRarity = ItemSettings.GetRandomRarity();
            Debug.Log(itemRarity + ":" + ItemSettings.GetRaritySettings(itemRarity).ItemRarity);
        }

        [Button("Split Rarity")]
        public void SplitRarity()
        {
            Array rarityList = Enum.GetValues(typeof(ItemRarity));
            ItemRarity itemRarity = ItemRarity.Common | ItemRarity.Epic;

            List<ItemRarity> randomRarities = new List<ItemRarity>();
            int raritiesCount = 0;
            foreach(var i in rarityList)
            {
                ItemRarity tempItemRarity = (ItemRarity)i;
                if (itemRarity.HasFlag(tempItemRarity))
                {
                    randomRarities.Add(tempItemRarity);
                    Debug.Log(i);

                    raritiesCount++;
                }
            }

            for(int i = 0; i < raritiesCount; i++)
            {

            }
        }

        [Button("Add EXP")]
        public void AddExp()
        {
            Account.AddExperience(100);
        }
    }
}
