using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    public static class InventorySave
    {
        private const string FILE_NAME = "Inventory";

        public static SaveData Load()
        {
            if (Serializer.FileExistsAtPDP(FILE_NAME))
            {
                SaveData saveData = Serializer.DeserializeFromPDP<SaveData>(FILE_NAME);
                if (saveData != null)
                {
                    return saveData;
                }
            }

            return new SaveData();
        }

        public static void Save(SaveData saveData)
        {
            Serializer.SerializeToPDP(saveData, FILE_NAME);
        }

        [System.Serializable]
        public class SaveData
        {
            public ItemHolder[] inventory = null;
            public MiscItemHolder[] resources = null;

            public SaveData()
            {
                inventory = new ItemHolder[] { };
                resources = new MiscItemHolder[] { };
            }

            public SaveData(ItemHolder[] inventory, MiscItemHolder[] resources)
            {
                this.inventory = inventory;
                this.resources = resources;
            }
        }
    }
}
