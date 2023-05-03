using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    public class RewardWindow : GameMenuWindow
    {
        private static RewardWindow rewardWindow;

        [SerializeField] Image background;
        [SerializeField] CanvasGroup canvasGroup;

        [Space]
        [SerializeField] GameObject previewElementPrefab;
        [SerializeField] Transform previewElementsContainer;

        private Pool previewElementPool;

        private void Awake()
        {
            rewardWindow = this;

            previewElementPool = new Pool(new PoolSettings("PreviewElement", previewElementPrefab, 5, true, previewElementsContainer));
        }

        private void SpawnPreviewElements(IItemPreview[] itemPreviews)
        {
            previewElementPool.ReturnToPoolEverything();

            GameObject tempPreviewObject;
            for(int i = 0; i < itemPreviews.Length; i++)
            {
                tempPreviewObject = previewElementPool.GetPooledObject();
                tempPreviewObject.transform.localPosition = Vector3.zero;
                tempPreviewObject.transform.localRotation = Quaternion.identity;
                tempPreviewObject.transform.localScale = Vector3.one;

                tempPreviewObject.GetComponent<ItemPreviewUI>().Init(itemPreviews[i]);
            }
        }

        protected override void OpenAnimation()
        {
            AudioController.PlaySound(AudioController.Settings.sounds.windowOpenSound);

            background.gameObject.SetActive(true);

            // Set default values
            background.color = background.color.SetAlpha(0.0f);
            canvasGroup.alpha = 0;

            // Play animation
            background.DOFade(0.7f, 0.3f);
            canvasGroup.DOFade(1, 0.3f);
        }

        protected override void CloseAnimation()
        {
            AudioController.PlaySound(AudioController.Settings.sounds.windowCloseSound);

            background.DOFade(0, 0.3f);
            canvasGroup.DOFade(0, 0.3f).OnComplete(delegate
            {
                background.gameObject.SetActive(false);
            });
        }

        public void CloseButton()
        {
            GameMenuWindow.HideWindow(this);
        }

        public static void DisplayItems(params IItemPreview[] itemPreviews)
        {
            RewardWindow itemPreviewWindow = rewardWindow;
            if(itemPreviewWindow != null)
            {
                itemPreviewWindow.SpawnPreviewElements(itemPreviews);

                GameMenuWindow.ShowWindow<RewardWindow>(itemPreviewWindow);
            }
        }
    }
}
