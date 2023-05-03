using UnityEngine;

namespace Watermelon
{
    public static class AccountSave
    {
        private const string FILE_NAME = "Account";

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
            public int level;
            public int experience;

            public SaveData()
            {
                level = 1;
                experience = 0;
            }
        }
    }
}