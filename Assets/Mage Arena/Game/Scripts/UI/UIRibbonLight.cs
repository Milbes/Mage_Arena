#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;
using Watermelon;

[RequireComponent(typeof(Image))]
public class UIRibbonLight : MonoBehaviour
{
    [SerializeField]
    private RectTransform parentRectTransform;

    [SerializeField]
    private float moveTime;

    [SerializeField, MinMaxSlider(1, 100)]
    private Vector2 duration;

    private float nextMoveTime;

    private float sidePosition;

    private Image ribbonImage;
    private RectTransform rectTransform;
    private TweenCase tweenCase;

    private bool isCreated;
    
    private void OnEnable()
    {
        nextMoveTime = Time.realtimeSinceStartup + Random.Range(duration.x, duration.y);
    }

    private void OnDisable()
    {
        if (tweenCase != null && !tweenCase.isCompleted)
            tweenCase.Kill();
    }

    private void Awake()
    {
        rectTransform = (RectTransform)transform;
        ribbonImage = GetComponent<Image>();
        ribbonImage.enabled = false;

        sidePosition = parentRectTransform.rect.width / 2 + rectTransform.rect.width;

        rectTransform.anchoredPosition = new Vector2(-sidePosition, 0);

        nextMoveTime = Time.realtimeSinceStartup + Random.Range(duration.x, duration.y);
    }

    private void Update()
    {
        if(!isCreated && Time.realtimeSinceStartup >= nextMoveTime)
        {
            MoveRibbon();
        }
    }

    public void MoveRibbon()
    {
        if (isCreated)
            return;

        isCreated = true;

        ribbonImage.enabled = true;

        nextMoveTime = Time.realtimeSinceStartup + moveTime + Random.Range(duration.x, duration.y);

        rectTransform.anchoredPosition = new Vector2(-sidePosition, 0);
        tweenCase = rectTransform.DOAnchoredPosition(new Vector3(sidePosition, 0), moveTime).OnComplete(delegate
        {
            isCreated = false;

            ribbonImage.enabled = false;
        });
    }
}
