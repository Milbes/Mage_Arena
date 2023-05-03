using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    public class CurrencyCloudController : MonoBehaviour
    {
        private static CurrencyCloudController currencyCloud;

        private const float CLOUD_SPHERE_RADIUS = 200;

        [SerializeField] Canvas mainCanvas;

        [Header("Coins")]
        [SerializeField] GameObject coinsPrefab;
        [SerializeField] RectTransform coinsContainerRectTransform;
        [SerializeField] RectTransform coinsTargetRectTransform;
        [SerializeField] Sprite[] coinSprites;

        [Header("Gems")]
        [SerializeField] GameObject gemsPrefab;
        [SerializeField] RectTransform gemsContainerRectTransform;
        [SerializeField] RectTransform gemsTargetRectTransform;
        [SerializeField] Sprite[] gemSprites;

        [Header("Floating Text")]
        [SerializeField] Text floatingText;

        private Pool coinsPool;
        private Pool gemsPool;

        private AudioClip collectAudioClip;
        private AudioClip appearAudioClip;

        private void Awake()
        {
            currencyCloud = this;

            coinsPool = new Pool(new PoolSettings("CoinsUI", coinsPrefab, 10, true, coinsContainerRectTransform));
            gemsPool = new Pool(new PoolSettings("GemsUI", gemsPrefab, 10, true, gemsContainerRectTransform));

            collectAudioClip = AudioController.Settings.sounds.collectSound;
            appearAudioClip = AudioController.Settings.sounds.cloudSound;
        }

        public static void SpawnCurrency(Currencies currency, RectTransform rectTransform, int elementsAmount, string text, System.Action onCurrencyHittedTarget = null)
        {
            Pool elementPool;
            RectTransform targetRectTransform;
            Sprite[] sprites;

            if(currency == Currencies.Gold)
            {
                elementPool = currencyCloud.coinsPool;
                targetRectTransform = currencyCloud.coinsTargetRectTransform;
                sprites = currencyCloud.coinSprites;
            }
            else
            {
                elementPool = currencyCloud.gemsPool;
                targetRectTransform = currencyCloud.gemsTargetRectTransform;
                sprites = currencyCloud.gemSprites;
            }

            elementPool.ReturnToPoolEverything();

            if(!string.IsNullOrEmpty(text))
            {
                currencyCloud.floatingText.gameObject.SetActive(true);
                currencyCloud.floatingText.text = text;
                currencyCloud.floatingText.transform.localScale = Vector3.zero;
                currencyCloud.floatingText.transform.position = rectTransform.position;
                currencyCloud.floatingText.color = currencyCloud.floatingText.color.SetAlpha(1.0f);
                currencyCloud.floatingText.transform.DOScale(1, 0.3f).SetEasing(Ease.Type.CubicOut).OnComplete(delegate
                {
                    currencyCloud.floatingText.DOFade(0, 0.5f).SetEasing(Ease.Type.ExpoIn);
                    currencyCloud.floatingText.transform.DOMove(currencyCloud.floatingText.transform.position.AddToY(150), 0.5f).SetEasing(Ease.Type.QuadIn).OnComplete(delegate
                    {
                        currencyCloud.floatingText.gameObject.SetActive(false);
                    });
                });
            }

            AudioController.PlaySound(currencyCloud.appearAudioClip);

            float defaultPitch = 0.9f;
            bool currencyHittedTarget = false;
            GameObject elementObject;
            for(int i = 0; i < elementsAmount; i++)
            {
                elementObject = elementPool.GetPooledObject();
                elementObject.transform.position = rectTransform.position;
                elementObject.transform.localRotation = Quaternion.identity;
                elementObject.transform.localScale = Vector3.one;
                elementObject.transform.SetParent(targetRectTransform.parent);
                elementObject.transform.SetAsFirstSibling();

                Image elementImage = elementObject.GetComponent<Image>();
                elementImage.sprite = sprites.GetRandomItem();
                elementImage.color = Color.white.SetAlpha(0);

                float moveTime = Random.Range(0.2f, 0.4f);

                TweenCase currencyTweenCase = null;
                RectTransform elementRectTransform = (RectTransform)elementObject.transform;
                elementImage.DOFade(1, 0.2f);
                elementRectTransform.DOMove(rectTransform.position + (Random.insideUnitSphere.SetZ(0) * CLOUD_SPHERE_RADIUS * currencyCloud.mainCanvas.scaleFactor), moveTime).SetEasing(Ease.Type.CubicOut).OnComplete(delegate
                {
                    Tween.DelayedCall(0.1f, delegate
                    {
                        elementRectTransform.DOScale(0.5f, 0.5f).SetEasing(Ease.Type.ExpoIn);
                        elementRectTransform.DOMove(targetRectTransform.position, 0.5f).SetEasing(Ease.Type.SineIn).OnComplete(delegate
                        {
                            if(!currencyHittedTarget)
                            {
                                if (onCurrencyHittedTarget != null)
                                    onCurrencyHittedTarget.Invoke();

                                currencyHittedTarget = true;
                            }

                            bool punchTarget = true;
                            if(currencyTweenCase != null)
                            {
                                if(currencyTweenCase.state < 0.8f)
                                {
                                    punchTarget = false;
                                }
                                else
                                {
                                    currencyTweenCase.Kill();
                                }
                            }

                            if(punchTarget)
                            {
                                AudioController.PlaySound(currencyCloud.collectAudioClip, pitch: defaultPitch);
                                defaultPitch += 0.01f;

                                currencyTweenCase = targetRectTransform.DOScale(1.2f, 0.15f).OnComplete(delegate
                                {
                                    currencyTweenCase = targetRectTransform.DOScale(1.0f, 0.1f);
                                });
                            }

                            elementRectTransform.gameObject.SetActive(false);
                        });
                    });
                });
            }
        }
    }
}
