#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

namespace Watermelon
{

    public class GamePanelBehavior : MonoBehaviour
    {

        private static GamePanelBehavior instance;

        [Header("Level Progression")]
        [SerializeField] Image progressFill;
        [SerializeField] Sprite regularFillSprite;
        [SerializeField] Sprite bossFillSprite;

        private static Sprite RegularFillSprite => instance.regularFillSprite;
        private static Sprite BossFillSprite => instance.bossFillSprite;

        [Space]
        [SerializeField] TMP_Text levelNumberText;
        [SerializeField] TMP_Text levelNumberTextShadow;
        [SerializeField] float minWidth;
        [SerializeField] float maxWidth;
        [SerializeField] Text currentLevelText;
        [SerializeField] Text nextLevelText;

        private static TMP_Text LevelNumberText => instance.levelNumberText;
        private static TMP_Text LevelNumberTextShadow => instance.levelNumberTextShadow;

        [Header("Pause")]
        [SerializeField] GameObject pauseObject;

        [Header("Second Live Popup")]

        [SerializeField] Canvas resurectCanvas;
        [SerializeField] Image resurectBackground;
        [SerializeField] Button resurectButton;

        private static Canvas ResurectCanvas => instance.resurectCanvas;
        private static RectTransform ResurectPanel => instance.resurectPanel;
        private static Button ResurectButtonRef => instance.resurectButton;

        [Space]
        [SerializeField] RectTransform resurectPanel;
        [SerializeField] Image resurectPopup;
        [SerializeField] Image resurectMenuText;

        private static Image ResurectPopup => instance.resurectPopup;
        private static Image ResurectBackground => instance.resurectBackground;
        private static Image ResurectMenuText => instance.resurectMenuText;
       

        [Space]
        [SerializeField] RectTransform consumablesButton;
        [SerializeField] RectTransform consumablesPanel;


        [Space]
        [SerializeField] LevelUpUIController levelUp;
        [SerializeField] Canvas joystickCanvas;
        [SerializeField] Canvas gameCanvas;

        [Space]
        [SerializeField] GameObject tutorialPanel;

        [Space]
        [SerializeField] GameObject greenOrbPanel;

        [Space]
        [SerializeField] Image blackImage;

        [Header("Boss Text")]
        [SerializeField] Image bossText;
        private static Image BossText => instance.bossText;

        

        private static LevelUpUIController LevelUp => instance.levelUp;
        private static Canvas JoystickCanvas => instance.joystickCanvas;
        private static Canvas GameCanvas => instance.gameCanvas;

        private static Image ProgressFill => instance.progressFill;
        private static float MinWidth => instance.minWidth;
        private static float MaxWidth => instance.maxWidth;
        private static Text CurrentLevelText => instance.currentLevelText;
        private static Text NextLevelText => instance.nextLevelText;

        private static Image BlackImage => instance.blackImage;

        private static GameObject GreenOrbPanel => instance.greenOrbPanel;

        [Header("Developement")]
        [SerializeField] GameObject devPanel;

        private void Awake()
        {
            instance = this;

            if (GameSettingsPrefs.Get<bool>("tutorial"))
            {
                BlackImage.enabled = true;
                BlackImage.color = BlackImage.color.SetAlpha(1f);

                BlackImage.DOFade(0, 0.3f).OnComplete(() => {
                    BlackImage.enabled = false;
                });
            }
        }

        public static void NewRoom()
        {
            ProgressFill.sprite = GameController.CurrentRoom.IsBossRoom ? BossFillSprite : RegularFillSprite;
        }

        public static void DoHidden(UnityAction action)
        {
            BlackImage.enabled = true;
            BlackImage.color = BlackImage.color.SetAlpha(0);
            BlackImage.DOFade(1, 0.3f).OnComplete(() => {
                action?.Invoke();
                BlackImage.DOFade(0, 0.3f).OnComplete(() => BlackImage.enabled = false);
            });
        }

        public static void ShowTutorial()
        {
            instance.tutorialPanel.SetActive(true);
            //instance.consumablesButton.GetComponent<Button>().enabled = false;

            Time.timeScale = 0;
        }

        public void HideTutorial()
        {
            tutorialPanel.SetActive(false);
            //consumablesButton.GetComponent<Button>().enabled = true;

            GameSettingsPrefs.Set("tutorial", true);
            Time.timeScale = 1;

        }

        public static void ShowBossLevelText()
        {
            Time.timeScale = 0.1f;

            Tween.DelayedCall(1f, () =>
            {
                BossText.gameObject.SetActive(true);

                BossText.color = BossText.color.SetAlpha(0);
                BossText.DOFade(1, 0.4f, true);

                Tween.DelayedCall(2, () =>
                {
                    BossText.DOFade(0, 0.4f, true).OnComplete(() =>
                    {
                        BossText.gameObject.SetActive(false);
                        Time.timeScale = 1;
                    });
                }, true);
            }, true);
        }

