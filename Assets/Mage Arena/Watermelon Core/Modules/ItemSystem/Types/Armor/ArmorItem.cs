using System.Text;
using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Armor Item", menuName = "Items/Armor Item")]
    public class ArmorItem : Item, IItemSpecialEffect
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

        public ArmorItem()
        {
            type = ItemType.Armor;
            equipableItemType = EquipableItem.Armor;
        }

        public override ItemHolder GetHolder()
        {
            return new ArmorItemHolder(this, minItemLevel, maxItemLevel);
        }

        public override ItemHolder GetDefaultHolder()
        {
            return new ArmorItemHolder(this, 1, ItemRarity.Common);
        }
    }
}
