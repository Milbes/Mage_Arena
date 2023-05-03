using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{

    public class PauseAbilityTemplateBehavior : MonoBehaviour
    {
        [SerializeField] Transform parent;

        [Space]
        [SerializeField] Image abilityImage;
        [SerializeField] Image abilityTierImage;

        [Space]
        [SerializeField] List<Sprite> tiers;

        public void Init(AbilityInfo info)
        {
            parent.localPosition = Vector3.zero;
            parent.localRotation = Quaternion.identity;
            parent.localScale = Vector3.one;

            abilityImage.sprite = info.data.icon;

            abilityTierImage.sprite = tiers[info.ability.stackAmount];
        }
    }

}