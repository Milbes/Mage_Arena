using System.Text;
using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Ring Item", menuName = "Items/Ring Item")]
    public class RingItem : Item, IItemSpecialEffect
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

        public RingItem()
        {
            type = ItemType.Ring;
            equipableItemType = EquipableItem.Ring;
        }

        public override ItemHolder GetHolder()
        {
            return new RingItemHolder(this, minItemLevel, maxItemLevel);
        }

        public override ItemHolder GetDefaultHolder()
        {
            return new RingItemHolder(this, 1, ItemRarity.Common);
        }
    }
}
