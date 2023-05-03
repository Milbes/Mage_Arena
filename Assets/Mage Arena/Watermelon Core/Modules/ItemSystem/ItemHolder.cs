using System;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    /// <summary>
    /// Base abstract holder (Add your own field for default item)
    /// </summary>
    [System.Serializable]
    public abstract class ItemHolder : IItemPreview
    {
        [SerializeField] int id;
        [SerializeField] int itemLevel = 1;
        [SerializeField] ItemRarity itemRarity = ItemRarity.Common;

        [NonSerialized] Item item;
        [NonSerialized] int inventoryID = -1;

        /// <summary>
        /// Item identifier
        /// </summary>
        public int ID => id;

        public int ItemLevel => itemLevel;

        public ItemRarity ItemRarity => itemRarity;

        /// <summary>
        /// Current item
        /// </summary>
        public Item Item
        {
            get 
            {
                if (item == null)
                    item = ItemDatabase.GetItem(id);

                return item; 
            }
        }

        public int InventoryID
        {
            get { return inventoryID; }
            set { inventoryID = value; }
        }

        public virtual int Amount => 1;
        public Sprite Preview => Item.Sprite;
        public Color BackgroundColor => RaritySettings.Color;
        public Sprite BackgroundSprite => RaritySettings.SlotBackground;
        public Sprite FrameSprite => RaritySettings.SlotFrame;

        public float ItemLevelMultiplier => RaritySettings.ItemLevelMultiplier;

        public ItemSettings.RaritySettings RaritySettings
        {
            get { return ItemSettings.GetRaritySettings(itemRarity); }
        }

        /// <summary>
        /// Base holder constructor
        /// </summary>
        public ItemHolder(Item item, ItemRarity itemRarity)
        {
            this.id = item.ID;

            this.item = item;
            this.itemRarity = itemRarity;
        }

        /// <summary>
        /// DEV If condition is true item in inventory will be removed
        /// </summary>
        public abstract bool Check();

        public abstract string GetStatsString();
        public abstract string GetCompareStatsString(ItemHolder itemHolder);

        public void SetItemLevel(int itemLevel)
        {
            this.itemLevel = itemLevel;
        }

        public void SetItemRarity(ItemRarity itemRarity)
        {
            this.itemRarity = itemRarity;
        }
    }

    public class MiscItemHolderComparer : IComparer<MiscItemHolder>
    {
        public int Compare(MiscItemHolder itemHolder, MiscItemHolder compareItemHolder)
        {
            int itemRarity = (int)itemHolder.ItemRarity;
            int compareItemRarity = (int)compareItemHolder.ItemRarity;

            if (itemRarity > compareItemRarity)
            {
                return -1;
            }
            else if (itemRarity < compareItemRarity)
            {
                return 1;
            }
            else
            {
                if (itemHolder.Amount > compareItemHolder.Amount)
                {
                    return -1;
                }
                else if(itemHolder.Amount < compareItemHolder.Amount)
                {
                    return 1;
                }

                Item mainItem = itemHolder.Item;
                Item compareItem = compareItemHolder.Item;

                return string.Compare(mainItem.ItemName, compareItem.ItemName, StringComparison.CurrentCulture);
            }
        }
    }

    public class EquipableItemHolderComparer : IComparer<ItemHolder>
    {
        public int Compare(ItemHolder itemHolder, ItemHolder compareItemHolder)
        {
            int itemRarity = (int)itemHolder.ItemRarity;
            int compareItemRarity = (int)compareItemHolder.ItemRarity;

            if (itemRarity > compareItemRarity)
            {
                return -1;
            }
            else if (itemRarity < compareItemRarity)
            {
                return 1;
            }
            else
            {
                Item mainItem = itemHolder.Item;
                Item compareItem = compareItemHolder.Item;

                int itemType = (int)mainItem.Type;
                int compareItemType = (int)compareItem.Type;

                if (itemType > compareItemType)
                {
                    return 1;
                }
                else if (itemType < compareItemType)
                {
                    return -1;
                }
                else
                {
                    if(itemHolder.ItemLevel < compareItemHolder.ItemLevel)
                    {
                        return 1;
                    }
                    else if(itemHolder.ItemLevel > compareItemHolder.ItemLevel)
                    {
                        return -1;
                    }
                }
            }

            return 0;
        }
    }
}