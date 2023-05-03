using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    public class CharacterPreviewWindow : GameMenuWindow
    {
        private static CharacterPreviewWindow characterPreviewWindow;

        [SerializeField] Image background;
        [SerializeField] Transform panel;

        [Space]
        [SerializeField] Text statsText;

        [Space]
        [SerializeField] Text upgradeItemText;
        [SerializeField] Text upgradePriceText;

        [SerializeField] Button upgradeButton;

        private SkinData.Upgrade nextUpgrade;

        private void Awake()
        {
            characterPreviewWindow = this;
        }

        private void OnEnable()
        {
            Character.OnLevelUp += OnLevelUp;
        }

        private void OnDisable()
        {
            Character.OnLevelUp -= OnLevelUp;
        }

        private void DisplayCharacter()
        {
            Character.Stats characterStats = Character.GetCharacterStats();

            nextUpgrade = Character.GetNextUpgrade();

            StringBuilder statsStringBuilder = new StringBuilder();
            statsStringBuilder.AppendLine(string.Format("Health: {0}{1}", characterStats.health.ToString("0.##"), (nextUpgrade.health > 0 ? string.Format(" (<color='{0}'>+{1}</color>)", ItemSettings.COMPARE_GREEN_COLOR, nextUpgrade.health) : string.Empty)));
            statsStringBuilder.AppendLine(string.Format("Damage: {0}{1}", characterStats.damage.ToString("0.##"), (nextUpgrade.damage > 0 ? string.Format(" (<color='{0}'>+{1}</color>)", ItemSettings.COMPARE_GREEN_COLOR, nextUpgrade.damage) : string.Empty)));
            //statsStringBuilder.AppendLine(string.Format("Attack Speed: {0}{1}", characterStats.attackSpeed, (nextUpgrade.AttackSpeedStep > 0 ? string.Format(" (<color='{0}'>+{1}</color>)", ItemSettings.COMPARE_GREEN_COLOR, nextUpgrade.AttackSpeedStep) : string.Empty)));
            //statsStringBuilder.AppendLine(string.Format("Running Speed: {0}{1}", characterStats.movementSpeed, (nextUpgrade.MovementSpeedStep > 0 ? string.Format(" (<color='{0}'>+{1}</color>)", ItemSettings.COMPARE_GREEN_COLOR, nextUpgrade.MovementSpeedStep) : string.Empty)));
            //statsStringBuilder.AppendLine(string.Format("Crit Chance: {0}{1}", characterStats.critChance, (nextUpgrade.CritChanceStep > 0 ? string.Format(" (<color='{0}'>+{1}</color>)", ItemSettings.COMPARE_GREEN_COLOR, nextUpgrade.CritChanceStep) : string.Empty)));
            //statsStringBuilder.AppendLine(string.Format("Armor: {0}{1}", characterStats.armor, (nextUpgrade.CritChanceStep > 0 ? string.Format(" (<color='{0}'>+{1}</color>)", ItemSettings.COMPARE_GREEN_COLOR, nextUpgrade.CritChanceStep ) : string.Empty)));

            statsText.text = statsStringBuilder.ToString();

            int upgradeItemAmount = Inventory.GetResourceAmount(ItemSettings.GetChracterUpgradeItem());

            upgradeItemText.text = string.Format("{0}/{1}", upgradeItemAmount, nextUpgrade.RequiredItemsCount);
            upgradePriceText.text = nextUpgrade.Cost.ToString();

            upgradeButton.interactable = Currency.Coins >= nextUpgrade.Cost && Inventory.HasResource(ItemSettings.GetChracterUpgradeItem(), nextUpgrade.RequiredItemsCount) && !Character.IsMaxLevel();
        }

        private void OnLevelUp(int newLevel, SkinData.Upgrade upgrade)
        {
            DisplayCharacter();
        }

        protected override void OpenAnimation()
        {
            AudioController.PlaySound(AudioController.Settings.sounds.windowOpenSound);

            background.gameObject.SetActive(true);

            panel.localScale = Vector3.zero;
            panel.DOScale(1, 0.2f).SetEasing(Ease.Type.CubicOut);
        }

        protected override void CloseAnimation()
        {
            AudioController.PlaySound(AudioController.Settings.sounds.windowCloseSound);

            background.gameObject.SetActive(false);
        }

        public void LevelUpButton()
        {
            Character.UpgradeLevel();
        }

        public void CloseButton()
        {
            GameMenuWindow.HideWindow(this);
        }

        public static void CharacterPreview()
        {
            if (characterPreviewWindow != null)
            {
                characterPreviewWindow.DisplayCharacter();

                GameMenuWindow.ShowWindow<CharacterPreviewWindow>(characterPreviewWindow);
            }
        }
    }
}
