using UnityEngine;
using System.Collections.Generic;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Item Settings", menuName = "Iventory/Item Settings")]
    public class ItemSettings : ScriptableObject
    {
        public const string COMPARE_RED_COLOR = "#ff482e";
        public const string COMPARE_GREEN_COLOR = "#4daa72";

        private static ItemSettings itemSettings;

        [Header("Rarity")]
        [SerializeField] RaritySettings[] raritySettings;
        private Dictionary<ItemRarity, RaritySettings> raritySettingsLink;
        private float rarityRandomWeightSumm;
        private float dailyDealRarityRandomWeightSumm;
        private System.Array rarityList;

        [Header("Default Weapon")]
        [SerializeField, DrawReference] WeaponItem defaultWeapon;

        [Header("Character")]
        [SerializeField, ItemPicker] int chracterUpdateItemID;

        [Header("Chests")]
        [SerializeField, ItemPicker] int keyItemID;
        [SerializeField, ItemPicker] int royalKeyItemID;

        [Header("Daily Deal")]
        [SerializeField] ItemRarity dailyDealAllowedRarities;

        public void Init()
        {
            itemSettings = this;

            rarityList = System.Enum.GetValues(typeof(ItemRarity));
            raritySettingsLink = new Dictionary<ItemRarity, RaritySettings>();
            for (int i = 0; i < raritySettings.Length; i++)
            {
                rarityRandomWeightSumm += raritySettings[i].ItemRarityRandomWeight;
                dailyDealRarityRandomWeightSumm += raritySettings[i].DailyDealRarityRandomWeight;

                raritySettingsLink.Add(raritySettings[i].ItemRarity, raritySettings[i]);
            }
        }

        public static RaritySettings GetRaritySettings(ItemRarity itemRarity)
        {
            if (itemSettings.raritySettingsLink.ContainsKey(itemRarity))
                return itemSettings.raritySettingsLink[itemRarity];

            return itemSettings.raritySettings[0]; // Return common settings
        }

        public static ItemRarity GetRandomRarity()
        {
            // This is the magic random number that will decide, which object is hit now
            double hitValue = WeightRandom.GetValue(itemSettings.rarityRandomWeightSumm);

            double runningValue = 0;
            // Find out in a loop which object's probability hits the random value...
            for (int i = 0; i < itemSettings.raritySettings.Length; i++)
            {
                runningValue += itemSettings.raritySettings[i].ItemRarityRandomWeight;
                if (hitValue < runningValue)
                {
                    return itemSettings.raritySettings[i].ItemRarity;
                }
            }

            return ItemRarity.Common;
        }

        public static ItemRarity GetRandomRarity(Item itemHolder)
        {
            return GetRandomRarity(itemHolder.AllowedItemRarity);
        }

        public static ItemRarity GetRandomRarity(ItemRarity allowedItemRarity)
        {
            float rarityRandomWeightSumm = 0;
            List<ItemRarity> randomRarities = new List<ItemRarity>();
            int raritiesCount = 0;
            foreach (var rarity in itemSettings.rarityList)
            {
                ItemRarity tempItemRarity = (ItemRarity)rarity;
                if (allowedItemRarity.HasFlag(tempItemRarity))
                {
                    randomRarities.Add(tempItemRarity);

                    raritiesCount++;

                    rarityRandomWeightSumm += itemSettings.raritySettingsLink[tempItemRarity].ItemRarityRandomWeight;
                }
            }

            // This is the magic random number that will decide, which object is hit now
            double hitValue = WeightRandom.GetValue(rarityRandomWeightSumm);

            double runningValue = 0;
            // Find out in a loop which object's probability hits the random value...
            for (int i = 0; i < raritiesCount; i++)
            {
                runningValue += itemSettings.raritySettingsLink[randomRarities[i]].ItemRarityRandomWeight;
                if (hitValue < runningValue)
                {
                    return randomRarities[i];
                }
            }

            return ItemRarity.Common;
        }

        public static ItemRarity GetDailyDealRandomRarity()
        {
            // This is the magic random number that will decide, which object is hit now
            double hitValue = WeightRandom.GetValue(itemSettings.dailyDealRarityRandomWeightSumm);

            double runningValue = 0;
            // Find out in a loop which object's probability hits the random value...
            for (int i = 0; i < itemSettings.raritySettings.Length; i++)
            {
                runningValue += itemSettings.raritySettings[i].DailyDealRarityRandomWeight;
                if (hitValue < runningValue)
                {
                    return itemSettings.raritySettings[i].ItemRarity;
                }
            }

            return ItemRarity.Common;
        }

        public static ItemRarity GetDailyDealRandomRarity(Item item)
        {
            ItemRarity allowedItemRarity = item.AllowedItemRarity;

            float rarityRandomWeightSumm = 0;
            List<ItemRarity> randomRarities = new List<ItemRarity>();
            int raritiesCount = 0;
            foreach (var rarity in itemSettings.rarityList)
            {
                ItemRarity tempItemRarity = (ItemRarity)rarity;
                if (allowedItemRarity.HasFlag(tempItemRarity) && itemSettings.dailyDealAllowedRarities.HasFlag(tempItemRarity))
                {
                    randomRarities.Add(tempItemRarity);

                    raritiesCount++;

                    rarityRandomWeightSumm += itemSettings.raritySettingsLink[tempItemRarity].DailyDealRarityRandomWeight;
                }
            }

            // This is the magic random number that will decide, which object is hit now
            double hitValue = WeightRandom.GetValue(rarityRandomWeightSumm);

            double runningValue = 0;
            // Find out in a loop which object's probability hits the random value...
            for (int i = 0; i < raritiesCount; i++)
            {
                runningValue += itemSettings.raritySettingsLink[randomRarities[i]].DailyDealRarityRandomWeight;
                if (hitValue < runningValue)
                {
                    return randomRarities[i];
                }
            }

            return ItemRarity.Common;
        }

        public static WeaponItem GetDefaultWeapon()
        {
            return itemSettings.defaultWeapon;
        }

        public static int GetChracterUpgradeItem()
        {
            return itemSettings.chracterUpdateItemID;
        }

        public static string FormatColorDiff(float valueDiff)
        {
            return valueDiff > 0 ? COMPARE_GREEN_COLOR : COMPARE_RED_COLOR;
        }

        public static string FormatValueDiff(float valueDiff)
        {
            return valueDiff > 0 ? "+" + valueDiff : valueDiff.ToString();
        }

        public static int GetKeyID()
        {
            return itemSettings.keyItemID;
        }

        public static int GetRoyalKeyID()
        {
            return itemSettings.royalKeyItemID;
        }

        public static int GetInventoryKeyAmount()
        {
            return Inventory.GetResourceAmount(itemSettings.keyItemID);
        }

        public static int GetInventoryRoyalKeyAmount()
        {
            return Inventory.GetResourceAmount(itemSettings.royalKeyItemID);
        }

        [System.Serializable]
        public class RaritySettings
        {
            [SerializeField, EnumSingle] ItemRarity itemRarity;

            [Space]
            [SerializeField] float itemLevelMultiplier = 1.3f;
            [SerializeField] float itemRarityRandomWeight = 1;

            [SerializeField] Sprite slotBackground;
            [SerializeField] Sprite slotFrame;

            [SerializeField] Color color = Color.white;

            [SerializeField] Sprite titleImage;
            [SerializeField] Sprite titleLeftImage;

            [Header("Daily Deal")]
            [SerializeField] float itemDailyDealRarityRandomWeight = 1;
            [SerializeField] float itemDailyDealCoinsPrice = 0.3f;
            [SerializeField] float itemDailyDealGemsPrice = 0.5f;

            [Space]
            [SerializeField] Sprite dailyDealBackground;

            [Space]
            [SerializeField] Sprite windowShining;

            public ItemRarity ItemRarity => itemRarity;
            
            public float ItemLevelMultiplier => itemLevelMultiplier;
            public float ItemRarityRandomWeight => itemRarityRandomWeight;

            public Sprite SlotBackground => slotBackground;
            public Sprite SlotFrame => slotFrame;

            public Color Color => color;

            public Sprite TitleImage => titleImage;
            public Sprite TitleLeftImage => titleLeftImage;

            public float DailyDealRarityRandomWeight => itemDailyDealRarityRandomWeight;
            public float DailyDealCoinsPrice => itemDailyDealCoinsPrice;
            public float DailyDealGemsPrice => itemDailyDealGemsPrice;

            public Sprite DailyDealBackground => dailyDealBackground;

            public Sprite WindowShining => windowShining;

            public float DailyDealPriceCoefficient(Currencies currency)
            {
                if (currency == Currencies.Gold)
                    return itemDailyDealCoinsPrice;

                return itemDailyDealGemsPrice;
            }
        }
    }
}