using System.Text;
using UnityEngine;

namespace Watermelon
{
    [System.Serializable]
    public class MagicBookItemHolder : ItemHolder
    {
        public MagicBookItemHolder(Item item, int itemLevel, ItemRarity itemRarity) : base(item, itemRarity)
        {
            SetItemLevel(itemLevel);
        }

        public MagicBookItemHolder(Item item, int minItemLevel, int maxItemLevel) : base(item, ItemSettings.GetRandomRarity(item))
        {
            SetItemLevel(Mathf.Clamp(Random.Range(Mathf.Clamp(Account.Level - 10, 1, int.MaxValue), Account.Level), minItemLevel, maxItemLevel + 1));
        }

        public override bool Check()
        {
            return false;
        }

        public Character.Stats GetScaledStats()
        {
            Character.Stats scaledStats = new Character.Stats(((MagicBookItem)Item).Stats);
            scaledStats.MultiplyStats(ItemLevel * ItemLevelMultiplier);

            return scaledStats;
        }

        public override string GetStatsString()
        {
            MagicBookItem magicBookItem = (MagicBookItem)Item;

            StringBuilder stringBuilder = GetScaledStats().FormatStats();

            ItemSpecialEffect[] itemSpecialEffects = magicBookItem.ItemSpecialEffects;
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
            MagicBookItemHolder comparedMagicBookItemHolder = (MagicBookItemHolder)comparedItemHolder;
            MagicBookItem magicBookItem = (MagicBookItem)Item;

            StringBuilder stringBuilder = GetScaledStats().FormatCompareStats(comparedMagicBookItemHolder.GetScaledStats());

            ItemSpecialEffect[] itemSpecialEffects = magicBookItem.ItemSpecialEffects;
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
