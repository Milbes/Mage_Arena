using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    public class GamePauseCanvasBehavior : MonoBehaviour
    {
        private static GamePauseCanvasBehavior instance;

        public static readonly string ABILITIE_TEMPLATE_POOL_NAME = "Pause Ability UI Template";

        [Header("Canvas")]
        [SerializeField] Canvas pauseCanvas;
        [SerializeField] Image backgroundImage;

        private static Canvas PauseCanvas => instance.pauseCanvas;
        private static Image BackgroundImage => instance.backgroundImage;

        [Header("Abilities")]
        [SerializeField] RectTransform abilitiesParent;
        [SerializeField] RectTransform abilitiesBackground;
        [SerializeField] RectTransform abilitiesGrid;

        private static RectTransform AbilitiesParent => instance.abilitiesParent;
        private static RectTransform AbilitiesBackground => instance.abilitiesBackground;
        private static RectTransform AbilitiesGrid => instance.abilitiesGrid;

        [Header("Buttons")]
        [SerializeField] CanvasGroup buttonsCanvasGroup;

        private static CanvasGroup ButtonsCanvasGroup => instance.buttonsCanvasGroup;

        [Header("References")]
        [SerializeField] GameObject abilityTemplate;

        [Header("Settings")]
        [SerializeField] float appeareDuration;

        private static float AppeareDuration => instance.appeareDuration;


        private static Pool abilityTemplatePool;

        private static int abilitiesHeight = 0;

        private void Awake()
        {
            instance = this;

            abilityTemplatePool = PoolManager.AddPool(new PoolSettings { 
                autoSizeIncrement = true,
                name = ABILITIE_TEMPLATE_POOL_NAME,
                singlePoolPrefab = abilityTemplate,
                objectsContainer = abilitiesGrid,
                size = 10,
                type = Pool.PoolType.Single, 
            });
        }

        public static void Show()
        {
            PauseCanvas.enabled = true;

            BackgroundImage.color = BackgroundImage.color.SetAlpha(0);
            BackgroundImage.DOFade(0.95f, AppeareDuration, true);

            InitAbilitiesGrid();

            AbilitiesBackground.sizeDelta = new Vector2(980, 260 + 180 * abilitiesHeight);

            AbilitiesParent.DOAnchoredPosition(abilitiesHeight == 2 ? Vector2.up * 150 : Vector2.zero, AppeareDuration, true).SetEasing(Ease.Type.SineOut);

            ButtonsCanvasGroup.blocksRaycasts = false;
            ButtonsCanvasGroup.alpha = 0;
            ButtonsCanvasGroup.DOFade(1, AppeareDuration, true).OnComplete(() => {
                ButtonsCanvasGroup.blocksRaycasts = true;
            });
        }

        private void Hide()
        {
            BackgroundImage.DOFade(0, AppeareDuration, true);

            AbilitiesParent.DOAnchoredPosition(Vector2.up * 1000, AppeareDuration, true).SetEasing(Ease.Type.SineIn);

            ButtonsCanvasGroup.blocksRaycasts = false;
            ButtonsCanvasGroup.DOFade(0, AppeareDuration, true).OnComplete(() => {
                PauseCanvas.enabled = false;
            });
        }

        private static void InitAbilitiesGrid()
        {
            List<AbilityInfo> abilities = AbilitiesController.GetActiveAbilities();

            abilityTemplatePool.ReturnToPoolEverything();

            for (int i = 0; i < abilities.Count; i++)
            {
                abilityTemplatePool.GetPooledObject().GetComponent<PauseAbilityTemplateBehavior>().Init(abilities[i]);
            }

            if (abilities.Count < 6)
            {
                abilitiesHeight = 0;
            } else if (abilities.Count < 11)
            {
                abilitiesHeight = 1;
            } else
            {
                abilitiesHeight = 2;
            }
        }

        public void ContinueButton()
        {
            Hide();

            Tween.DelayedCall(appeareDuration, GamePanelBehavior.Resume, true);
        }

        public void HomeButton()
        {
            Time.timeScale = 1;
            GameController.ReturnToMap();
        }
    }
}