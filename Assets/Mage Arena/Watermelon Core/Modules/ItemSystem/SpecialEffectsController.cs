using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    public class SpecialEffectsController : MonoBehaviour
    {
        private static SpecialEffectsController effectsController;

        private List<ItemSpecialEffect> itemSpecialEffects;
        private static List<ItemSpecialEffect> ItemSpecialEffects => effectsController.itemSpecialEffects;

        private void Awake()
        {
            effectsController = this;

            // Load special effects
            itemSpecialEffects = new List<ItemSpecialEffect>();

            // Get weapon effects
            GetItemSpecialEffects(Character.GetEquipedItem(EquipableItem.Weapon), ref itemSpecialEffects);

            // Get magic book effects
            GetItemSpecialEffects(Character.GetEquipedItem(EquipableItem.MagicBook), ref itemSpecialEffects);

            // Get ring effects
            GetItemSpecialEffects(Character.GetEquipedItem(EquipableItem.Ring), ref itemSpecialEffects);

            // Get armor effects
            GetItemSpecialEffects(Character.GetEquipedItem(EquipableItem.Armor), ref itemSpecialEffects);

            // Get necklaces effects
            GetItemSpecialEffects(Character.GetEquipedItem(EquipableItem.Necklaces), ref itemSpecialEffects);

        }

        private void OnDestroy()
        {
            for (int i = 0; i < effectsController.itemSpecialEffects.Count; i++)
            {
                effectsController.itemSpecialEffects[i].OnEffectDisabled();
            }
        }

        private void GetItemSpecialEffects(ItemHolder itemHolder, ref List<ItemSpecialEffect> itemSpecialEffectsList)
        {
            if (itemHolder != null)
            {
                IItemSpecialEffect itemSpecialEffects = itemHolder.Item as IItemSpecialEffect;

                if (!itemSpecialEffects.ItemSpecialEffects.IsNullOrEmpty())
                    itemSpecialEffectsList.AddRange(itemSpecialEffects.ItemSpecialEffects);
            }
        }

        public static void AddSpecialEffect(ItemSpecialEffect specialEffect)
        {
            if (!ItemSpecialEffects.Contains(specialEffect))
            {
                ItemSpecialEffects.Add(specialEffect);
            }
        }

        public static void RemoveSpecialEffect(ItemSpecialEffect specialEffect)
        {
            ItemSpecialEffects.Remove(specialEffect);
        }

        #region Callbacks
        public static void OnRoomStarted()
        {
            for(int i = 0; i < effectsController.itemSpecialEffects.Count; i++)
            {
                effectsController.itemSpecialEffects[i].OnRoomStarted();
            }
        }

        public static void OnLevelStarted()
        {
            for (int i = 0; i < effectsController.itemSpecialEffects.Count; i++)
            {
                effectsController.itemSpecialEffects[i].OnLevelStarted();
            }
        }

        public static void OnEnemyHitted(EnemyBehavior enemy, float damage, bool isCritical)
        {
            for (int i = 0; i < effectsController.itemSpecialEffects.Count; i++)
            {
                effectsController.itemSpecialEffects[i].OnEnemyHitted(enemy, damage, isCritical);
            }
        }

        public static void OnEnemyDies(EnemyBehavior enemy)
        {
            for (int i = 0; i < effectsController.itemSpecialEffects.Count; i++)
            {
                effectsController.itemSpecialEffects[i].OnEnemyDies(enemy);
            }
        }

        public static float OnPlayerHitted(float damage)
        {
            for (int i = 0; i < effectsController.itemSpecialEffects.Count; i++)
            {
                damage = effectsController.itemSpecialEffects[i].OnPlayerHitted(damage);
            }

            return damage;
        }
        #endregion
    }
}
