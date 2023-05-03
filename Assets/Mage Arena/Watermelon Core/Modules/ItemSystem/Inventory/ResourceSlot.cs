using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Watermelon
{
    public class ResourceSlot : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] Image background;
        [SerializeField] Image icon;
        [SerializeField] Text amount;

        private MiscItemHolder currentItemHolder;

        public virtual void SetItem(MiscItemHolder itemHolder)
        {
            MiscItem miscItem = (MiscItem)itemHolder.Item;

            icon.sprite = miscItem.Sprite;

            ItemSettings.RaritySettings raritySettings = itemHolder.RaritySettings;
            background.sprite = raritySettings.SlotBackground;

            if (miscItem.MaxStackSize > 1)
            {
                amount.gameObject.SetActive(true);
                amount.text = string.Format("x{0}", itemHolder.Amount);
            }
            else
            {
                amount.gameObject.SetActive(false);
            }

            currentItemHolder = itemHolder;
        }

        public virtual void OnClick()
        {
            ItemPreviewWindow.PreviewResource(currentItemHolder);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick();
        }
    }
}
