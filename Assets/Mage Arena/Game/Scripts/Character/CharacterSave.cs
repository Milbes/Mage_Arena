using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    public static class CharacterSave
    {
        private const string FILE_NAME = "Character";

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
            public Character.Equipment equipment;
            public int characterLevel;

            public SaveData()
            {
                equipment = new Character.Equipment();
                characterLevel = 0;
            }
        }
    }
}
