using System.Text;
using UnityEngine;

namespace Watermelon
{
    [System.Serializable]
    public class RingItemHolder : ItemHolder
    {
        public RingItemHolder(Item item, int itemLevel, ItemRarity itemRarity) : base(item, itemRarity)
        {
            SetItemLevel(itemLevel);
        }

        public RingItemHolder(Item item, int minItemLevel, int maxItemLevel) : base(item, ItemSettings.GetRandomRarity(item))
        {
            SetItemLevel(Mathf.Clamp(Random.Range(Mathf.Clamp(Account.Level - 10, 1, int.MaxValue), Account.Level), minItemLevel, maxItemLevel + 1));
        }

        public override bool Check()
        {
            return false;
        }

        public Character.Stats GetScaledStats()
        {
            Character.Stats scaledStats = new Character.Stats(((RingItem)Item).Stats);
            scaledStats.MultiplyStats(ItemLevel * ItemLevelMultiplier);

            return scaledStats;
        }

        public override string GetStatsString()
        {
            StringBuilder stringBuilder = GetScaledStats().FormatStats();

            RingItem ringItem = (RingItem)Item;

            ItemSpecialEffect[] itemSpecialEffects = ringItem.ItemSpecialEffects;
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
            RingItem ringItem = (RingItem)Item;
            RingItemHolder comparedRingItemHolder = (RingItemHolder)comparedItemHolder;

            StringBuilder stringBuilder = GetScaledStats().FormatCompareStats(comparedRingItemHolder.GetScaledStats());

            ItemSpecialEffect[] itemSpecialEffects = ringItem.ItemSpecialEffects;
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
