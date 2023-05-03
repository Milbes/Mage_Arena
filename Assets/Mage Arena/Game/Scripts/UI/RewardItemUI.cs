using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    public class RewardItemUI : MonoBehaviour
    {
        [SerializeField] RectTransform scaleParent;
        [Space]
        [SerializeField] Image backgroundImage;
        [SerializeField] Image iconImage;

        public void Init(ItemHolder itemHolder, float duration)
        {
            backgroundImage.sprite = itemHolder.BackgroundSprite;
            iconImage.sprite = itemHolder.Item.Sprite;

            scaleParent.localScale = Vector3.zero;

            scaleParent.DOScale(1, duration, true).SetEasing(Ease.Type.BackOut);
        }
    }
}

