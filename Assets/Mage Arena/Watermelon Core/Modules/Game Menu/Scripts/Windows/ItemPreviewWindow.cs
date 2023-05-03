using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    public class ItemPreviewWindow : GameMenuWindow
    {
        private const int WINDOW_ITEM_HEIGHT = 520;
        private const int WINDOW_MISC_HEIGHT = 415;

        private static ItemPreviewWindow itemPreviewWindow;

        [SerializeField] Image background;

        [Header("Equiped Item")]
        [SerializeField] RectTransform equipedItemPanel;

        [Space]
        [SerializeField] TextMeshProUGUI equipedItemNameText;
        [SerializeField] TextMeshProUGUI equipedItemNameShadowText;

        [Space]
        [SerializeField] Image equipedItemIconImage;
        [SerializeField] Image equipedItemBackgroundImage;
        [SerializeField] Image equipedItemFrameImage;

        [Space]
        [SerializeField] Image equipedItemRarityImage;

        [Space]
        [SerializeField] Image equipedItemShiningImage;

        [Space]
        [SerializeField] TextMeshProUGUI equipedItemActionText;
        [SerializeField] TextMeshProUGUI equipedItemActionShadowText;

        [Space]
        [SerializeField] GameObject equipedItemStatsContainer;
        [SerializeField] RectTransform equipedItemStatsPanel;
        [SerializeField] Text equipedItemStatsText;

        [Header("Resource Item")]
        [SerializeField] RectTransform miscItemPanel;

        [Space]
        [SerializeField] TextMeshProUGUI miscItemNameText;
        [SerializeField] TextMeshProUGUI miscItemNameShadowText;

        [Space]
        [SerializeField] Image miscItemIconImage;
        [SerializeField] Image miscItemBackgroundImage;
        [SerializeField] Image miscItemFrameImage;

        [Space]
        [SerializeField] Image miscItemShiningImage;

        [Space]
        [SerializeField] Text miscItemQuantityText;

        [Space]
        [SerializeField] GameObject miscItemDescriptionContainer;
        [SerializeField] RectTransform miscItemDescriptionPanel;
        [SerializeField] Text miscItemDescriptionText;

        [Space]
        [SerializeField] Vector2 panelPadding;

        private ItemHolder currentItemHolder;

        private WindowType windowType;

        private void Awake()
        {
            itemPreviewWindow = this;
        }

        private void DisplayItem(ItemHolder itemHolder)
        {
            currentItemHolder = itemHolder;

            Item item = itemHolder.Item;
            if (windowType == WindowType.Inventory || windowType == WindowType.Equipable)
            {
                if (windowType == WindowType.Equipable)
                {
                    equipedItemActionText.text = "UNEQUIP";
                    equipedItemActionShadowText.text = equipedItemActionText.text;

                    equipedItemStatsText.text = itemHolder.GetStatsString();
                }
                else if (windowType == WindowType.Inventory)
                {
                    equipedItemActionText.text = "EQUIP";
                    equipedItemActionShadowText.text = equipedItemActionText.text;

                    ItemHolder equipedItem = Character.GetEquipedItem(item.EquipableItemType);
                    equipedItemStatsText.text = equipedItem != null ? itemHolder.GetCompareStatsString(equipedItem) : itemHolder.GetStatsString();
                }

                // Resize window
                if(!string.IsNullOrEmpty(equipedItemStatsText.text))
                {
                    equipedItemStatsContainer.SetActive(true);

                    Tween.NextFrame(delegate
                    {
                        LayoutRebuilder.ForceRebuildLayoutImmediate(equipedItemStatsText.rectTransform);

                        equipedItemStatsPanel.sizeDelta = equipedItemStatsText.rectTransform.sizeDelta + panelPadding;

                        equipedItemPanel.sizeDelta = new Vector2(equipedItemPanel.sizeDelta.x, WINDOW_ITEM_HEIGHT + equipedItemStatsPanel.sizeDelta.y);
                    });
                }
                else
                {
                    equipedItemStatsContainer.SetActive(false);

                    equipedItemPanel.sizeDelta = new Vector2(equipedItemPanel.sizeDelta.x, WINDOW_ITEM_HEIGHT);
                }

                ItemSettings.RaritySettings raritySettings = itemHolder.RaritySettings;

                equipedItemNameText.text = item.ItemName;
                equipedItemNameShadowText.text = equipedItemNameText.text;

                equipedItemIconImage.sprite = item.Sprite;

                equipedItemBackgroundImage.sprite = raritySettings.SlotBackground;
                equipedItemFrameImage.sprite = raritySettings.SlotFrame;

                equipedItemShiningImage.sprite = raritySettings.WindowShining;
                equipedItemRarityImage.sprite = raritySettings.TitleImage;
            }
            else if (windowType == WindowType.Resource)
            {
                miscItemDescriptionText.text = item.Description;

                // Resize window
                if (!string.IsNullOrEmpty(miscItemDescriptionText.text))
                {
                    miscItemDescriptionContainer.SetActive(true);

                    Tween.NextFrame(delegate
                    {
                        LayoutRebuilder.ForceRebuildLayoutImmediate(miscItemDescriptionText.rectTransform);

                        miscItemDescriptionPanel.sizeDelta = miscItemDescriptionText.rectTransform.sizeDelta + new Vector2(15, 57);

                        miscItemPanel.sizeDelta = new Vector2(miscItemPanel.sizeDelta.x, WINDOW_MISC_HEIGHT + miscItemDescriptionPanel.sizeDelta.y);
                    });
                }
                else
                {
                    miscItemDescriptionContainer.SetActive(false);

                    miscItemPanel.sizeDelta = new Vector2(miscItemDescriptionPanel.sizeDelta.x, WINDOW_MISC_HEIGHT);
                }

                miscItemQuantityText.text = string.Format("Quantity: {0}", itemHolder.Amount);

                ItemSettings.RaritySettings raritySettings = itemHolder.RaritySettings;
                
                miscItemNameText.text = item.ItemName;
                miscItemNameShadowText.text = miscItemNameText.text;

                miscItemIconImage.sprite = item.Sprite;

                miscItemBackgroundImage.sprite = raritySettings.SlotBackground;
                miscItemFrameImage.sprite = raritySettings.SlotFrame;

                miscItemShiningImage.sprite = raritySettings.WindowShining;
            }
        }

        protected override void OpenAnimation()
        {
            AudioController.PlaySound(AudioController.Settings.sounds.windowOpenSound);

            background.gameObject.SetActive(true);

            Item item = currentItemHolder.Item;
            if (windowType == WindowType.Equipable || windowType == WindowType.Inventory)
            {
                equipedItemPanel.gameObject.SetActive(true);
                equipedItemPanel.transform.localScale = Vector3.zero;
                equipedItemPanel.transform.DOScale(1, 0.2f).SetEasing(Ease.Type.CubicOut);

                equipedItemShiningImage.color = equipedItemShiningImage.color.SetAlpha(0.0f);
                equipedItemShiningImage.DOFade(1, 0.8f);
            }
            else if(windowType == WindowType.Resource)
            {
                miscItemPanel.gameObject.SetActive(true);
                miscItemPanel.transform.localScale = Vector3.zero;
                miscItemPanel.transform.DOScale(1, 0.2f).SetEasing(Ease.Type.CubicOut);

                miscItemShiningImage.color = miscItemShiningImage.color.SetAlpha(0.0f);
                miscItemShiningImage.DOFade(1, 0.8f);
            }
        }

        protected override void CloseAnimation()
        {
            AudioController.PlaySound(AudioController.Settings.sounds.windowCloseSound);

            Item item = currentItemHolder.Item;
            if (windowType == WindowType.Equipable || windowType == WindowType.Inventory)
            {
                equipedItemPanel.gameObject.SetActive(false);
            }
            else if (windowType == WindowType.Resource)
            {
                miscItemPanel.gameObject.SetActive(false);
            }

            background.gameObject.SetActive(false);

            currentItemHolder = null;
        }

        public void ActionButton()
        {
            if(windowType == WindowType.Inventory)
            {
                EquipableItem equipableItemType = currentItemHolder.Item.EquipableItemType;
                if (equipableItemType != EquipableItem.None)
                {
                    Inventory.RemoveItem(currentItemHolder.InventoryID);

                    Character.EquipItem(currentItemHolder);
                }
            }
            else if(windowType == WindowType.Equipable)
            {
                EquipableItem equipableItemType = currentItemHolder.Item.EquipableItemType;
                if (equipableItemType != EquipableItem.None)
                {
                    Character.UnequipItem(equipableItemType);
                }
            }

            GameMenuWindow.HideWindow(this);
        }

        public void CloseButton()
        {
            GameMenuWindow.HideWindow(this);
        }

        public static void PreviewItem(ItemHolder itemHolder)
        {
            if (itemPreviewWindow != null)
            {
                itemPreviewWindow.windowType = WindowType.Inventory;
                itemPreviewWindow.DisplayItem(itemHolder);

                GameMenuWindow.ShowWindow<ItemPreviewWindow>(itemPreviewWindow);
            }
        }

        public static void PreviewResource(ItemHolder itemHolder)
        {
            if (itemPreviewWindow != null)
            {
                itemPreviewWindow.windowType = WindowType.Resource;
                itemPreviewWindow.DisplayItem(itemHolder);

                GameMenuWindow.ShowWindow<ItemPreviewWindow>(itemPreviewWindow);
            }
        }

        public static void PreviewEquipedItem(ItemHolder itemHolder)
        {
            if (itemPreviewWindow != null)
            {
                itemPreviewWindow.windowType = WindowType.Equipable;
                itemPreviewWindow.DisplayItem(itemHolder);

                GameMenuWindow.ShowWindow<ItemPreviewWindow>(itemPreviewWindow);
            }
        }

        public enum WindowType
        {
            Inventory = 0,
            Resource = 1,
            Equipable = 2
        }
    }
}
