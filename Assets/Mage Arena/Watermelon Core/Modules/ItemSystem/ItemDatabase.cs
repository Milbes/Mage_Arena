using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//ToDo:
//Add IInizialized attribute

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Item Database", menuName = "Iventory/Item Database")]
    public class ItemDatabase : ScriptableObject
    {
        private static ItemDatabase instance;

        [SerializeField] Item[] items;
        /// <summary>
        /// Items database. All allowed items in game
        /// </summary>
        public Item[] Items
        {
            get { return items; }
        }

        /// <summary>
        /// Cached items ids
        /// </summary>
        private Dictionary<int, int> itemIds = new Dictionary<int, int>();
        
        public void Init()
        {
            instance = this;

            //Cache items ids connections
            int itemsLength = items.Length;
            itemIds.Clear();

            for (int i = 0; i < itemsLength; i++)
            {
                itemIds.Add(items[i].ID, i);
            }
        }

        public bool HasItemById(int id)
        {
            return itemIds.ContainsKey(id);
        }

        /// <summary>
        /// Get item by id
        /// </summary>
        public Item GetItemById(int id)
        {
            if(items.IsInRange(id))
            {
                return items[itemIds[id]];
            }

            return null;
        }

        public static bool HasItem(int id)
        {
            return instance.HasItemById(id);
        }

        /// <summary>
        /// Get item by id
        /// </summary>
        public static Item GetItem(int id)
        {
            return instance.GetItemById(id);
        }

        /// <summary>
        /// Get all equipable items by rarity
        /// </summary>
        public static Item[] GetEquipableItemsByRarity(ItemRarity itemRarities)
        {
            List<Item> tempItems = new List<Item>();
            for(int i = 0; i < instance.items.Length; i++)
            {
                if(instance.items[i].IsItemEquipable() && instance.items[i].AllowedItemRarity.HasFlag(itemRarities))
                {
                    tempItems.Add(instance.items[i]);
                }
            }

            return tempItems.ToArray();
        }

        public static Item[] GetItems()
        {
            return instance.items;
        }

#if UNITY_EDITOR
        public static ItemDatabase GetDatabase()
        {
            return RuntimeEditorUtils.GetAssetByName<ItemDatabase>("Item Database");
        }
#endif
    }
}