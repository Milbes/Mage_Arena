using System.Text;
using UnityEngine;

namespace Watermelon
{
    [System.Serializable]
    public class ArmorItemHolder : ItemHolder
    {
        public ArmorItemHolder(Item item, int itemLevel, ItemRarity itemRarity) : base(item, itemRarity)
        {
            SetItemLevel(itemLevel);
        }

        public ArmorItemHolder(Item item, int minItemLevel, int maxItemLevel) : base(item, ItemSettings.GetRandomRarity(item))
        {
            SetItemLevel(Mathf.Clamp(Random.Range(Mathf.Clamp(Account.Level - 10, 1, int.MaxValue), Account.Level), minItemLevel, maxItemLevel + 1));
        }

        public override bool Check()
        {
            return false;
        }

        public Character.Stats GetScaledStats()
        {
            Character.Stats scaledStats = new Character.Stats(((ArmorItem)Item).Stats);
            scaledStats.MultiplyStats(ItemLevel * ItemLevelMultiplier);

            return scaledStats;
        }

        public override string GetStatsString()
        {
            StringBuilder stringBuilder = GetScaledStats().FormatStats();

            ArmorItem armorItem = (ArmorItem)Item;

            ItemSpecialEffect[] itemSpecialEffects = armorItem.ItemSpecialEffects;
            for (int i = 0; i < itemSpecialEffects.Length; i++)
            {
                if (!string.IsNullOrEmpty(itemSpecialEffects[i].Description))
                {
                    stringBuilder.AppendLine(itemSpecialEffects[i].Description);
                }
            }

            return stringBuilder.ToString();
        }

        public override string GetCompareStatsString(ItemHolder comparedItemHolder)
        {
            ArmorItem armorItem = (ArmorItem)Item;
            ArmorItemHolder comparedArmorItemHolder = (ArmorItemHolder)comparedItemHolder;

            StringBuilder stringBuilder = GetScaledStats().FormatCompareStats(comparedArmorItemHolder.GetScaledStats());

            ItemSpecialEffect[] itemSpecialEffects = armorItem.ItemSpecialEffects;
            for (int i = 0; i < itemSpecialEffects.Length; i++)
            {
                if (!string.IsNullOrEmpty(itemSpecialEffects[i].Description))
                {
                    stringBuilder.AppendLine(itemSpecialEffects[i].Description);
                }
            }

            return stringBuilder.ToString();
        }
    }
}
