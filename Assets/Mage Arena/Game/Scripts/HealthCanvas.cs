#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Watermelon;
using TMPro;

namespace Watermelon
{

    public class HealthCanvas : MonoBehaviour
    {
        [SerializeField] Canvas healthCanvas;
        [SerializeField] Image healthFill;
        [SerializeField] Image whiteFill;

        //[SerializeField] Text healthText;
        [SerializeField] TMP_Text healthText;
        [SerializeField] TMP_Text healthTextShadow;

        [Space]

        [SerializeField] List<TMP_Text> textList;

        [Header("Settings Preset")]
        [SerializeField] HealthCanvasSettings settings;

        TweenCase whiteDelayedCall;
        TweenCase whiteFillCase;

        private void Update()
        {
            healthCanvas.transform.forward = -CameraController.MainCamera.transform.forward;
        }

        public void Show()
        {
            healthCanvas.enabled = true;

            textList.ForEach((text) =>
            {
                text.enabled = false;
            });
        }

        public void Hide()
        {
            healthCanvas.enabled = false;
        }

        public TMP_Text GetAvailableText()
        {
            for(int i = 0; i < textList.Count; i++)
            {
                if (!textList[i].enabled) return textList[i];
            }

            return null;
        }

        public void ShowText(HealthTextType type, string text, bool isCrit = false)
        {
            var label = GetAvailableText();
            if (label == null) return;

            var preset = settings.GetPreset(type);
            if (preset == null) return;

            label.enabled = true;
            label.text = text;

            Vector2 spawnPosition = settings.textSpawnPosition +  new Vector2().RandomVector(-settings.textSpawnOffset.x, settings.textSpawnOffset.x, -settings.textSpawnOffset.y, settings.textSpawnOffset.y);
            Vector2 movePosition = settings.textMovePosition + new Vector2().RandomVector(-settings.textMoveOffset.x, settings.textMoveOffset.x, -settings.textMoveOffset.y, settings.textMoveOffset.y);

            label.color = isCrit ? preset.textColorCrit : preset.textColor;
            label.color = label.color.SetAlpha(0);

            label.outlineColor = preset.outlineColor;

            label.fontSize = isCrit ? preset.spawnFontSizeCrit : preset.spawnFontSize;
            label.rectTransform.anchoredPosition = spawnPosition;

            label.enabled = true;

            label.DOFade(1, preset.fadeInDuration);
            label.DOFontSize(isCrit ? preset.fontSizeCrit : preset.fontSize, preset.fontSizeIncreaseDuration).SetEasing(preset.fontSizeEasing);
            label.rectTransform.DOAnchoredPosition(movePosition, preset.moveDuration).SetEasing(preset.moveEasing);

            Tween.DelayedCall(preset.moveDuration + preset.stayDuration, () => {
                label.DOFade(0, preset.fadeOutDuration).OnComplete(() => {
                    label.enabled = false;
                });
            });

        }

        public void SetFill(float fill, bool transitionEffect = false, int healthAmount = 0)
        {

            if (!transitionEffect)
            {
                healthFill.fillAmount = fill;
            }
            else
            {
                float transitionFill = healthFill.fillAmount - fill;
                float whitePosition = Mathf.Lerp(0, -125f, fill);

                whiteFill.enabled = true;
                whiteFill.rectTransform.anchoredPosition = new Vector3(whitePosition, 0, 0);
                whiteFill.fillAmount = transitionFill;

                if (whiteDelayedCall != null && !whiteDelayedCall.isActive) whiteDelayedCall.Kill();
                if (whiteFillCase != null && !whiteFillCase.isActive) whiteFillCase.Kill();

                whiteDelayedCall = Tween.DelayedCall(0.5f, () => {
                    whiteFillCase = whiteFill.DOAction((start, result, time) => { }, transitionFill, 0, 0.5f).SetEasing(Ease.Type.SineInOut).OnComplete(() => {
                        if (whiteFill != null)
                        {
                            whiteFill.enabled = false;
                        }
                        else
                        {
                            whiteFillCase.Kill();
                        }

                    });
                });

                healthFill.fillAmount = fill;
            }

            if (healthText != null)
            {
                if (healthAmount < 0) healthAmount = 0;
                healthText.text = healthAmount.ToString();
                healthTextShadow.text = healthText.text;
            }

        }

        private void OnDestroy()
        {
            if (whiteFillCase != null && !whiteFillCase.isCompleted)
                whiteFillCase.Kill();
        }

        public void TakeHeal(int damage)
        {

            ShowText(HealthTextType.Heal, damage.ToString(), true);

            /*Text healText = null;

            for (int i = 0; i < healLabels.Count; i++)
            {
                if (!healLabels[i].enabled) healText = healLabels[i];
            }

            if (healText != null)
            {
                FlyText(healText, "+" + damage);
            }*/
        }

        public void TakeDamage(int damage)
        {
            /*Text damageText = null;

            for (int i = 0; i < damageLabels.Count; i++)
            {
                if (!damageLabels[i].enabled) damageText = damageLabels[i];
            }

            if (damageText != null)
            {
                FlyText(damageText, "-" + damage);
            }*/
        }

        public void TakeCritDamage(int damage)
        {
            /*Text damageText = null;

            for (int i = 0; i < critLabels.Count; i++)
            {
                if (!critLabels[i].enabled) damageText = critLabels[i];
            }

            if (damageText != null)
            {
                FlyText(damageText, "-" + damage);
            }*/
        }

        TweenCase textPositionCase;
        TweenCase textFadePosition1;
        TweenCase textFadePosition2;
        TweenCase delayCase;

        private void FlyText(Text damageText, string label)
        {
            damageText.enabled = true;
            damageText.text = label;

            damageText.rectTransform.anchoredPosition = new Vector3(0, -30, 0);

            textPositionCase = damageText.rectTransform.DOAnchoredPosition(new Vector3(0, 30, 0), 1f).SetEasing(Ease.Type.SineInOut).OnComplete(() => {
                damageText.enabled = false;
            });

            damageText.color = damageText.color.SetAlpha(0);

            textFadePosition1 = damageText.DOFade(1, 0.20f).SetEasing(Ease.Type.SineIn).OnComplete(() => {
                delayCase = Tween.DelayedCall(0.6f, () => {
                    textFadePosition2 = damageText.DOFade(0, 0.2f).SetEasing(Ease.Type.SineOut);
                });
            });
        }

        public void OnDisable()
        {
            if (textFadePosition1 != null && !textFadePosition1.isCompleted) textFadePosition1.Kill();
            if (textFadePosition2 != null && !textFadePosition2.isCompleted) textFadePosition2.Kill();
            if (textPositionCase != null && !textPositionCase.isCompleted) textPositionCase.Kill();
            if (delayCase != null && !delayCase.isCompleted) delayCase.Kill();
        }

        public static HealthTextType GetTextType(DamageType damageType)
        {
            switch (damageType)
            {
                case DamageType.Base:
                    return HealthTextType.Base;

                case DamageType.Fire:
                    return HealthTextType.Fire;

                case DamageType.Ice:
                    return HealthTextType.Ice;

                case DamageType.Shadow:
                    return HealthTextType.Shadow;

                case DamageType.Storm:
                    return HealthTextType.Lightning;
            }

            return HealthTextType.Base;
        }

    }

    
    public static class CustomExtentions
    {
        public static Vector2 RandomVector(this Vector2 vector, float minX, float maxX, float minY, float maxY)
        {

            return new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
            
        }
    }

}