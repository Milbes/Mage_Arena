using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Watermelon
{

    public class AbilitiesCanvasBehavior : MonoBehaviour
    {
        private static AbilitiesCanvasBehavior abilitiesCanvasBehavior;

        [SerializeField] CanvasGroup joystickCanvasGroup;
        private static CanvasGroup JoystickCanvasGroup => abilitiesCanvasBehavior.joystickCanvasGroup;

        [SerializeField] Canvas abilitiesCanvas;
        private static Canvas AbilitiesCanvas => abilitiesCanvasBehavior.abilitiesCanvas;

        [SerializeField] RectTransform backgroundRect;
        private static RectTransform BackgroundRect => abilitiesCanvasBehavior.backgroundRect;

        [Space]

        [SerializeField] Sprite lockSprite;
        private static Sprite LockSprite => abilitiesCanvasBehavior.lockSprite;

        [SerializeField] List<Sprite> tierImages;
        private static List<Sprite> TierImages => abilitiesCanvasBehavior.tierImages;

        [SerializeField] List<AbilityCardUI> abilityCards;
        private static List<AbilityCardUI> AbilityCards => abilitiesCanvasBehavior.abilityCards;

        private static List<AbilityData> abilitiesToShow;

        private void Awake()
        {
            abilitiesCanvasBehavior = this;
        }

        public static void Show(List<AbilityData> abilities)
        {
            abilitiesToShow = abilities;

            for(int i = 0; i < AbilityCards.Count - 1; i++)
            {
                AbilityCards[i].Init(abilities[i]);
            }

            AbilityCards[AbilityCards.Count - 1].InitLock(LockSprite);

            AbilitiesCanvas.enabled = true;

            BackgroundRect.anchoredPosition = new Vector3(0, -400);
            BackgroundRect.DOAnchoredPosition(new Vector3(0, 500), 0.5f).SetEasing(Ease.Type.SineOut);

            JoystickCanvasGroup.DOFade(0, 0.5f).OnComplete(() => {
                JoystickCanvasGroup.gameObject.SetActive(false);

                PlayerController.Joystick.DisableJoystick();
            });
        }

        public static void Hide()
        {
            BackgroundRect.anchoredPosition = new Vector3(0, 500);
            BackgroundRect.DOAnchoredPosition(new Vector3(0, -400), 0.5f).SetEasing(Ease.Type.SineOut).OnComplete(() => {
                AbilitiesCanvas.enabled = false;
            });

            JoystickCanvasGroup.gameObject.SetActive(true);
            JoystickCanvasGroup.DOFade(1, 0.5f);

            PlayerController.Joystick.EnableJoystick();

            AbilitiesController.DisappeareOrb();
        }

        public void OnCardClick(int cardId)
        {
            var card = AbilityCards[cardId];

            if (card.isLocked)
            {
                AdsManager.ShowRewardBasedVideo(RevealAbilityForAd);

                return;
            }

            AbilitiesController.ActivateAbility(abilitiesToShow[cardId], PlayerController.playerController);

            LevelController.SetFinishState();

            Hide();
        }

        private void RevealAbilityForAd(bool finishedWatching)
        {
            Tween.NextFrame(delegate
            {
                if (finishedWatching)
                {
                    int index = AbilityCards.Count - 1;

                    var card = AbilityCards[index];
                    card.Init(abilitiesToShow[index]);
                }
            });
        }

        [System.Serializable]
        class AbilityCardUI
        {
            public Image abilityImage;
            public Image stackImage;
            [Space]
            public TMP_Text abilityNameText;
            public TMP_Text abilityNameShadow;
            [Space]
            public GameObject showAdObject;

            [System.NonSerialized] public bool isLocked;

            public void Init(AbilityData ability)
            {
                abilityImage.sprite = ability.icon;

                stackImage.gameObject.SetActive(true);
                stackImage.sprite = TierImages[AbilitiesController.GetStackAmount(ability)];

                abilityNameText.gameObject.SetActive(true);
                abilityNameShadow.gameObject.SetActive(true);

                abilityNameText.text = ability.name;
                abilityNameShadow.text = ability.name;

                if(showAdObject != null) showAdObject.gameObject.SetActive(false);

                isLocked = false;
            }

            public void InitLock(Sprite lockSprite)
            {
                abilityImage.sprite = lockSprite;
                stackImage.gameObject.SetActive(false);

                abilityNameText.gameObject.SetActive(false);
                abilityNameShadow.gameObject.SetActive(false);

                showAdObject.gameObject.SetActive(true);

                isLocked = true;
            }
        }
    }

    
}