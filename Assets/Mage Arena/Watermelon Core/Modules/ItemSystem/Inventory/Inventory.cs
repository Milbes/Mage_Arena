using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    public static class Inventory
    {
        private static List<ItemHolder> inventory;
        private static List<MiscItemHolder> resources;

        private static bool saveRequired = false;

        public static OnNewItemAddedCallback OnNewItemAdded;
        public static OnItemStateChangedCallback OnItemStateChanged;
        public static OnItemRemovedCallback OnItemRemoved;

        public static OnNewResourceAddedCallback OnNewResourceAdded;
        public static OnResourceStateChangedCallback OnResourceStateChanged;
        public static OnResourceRemovedCallback OnResourceRemoved;

        private static EquipableItemHolderComparer equipableItemHolderComparer;
        private static MiscItemHolderComparer miscItemHolderComparer;

        public static void Init()
        {
            inventory = new List<ItemHolder>();
            resources = new List<MiscItemHolder>();

            equipableItemHolderComparer = new EquipableItemHolderComparer();
            miscItemHolderComparer = new MiscItemHolderComparer();

            Load();
        }

        private static void RecalculateResourcesIDs(int startFrom = 0)
        {
            int resourcesSize = resources.Count;
            for (int i = startFrom; i < resourcesSize; i++)
            {
                resources[i].InventoryID = i;
            }
        }

        public static void RemoveItem(int itemIndex)
        {
            if(inventory.IsInRange(itemIndex))
            {
                ItemHolder itemHolder = inventory[itemIndex];

                inventory[itemIndex].InventoryID = -1;
                inventory.RemoveAt(itemIndex);

                RecalculateItemIDs(itemIndex);

                if (OnItemRemoved != null)
                    OnItemRemoved.Invoke(itemIndex, itemHolder);

                saveRequired = true;
            }
        }

        public static bool HasResource(int itemIndex, int amount)
        {
            int tempIndex = resources.FindIndex(x => x.Item.ID == itemIndex && x.Amount >= amount);
            if (tempIndex != -1)
                return true;

            return false;
        }

        public static bool HasResource(int itemIndex, int amount, ref int inventoryItemIndex)
        {
            inventoryItemIndex = resources.FindIndex(x => x.Item.ID == itemIndex && x.Amount >= amount);
            if (inventoryItemIndex != -1)
                return true;

            return false;
        }

        public static int GetResourceAmount(int itemIndex)
        {
            int tempIndex = resources.FindIndex(x => x.Item.ID == itemIndex);
            if (tempIndex != -1)
                return resources[tempIndex].Amount;

            return 0;
        }

        public static int GetResourceAmount(int itemIndex, ref int inventoryItemIndex)
        {
            inventoryItemIndex = resources.FindIndex(x => x.Item.ID == itemIndex);
            if (inventoryItemIndex != -1)
                return resources[inventoryItemIndex].Amount;

            return 0;
        }

        public static void RemoveResource(int itemIndex, int amount)
        { 
            if (resources.IsInRange(itemIndex))
            {
                if (resources[itemIndex].Amount > amount)
                {
                    resources[itemIndex].ChangeAmount(-amount);

                    if (OnResourceStateChanged != null)
                        OnResourceStateChanged.Invoke(itemIndex, resources[itemIndex]);
                }
                else
                {
                    MiscItemHolder miscItemHolder = resources[itemIndex];

                    resources[itemIndex].InventoryID = -1;
                    resources.RemoveAt(itemIndex);

                    RecalculateResourcesIDs(itemIndex);

                    if (OnResourceRemoved != null)
                        OnResourceRemoved.Invoke(itemIndex, miscItemHolder);
                }

                saveRequired = true;
            }
        }

        private static void RecalculateItemIDs(int startFrom = 0)
        {
            int inventorySize = inventory.Count;
            for(int i = startFrom; i < inventorySize; i++)
            {
                inventory[i].InventoryID = i;
            }
        }

        public static void AddItem(ItemHolder itemHolder)
        {
            Item item = itemHolder.Item;
            if(item.Type != ItemType.Misc)
            {
                int itemIndex = inventory.BinarySearch(itemHolder, equipableItemHolderComparer);
                if(itemIndex < 0)
                    itemIndex = ~itemIndex;

                itemHolder.InventoryID = itemIndex;

                inventory.Insert(itemIndex, itemHolder);

                RecalculateItemIDs(itemIndex);

                if (OnNewItemAdded != null)
                    OnNewItemAdded.Invoke(itemIndex, itemHolder);

                saveRequired = true;
            }
            else
            {
                MiscItemHolder miscItemHolder = (MiscItemHolder)itemHolder;

                // Add resources
                MiscItem miscItem = (MiscItem)item;
                if (miscItem.MaxStackSize > 1)
                {
                    bool resourceAdded = false;
                    int amount = itemHolder.Amount;

                    // Find same resource in resources
                    int resourcesSize = resources.Count;
                    for (int i = 0; i < resourcesSize; i++)
                    {
                        if (item.ID == resources[i].Item.ID)
                        {
                            if (resources[i].Amount + amount <= miscItem.MaxStackSize)
                            {
                                resources[i].ChangeAmount(amount);

                                resourceAdded = true;

                                if (OnResourceStateChanged != null)
                                    OnResourceStateChanged.Invoke(i, resources[i]);

                                break;
                            }
                            else
                            {
                                int tempAmount = miscItem.MaxStackSize - resources[i].Amount;

                                resources[i].ChangeAmount(tempAmount);

                                amount -= tempAmount;

                                if (OnResourceStateChanged != null)
                                    OnResourceStateChanged.Invoke(i, resources[i]);

                                if (amount == 0)
                                {
                                    resourceAdded = true;

                                    break;
                                }
                            }
                        }
                    }

                    if (!resourceAdded)
                    {
                        miscItemHolder.SetAmount(amount);

                        int miscItemIndex = resources.BinarySearch(miscItemHolder, miscItemHolderComparer);
                        if (miscItemIndex < 0)
                            miscItemIndex = ~miscItemIndex;

                        miscItemHolder.InventoryID = miscItemIndex;

                        resources.Insert(miscItemIndex, miscItemHolder);

                        RecalculateResourcesIDs(miscItemIndex);

                        if (OnNewResourceAdded != null)
                            OnNewResourceAdded.Invoke(miscItemIndex, miscItemHolder);
                    }
                }
                else
                {
                    int miscItemIndex = resources.BinarySearch(miscItemHolder, miscItemHolderComparer);
                    if (miscItemIndex < 0)
                        miscItemIndex = ~miscItemIndex;

                    miscItemHolder.InventoryID = miscItemIndex;

                    resources.Insert(miscItemIndex, miscItemHolder);

                    RecalculateResourcesIDs(miscItemIndex);

                    if (OnNewResourceAdded != null)
                        OnNewResourceAdded.Invoke(miscItemIndex, miscItemHolder);
                }

                saveRequired = true;
            }
        }

        #region Save/Load
        private static void Load()
        {
            InventorySave.SaveData saveData = InventorySave.Load();

            // Load items from save
            inventory = new List<ItemHolder>(saveData.inventory);
            inventory.Sort(equipableItemHolderComparer);

            for(int i = 0; i < saveData.inventory.Length; i++)
            {
                inventory[i].InventoryID = i;
            }

            resources = new List<MiscItemHolder>(saveData.resources);
            resources.Sort(miscItemHolderComparer);
            for(int i = 0; i < saveData.resources.Length; i++)
            {
                resources[i].InventoryID = i;
            }
        }

        private static void Save()
        {
            InventorySave.Save(new InventorySave.SaveData(inventory.ToArray(), resources.ToArray()));

            saveRequired = false;
        }

        public static void SaveIfRequired()
        {
            if (!saveRequired)
                return;

            Save();
        }
        #endregion

        public static List<ItemHolder> GetInventory()
        {
            return inventory;
        }

        public static List<MiscItemHolder> GetResources()
        {
            return resources;
        }

        public delegate void OnNewItemAddedCallback(int itemIndex, ItemHolder itemHolder);
        public delegate void OnItemStateChangedCallback(int itemIndex, ItemHolder itemHolder);
        public delegate void OnItemRemovedCallback(int itemIndex, ItemHolder removedItemHolder);

        public delegate void OnNewResourceAddedCallback(int itemIndex, MiscItemHolder miscItemHolder);
        public delegate void OnResourceStateChangedCallback(int resouceIndex, MiscItemHolder miscItemHolder);
        public delegate void OnResourceRemovedCallback(int resouceIndex, MiscItemHolder removedMiscItemHolder);
    }
}
