using System.Text;
using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Necklaces Item", menuName = "Items/Necklaces Item")]
    public class NecklacesItem : Item, IItemSpecialEffect
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

        public NecklacesItem()
        {
            type = ItemType.Necklaces;
            equipableItemType = EquipableItem.Necklaces;
        }

        public override ItemHolder GetHolder()
        {
            return new NecklacesItemHolder(this, minItemLevel, maxItemLevel);
        }

        public override ItemHolder GetDefaultHolder()
        {
            return new NecklacesItemHolder(this, 1, ItemRarity.Common);
        }
    }
}
