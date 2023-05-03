using System.Text;
using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "MagicBook Item", menuName = "Items/MagicBook Item")]
    public class MagicBookItem : Item, IItemSpecialEffect
    {
        [Header("Stats")]
        [SerializeField] Character.Stats stats;

        [Header("Stats Scaling")]
        [SerializeField] int minItemLevel = 1;
        [SerializeField] int maxItemLevel = 1;

        [Space]
        [SerializeField] ItemSpecialEffect[] itemSpecialEffects;

        public Character.Stats Stats => stats;

        public int MinItemLevel => minItemLevel;
        public int MaxItemLevel => maxItemLevel;

        public ItemSpecialEffect[] ItemSpecialEffects => itemSpecialEffects;

        public MagicBookItem()
        {
            type = ItemType.MagicBook;
            equipableItemType = EquipableItem.MagicBook;
        }

        public override ItemHolder GetHolder()
        {
            return new MagicBookItemHolder(this, minItemLevel, maxItemLevel);
        }

        public override ItemHolder GetDefaultHolder()
        {
            return new MagicBookItemHolder(this, 1, ItemRarity.Common);
        }
    }
}
