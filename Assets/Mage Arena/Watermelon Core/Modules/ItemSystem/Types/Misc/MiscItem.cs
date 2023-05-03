using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Misc Item", menuName = "Items/Misc Item")]
    public class MiscItem : Item
    {
        [SerializeField] int maxStackSize = 1;

        /// <summary>
        /// Maximum amount items in stack
        /// </summary>
        public int MaxStackSize => maxStackSize;

        public MiscItem()
        {
            type = ItemType.Misc;
            equipableItemType = EquipableItem.None;
        }

        public override ItemHolder GetHolder()
        {
            return new MiscItemHolder(this, 1, AllowedItemRarity);
        }

        public override ItemHolder GetDefaultHolder()
        {
            return new MiscItemHolder(this, 1, AllowedItemRarity);
        }
    }
}
