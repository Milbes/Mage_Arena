using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Watermelon
{
    public class ChestButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] int gemsAmount;
        [SerializeField] KeyType keyType;
        [SerializeField] ItemRarity allowedItemRarities;
        [SerializeField] string chestName;

        [Header("Refferences")]
        [SerializeField] GameObject buyButton;
        [SerializeField] GameObject disabledButton;
        [SerializeField] GameObject keyButton;

        [Space]
        [SerializeField] Image chestImage;

        [Space]
        [SerializeField] TextMeshProUGUI costText;
        [SerializeField] TextMeshProUGUI costShadowText;

        [Space]
        [SerializeField] TextMeshProUGUI costDisabledText;
        [SerializeField] TextMeshProUGUI costDisabledShadowText;

        [Space]
        [SerializeField] TextMeshProUGUI keysText;
        [SerializeField] TextMeshProUGUI keysShadowText;

        private RectTransform rectTransform;

        private bool isButtonActive = true;

        private int keyItemID;

        private void OnEnable()
        {
            Inventory.OnResourceStateChanged += OnItemAmountChanged;
            Inventory.OnNewResourceAdded += OnNewItemAdded;
            Inventory.OnResourceRemoved += OnItemRemoved;

            Currency.OnCurrencyUpdated += OnCurrencyUpdated;
        }

        private void OnDisable()
        {
            Inventory.OnResourceStateChanged -= OnItemAmountChanged;
            Inventory.OnNewResourceAdded -= OnNewItemAdded;
            Inventory.OnResourceRemoved -= OnItemRemoved;

            Currency.OnCurrencyUpdated -= OnCurrencyUpdated;
        }

        private void OnCurrencyUpdated(Currencies currency, int value, int oldValue, int valueDifference)
        {
            InitPanel();
        }

        private void OnItemRemoved(int itemIndex, MiscItemHolder miscItemHolder)
        {
            if (miscItemHolder.ID == keyItemID)
                InitPanel();
        }

        private void OnNewItemAdded(int itemIndex, MiscItemHolder miscItemHolder)
        {
            if (miscItemHolder.ID == keyItemID)
                InitPanel();
        }

        private void OnItemAmountChanged(int itemIndex, MiscItemHolder miscItemHolder)
        {
            if (miscItemHolder.ID == keyItemID)
                InitPanel();
        }

        private void Awake()
        {
            rectTransform = (RectTransform)transform;

            keyItemID = keyType == KeyType.Key ? ItemSettings.GetKeyID() : ItemSettings.GetRoyalKeyID();
        }

        private void Start()
        {
            InitPanel();
        }

        private void InitPanel()
        {
            buyButton.SetActive(false);
            disabledButton.SetActive(false);
            keyButton.SetActive(false);

            int keysAmount = Inventory.GetResourceAmount(keyItemID);

            if (keysAmount > 0)
            {
                keysText.text = string.Format("{0}/1", keysAmount);
                keysShadowText.text = keysText.text;

                keyButton.SetActive(true);
            }
            else
            {
                if (Currency.Gems >= gemsAmount)
                {
                    isButtonActive = true;

                    costText.text = gemsAmount.ToString();
                    costShadowText.text = costText.text;

                    buyButton.SetActive(true);
                }
                else
                {
                    isButtonActive = false;

                    costDisabledText.text = gemsAmount.ToString();
                    costDisabledShadowText.text = costDisabledText.text;

                    disabledButton.SetActive(true);
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!isButtonActive)
                return;

            rectTransform.DOScaleX(1.05f, 0.15f);
            rectTransform.DOScaleY(1.05f, 0.2f);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!isButtonActive)
                return;

            rectTransform.DOScaleX(1.0f, 0.15f);
            rectTransform.DOScaleY(1.0f, 0.2f);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!isButtonActive)
                return;

            AudioController.PlaySound(AudioController.Settings.sounds.buttonSound);

            int inventoryKeyIndex = -1;
            int keysAmount = Inventory.GetResourceAmount(keyItemID, ref inventoryKeyIndex);

            if (keysAmount > 0 && inventoryKeyIndex != -1)
            {
                Inventory.RemoveResource(inventoryKeyIndex, 1);

                OpenChest();
            }
            else
            {
                if (Currency.Gems >= gemsAmount)
                {
                    Currency.ChangeGems(-gemsAmount, true);

                    OpenChest();
                }
            }
        }

        private void OpenChest()
        {
            Item[] allowedItems = ItemDatabase.GetEquipableItemsByRarity(allowedItemRarities);

            ItemHolder itemHolder = allowedItems.GetRandomItem().GetDefaultHolder();

            ItemRarity itemRarity = ItemSettings.GetRandomRarity(allowedItemRarities);
            itemHolder.SetItemRarity(itemRarity);
            itemHolder.SetItemLevel(Account.Level);

            Inventory.AddItem(itemHolder);

            ChestOpenWindow.Display(itemHolder, keyType);
        }

        public void OpenInfoWindowButton()
        {
            ChestInfoWindow.Display(keyType, chestImage.sprite, chestName);
        }
    }

    public enum KeyType
    {
        Key = 0,
        RoyalKey = 1
    }
}
