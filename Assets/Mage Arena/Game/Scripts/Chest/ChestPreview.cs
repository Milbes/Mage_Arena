using UnityEngine;
using Watermelon;

public class ChestPreview : MonoBehaviour
{
    private static readonly int ANIMATION_BEFOR_APPEARING_HASH = Animator.StringToHash("BeforeAppearing");
    private static readonly int ANIMATION_OPEN_TRIGGER_HASH = Animator.StringToHash("Open");

    private static ChestPreview chestPreview;

    [SerializeField] GameObject chestGameObject;
    [SerializeField] GameObject royalChestGameObject;

    private Animator chestAnimator;

    private void Awake()
    {
        chestPreview = this;
    }

    public static void DisplayChest(KeyType keyType)
    {
        if (keyType == KeyType.Key)
        {
            chestPreview.royalChestGameObject.SetActive(false);
            chestPreview.chestGameObject.SetActive(true);

            chestPreview.chestAnimator = chestPreview.chestGameObject.GetComponent<Animator>();
        }
        else
        {
            chestPreview.chestGameObject.SetActive(false);
            chestPreview.royalChestGameObject.SetActive(true);

            chestPreview.chestAnimator = chestPreview.royalChestGameObject.GetComponent<Animator>();
        }

        chestPreview.chestAnimator.Play(ANIMATION_BEFOR_APPEARING_HASH, -1, 0);
    }

    public static void OpenChest()
    {
        chestPreview.chestAnimator.SetTrigger(ANIMATION_OPEN_TRIGGER_HASH);
    }

    public static void HideChest()
    {
        chestPreview.royalChestGameObject.SetActive(false);
        chestPreview.chestGameObject.SetActive(false);
    }
}