        public static void SetLevelNumber(int levelNumber)
        {
            CurrentLevelText.text = (levelNumber + 1).ToString();
            NextLevelText.text = (levelNumber + 2).ToString();
        }

        public static void SetProgression(float progression, bool levelingUp, bool withAnimation = false)
        {

            if (levelingUp)
            {
                ProgressFill.rectTransform.DOSize(new Vector2(Mathf.Lerp(MinWidth, MaxWidth, 1), ProgressFill.rectTransform.sizeDelta.y), 0.5f).SetEasing(Ease.Type.SineInOut).OnComplete(() => {
                    LevelNumberText.text = (LevelController.ProgressionLevel + 1).ToString();
                    LevelNumberTextShadow.text = LevelNumberText.text;

                    ProgressFill.rectTransform.sizeDelta = ProgressFill.rectTransform.sizeDelta.SetX(MinWidth);

                    ProgressFill.rectTransform.DOSize(new Vector2(Mathf.Lerp(MinWidth, MaxWidth, progression), ProgressFill.rectTransform.sizeDelta.y), 0.5f).SetEasing(Ease.Type.SineInOut);
                });

            } else
            {
                LevelNumberText.text = (LevelController.ProgressionLevel + 1).ToString();
                LevelNumberTextShadow.text = LevelNumberText.text;

                if (withAnimation)
                {
                    ProgressFill.rectTransform.DOSize(new Vector2(Mathf.Lerp(MinWidth, MaxWidth, progression), ProgressFill.rectTransform.sizeDelta.y), 0.5f).SetEasing(Ease.Type.SineInOut);
                }
                else
                {
                    ProgressFill.rectTransform.sizeDelta = ProgressFill.rectTransform.sizeDelta.SetX(Mathf.Lerp(MinWidth, MaxWidth, progression));
                }
            }
        }

        public static void ShowLevelUpPanel()
        {
            GameCanvas.enabled = false;
            JoystickCanvas.enabled = false;
            LevelUp.Show();
        }

        public static void HideLevelUpPanel()
        {
            GameCanvas.enabled = true;
            JoystickCanvas.enabled = true;
            LevelUp.Hide();
        }

        //static bool resurectForAd = false;

        public static void ShowResurectPopup()
        {
            ResurectCanvas.enabled = true;

            ResurectBackground.DOFade(0.9f, 0.5f, true);
            ResurectPanel.DOAnchoredPosition(Vector3.down * 15, 0.5f, true).SetEasing(Ease.Type.SineOut);

            ResurectButtonRef.enabled = false;

            Tween.DelayedCall(3, () => {
                ResurectButtonRef.enabled = true;

                ResurectMenuText.DOFade(1f, 0.5f, true);
            }, true);

            ResurectPopup.gameObject.SetActive(true);

            //resurectForAd = StoreController.GetBoughtAmountOfConsumable(ConsumableType.SecondLife) == 0;
            //if (resurectForAd)
            //{
            //    ResurectText.text = "Revive for ad?";
            //} else
            //{
            //    ResurectText.text = "Use <color=#82C1FC> 1X</color> Life?";
            //}

            SlowDownTime();
        }

        public void ResurectButton(bool ad)
        {
            if (ad)
            {
                AdsManager.ShowRewardBasedVideo((finished) =>
                {
                    Tween.NextFrame(delegate
                    {
                        if (finished)
                        {
                            CloseResurectionPanel();

                            PlayerController.Resurrect();
                        }
                        else
                        {
                            DoNotResurectButton();
                        }
                    });
                });
            }
            else
            {
                if (Currency.Gems >= 10)
                {
                    ResurectPopup.gameObject.SetActive(false);

                    Currency.ChangeGems(-10);

                    CloseResurectionPanel();

                    PlayerController.Resurrect();
                }
                else
                {

                }
            }
        }

        public void CloseResurectionPanel(UnityAction action = null)
        {
            SpeedUpTime();

            ResurectButtonRef.enabled = false;

            ResurectMenuText.DOFade(0, 0.5f, true);
            ResurectPanel.DOAnchoredPosition(Vector3.down * 1500, 0.5f, true).SetEasing(Ease.Type.SineIn);
            ResurectBackground.DOFade(0, 0.5f, true).OnComplete(() => {
                ResurectCanvas.enabled = false;

                action?.Invoke();
            });
        }

        public void DoNotResurectButton()
        {
            CloseResurectionPanel(PlayerController.Finish);
        }


        static TweenCase consumableButtonTween, consumableIconTween, timeScaleTween, fixedTimeTween;

