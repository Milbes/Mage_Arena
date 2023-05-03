using UnityEngine;
using UnityEngine.Events;

namespace Watermelon
{
    /// <summary>
    /// Base abstract item class
    /// </summary>
    public abstract class Item : ScriptableObject
    {
        [SerializeField] int id;
        [SerializeField] string itemName = "Unknown";
        [SerializeField] string description;
        [SerializeField] Sprite sprite;
        [SerializeField] ItemRarity allowedItemRarity = ItemRarity.Common;

        protected ItemType type;
        protected EquipableItem equipableItemType;

        /// <summary>
        /// Unique item id
        /// </summary>
        public int ID => id;

        /// <summary>
        /// Item name
        /// </summary>
        public string ItemName => itemName;

        /// <summary>
        /// Information about item
        /// </summary>
        public string Description => description;

        /// <summary>
        /// Item icon (Use icons with same sizes)
        /// </summary>
        public Sprite Sprite => sprite;

        /// <summary>
        /// Item type (do initialization in constructors)
        /// </summary>
        public ItemType Type => type;

        public EquipableItem EquipableItemType => equipableItemType;

        public ItemRarity AllowedItemRarity => allowedItemRarity;

        /// <summary>
        /// Returns holder for current item type with random rarity and item level
        /// </summary>
        public abstract ItemHolder GetHolder();

        /// <summary>
        /// Returns holder for current item type with uncommon rarity and 1 item level
        /// </summary>
        public abstract ItemHolder GetDefaultHolder();

        public bool IsItemEquipable()
        {
            return equipableItemType != EquipableItem.None;
        }

#if UNITY_EDITOR
        public void SetName(string name)
        {
            itemName = name;
        }

        public void SetId(int id)
        {
            this.id = id;
        }
#endif

        [System.Serializable]
        public class InventoryAction
        {
            [SerializeField] string name;
            public string Name => name;

            [SerializeField] UnityAction callback;
            public UnityAction Callback => callback;

            public InventoryAction(string name, UnityAction callback)
            {
                this.name = name;
                this.callback = callback;
            }
        }
    }
}
