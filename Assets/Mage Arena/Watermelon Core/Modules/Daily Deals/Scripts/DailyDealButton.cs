using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Watermelon
{
    public class DailyDealButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] Image backgroundImage;

        [Space]
        [SerializeField] TextMeshProUGUI productTitleText;
        [SerializeField] TextMeshProUGUI productTitleShadowText;

        [Space]
        [SerializeField] Image productIconImage;
        [SerializeField] Image productBackgroundImage;
        [SerializeField] Image productFrameImage;

        [Space]
        [SerializeField] Image rarityImage;
        [SerializeField] Text productStatsText;

        [Space]
        [SerializeField] TextMeshProUGUI productPriceText;
        [SerializeField] TextMeshProUGUI productPriceShadowText;
        [SerializeField] Image productCurrencyImage;

        private RectTransform rectTransform;

        private DailyDealProduct dailyDealProduct;
        private ItemHolder dailyDealItemHolder;

        private int productPrice;

        private void Awake()
        {
            rectTransform = (RectTransform)transform;
        }

        public void Init(DailyDealProduct dailyDealProduct, DailyDealSave.Deal saveData)
        {
            this.dailyDealProduct = dailyDealProduct;

            Item item = dailyDealProduct.Item;
            dailyDealItemHolder = item.GetDefaultHolder();
            dailyDealItemHolder.SetItemLevel(saveData.ItemLevel);
            dailyDealItemHolder.SetItemRarity(saveData.ItemRarity);

            productIconImage.sprite = item.Sprite;
            
            productTitleText.text = item.ItemName;
            productTitleShadowText.text = productTitleText.text;

            ItemHolder equipedItem = Character.GetEquipedItem(item.EquipableItemType);
            productStatsText.text = equipedItem != null ? dailyDealItemHolder.GetCompareStatsString(equipedItem) : dailyDealItemHolder.GetStatsString();

            ItemSettings.RaritySettings raritySettings = dailyDealItemHolder.RaritySettings;
            backgroundImage.sprite = raritySettings.DailyDealBackground;
            rarityImage.sprite = raritySettings.TitleLeftImage;

            productBackgroundImage.sprite = raritySettings.SlotBackground;
            productFrameImage.sprite = raritySettings.SlotFrame;

            float scaledPrice = Mathf.Round(dailyDealProduct.Cost + (dailyDealProduct.Cost * raritySettings.DailyDealPriceCoefficient(dailyDealProduct.ProductCurrency)));
            
            productPrice = (Mathf.RoundToInt(scaledPrice / 10) * 10) /*- 1*/;

            productPriceText.text = productPrice.ToString();
            productPriceShadowText.text = productPriceText.text;

            productCurrencyImage.sprite = Currency.GetCurrencyIcon(dailyDealProduct.ProductCurrency);
        }

        public void ReinitStats()
        {
            Item item = dailyDealItemHolder.Item;

            ItemHolder equipedItem = Character.GetEquipedItem(item.EquipableItemType);
            productStatsText.text = equipedItem != null ? dailyDealItemHolder.GetCompareStatsString(equipedItem) : dailyDealItemHolder.GetStatsString();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            rectTransform.DOScaleX(1.05f, 0.15f);
            rectTransform.DOScaleY(1.05f, 0.2f);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            rectTransform.DOScaleX(1.0f, 0.15f);
            rectTransform.DOScaleY(1.0f, 0.2f);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            StorePreviewWindow.PreviewStoreItem(dailyDealItemHolder, productPrice, dailyDealProduct.ProductCurrency);
        }
    }
}