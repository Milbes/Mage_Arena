using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Watermelon
{
    public class ItemSlot : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] Image background;
        [SerializeField] Image icon;

        private ItemHolder currentItemHolder;

        public virtual void SetItem(ItemHolder itemHolder)
        {
            Item tempItem = itemHolder.Item;

            icon.sprite = tempItem.Sprite;

            ItemSettings.RaritySettings raritySettings = itemHolder.RaritySettings;
            background.sprite = raritySettings.SlotBackground;

            currentItemHolder = itemHolder;
        }

        public virtual void OnClick()
        {
            ItemPreviewWindow.PreviewItem(currentItemHolder);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick();
        }
    }
}
