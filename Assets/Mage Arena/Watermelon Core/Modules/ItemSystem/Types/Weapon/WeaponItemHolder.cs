using System.Text;
using UnityEngine;

namespace Watermelon
{
    [System.Serializable]
    public class WeaponItemHolder : ItemHolder
    {
        public WeaponItemHolder(Item item, int itemLevel, ItemRarity itemRarity) : base(item, itemRarity)
        {
            SetItemLevel(itemLevel);
        }

        public WeaponItemHolder(Item item, int minItemLevel, int maxItemLevel) : base(item, ItemSettings.GetRandomRarity(item))
        {
            SetItemLevel(Mathf.Clamp(Random.Range(Mathf.Clamp(Account.Level - 10, 1, int.MaxValue), Account.Level), minItemLevel, maxItemLevel + 1));
        }

        public override bool Check()
        {
            return false;
        }

        public Character.Stats GetScaledStats()
        {
            Character.Stats scaledStats = new Character.Stats(((WeaponItem)Item).Stats);
            scaledStats.MultiplyStats(ItemLevel * ItemLevelMultiplier);

            return scaledStats;
        }

        public override string GetStatsString()
        {
            StringBuilder stringBuilder = GetScaledStats().FormatStats();

            WeaponItem weaponItem = (WeaponItem)Item;

            ItemSpecialEffect[] itemSpecialEffects = weaponItem.ItemSpecialEffects;
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
            WeaponItemHolder weaponComparedItemHolder = (WeaponItemHolder)comparedItemHolder;
            WeaponItem weaponItem = (WeaponItem)Item;

            StringBuilder stringBuilder = GetScaledStats().FormatCompareStats(weaponComparedItemHolder.GetScaledStats());

            ItemSpecialEffect[] itemSpecialEffects = weaponItem.ItemSpecialEffects;
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
