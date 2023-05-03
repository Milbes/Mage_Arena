using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    public static class Account
    {
        private static int level;
        public static int Level => level;

        private static int experience;
        public static int Experience => experience;

        private static int nextLevelExperience;
        public static int NextLevelExperience => nextLevelExperience;

        private static bool saveRequired = false;

        public static OnLevelUpCallback OnLevelUp;
        public static OnExperienceGainCallback OnExperienceGain;

        public static void Init()
        {
            Load();

            RecalculateNextLevelExperience();
        }

        private static void RecalculateNextLevelExperience()
        {
            nextLevelExperience = (8 * level) * (45 + (5 * level));
        }

        public static void AddExperience(int experience)
        {
            Account.experience += experience;

            while(Account.experience >= nextLevelExperience)
            {
                int experienceDiff = Account.experience - nextLevelExperience;

                // Next level
                level++;

                if (OnLevelUp != null)
                    OnLevelUp.Invoke(level);

                RecalculateNextLevelExperience();

                Account.experience = experienceDiff;
            }

            if (OnExperienceGain != null)
                OnExperienceGain.Invoke();

            saveRequired = true;
        }

        #region Save/Load
        private static void Load()
        {
            AccountSave.SaveData saveData = AccountSave.Load();

            // Load items from save
            level = saveData.level;
            experience = saveData.experience;
        }

        private static void Save()
        {
            AccountSave.Save(new AccountSave.SaveData()
            {
                level = Account.level,
                experience = Account.experience                
            });

            saveRequired = false;
        }

        public static void SaveIfRequired()
        {
            if (!saveRequired)
                return;

            Save();
        }

        public static void ResetLevelDev()
        {
            Account.experience = 0;
            Account.level = 1;

            Save();
        }
        #endregion

        public delegate void OnLevelUpCallback(int level);
        public delegate void OnExperienceGainCallback();
    }
}
