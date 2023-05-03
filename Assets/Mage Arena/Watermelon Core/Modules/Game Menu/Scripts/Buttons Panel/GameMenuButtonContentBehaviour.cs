using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
	public class GameMenuButtonContentBehaviour : MonoBehaviour
    {
        private readonly Vector2 SELECTED_SCALE = new Vector2(1.1f, 1.1f);
        private readonly Vector2 DEFAULT_SCALE = new Vector2(1.0f, 1.0f);

        private readonly Vector2 SELECTED_POSITION = new Vector2(0, 40);
        private readonly Vector2 DEFAULT_POSITION = new Vector2(0, 0);

        [SerializeField] Image icon;
        [SerializeField] Text title;

        private RectTransform rectTransform;
        public RectTransform RectTransform => rectTransform;

        private void Awake()
        {
            rectTransform = (RectTransform)transform;

            title.gameObject.SetActive(false);
        }

        public void Select(bool animation = true)
        {
            if(animation)
            {
                icon.rectTransform.DOAnchoredPosition(SELECTED_POSITION, 0.2f);
                icon.rectTransform.DOScale(SELECTED_SCALE, 0.2f).SetEasing(Ease.Type.BounceOut);

                title.gameObject.SetActive(true);
                title.color = title.color.SetAlpha(0.0f);
                title.DOFade(1, 0.2f);
            }
            else
            {
                icon.rectTransform.anchoredPosition = SELECTED_POSITION;
                icon.rectTransform.localScale = SELECTED_SCALE;

                title.gameObject.SetActive(true);
                title.color = title.color.SetAlpha(1.0f);
            }
        }

        public void Unselect(bool animation = true)
        {
            if(animation)
            {
                icon.rectTransform.DOAnchoredPosition(DEFAULT_POSITION, 0.2f);
                icon.rectTransform.DOScale(DEFAULT_SCALE, 0.2f).SetEasing(Ease.Type.BounceOut);

                title.DOFade(0, 0.2f).OnComplete(delegate
                {
                    title.gameObject.SetActive(false);
                });
            }
            else
            {
                icon.rectTransform.anchoredPosition = DEFAULT_POSITION;
                icon.rectTransform.localScale = DEFAULT_SCALE;

                title.gameObject.SetActive(false);
            }
        }
    }
}
