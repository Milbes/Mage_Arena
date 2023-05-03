using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Watermelon
{
    public class SparksUIAnimation : MonoBehaviour
    {
        private const string SPARK_COIN_POOL_NAME = "CoinSpark";
        private const string SPARK_GEM_POOL_NAME = "GemSpark";

        [SerializeField] GameMenuPage.Type parentPage;
        [SerializeField] RectTransform[] sparkPositions;
        [SerializeField] Currencies sparkType = Currencies.Gold;

        private Pool sparkPool;
        private int parentPageIndex;

        private Coroutine sparksCoroutine;

        private void OnEnable()
        {
            GameMenuController.OnSelectionChanged += OnPageSelectionChanged;
        }

        private void OnDisable()
        {
            GameMenuController.OnSelectionChanged -= OnPageSelectionChanged;
        }

        private void Awake()
        {
            sparkPool = PoolManager.GetPoolByName(sparkType == Currencies.Gold ? SPARK_COIN_POOL_NAME : SPARK_GEM_POOL_NAME);

            parentPageIndex = (int)parentPage;
        }

        private void OnPageSelectionChanged(int prevSelectedPageIndex, int selectedPageIndex)
        {
            if (selectedPageIndex == parentPageIndex)
            {
                if (sparkPositions.Length > 0)
                    sparksCoroutine = StartCoroutine(SparkAnimation());
            }
            else
            {
                if (sparksCoroutine != null)
                    StopCoroutine(sparksCoroutine);
            }
        }

        private IEnumerator SparkAnimation()
        {
            WaitForSeconds waitForSeconds;

            RectTransform[] tempSparkObjects;

            while (true)
            {
                waitForSeconds = new WaitForSeconds(UnityEngine.Random.Range(0.4f, 1.0f));

                tempSparkObjects = sparkPositions.Where(x => !x.gameObject.activeSelf).ToArray();
                if (!tempSparkObjects.IsNullOrEmpty())
                {
                    RectTransform parentSpark = tempSparkObjects.GetRandomItem();
                    parentSpark.gameObject.SetActive(true);

                    GameObject sparkObject = sparkPool.GetPooledObject();
                    sparkObject.gameObject.SetActive(true);
                    sparkObject.transform.SetParent(parentSpark);
                    sparkObject.transform.localPosition = Vector3.zero;
                    sparkObject.transform.localScale = Vector3.zero;
                    sparkObject.transform.localRotation = Quaternion.identity;

                    sparkObject.transform.DOScale(UnityEngine.Random.Range(0.4f, 1.2f), 0.5f).SetEasing(Ease.Type.CircOut).OnComplete(delegate
                    {
                        sparkObject.transform.DOScale(0, 0.4f).SetEasing(Ease.Type.CircIn).OnComplete(delegate
                        {
                            sparkObject.SetActive(false);
                            sparkObject.transform.SetParent(null);

                            parentSpark.gameObject.SetActive(false);
                        });
                    });
                }

                yield return waitForSeconds;
            }
        }
    }
}