        public void ConsumableButtonClick()
        {

            if (consumableButtonTween != null && !consumableButtonTween.isCompleted) consumableButtonTween.Kill();
            if (consumableIconTween != null && !consumableIconTween.isCompleted) consumableIconTween.Kill();
            if (timeScaleTween != null && !timeScaleTween.isCompleted) timeScaleTween.Kill();
            if (fixedTimeTween != null && !fixedTimeTween.isCompleted) fixedTimeTween.Kill();

            consumableButtonTween = consumablesButton.DOAnchoredPosition(new Vector2(-150, 250), 0.4f, true).SetEasing(Ease.Type.SineInOut);
            consumableIconTween = consumablesPanel.DOAnchoredPosition(new Vector2(0, 200), 0.4f, true).SetEasing(Ease.Type.SineInOut);


            SlowDownTime();

            PlayerController.Joystick.OnPointerDownCallback += HideConsumablesPanel;
        }

        public static void SlowDownTime()
        {
            timeScaleTween = instance.DOAction((start, final, t) =>
            {
                Time.timeScale = start + (final - start) * t;
            }, 1, 0.1f, 0.2f);

            fixedTimeTween = instance.DOAction((start, final, t) =>
            {
                Time.fixedDeltaTime = start + (final - start) * t;
            }, Time.fixedDeltaTime, Time.fixedDeltaTime / 10f, 0.2f);
        }

        public static void SpeedUpTime()
        {
            timeScaleTween = instance.DOAction((start, final, t) =>
            {
                Time.timeScale = start + (final - start) * t;
            }, 0.1f, 1, 0.2f);

            fixedTimeTween = instance.DOAction((start, final, t) =>
            {
                Time.fixedDeltaTime = start + (final - start) * t;
            }, Time.fixedDeltaTime, Time.fixedDeltaTime * 10f, 0.2f);
        }

        public static void SpeedUpInstantly()
        {
            Time.timeScale = 1;

            Time.fixedDeltaTime *= 10;
        }

        public void HideConsumablesPanel()
        {
            if (consumableButtonTween != null && !consumableButtonTween.isCompleted) consumableButtonTween.Kill();
            if (consumableIconTween != null && !consumableIconTween.isCompleted) consumableIconTween.Kill();
            if (timeScaleTween != null && !timeScaleTween.isCompleted) timeScaleTween.Kill();
            if (fixedTimeTween != null && !fixedTimeTween.isCompleted) fixedTimeTween.Kill();

            consumableButtonTween = consumablesButton.DOAnchoredPosition(new Vector2(0, 250), 0.4f, true).SetEasing(Ease.Type.SineInOut);
            consumableIconTween = consumablesPanel.DOAnchoredPosition(new Vector2(-250, 200), 0.4f, true).SetEasing(Ease.Type.SineInOut);

            SpeedUpTime();

            PlayerController.Joystick.OnPointerDownCallback -= HideConsumablesPanel;
        }

        public static void Resume()
        {
            Time.timeScale = 1;
            paused = false;
        }

        public void ResumeButton()
        {
            //pauseObject.SetActive(false);
            Time.timeScale = 1;
            paused = false;
        }

        public void ReturnToMapButton()
        {
            Time.timeScale = 1;
            GameController.ReturnToMap();
        }

        static bool paused = false;

        public void OpenPausePanel()
        {
            //pauseObject.SetActive(true);

            GamePauseCanvasBehavior.Show();

            Time.timeScale = 0;

            paused = true;
        }

        public static void ShowGreenOrbPanel()
        {
            GreenOrbPanel.SetActive(true);

            Time.timeScale = 0;

            paused = true;
        }

        public void HealButton()
        {
            Time.timeScale = 1;
            paused = false;

            GreenOrbPanel.SetActive(false);

            //PlayerController.HealCompletly();
        }

        public void IncreaseMaxHealthButton()
        {
            Time.timeScale = 1;
            paused = false;

            GreenOrbPanel.SetActive(false);

            // PlayerController.IncreaseMaxHealth();
        }

        private void Update()
        {
            if (!paused && Input.GetKeyDown(KeyCode.Escape))
            {
                OpenPausePanel();
            }
        }

        #region Dev

        public void FirstLevelDevButton()
        {
            Debug.Log("[UI Module] Dev button pressed");
        }

        public void PrevLevelDevButton()
        {
            Debug.Log("[UI Module] Dev button pressed");
        }

        public void NextLevelDevButton()
        {
            Debug.Log("[UI Module] Dev button pressed");
        }

        public void ColorDevButton()
        {
            Debug.Log("[UI Module] Dev button pressed");
        }

        public void HideDevButton()
        {
            devPanel.SetActive(false);
        }

        #endregion
    }

    [System.Serializable]
    public enum ConsumableType
    {
        Health, Damage, InstantKill, SecondLife, Armor
    }

}
