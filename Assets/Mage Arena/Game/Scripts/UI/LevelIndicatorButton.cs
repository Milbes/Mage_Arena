using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Watermelon
{
    public class LevelIndicatorButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] TextMeshProUGUI levelText;
        [SerializeField] TextMeshProUGUI levelShadowText;
        [SerializeField] Image experienceFillBarImage;

        private RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = (RectTransform)transform;
        }

        private void Start()
        {
            levelText.text = Account.Level.ToString();
            levelShadowText.text = levelText.text;

            experienceFillBarImage.fillAmount = Mathf.Clamp01((float)Account.Experience / Account.NextLevelExperience);
        }

        private void OnEnable()
        {
            Account.OnExperienceGain += OnExperienceGain;
            Account.OnLevelUp += OnLevelUp;
        }

        private void OnDisable()
        {
            Account.OnExperienceGain -= OnExperienceGain;
            Account.OnLevelUp -= OnLevelUp;
        }

        private void OnExperienceGain()
        {
            experienceFillBarImage.DOFillAmount(Mathf.Clamp01((float)Account.Experience / Account.NextLevelExperience), 0.2f).SetEasing(Ease.Type.CircOut);
        }

        private void OnLevelUp(int level)
        {
            levelText.text = level.ToString();
            levelShadowText.text = levelText.text;

            experienceFillBarImage.DOFillAmount(1.0f, 0.2f).SetEasing(Ease.Type.CircOut).OnComplete(delegate
            {
                experienceFillBarImage.fillAmount = 0;
                experienceFillBarImage.DOFillAmount(Mathf.Clamp01((float)Account.Experience / Account.NextLevelExperience), 0.2f).SetEasing(Ease.Type.CircOut);
            });
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            rectTransform.DOScaleX(1.05f, 0.15f);
            rectTransform.DOScaleY(1.05f, 0.2f);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            rectTransform.DOScaleX(1.0f, 0.15f);
            rectTransform.DOScaleY(1.0f, 0.2f);
        }
    }
}