using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace Watermelon
{
    public class LevelCompleteBehavior : MonoBehaviour
    {
        private static LevelCompleteBehavior instance;

        [SerializeField] Text titleText;
        private static Text TitleText => instance.titleText;

        [SerializeField] Canvas canvas;
        private static Canvas Canvas => instance.canvas;

        private static UnityAction OnCloseAction { get; set; }

        private static Pool previewElementPool;

        [Space]
        [SerializeField] Image background;
        [SerializeField] Color backgroundColor;

        private static Image Background => instance.background;
        private static Color BackgroundColor => instance.backgroundColor;

        [Header("Info Panel")]
        [SerializeField] RectTransform levelCompletePanel;

        private static RectTransform LevelCompletePanel => instance.levelCompletePanel;

        [Space]
        [SerializeField] GameObject levelCompleteText;
        [SerializeField] GameObject gameOverText;

        private static GameObject LevelCompleteText => instance.levelCompleteText;
        private static GameObject GameOverText => instance.gameOverText;

        [Space]
        [SerializeField] TMP_Text stagesText;

        private static TMP_Text StagesText => instance.stagesText;

        [Header("Account level info")]
        [SerializeField] TMP_Text levelText;
        [SerializeField] Image levelShine;

        private static TMP_Text LevelText => instance.levelText;
        private static Image LevelShine => instance.levelShine;

        [Space]
        [SerializeField] Image levelFill;
        [SerializeField] float levelFillMinWidth;
        [SerializeField] float levelFillMaxWidth;

        private static Image LevelFill => instance.levelFill;
        private static float LevelFillMinWidth => instance.levelFillMinWidth;
        private static float LevelFillMaxWidth => instance.levelFillMaxWidth;

        [Header("Items")]
        [SerializeField] RectTransform itemsRewardParent;
        [SerializeField] GameObject itemPreviewObject;

        private static RectTransform ItemsRewardParent => instance.itemsRewardParent;
        private static GameObject ItemPreviewObject => instance.itemPreviewObject;

        [Space]
        [SerializeField] RectTransform itemsRewardPanel;
        [SerializeField] float rewardMinHeight;
        [SerializeField] float rewardStepHeight;

        private static RectTransform ItemsRewardPanel => instance.itemsRewardPanel;
        private static float RewardMinHeight => instance.rewardMinHeight;
        private static float RewardStepHeing => instance.rewardStepHeight;

        [Space]
        [SerializeField] CanvasGroup tapToContinueText;
        [SerializeField] Button closeButton;

        private static CanvasGroup TapToContinueText => instance.tapToContinueText;
        private static Button CloseButtonRef => instance.closeButton;

        [Header("Settings")]
        [SerializeField] float mainPanelMovementDuration = 1f;
        [SerializeField] float itemsPanelMovementDuration = 1f;

        private static float MainPanelMovementDuration => instance.mainPanelMovementDuration;
        private static float ItemsPanelMovementDuration => instance.itemsPanelMovementDuration;

        [Space]
        [SerializeField] float progressFillDuration = 1f;

        private static float ProgressFillDuration => instance.progressFillDuration;

        [Space]
        [SerializeField] float itemScaleDuration = 0.4f;
        [SerializeField] float delayBetweenItems = 0.2f;
        [SerializeField] float panelWideningDuration = 0.4f;

        private static float ItemScaleDuration => instance.itemScaleDuration;
        private static float DelayBetweenItems => instance.delayBetweenItems;
        private static float PanelWideningDuration => instance.panelWideningDuration;

        [Space]
        [SerializeField] float tapToPlayTextFadeDuration = 0.5f;

        private static float TapToPlayTextFadeDuration => instance.tapToPlayTextFadeDuration;

        [Space]
        [SerializeField] bool spawnLotsOfTestItems = false;

        private static bool SpawnLotsOfTestItems => instance.spawnLotsOfTestItems;

        //[SerializeField] GameObject previewElementPrefab;
        //[SerializeField] Transform previewElementsContainer;

        private void Awake()
        {
            instance = this;

            previewElementPool = new Pool(new PoolSettings("PreviewElement", ItemPreviewObject, 5, true, ItemsRewardPanel));
        }

        public void CloseButton()
        {
            Hide();

            OnCloseAction?.Invoke();
        }

        public static void Show(bool successful, UnityAction onCloseAction = null)
        {
            if (SpawnLotsOfTestItems)
            {
                for (int i = 0; i < 16; i++)
                {
                    DropController.pickedUpItems.Add(LevelController.TestDropItem);
                }
            }

            Canvas.enabled = true;

            Time.timeScale = 0;

            OnCloseAction = onCloseAction;

            Background.color = BackgroundColor.SetAlpha(0);
            Background.DOFade(BackgroundColor.a, 0.5f, true);

            LevelCompleteText.SetActive(successful);
            GameOverText.SetActive(!successful);

            StagesText.text = "Stage " + (GameController.CurrentRoomId + 1);

            Account.ResetLevelDev();

            int prevLevel = Account.Level;

            LevelText.text = prevLevel.ToString();
            LevelShine.color = LevelShine.color.SetAlpha(1f);

            LevelFill.rectTransform.sizeDelta = LevelFill.rectTransform.sizeDelta.SetX(Mathf.Lerp(LevelFillMinWidth, LevelFillMaxWidth, Account.Experience / (float)Account.NextLevelExperience));

            LevelCompletePanel.anchoredPosition = Vector3.up * 1000;
            LevelCompletePanel.DOAnchoredPosition(DropController.pickedUpItems.Count > 10 ? Vector3.up * 150 : Vector3.zero, MainPanelMovementDuration, true).SetEasing(Ease.Type.SineOut).OnComplete(() => {

                LevelController.ApplyExpirience();

                instance.StartCoroutine(LevelProgressbarCoroutine(prevLevel));

            });
        }

        private static IEnumerator LevelProgressbarCoroutine(int prevLevel)
        {
            int currentLevel = Account.Level;

            float finalSize = Mathf.Lerp(LevelFillMinWidth, LevelFillMaxWidth, Account.Experience / (float)Account.NextLevelExperience);

            if (currentLevel == prevLevel)
            {
                LevelFill.rectTransform.DOSize(LevelFill.rectTransform.sizeDelta.SetX(finalSize), ProgressFillDuration, true).SetEasing(Ease.Type.SineInOut).OnComplete(() => {
                    LevelShine.DOFade(0, 1f, true);
                });
            } else
            {
                for(int i = prevLevel; i <= currentLevel; i++)
                {
                    if(i != prevLevel) LevelFill.rectTransform.sizeDelta = LevelFill.rectTransform.sizeDelta.SetX(LevelFillMinWidth);

                    if (i == currentLevel)
                    {
                        LevelFill.rectTransform.DOSize(LevelFill.rectTransform.sizeDelta.SetX(finalSize), ProgressFillDuration, true).SetEasing(Ease.Type.SineInOut);
                    } else
                    {
                        LevelFill.rectTransform.DOSize(LevelFill.rectTransform.sizeDelta.SetX(LevelFillMaxWidth), ProgressFillDuration, true).SetEasing(Ease.Type.SineInOut);

                        yield return new WaitForSecondsRealtime(1f);

                        LevelText.text = (i + 1).ToString();
                    }
                }

                LevelShine.DOFade(0, 1f, true);
            }

            if (DropController.pickedUpItems.Count > 0)
            {
                ItemsRewardParent.gameObject.SetActive(true);

                ItemsRewardParent.DOAnchoredPosition(Vector3.zero, ItemsPanelMovementDuration, true).SetEasing(Ease.Type.SineOut).OnComplete(() => {
                    instance.StartCoroutine(InitItems());
                });
            }
            else
            {
                TapToContinueText.DOFade(1, TapToPlayTextFadeDuration, true);

                CloseButtonRef.enabled = true;
            }
        }

        private static IEnumerator InitItems()
        {
            previewElementPool.ReturnToPoolEverything();

            for (int i = 0; i < DropController.pickedUpItems.Count; i++)
            {
                if (i == 5)
                {
                    ItemsRewardPanel.DOSize(new Vector3(930, (RewardMinHeight + RewardStepHeing)), PanelWideningDuration, true).SetEasing(Ease.Type.SineInOut);

                    yield return new WaitForSecondsRealtime(PanelWideningDuration);
                }
                else if (i == 10)
                {
                    ItemsRewardPanel.DOSize(new Vector3(930, (RewardMinHeight + RewardStepHeing * 2)), PanelWideningDuration, true).SetEasing(Ease.Type.SineInOut);

                    yield return new WaitForSecondsRealtime(PanelWideningDuration);
                }
                else if (i == 15) break;

                ItemHolder holder = DropController.pickedUpItems[i];

                var tempPreviewObject = previewElementPool.GetPooledObject().GetComponent<RewardItemUI>();

                tempPreviewObject.transform.localPosition = Vector3.zero;
                tempPreviewObject.transform.localRotation = Quaternion.identity;
                tempPreviewObject.transform.localScale = Vector3.one;

                tempPreviewObject.Init(holder, ItemScaleDuration);

                Inventory.AddItem(holder);

                yield return new WaitForSecondsRealtime(DelayBetweenItems);
            }

            yield return new WaitForSecondsRealtime(ItemScaleDuration);

            TapToContinueText.DOFade(1, TapToPlayTextFadeDuration, true);
            CloseButtonRef.enabled = true;
        }

        public static void Hide()
        {
            Time.timeScale = 1;

            //Canvas.enabled = false;
        }
          
    }

}