using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Watermelon
{
    public static class Character
    {
        private static readonly SkinData.Upgrade EMPTY_UPGRADE = new SkinData.Upgrade();

        private static Equipment equipment;

        private static SkinData skinData;

        private static Stats stats = new Stats();

        private static int level;
        public static int Level => level;

        private static bool saveRequired = false;
        private static bool recalculatingRequired = true;

        public static OnStatsRecalculatedCallback OnStatsRecalculated;
        public static OnItemEquipedCallback OnItemEquiped;
        public static OnLevelUpCallback OnLevelUp;

        public static void Init(SkinData skinData)
        {
            Load();

            Character.skinData = skinData;

            RecalculateStats();
        }

        public static Stats GetStats()
        {
            if (recalculatingRequired)
                RecalculateStats();

            return stats;
        }

        public static Stats GetCharacterStats()
        {
            Stats characterStats = new Stats();

            // Add initital values
            characterStats.AddStats(skinData.InitialStats);

            // Add level values
            for (int i = 0; i < Level; i++)
            {
                characterStats.AddStats(skinData.Upgrades[i]);
            }

            return characterStats;
        }

        public static int GetAverageItemLevel()
        {
            int itemLevel = 0;

            // Weapon
            if (equipment.weapon != null)
                itemLevel += equipment.weapon.ItemLevel;

            // Armor
            if (equipment.armor != null)
                itemLevel += equipment.armor.ItemLevel;

            // Magic Book
            if (equipment.magicBook != null)
                itemLevel += equipment.magicBook.ItemLevel;

            // Ring
            if (equipment.ring != null)
                itemLevel += equipment.ring.ItemLevel;

            // Necklacess
            if (equipment.necklaces != null)
                itemLevel += equipment.necklaces.ItemLevel;

            itemLevel = Mathf.RoundToInt((float)itemLevel / Equipment.EQUIPED_SLOTS_COUNT);

            return itemLevel > 0 ? itemLevel : 1;
        }

        public static SkinData.Upgrade GetNextUpgrade()
        {
            if(skinData.Upgrades.IsInRange(level))
                return skinData.Upgrades[level];

            return EMPTY_UPGRADE;
        }

        public static bool IsMaxLevel()
        {
            if (skinData.Upgrades.IsInRange(level + 1))
                return false;

            return true;
        }

        public static void UpgradeLevel()
        {
            if (IsMaxLevel())
                return;

            int resourceInventoryIndex = -1;
            if (Currency.Coins < skinData.Upgrades[level].Cost || !Inventory.HasResource(ItemSettings.GetChracterUpgradeItem(), skinData.Upgrades[level].RequiredItemsCount, ref resourceInventoryIndex) || resourceInventoryIndex == -1)
                return;

            Currency.ChangeCoins(-skinData.Upgrades[level].Cost);
            Inventory.RemoveResource(resourceInventoryIndex, skinData.Upgrades[level].RequiredItemsCount);

            level++;

            recalculatingRequired = true;
            saveRequired = true;

            RecalculateStats();

            if (OnLevelUp != null)
                OnLevelUp.Invoke(level, skinData.Upgrades[level]);
        }

        private static void RecalculateStats()
        {
            stats.Reset();

            // Add initital values
            stats.AddStats(skinData.InitialStats);

            // Add level values
            for (int i = 0; i < Level; i++)
            {
                stats.AddStats(skinData.Upgrades[i]);
            }

            // Add equipment values
            // Weapon
            if (equipment.weapon != null)
            {
                stats.AddStats(equipment.weapon.GetScaledStats());
            }

            // Armor
            if (equipment.armor != null)
            {
                stats.AddStats(equipment.armor.GetScaledStats());
            }

            // Magic Book
            if (equipment.magicBook != null)
            {
                stats.AddStats(equipment.magicBook.GetScaledStats());
            }

            // Ring
            if (equipment.ring != null)
            {
                stats.AddStats(equipment.ring.GetScaledStats());
            }

            // Necklacess
            if (equipment.necklaces != null)
            {
                stats.AddStats(equipment.necklaces.GetScaledStats());
            }

            recalculatingRequired = false;

            if (OnStatsRecalculated != null)
                OnStatsRecalculated.Invoke(stats);
        }

        public static void UnequipItem(EquipableItem equipableItemType)
        {
            ItemHolder itemHolder = GetEquipedItem(equipableItemType);
            if(itemHolder != null)
            {
                Inventory.AddItem(itemHolder);

                switch (equipableItemType)
                {
                    case EquipableItem.Weapon:
                        equipment.weapon = null;
                        break;
                    case EquipableItem.Armor:
                        equipment.armor = null;
                        break;
                    case EquipableItem.MagicBook:
                        equipment.magicBook = null;
                        break;
                    case EquipableItem.Ring:
                        equipment.ring = null;
                        break;
                    case EquipableItem.Necklaces:
                        equipment.necklaces = null;
                        break;
                }

                recalculatingRequired = true;
                saveRequired = true;

                if (OnItemEquiped != null)
                    OnItemEquiped.Invoke(equipableItemType, null, itemHolder);
            }
        }

        public static void EquipItem(ItemHolder itemHolder)
        {
            if(itemHolder != null)
            {
                Item item = itemHolder.Item;
                if(item.IsItemEquipable())
                {
                    ItemHolder equipedItem = null;

                    switch(item.EquipableItemType)
                    {
                        case EquipableItem.Weapon:
                            equipedItem = equipment.weapon;

                            equipment.weapon = (WeaponItemHolder)itemHolder;
                            break;
                        case EquipableItem.Armor:
                            equipedItem = equipment.armor;

                            equipment.armor = (ArmorItemHolder)itemHolder;
                            break;
                        case EquipableItem.MagicBook:
                            equipedItem = equipment.magicBook;

                            equipment.magicBook = (MagicBookItemHolder)itemHolder;
                            break;
                        case EquipableItem.Ring:
                            equipedItem = equipment.ring;

                            equipment.ring = (RingItemHolder)itemHolder;
                            break;
                        case EquipableItem.Necklaces:
                            equipedItem = equipment.necklaces;

                            equipment.necklaces = (NecklacesItemHolder)itemHolder;
                            break;
                    }

                    if(equipedItem != null)
                        Inventory.AddItem(equipedItem);

                    recalculatingRequired = true;
                    saveRequired = true;

                    if (OnItemEquiped != null)
                        OnItemEquiped.Invoke(item.EquipableItemType, itemHolder, equipedItem);
                }
            }
        }

        public static ItemHolder GetEquipedItem(EquipableItem equipableItemType)
        {
            switch(equipableItemType)
            {
                case EquipableItem.Weapon:
                    return equipment.weapon;
                case EquipableItem.Armor:
                    return equipment.armor;
                case EquipableItem.MagicBook:
                    return equipment.magicBook;
                case EquipableItem.Ring:
                    return equipment.ring;
                case EquipableItem.Necklaces:
                    return equipment.necklaces;
                case EquipableItem.None:
                default:
                    return null;
            }
        }

        #region Save/Load
        private static void Load()
        {
            CharacterSave.SaveData saveData = CharacterSave.Load();

            // Load items from save
            equipment = saveData.equipment;
            level = saveData.characterLevel;
        }

        private static void Save()
        {
            CharacterSave.Save(new CharacterSave.SaveData()
            {
                equipment = Character.equipment,
                characterLevel = level
            });

            saveRequired = false;
        }

        public static void SaveIfRequired()
        {
            if (!saveRequired)
                return;

            Save();
        }
        #endregion

        public delegate void OnStatsRecalculatedCallback(Stats stats);
        public delegate void OnItemEquipedCallback(EquipableItem equipableItemType, ItemHolder itemHolder, ItemHolder previousItemHolder);
        public delegate void OnLevelUpCallback(int newLevel, SkinData.Upgrade upgrade);

        [System.Serializable]
        public class Stats
        {
            public const string LABEL_HEALTH = "HP";
            public const string LABEL_HEALTH_PERCENT = "HP %";
            public const string LABEL_DAMAGE = "Dmg";
            public const string LABEL_DAMAGE_PERCENT = "Dmg %";
            public const string LABEL_CRIT_CHANCE = "Crit";
            public const string LABEL_CRIT_DAMAGE_PERCENT = "Crit Dmg %";
            public const string LABEL_MOVEMENT_SPEED = "Movement Speed";
            public const string LABEL_ATTACK_SPEED = "Hit Speed";
            public const string LABEL_ARMOR = "Armor";
            public const string LABEL_EVASION_CHANCE = "Evasion %";

            public const string LABEL_FIRE_DAMAGE = "Fire Damage";
            public const string LABEL_FIRE_RESISTANCE = "Fire Resistance";
            public const string LABEL_FIRE_EFFECT_MULTIPLIER = "Fire Multiplier";

            public const string LABEL_STORM_DAMAGE = "Storm Damage";
            public const string LABEL_STORM_RESISTANCE = "Storm Resistance";
            public const string LABEL_STORM_EFFECT_MULTIPLIER = "Storm Multiplier";

            public const string LABEL_LIGHT_DAMAGE = "Light Damage";
            public const string LABEL_LIGHT_RESISTANCE = "Light Resistance";
            public const string LABEL_LIGHT_EFFECT_MULTIPLIER = "Light Multiplier";

            public const string LABEL_SHADOW_DAMAGE = "Shadow Damage";
            public const string LABEL_SHADOW_RESISTANCE = "Shadow Resistance";
            public const string LABEL_SHADOW_EFFECT_MULTIPLIER = "Shadow Multiplier";

            public const string LABEL_ICE_DAMAGE = "Ice Damage";
            public const string LABEL_ICE_RESISTANCE = "Ice Resistance";
            public const string LABEL_ICE_EFFECT_MULTIPLIER = "Ice Multiplier";

            [Header("Health")]
            public float health;
            public float healthPercent;

            [Header("Damage")]
            public float damage;
            public float damagePercent;

            private MagicStat baseDamage = new MagicStat(DamageType.Base);

            [Space]
            public float critChance;
            public float critDamagePercent;

            [Header("Speed")]
            public float movementSpeed;
            public float attackSpeed;

            [Header("Protection")]
            public float armor;
            public float evasionChance;

            [Header("Magic")]
            public MagicStat fireDamage = new MagicStat(DamageType.Fire);
            public MagicStat fireResistance = new MagicStat(DamageType.Fire);
            public MagicStat fireEffectMultiplier = new MagicStat(DamageType.Fire);

            [Space]
            public MagicStat stormDamage = new MagicStat(DamageType.Storm);
            public MagicStat stormResistance = new MagicStat(DamageType.Storm);
            public MagicStat stormEffectMultiplier = new MagicStat(DamageType.Storm);

            [Space]
            public MagicStat shadowDamage = new MagicStat(DamageType.Shadow);
            public MagicStat shadowResistance = new MagicStat(DamageType.Shadow);
            public MagicStat shadowEffectMultiplier = new MagicStat(DamageType.Shadow);

            [Space]
            public MagicStat iceDamage = new MagicStat(DamageType.Ice);
            public MagicStat iceResistance = new MagicStat(DamageType.Ice);
            public MagicStat iceEffectMultiplier = new MagicStat(DamageType.Ice);

            public float GetTotalHealth
            {
                get { return health + health * (healthPercent / 100); }
            }

            public float GetTotalDamage
            {
                get { return damage + damage * (damagePercent / 100); }
            }

            public Stats()
            {
                Reset();
            }

            public Stats(Stats stats)
            {
                health = stats.health;
                healthPercent = stats.healthPercent;

                damage = stats.damage;
                damagePercent = stats.damagePercent;

                baseDamage.SetValue(damage + damage * damagePercent);

                critChance = stats.critChance;
                critDamagePercent = stats.critDamagePercent;

                movementSpeed = stats.movementSpeed;

                attackSpeed = stats.attackSpeed;

                armor = stats.armor;
                evasionChance = stats.evasionChance;

                fireDamage = stats.fireDamage;
                fireResistance = stats.fireResistance;
                fireEffectMultiplier = stats.fireEffectMultiplier;

                stormDamage = stats.stormDamage;
                stormResistance = stats.stormResistance;
                stormEffectMultiplier = stats.stormEffectMultiplier;

                shadowDamage = stats.shadowDamage;
                shadowResistance = stats.shadowResistance;
                shadowEffectMultiplier = stats.shadowEffectMultiplier;

                iceDamage = stats.iceDamage;
                iceResistance = stats.iceResistance;
                iceEffectMultiplier = stats.iceEffectMultiplier;
            }

            public void Reset()
            {
                health = 0;
                healthPercent = 0;

                damage = 0;
                damagePercent = 0;

                baseDamage.SetValue(0);

                critChance = 0;
                critDamagePercent = 0;

                movementSpeed = 0;

                attackSpeed = 0;

                armor = 0;
                evasionChance = 0;

                fireDamage.SetValue(0);
                fireResistance.SetValue(0);
                fireEffectMultiplier.SetValue(0);

                stormDamage.SetValue(0);
                stormResistance.SetValue(0);
                stormEffectMultiplier.SetValue(0);

                shadowDamage.SetValue(0);
                shadowResistance.SetValue(0);
                shadowEffectMultiplier.SetValue(0);

                iceDamage.SetValue(0);
                iceResistance.SetValue(0);
                iceEffectMultiplier.SetValue(0);
            }

            private void RoundValues()
            {
                health = Mathf.RoundToInt(health);
                healthPercent = Mathf.RoundToInt(healthPercent);

                damage = Mathf.RoundToInt(damage);
                damagePercent = Mathf.RoundToInt(damagePercent);

                baseDamage.SetValue(Mathf.RoundToInt(baseDamage.Value));

                critChance = Mathf.RoundToInt(critChance);
                critDamagePercent = Mathf.RoundToInt(critDamagePercent);

                movementSpeed = Mathf.RoundToInt(movementSpeed);

                attackSpeed = Mathf.RoundToInt(attackSpeed);

                armor = Mathf.RoundToInt(armor);
                evasionChance = Mathf.RoundToInt(evasionChance);

                fireDamage.SetValue(Mathf.RoundToInt(fireDamage.Value));
                fireResistance.SetValue(Mathf.RoundToInt(fireResistance.Value));
                fireEffectMultiplier.SetValue(Mathf.RoundToInt(fireEffectMultiplier.Value));

                stormDamage.SetValue(Mathf.RoundToInt(stormDamage.Value));
                stormResistance.SetValue(Mathf.RoundToInt(stormResistance.Value));
                stormEffectMultiplier.SetValue(Mathf.RoundToInt(stormEffectMultiplier.Value));

                shadowDamage.SetValue(Mathf.RoundToInt(shadowDamage.Value));
                shadowResistance.SetValue(Mathf.RoundToInt(shadowResistance.Value));
                shadowEffectMultiplier.SetValue(Mathf.RoundToInt(shadowEffectMultiplier.Value));

                iceDamage.SetValue(Mathf.RoundToInt(iceDamage.Value));
                iceResistance.SetValue(Mathf.RoundToInt(iceResistance.Value));
                iceEffectMultiplier.SetValue(Mathf.RoundToInt(iceEffectMultiplier.Value));
            }

            public void AddStats(Stats stats)
            {
                health += stats.health;
                healthPercent += stats.healthPercent;

                damage += stats.damage;
                damagePercent += stats.damagePercent;

                baseDamage.SetValue(damage + damage * damagePercent);

                critChance += stats.critChance;
                critDamagePercent += stats.critDamagePercent;

                movementSpeed += stats.movementSpeed;

                attackSpeed += stats.attackSpeed;

                armor += stats.armor;
                evasionChance += stats.evasionChance;

                fireDamage.AddValue(stats.fireDamage.Value);
                fireResistance.AddValue(stats.fireResistance.Value);
                fireEffectMultiplier.AddValue(stats.fireEffectMultiplier.Value);

                stormDamage.AddValue(stats.stormDamage.Value);
                stormResistance.AddValue(stats.stormResistance.Value);
                stormEffectMultiplier.AddValue(stats.stormEffectMultiplier.Value);

                shadowDamage.AddValue(stats.shadowDamage.Value);
                shadowResistance.AddValue(stats.shadowResistance.Value);
                shadowEffectMultiplier.AddValue(stats.shadowEffectMultiplier.Value);

                iceDamage.AddValue(stats.iceDamage.Value);
                iceResistance.AddValue(stats.iceResistance.Value);
                iceEffectMultiplier.AddValue(stats.iceEffectMultiplier.Value);

                RoundValues();
            }

            public void MultiplyStats(float multiplier)
            {
                health = health + (health * multiplier);
                healthPercent = healthPercent + (healthPercent * multiplier);

                damage = damage + (damage * multiplier);
                damagePercent = damagePercent + (damagePercent * multiplier);

                baseDamage.SetValue(damage + damage * damagePercent);

                critChance = critChance + (critChance * multiplier);
                critDamagePercent = critDamagePercent + (critDamagePercent * multiplier);

                movementSpeed = movementSpeed + (movementSpeed * multiplier);

                attackSpeed = attackSpeed + (attackSpeed * multiplier);

                armor = armor + (armor * multiplier);
                evasionChance = evasionChance + (evasionChance * multiplier);

                fireDamage.SetValue(fireDamage.Value + (fireDamage.Value * multiplier));
                fireResistance.SetValue(fireResistance.Value + (fireResistance.Value * multiplier));
                fireEffectMultiplier.SetValue(fireEffectMultiplier.Value + (fireEffectMultiplier.Value * multiplier));

                stormDamage.SetValue(stormDamage.Value + (stormDamage.Value * multiplier));
                stormResistance.SetValue(stormResistance.Value + (stormResistance.Value * multiplier));
                stormEffectMultiplier.SetValue(stormEffectMultiplier.Value + (stormEffectMultiplier.Value * multiplier));

                shadowDamage.SetValue(shadowDamage.Value + (shadowDamage.Value * multiplier));
                shadowResistance.SetValue(shadowResistance.Value + (shadowResistance.Value * multiplier));
                shadowEffectMultiplier.SetValue(shadowEffectMultiplier.Value + (shadowEffectMultiplier.Value * multiplier));

                iceDamage.SetValue(iceDamage.Value + (iceDamage.Value * multiplier));
                iceResistance.SetValue(iceResistance.Value + (iceResistance.Value * multiplier));
                iceEffectMultiplier.SetValue(iceEffectMultiplier.Value + (iceEffectMultiplier.Value * multiplier));

                RoundValues();
            }

            public StringBuilder FormatStats()
            {
                StringBuilder stringBuilder = new StringBuilder();

                if (health > 0)
                    stringBuilder.AppendLine(string.Format("{0}: {1}", LABEL_HEALTH, health));
                if (healthPercent > 0)
                    stringBuilder.AppendLine(string.Format("{0}: {1}", LABEL_HEALTH_PERCENT, healthPercent));

                if (damage > 0)
                    stringBuilder.AppendLine(string.Format("{0}: {1}", LABEL_DAMAGE, damage));
                if (damagePercent > 0)
                    stringBuilder.AppendLine(string.Format("{0}: {1}", LABEL_DAMAGE_PERCENT, damage));

                if (critChance > 0)
                    stringBuilder.AppendLine(string.Format("{0}: {1}", LABEL_CRIT_CHANCE, critChance));
                if (critDamagePercent > 0)
                    stringBuilder.AppendLine(string.Format("{0}: {1}", LABEL_CRIT_DAMAGE_PERCENT, critDamagePercent));

                if (movementSpeed > 0)
                    stringBuilder.AppendLine(string.Format("{0}: {1}", LABEL_MOVEMENT_SPEED, movementSpeed));

                if (attackSpeed > 0)
                    stringBuilder.AppendLine(string.Format("{0}: {1}", LABEL_ATTACK_SPEED, attackSpeed));

                if (armor > 0)
                    stringBuilder.AppendLine(string.Format("{0}: {1}", LABEL_ARMOR, armor));
                if (evasionChance > 0)
                    stringBuilder.AppendLine(string.Format("{0}: {1}", LABEL_EVASION_CHANCE, evasionChance));

                if (fireDamage.Value > 0)
                    stringBuilder.AppendLine(string.Format("{0}: {1}", LABEL_FIRE_DAMAGE, fireDamage.Value));
                if (fireResistance.Value > 0)
                    stringBuilder.AppendLine(string.Format("{0}: {1}", LABEL_FIRE_RESISTANCE, fireResistance.Value));
                if (fireEffectMultiplier.Value > 0)
                    stringBuilder.AppendLine(string.Format("{0}: {1}", LABEL_FIRE_EFFECT_MULTIPLIER, fireEffectMultiplier.Value));

                if (stormDamage.Value > 0)
                    stringBuilder.AppendLine(string.Format("{0}: {1}", LABEL_STORM_DAMAGE, stormDamage.Value));
                if (stormResistance.Value > 0)
                    stringBuilder.AppendLine(string.Format("{0}: {1}", LABEL_STORM_RESISTANCE, stormResistance.Value));
                if (stormEffectMultiplier.Value > 0)
                    stringBuilder.AppendLine(string.Format("{0}: {1}", LABEL_STORM_EFFECT_MULTIPLIER, stormEffectMultiplier.Value));

                if (shadowDamage.Value > 0)
                    stringBuilder.AppendLine(string.Format("{0}: {1}", LABEL_SHADOW_DAMAGE, shadowDamage.Value));
                if (shadowResistance.Value > 0)
                    stringBuilder.AppendLine(string.Format("{0}: {1}", LABEL_SHADOW_RESISTANCE, shadowResistance.Value));
                if (shadowEffectMultiplier.Value > 0)
                    stringBuilder.AppendLine(string.Format("{0}: {1}", LABEL_SHADOW_EFFECT_MULTIPLIER, shadowEffectMultiplier.Value));

                if (iceDamage.Value > 0)
                    stringBuilder.AppendLine(string.Format("{0}: {1}", LABEL_ICE_DAMAGE, iceDamage.Value));
                if (iceResistance.Value > 0)
                    stringBuilder.AppendLine(string.Format("{0}: {1}", LABEL_ICE_RESISTANCE, iceResistance.Value));
                if (iceEffectMultiplier.Value > 0)
                    stringBuilder.AppendLine(string.Format("{0}: {1}", LABEL_ICE_EFFECT_MULTIPLIER, iceEffectMultiplier.Value));

                return stringBuilder;
            }

            public StringBuilder FormatCompareStats(Stats comparedStats)
            {
                StringBuilder stringBuilder = new StringBuilder();

                float valueDiff = 0;

                if (health > 0)
                {
                    valueDiff = health - comparedStats.health;

                    stringBuilder.Append(string.Format("{0}: {1}", LABEL_HEALTH, health));
                    if (valueDiff != 0)
                        FormatValueDiff(valueDiff, ref stringBuilder);

                    stringBuilder.AppendLine();
                }

                if (healthPercent > 0)
                {
                    valueDiff = healthPercent - comparedStats.healthPercent;

                    stringBuilder.Append(string.Format("{0}: {1}", LABEL_HEALTH_PERCENT, healthPercent));
                    if (valueDiff != 0)
                        FormatValueDiff(valueDiff, ref stringBuilder);

                    stringBuilder.AppendLine();
                }

                if (damage > 0)
                {
                    valueDiff = damage - comparedStats.damage;

                    stringBuilder.Append(string.Format("{0}: {1}", LABEL_DAMAGE, damage));
                    if (valueDiff != 0)
                        FormatValueDiff(valueDiff, ref stringBuilder);

                    stringBuilder.AppendLine();
                }

                if (damagePercent > 0)
                {
                    valueDiff = damagePercent - comparedStats.damagePercent;

                    stringBuilder.Append(string.Format("{0}: {1}", LABEL_DAMAGE_PERCENT, damagePercent));
                    if (valueDiff != 0)
                        FormatValueDiff(valueDiff, ref stringBuilder);

                    stringBuilder.AppendLine();
                }

                if (critChance > 0)
                {
                    valueDiff = critChance - comparedStats.critChance;

                    stringBuilder.Append(string.Format("{0}: {1}", LABEL_CRIT_CHANCE, critChance));
                    if (valueDiff != 0)
                        FormatValueDiff(valueDiff, ref stringBuilder);

                    stringBuilder.AppendLine();
                }

                if (critDamagePercent > 0)
                {
                    valueDiff = critDamagePercent - comparedStats.critDamagePercent;

                    stringBuilder.Append(string.Format("{0}: {1}", LABEL_CRIT_DAMAGE_PERCENT, critDamagePercent));
                    if (valueDiff != 0)
                        FormatValueDiff(valueDiff, ref stringBuilder);

                    stringBuilder.AppendLine();
                }

                if (movementSpeed > 0)
                {
                    valueDiff = movementSpeed - comparedStats.movementSpeed;

                    stringBuilder.Append(string.Format("{0}: {1}", LABEL_MOVEMENT_SPEED, movementSpeed));
                    if (valueDiff != 0)
                        FormatValueDiff(valueDiff, ref stringBuilder);

                    stringBuilder.AppendLine();
                }

                if (attackSpeed > 0)
                {
                    valueDiff = attackSpeed - comparedStats.attackSpeed;

                    stringBuilder.Append(string.Format("{0}: {1}", LABEL_ATTACK_SPEED, attackSpeed));
                    if (valueDiff != 0)
                        FormatValueDiff(valueDiff, ref stringBuilder);

                    stringBuilder.AppendLine();
                }

                if (armor > 0)
                {
                    valueDiff = armor - comparedStats.armor;

                    stringBuilder.Append(string.Format("{0}: {1}", LABEL_ARMOR, armor));
                    if (valueDiff != 0)
                        FormatValueDiff(valueDiff, ref stringBuilder);

                    stringBuilder.AppendLine();
                }

                if (evasionChance > 0)
                {
                    valueDiff = evasionChance - comparedStats.evasionChance;

                    stringBuilder.Append(string.Format("{0}: {1}", LABEL_EVASION_CHANCE, evasionChance));
                    if (valueDiff != 0)
                        FormatValueDiff(valueDiff, ref stringBuilder);

                    stringBuilder.AppendLine();
                }

                // Fire
                if (fireDamage.Value > 0)
                {
                    valueDiff = fireDamage.Value - comparedStats.fireDamage.Value;

                    stringBuilder.Append(string.Format("{0}: {1}", LABEL_FIRE_DAMAGE, fireDamage.Value));
                    if (valueDiff != 0)
                        FormatValueDiff(valueDiff, ref stringBuilder);

                    stringBuilder.AppendLine();
                }
                if (fireResistance.Value > 0)
                {
                    valueDiff = fireResistance.Value - comparedStats.fireResistance.Value;

                    stringBuilder.Append(string.Format("{0}: {1}", LABEL_FIRE_RESISTANCE, fireResistance.Value));
                    if (valueDiff != 0)
                        FormatValueDiff(valueDiff, ref stringBuilder);

                    stringBuilder.AppendLine();
                }
                if (fireEffectMultiplier.Value > 0)
                {
                    valueDiff = fireEffectMultiplier.Value - comparedStats.fireEffectMultiplier.Value;

                    stringBuilder.Append(string.Format("{0}: {1}", LABEL_FIRE_EFFECT_MULTIPLIER, fireEffectMultiplier.Value));
                    if (valueDiff != 0)
                        FormatValueDiff(valueDiff, ref stringBuilder);

                    stringBuilder.AppendLine();
                }

                // Storm
                if (stormDamage.Value > 0)
                {
                    valueDiff = stormDamage.Value - comparedStats.stormDamage.Value;

                    stringBuilder.Append(string.Format("{0}: {1}", LABEL_STORM_DAMAGE, stormDamage.Value));
                    if (valueDiff != 0)
                        FormatValueDiff(valueDiff, ref stringBuilder);

                    stringBuilder.AppendLine();
                }
                if (stormResistance.Value > 0)
                {
                    valueDiff = stormResistance.Value - comparedStats.stormResistance.Value;

                    stringBuilder.Append(string.Format("{0}: {1}", LABEL_STORM_RESISTANCE, stormResistance.Value));
                    if (valueDiff != 0)
                        FormatValueDiff(valueDiff, ref stringBuilder);

                    stringBuilder.AppendLine();
                }
                if (stormEffectMultiplier.Value > 0)
                {
                    valueDiff = stormEffectMultiplier.Value - comparedStats.stormEffectMultiplier.Value;

                    stringBuilder.Append(string.Format("{0}: {1}", LABEL_STORM_EFFECT_MULTIPLIER, stormEffectMultiplier.Value));
                    if (valueDiff != 0)
                        FormatValueDiff(valueDiff, ref stringBuilder);

                    stringBuilder.AppendLine();
                }

                // Shadow
                if (shadowDamage.Value > 0)
                {
                    valueDiff = shadowDamage.Value - comparedStats.shadowDamage.Value;

                    stringBuilder.Append(string.Format("{0}: {1}", LABEL_SHADOW_DAMAGE, shadowDamage.Value));
                    if (valueDiff != 0)
                        FormatValueDiff(valueDiff, ref stringBuilder);

                    stringBuilder.AppendLine();
                }
                if (shadowResistance.Value > 0)
                {
                    valueDiff = shadowResistance.Value - comparedStats.shadowResistance.Value;

                    stringBuilder.Append(string.Format("{0}: {1}", LABEL_SHADOW_RESISTANCE, shadowResistance.Value));
                    if (valueDiff != 0)
                        FormatValueDiff(valueDiff, ref stringBuilder);

                    stringBuilder.AppendLine();
                }
                if (shadowEffectMultiplier.Value > 0)
                {
                    valueDiff = shadowEffectMultiplier.Value - comparedStats.shadowEffectMultiplier.Value;

                    stringBuilder.Append(string.Format("{0}: {1}", LABEL_SHADOW_EFFECT_MULTIPLIER, shadowEffectMultiplier.Value));
                    if (valueDiff != 0)
                        FormatValueDiff(valueDiff, ref stringBuilder);

                    stringBuilder.AppendLine();
                }

                // Ice
                if (iceDamage.Value > 0)
                {
                    valueDiff = iceDamage.Value - comparedStats.iceDamage.Value;

                    stringBuilder.Append(string.Format("{0}: {1}", LABEL_ICE_DAMAGE, iceDamage.Value));
                    if (valueDiff != 0)
                        FormatValueDiff(valueDiff, ref stringBuilder);

                    stringBuilder.AppendLine();
                }
                if (iceResistance.Value > 0)
                {
                    valueDiff = iceResistance.Value - comparedStats.iceResistance.Value;

                    stringBuilder.Append(string.Format("{0}: {1}", LABEL_ICE_RESISTANCE, iceResistance.Value));
                    if (valueDiff != 0)
                        FormatValueDiff(valueDiff, ref stringBuilder);

                    stringBuilder.AppendLine();
                }
                if (iceEffectMultiplier.Value > 0)
                {
                    valueDiff = iceEffectMultiplier.Value - comparedStats.iceEffectMultiplier.Value;

                    stringBuilder.Append(string.Format("{0}: {1}", LABEL_ICE_EFFECT_MULTIPLIER, iceEffectMultiplier.Value));
                    if (valueDiff != 0)
                        FormatValueDiff(valueDiff, ref stringBuilder);

                    stringBuilder.AppendLine();
                }

                return stringBuilder;
            }

            private void FormatValueDiff(float diffValue, ref StringBuilder stringBuilder)
            {
                stringBuilder.Append(string.Format(" (<color='{0}'>{1}</color>)", diffValue > 0 ? ItemSettings.COMPARE_GREEN_COLOR : ItemSettings.COMPARE_RED_COLOR, diffValue > 0 ? "+" + diffValue : diffValue.ToString()));
            }

            public Damage GetBaseDamage()
            {
                return new Damage() { type = DamageType.Base, amount = baseDamage.Value, isCrit = false };
            }

            public Damage[] GetDamages()
            {
                Damage[] damages = new Damage[]
                {
                    new Damage() { type = DamageType.Base, amount = baseDamage.Value, isCrit = false },
                    new Damage() { type = DamageType.Fire, amount = fireDamage.Value, isCrit = false },
                    new Damage() { type = DamageType.Storm, amount = stormDamage.Value, isCrit = false },
                    new Damage() { type = DamageType.Shadow, amount = shadowDamage.Value, isCrit = false },
                    new Damage() { type = DamageType.Ice, amount = iceDamage.Value, isCrit = false }
                };

                if (UnityEngine.Random.Range(0, 100) < critChance)
                {
                    Debug.Log("CRIT");

                    damages[0].isCrit = true;
                    damages[0].amount *= 3;
                }

                return damages;
            }
        }

        [System.Serializable]
        public class Equipment 
        {
            public const int EQUIPED_SLOTS_COUNT = 5; // Change this value if you want to add or remove equipable slots

            public WeaponItemHolder weapon = null;
            public ArmorItemHolder armor = null;
            public MagicBookItemHolder magicBook = null;
            public RingItemHolder ring = null;
            public NecklacesItemHolder necklaces = null;

            public Equipment()
            {

            }

            public Equipment(Equipment equipment)
            {
                weapon = equipment.weapon;
                armor = equipment.armor;
                magicBook = equipment.magicBook;
                ring = equipment.ring;
                necklaces = equipment.necklaces;
            }
        }
    }
}
