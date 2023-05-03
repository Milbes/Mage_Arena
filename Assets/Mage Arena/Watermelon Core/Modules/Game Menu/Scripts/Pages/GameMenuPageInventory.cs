using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Watermelon
{
    public class GameMenuPageInventory : GameMenuPage
    {
        private const float ITEMS_PANEL_DEFAULT_HEIGHT = 345.0f;
        private const float ITEMS_PANEL_STEP_HEIGHT = 185.0f;

        [SerializeField] RectTransform pageRectTransform;

        [Header("Character")]
        [SerializeField] TextMeshProUGUI statsHealthText;
        [SerializeField] TextMeshProUGUI statsHealthShadowText;
        [SerializeField] TextMeshProUGUI statsDamageText;
        [SerializeField] TextMeshProUGUI statsDamageShadowText;

        [Header("Equipable Panel")]
        [SerializeField] GameObject itemSlotPrefab;
        [SerializeField] RectTransform itemsPanelRectTransform;
        [SerializeField] Transform itemsContainer;

        [Header("Resources Panel")]
        [SerializeField] GameObject resourceSlotPrefab;
        [SerializeField] RectTransform resourcesPanelRectTransform;
        [SerializeField] Transform resourcesContainer;

        [Space]
        [SerializeField] RectTransform resourcesChainRectTransform;

        [Space]
        [SerializeField] RectTransform[] contentElements;

        private List<ItemSlot> inventorySlots = new List<ItemSlot>();
        private List<ResourceSlot> resourcesSlots = new List<ResourceSlot>();

        private Pool itemSlotPool;
        private Pool resourceSlotPool;

        private void Awake()
        {
            itemSlotPool = new Pool(new PoolSettings("Item Slot", itemSlotPrefab, 5, true));
            resourceSlotPool = new Pool(new PoolSettings("Resource Slot", resourceSlotPrefab, 0, true));
        }

        private void Start()
        {
            List<ItemHolder> inventory = Inventory.GetInventory();
            if (inventory != null)
            {
                int inventorySize = inventory.Count;

                GameObject uiItemObject;
                for (int i = 0; i < inventorySize; i++)
                {
                    uiItemObject = itemSlotPool.GetPooledObject();
                    uiItemObject.transform.SetParent(itemsContainer);
                    uiItemObject.transform.localPosition = Vector3.zero;
                    uiItemObject.transform.localRotation = Quaternion.identity;
                    uiItemObject.transform.localScale = Vector3.one;
                    uiItemObject.transform.SetAsLastSibling();
                    uiItemObject.SetActive(true);

                    ItemSlot itemSlot = uiItemObject.GetComponent<ItemSlot>();
                    itemSlot.SetItem(inventory[i]);

                    inventorySlots.Add(itemSlot);
                }
            }

            List<MiscItemHolder> resources = Inventory.GetResources();
            if (resources != null)
            {
                int resourcesSize = resources.Count;

                GameObject uiItemObject;
                for (int i = 0; i < resourcesSize; i++)
                {
                    uiItemObject = resourceSlotPool.GetPooledObject();
                    uiItemObject.transform.SetParent(resourcesContainer);
                    uiItemObject.transform.localPosition = Vector3.zero;
                    uiItemObject.transform.localRotation = Quaternion.identity;
                    uiItemObject.transform.localScale = Vector3.one;
                    uiItemObject.transform.SetAsLastSibling();
                    uiItemObject.SetActive(true);

                    ResourceSlot resourceSlot = uiItemObject.GetComponent<ResourceSlot>();
                    resourceSlot.SetItem(resources[i]);

                    resourcesSlots.Add(resourceSlot);
                }
            }

            InitStats(Character.GetStats());

            RecalculatePanels();
        }

        private void RecalculatePanels()
        {
            // Items panel size
            int inventorySize = inventorySlots.Count;
            if (inventorySize == 0)
                inventorySize = 1;

            float itemsPanelHeight = ITEMS_PANEL_DEFAULT_HEIGHT + ((inventorySize / 5) + (inventorySize % 5 > 0 ? 0 : -1)) * ITEMS_PANEL_STEP_HEIGHT;
            itemsPanelRectTransform.sizeDelta = new Vector2(itemsPanelRectTransform.sizeDelta.x, itemsPanelHeight);

            // Resources panel size
            int resourcesSize = resourcesSlots.Count;
            if (resourcesSize != 0)
            {
                resourcesPanelRectTransform.gameObject.SetActive(true);
                resourcesChainRectTransform.gameObject.SetActive(true);

                float resourcesPanelHeight = ITEMS_PANEL_DEFAULT_HEIGHT + ((resourcesSize / 5) + (resourcesSize % 5 > 0 ? 0 : -1)) * ITEMS_PANEL_STEP_HEIGHT;
                resourcesPanelRectTransform.anchoredPosition = new Vector2(0, itemsPanelRectTransform.anchoredPosition.y - itemsPanelHeight - 50);
                resourcesPanelRectTransform.sizeDelta = new Vector2(resourcesPanelRectTransform.sizeDelta.x, resourcesPanelHeight);

                resourcesChainRectTransform.anchoredPosition = resourcesPanelRectTransform.anchoredPosition.AddToY(95);
            }
            else
            {
                resourcesPanelRectTransform.gameObject.SetActive(false);
                resourcesChainRectTransform.gameObject.SetActive(false);

                resourcesPanelRectTransform.anchoredPosition = Vector2.zero;
                resourcesPanelRectTransform.sizeDelta = Vector2.zero;
            }

            // Calculate total page size
            float totalPageHeight = 0;
            for(int i = 0; i < contentElements.Length; i++)
            {
                if(contentElements[i].gameObject.activeSelf)
                {
                    // Add offset
                    totalPageHeight += -contentElements[i].anchoredPosition.y - totalPageHeight;
                    totalPageHeight += contentElements[i].sizeDelta.y;
                }
            }

            totalPageHeight += GameMenuDragHandler.MENU_HEIGHT;
            totalPageHeight += 50;

            pageRectTransform.sizeDelta = new Vector2(pageRectTransform.sizeDelta.x, totalPageHeight);
        }

        private void OnEnable()
        {
            Inventory.OnItemRemoved += OnItemRemoved;
            Inventory.OnItemStateChanged += OnItemChanged;
            Inventory.OnNewItemAdded += OnItemAdded;

            Inventory.OnResourceRemoved += OnResourceRemoved;
            Inventory.OnResourceStateChanged += OnResourceStateChanged;
            Inventory.OnNewResourceAdded += OnResourceAdded;

            Character.OnStatsRecalculated += OnStatsRecalculated;
            Character.OnItemEquiped += OnItemEquiped;
        }

        private void OnDisable()
        {
            Inventory.OnItemRemoved -= OnItemRemoved;
            Inventory.OnItemStateChanged -= OnItemChanged;
            Inventory.OnNewItemAdded -= OnItemAdded;

            Inventory.OnResourceRemoved -= OnResourceRemoved;
            Inventory.OnResourceStateChanged -= OnResourceStateChanged;
            Inventory.OnNewResourceAdded -= OnResourceAdded;

            Character.OnStatsRecalculated -= OnStatsRecalculated;
            Character.OnItemEquiped -= OnItemEquiped;
        }

        private void OnItemEquiped(EquipableItem equipableItemType, ItemHolder itemHolder, ItemHolder previousItemHolder)
        {
            InitStats(Character.GetStats());
        }

        private void OnStatsRecalculated(Character.Stats stats)
        {
            InitStats(stats);
        }

        private void InitStats(Character.Stats stats)
        {
            statsHealthText.text = ((int)stats.GetTotalHealth).ToString();
            statsHealthShadowText.text = statsHealthText.text;

            statsDamageText.text = ((int)stats.GetTotalDamage).ToString();
            statsDamageShadowText.text = statsDamageText.text;
        }

        private void OnItemAdded(int itemIndex, ItemHolder itemHolder)
        {
            List<ItemHolder> inventory = Inventory.GetInventory();

            GameObject uiItemObject = itemSlotPool.GetPooledObject();
            uiItemObject.transform.SetParent(itemsContainer);
            uiItemObject.transform.localPosition = Vector3.zero;
            uiItemObject.transform.localRotation = Quaternion.identity;
            uiItemObject.transform.localScale = Vector3.one;
            uiItemObject.SetActive(true);

            ItemSlot itemSlot = uiItemObject.GetComponent<ItemSlot>();
            itemSlot.SetItem(itemHolder);

            inventorySlots.Add(itemSlot);

            RecalculatePanels();

            int inventoryCount = inventory.Count;
            for (int i = 0; i < inventoryCount; i++)
            {
                inventorySlots[i].SetItem(inventory[i]);
                inventorySlots[i].transform.SetSiblingIndex(i);
            }
        }

        private void OnItemChanged(int itemIndex, ItemHolder itemHolder)
        {
            if(inventorySlots.IsInRange(itemIndex))
            {
                inventorySlots[itemIndex].SetItem(itemHolder);
            }
        }

        private void OnItemRemoved(int itemIndex, ItemHolder itemHolder)
        {
            if (inventorySlots.IsInRange(itemIndex))
            {
                inventorySlots[itemIndex].gameObject.SetActive(false);
                inventorySlots.RemoveAt(itemIndex);

                RecalculatePanels();
            }
        }

        private void OnResourceAdded(int itemIndex, MiscItemHolder miscItemHolder)
        {
            List<MiscItemHolder> resources = Inventory.GetResources();

            GameObject uiItemObject = resourceSlotPool.GetPooledObject();
            uiItemObject.transform.SetParent(resourcesContainer);
            uiItemObject.transform.localPosition = Vector3.zero;
            uiItemObject.transform.localRotation = Quaternion.identity;
            uiItemObject.transform.localScale = Vector3.one;
            uiItemObject.SetActive(true);

            ResourceSlot resourceSlot = uiItemObject.GetComponent<ResourceSlot>();
            resourceSlot.SetItem(miscItemHolder);

            resourcesSlots.Add(resourceSlot);

            RecalculatePanels();

            int resourcesCount = resources.Count;
            for (int i = 0; i < resourcesCount; i++)
            {
                resourcesSlots[i].SetItem(resources[i]);
                resourcesSlots[i].transform.SetSiblingIndex(i);
            }
        }

        private void OnResourceStateChanged(int resouceIndex, MiscItemHolder miscItemHolder)
        {
            if (resourcesSlots.IsInRange(resouceIndex))
            {
                resourcesSlots[resouceIndex].SetItem(miscItemHolder);
            }
        }

        private void OnResourceRemoved(int resouceIndex, MiscItemHolder miscItemHolder)
        {
            if (resourcesSlots.IsInRange(resouceIndex))
            {
                resourcesSlots[resouceIndex].gameObject.SetActive(false);
                resourcesSlots.RemoveAt(resouceIndex);

                RecalculatePanels();
            }
        }

        public void UpgradeCharacterButton()
        {
            CharacterPreviewWindow.CharacterPreview();
        }

        public void RemoveItemDev(int index)
        {
            Inventory.RemoveItem(index);
        }

        public void AddItemDev(int index, int amount = 1)
        {
            Item item = ItemDatabase.GetItem(index);

            ItemHolder itemHolder = item.GetHolder();
            if (item.Type == ItemType.Misc)
                ((MiscItemHolder)itemHolder).SetAmount(amount);

            Inventory.AddItem(itemHolder);
        }
    }
}
