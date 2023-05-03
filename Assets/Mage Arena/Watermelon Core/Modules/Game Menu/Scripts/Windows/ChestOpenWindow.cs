using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Watermelon;

public class ChestOpenWindow : GameMenuWindow
{
    private static ChestOpenWindow chestOpenWindow;

    [SerializeField] Image background;

    [Space]
    [SerializeField] CanvasGroup itemTitleCanvasGroup;
    [SerializeField] TextMeshProUGUI itemNameText;
    [SerializeField] TextMeshProUGUI itemNameShadowText;

    [Space]
    [SerializeField] Image itemRarityImage;

    [Space]
    [SerializeField] RectTransform itemSlotRectTransform;
    [SerializeField] Image itemIconImage;
    [SerializeField] Image itemBackgroundImage;
    [SerializeField] Image itemFrameImage;

    [Space]
    [SerializeField] Image itemShiningImage;

    [Space]
    [SerializeField] CanvasGroup actionButtonCanvasGroup;
    [SerializeField] GameObject tapToCloseGameObject;

    private ItemHolder currentItemHolder;

    private bool isChestOpened;

    private void Awake()
    {
        chestOpenWindow = this;
    }

    private void OnEnable()
    {
        ChestAnimationEvent.OnChestOpened += OnChestOpened;
    }

    private void OnDisable()
    {
        ChestAnimationEvent.OnChestOpened -= OnChestOpened;
    }

    private void OnChestOpened()
    {
        itemSlotRectTransform.DOScaleX(1.1f, 0.3f).OnComplete(delegate
        {
            itemSlotRectTransform.DOScaleX(1.0f, 0.1f);
        });

        itemSlotRectTransform.DOScaleY(1.0f, 0.25f);

        Tween.DelayedCall(0.25f, delegate
        {
            itemTitleCanvasGroup.DOFade(1.0f, 0.2f);
        });

        itemSlotRectTransform.DOAnchoredPosition(new Vector3(0, 350), 0.3f);
    }

    private void DisplayItem(ItemHolder itemHolder)
    {
        isChestOpened = false;

        currentItemHolder = itemHolder;

        Item item = itemHolder.Item;

        actionButtonCanvasGroup.alpha = 0;
        actionButtonCanvasGroup.gameObject.SetActive(true);
        actionButtonCanvasGroup.DOFade(1, 0.3f);

        tapToCloseGameObject.SetActive(false);

        itemSlotRectTransform.localPosition = Vector3.zero;
        itemSlotRectTransform.localScale = Vector3.zero;
        itemTitleCanvasGroup.alpha = 0.0f;

        ItemSettings.RaritySettings raritySettings = itemHolder.RaritySettings;

        itemNameText.text = item.ItemName;
        itemNameShadowText.text = itemNameText.text;

        itemIconImage.sprite = item.Sprite;

        itemBackgroundImage.sprite = raritySettings.SlotBackground;
        itemFrameImage.sprite = raritySettings.SlotFrame;

        // DEV 
        itemShiningImage.gameObject.SetActive(false);

        itemShiningImage.sprite = raritySettings.WindowShining;
        itemRarityImage.sprite = raritySettings.TitleImage;
    }

    protected override void OpenAnimation()
    {
        AudioController.PlaySound(AudioController.Settings.sounds.windowOpenSound);

        background.gameObject.SetActive(true);
    }

    protected override void CloseAnimation()
    {
        AudioController.PlaySound(AudioController.Settings.sounds.windowCloseSound);

        Item item = currentItemHolder.Item;

        background.gameObject.SetActive(false);

        currentItemHolder = null;
    }

    public void ActionButton()
    {
        if(!isChestOpened)
        {
            actionButtonCanvasGroup.DOFade(0, 0.3f).OnComplete(delegate
            {
                actionButtonCanvasGroup.gameObject.SetActive(false);

                tapToCloseGameObject.SetActive(true);
            });

            ChestPreview.OpenChest();

            isChestOpened = true;
        }
        else
        {
            ChestPreview.HideChest();

            GameMenuWindow.HideWindow(this);
        }
    }

    public void CloseButton()
    {
        if(isChestOpened)
            GameMenuWindow.HideWindow(this);
    }

    public static void Display(ItemHolder itemHolder, KeyType keyType)
    {
        if (chestOpenWindow != null)
        {
            chestOpenWindow.DisplayItem(itemHolder);

            ChestPreview.DisplayChest(keyType);

            GameMenuWindow.ShowWindow<ChestOpenWindow>(chestOpenWindow);
        }
    }
}
