using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Watermelon
{
    public class EquipableSlotUI : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] EquipableItem slotType = EquipableItem.None;

        [Space]
        [SerializeField] Image background;
        [SerializeField] Image icon;
        [SerializeField] Image defaultIcon;

        [Space]
        [SerializeField] Sprite defaultBackgroundSprite;

        private ItemHolder currentItem;

        private void Start()
        {
            currentItem = Character.GetEquipedItem(slotType);    
            if(currentItem != null)
            {
                InitItem(currentItem);
            }
        }

        private void OnEnable()
        {
            Character.OnItemEquiped += OnItemEquiped;
        }

        private void OnDisable()
        {
            Character.OnItemEquiped -= OnItemEquiped;
        }

        private void OnItemEquiped(EquipableItem equipableItemType, ItemHolder itemHolder, ItemHolder previousItemHolder)
        {
            if(equipableItemType == slotType)
            {
                InitItem(itemHolder);
            }
        }

        private void InitItem(ItemHolder itemHolder)
        {
            // Equip item
            if (itemHolder != null)
            {
                Item item = itemHolder.Item;

                // Disable default slot icon
                defaultIcon.gameObject.SetActive(false);

                // Set slot background by rarity
                ItemSettings.RaritySettings raritySettings = itemHolder.RaritySettings;
                background.sprite = raritySettings.SlotBackground;

                // Set slot icon and enable it
                icon.gameObject.SetActive(true);
                icon.sprite = item.Sprite;
            }
            // Unequip item
            else
            {
                // Enable default slot icon
                defaultIcon.gameObject.SetActive(true);

                // Reset background color and sprite
                background.color = Color.white;
                background.sprite = defaultBackgroundSprite;

                // Disable item icon
                icon.gameObject.SetActive(false);
            }

            currentItem = itemHolder;
        }

        public virtual void OnClick()
        {
            if(currentItem != null)
            {
                ItemPreviewWindow.PreviewEquipedItem(currentItem);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick();
        }
    }
}
