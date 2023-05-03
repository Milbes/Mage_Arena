using UnityEngine;
using System.Collections;

namespace Watermelon
{
    /// <summary>
    /// Base behaviour for item containers (inventory, storages, loot and etc.)
    /// </summary>
    [System.Serializable]
    public class ItemsContainer
    {
        /// <summary>
        /// Current inventory size
        /// </summary>
        private int inventorySize = 12;

        [SerializeField] ItemHolder[] inventory;

        /// <summary>
        /// Container inventory
        /// </summary>
        public ItemHolder[] Inventory => inventory;

        /// <summary>
        /// Container constructor
        /// </summary>
        public ItemsContainer(int inventorySize)
        {
            this.inventorySize = inventorySize;

            inventory = new ItemHolder[this.inventorySize];
        }

        /// <summary>
        /// Add item by id
        /// </summary>
        public bool AddItem(Item item)
        {
            for (int i = 0; i < inventorySize; i++)
            {
                if (inventory[i] == null)
                {
                    inventory[i] = item.GetHolder();

                    return true;
                }
            }

            return false;
        }
    }
}