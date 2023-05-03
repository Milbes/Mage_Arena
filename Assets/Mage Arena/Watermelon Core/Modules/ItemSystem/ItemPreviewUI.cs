using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    public class ItemPreviewUI : MonoBehaviour
    {
        [SerializeField] Image previewImage;
        [SerializeField] Image backgroundImage;
        [SerializeField] Image frameImage;
        [SerializeField] Text amountText;

        public void Init(IItemPreview itemPreview)
        {
            previewImage.sprite = itemPreview.Preview;

            backgroundImage.sprite = itemPreview.BackgroundSprite;
            frameImage.sprite = itemPreview.FrameSprite;

            if (itemPreview.Amount > 1)
            {
                amountText.gameObject.SetActive(true);
                amountText.text = itemPreview.Amount.ToString();
            }
            else
            {
                amountText.gameObject.SetActive(false);
            }
        }
    }
}
