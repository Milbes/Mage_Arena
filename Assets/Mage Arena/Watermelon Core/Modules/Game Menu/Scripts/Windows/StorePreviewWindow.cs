using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Watermelon
{
    public class StorePreviewWindow : GameMenuWindow
    {
        private const int WINDOW_HEIGHT = 520;

        private static StorePreviewWindow storePreviewWindow;

        [SerializeField] Image background;
        [SerializeField] CanvasGroup panelCanvasGroup;

        [Header("Store Item")]
        [SerializeField] RectTransform equipedItemPanel;

        [SerializeField] TextMeshProUGUI equipedItemNameText;
        [SerializeField] TextMeshProUGUI equipedItemNameShadowText;

        [SerializeField] RectTransform itemSlotContainer;
        [SerializeField] Image itemIconImage;
        [SerializeField] Image itemBackgroundImage;
        [SerializeField] Image itemFrameImage;

        [Space]
        [SerializeField] Image itemRarityImage;

        [Space]
        [SerializeField] Image itemShiningImage;

        [Space]
        [SerializeField] GameObject itemStatsContainer;
        [SerializeField] RectTransform itemStatsPanel;
        [SerializeField] Text itemStatsText;

        [Space]
        [SerializeField] GameObject priceButton;
        [SerializeField] GameObject disabledButton;

        [SerializeField] Image currencyImage;
        [SerializeField] Image currencyDisabledImage;

        [Space]
        [SerializeField] TextMeshProUGUI costText;
        [SerializeField] TextMeshProUGUI costShadowText;

        [Space]
        [SerializeField] TextMeshProUGUI costDisabledText;
        [SerializeField] TextMeshProUGUI costDisabledShadowText;

        [Header("Preview Animation")]
        [SerializeField] RectTransform previewContainer;
        [SerializeField] Image previewIconImage;
        [SerializeField] Image previewBackgroundImage;
        [SerializeField] Image previewFrameImage;
        [SerializeField] Image previewShiningImage;

        [Space]
        [SerializeField] Vector2 panelPadding;

        private bool preview;

        private ItemHolder currentItemHolder;

        // Store preview
        private int cost;
        private Currencies currency;

        private void Awake()
        {
            storePreviewWindow = this;
        }

        private void DisplayItem(ItemHolder itemHolder)
        {
            currentItemHolder = itemHolder;

            Item item = itemHolder.Item;

            if (Currency.EnoughMoney(currency, cost))
            {
                currencyImage.sprite = Currency.GetCurrencyIcon(currency, true);

                disabledButton.SetActive(false);
                priceButton.SetActive(true);

                costText.text = cost.ToString();
                costShadowText.text = costText.text;
            }
            else
            {
                currencyDisabledImage.sprite = Currency.GetCurrencyIcon(currency, false);

                priceButton.SetActive(false);
                disabledButton.SetActive(true);

                costDisabledText.text = cost.ToString();
                costDisabledShadowText.text = costDisabledText.text;
            }

            ItemHolder equipedItem = Character.GetEquipedItem(item.EquipableItemType);
            itemStatsText.text = equipedItem != null ? itemHolder.GetCompareStatsString(equipedItem) : itemHolder.GetStatsString();

            // Resize window
            if (!string.IsNullOrEmpty(itemStatsText.text))
            {
                itemStatsContainer.SetActive(true);

                Tween.NextFrame(delegate
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(itemStatsText.rectTransform);

                    itemStatsPanel.sizeDelta = itemStatsText.rectTransform.sizeDelta + panelPadding;

                    equipedItemPanel.sizeDelta = new Vector2(equipedItemPanel.sizeDelta.x, WINDOW_HEIGHT + itemStatsPanel.sizeDelta.y);
                });
            }
            else
            {
                itemStatsContainer.SetActive(false);

                equipedItemPanel.sizeDelta = new Vector2(equipedItemPanel.sizeDelta.x, WINDOW_HEIGHT);
            }

            equipedItemNameText.text = item.ItemName;
            equipedItemNameShadowText.text = equipedItemNameText.text;

            itemIconImage.sprite = item.Sprite;

            ItemSettings.RaritySettings raritySettings = itemHolder.RaritySettings;
            itemBackgroundImage.sprite = raritySettings.SlotBackground;
            itemFrameImage.sprite = raritySettings.SlotFrame;
            itemShiningImage.sprite = raritySettings.WindowShining;
            itemRarityImage.sprite = raritySettings.TitleImage;

            previewShiningImage.color = raritySettings.Color;

            itemSlotContainer.gameObject.SetActive(true);
            previewContainer.gameObject.SetActive(false);
        }

        protected override void OpenAnimation()
        {
            AudioController.PlaySound(AudioController.Settings.sounds.windowOpenSound);

            background.gameObject.SetActive(true);
            panelCanvasGroup.alpha = 1;

            equipedItemPanel.gameObject.SetActive(true);
            equipedItemPanel.transform.localScale = Vector3.zero;
            equipedItemPanel.transform.DOScale(1, 0.2f).SetEasing(Ease.Type.CubicOut);

            itemShiningImage.color = itemShiningImage.color.SetAlpha(0.0f);
            itemShiningImage.DOFade(1, 0.8f);
        }

        protected override void CloseAnimation()
        {
            AudioController.PlaySound(AudioController.Settings.sounds.windowCloseSound);

            equipedItemPanel.gameObject.SetActive(false);
            background.gameObject.SetActive(false);

            currentItemHolder = null;
        }

        public void ActionButton()
        {
            if (preview)
                return;

            if (Currency.EnoughMoney(currency, cost))
            {
                if (currency == Currencies.Gold)
                {
                    Currency.ChangeCoins(-cost);
                } 
                else
                {
                    Currency.ChangeGems(-cost);
                }

                Inventory.AddItem(currentItemHolder);

                // Preview animation
                preview = true;

                panelCanvasGroup.DOFade(0, 0.4f);
                itemSlotContainer.gameObject.SetActive(false);
                previewContainer.gameObject.SetActive(true);
                previewContainer.transform.localScale = Vector3.one;

                previewContainer.transform.position = itemSlotContainer.transform.position;
                previewBackgroundImage.sprite = itemBackgroundImage.sprite;
                previewFrameImage.sprite = itemFrameImage.sprite;
                previewIconImage.sprite = itemIconImage.sprite;

                previewContainer.DOScale(1.5f, scaleTime).SetEasing(scaleEasing);
                previewContainer.DOAnchoredPosition(Vector3.zero, moveTime).SetEasing(anchoredEasing).OnComplete(delegate
                {

                });

                //previewShiningImage.transform.localScale = Vector3.zero;
                //previewShiningImage.transform.DOScale(1, 0.8f).SetEasing(Ease.Type.QuintOut);
            }
        }

        public float scaleTime = 0.5f;
        public Ease.Type scaleEasing;
        public float moveTime = 0.5f;
        public Ease.Type anchoredEasing;

        public void CloseButton()
        {
            GameMenuWindow.HideWindow(this);
        }

        public static void PreviewStoreItem(ItemHolder itemHolder, int cost, Currencies currency)
        {
            if (storePreviewWindow != null)
            {
                storePreviewWindow.cost = cost;
                storePreviewWindow.currency = currency;
                storePreviewWindow.preview = false;

                storePreviewWindow.DisplayItem(itemHolder);

                GameMenuWindow.ShowWindow<StorePreviewWindow>(storePreviewWindow);
            }
        }
    }
}
