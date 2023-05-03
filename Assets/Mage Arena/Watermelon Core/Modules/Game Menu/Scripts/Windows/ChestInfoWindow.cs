using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    public class ChestInfoWindow : GameMenuWindow
    {
        private static ChestInfoWindow chestInfoWindow;

        [SerializeField] Image background;
        [SerializeField] RectTransform panel;

        [SerializeField] Image shiningImage;
        [SerializeField] Image chestImage;

        [SerializeField] TextMeshProUGUI chestNameText;
        [SerializeField] TextMeshProUGUI chestNameShadowText;

        [SerializeField] GameObject treasureChestObject;
        [SerializeField] GameObject royalChestObject;

        [Space]
        [SerializeField] Sprite treasureChestShiningSprite;
        [SerializeField] Sprite royalChestShiningSprite;

        private void Awake()
        {
            chestInfoWindow = this;    
        }

        private void Init(KeyType keyType, Sprite chestSprite, string chestName)
        {
            chestImage.sprite = chestSprite;

            chestNameText.text = chestName;
            chestNameShadowText.text = chestNameText.text;

            if(keyType == KeyType.Key)
            {
                treasureChestObject.SetActive(true);
                royalChestObject.SetActive(false);

                shiningImage.sprite = treasureChestShiningSprite;
            }
            else
            {
                treasureChestObject.SetActive(false);
                royalChestObject.SetActive(true);

                shiningImage.sprite = royalChestShiningSprite;
            }
        }

        protected override void CloseAnimation()
        {
            AudioController.PlaySound(AudioController.Settings.sounds.windowCloseSound);

            background.gameObject.SetActive(false);
        }

        protected override void OpenAnimation()
        {
            background.gameObject.SetActive(true);

            panel.gameObject.SetActive(true);
            panel.transform.localScale = Vector3.zero;
            panel.transform.DOScale(1, 0.2f).SetEasing(Ease.Type.CubicOut);
        }

        public void CloseButton()
        {
            GameMenuWindow.HideWindow(this);
        }

        public static void Display(KeyType keyType, Sprite chestSprite, string chestName)
        {
            if (chestInfoWindow != null)
            {
                chestInfoWindow.Init(keyType, chestSprite, chestName);

                GameMenuWindow.ShowWindow<ChestInfoWindow>(chestInfoWindow);
            }
        }
    }
}
