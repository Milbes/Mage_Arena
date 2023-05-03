using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Watermelon
{
    public class DailyDealController : MonoBehaviour
    {
        private const string DAILY_DEAL_TIME_TEXT = "Refreshes in:";
        private const string DAILY_DEAL_PREFS = "DAILY_DEAL";
        private const int DAILY_DEALS_AMOUNT = 3;
        private const int DAY_HALF_SECONDS = 43200;

        [SerializeField] DailyDealDatabase dailyDealDatabase;

        [Space]
        [SerializeField] GameObject dailyDealPrefab;
        [SerializeField] Transform dailyDealsContainer;

        [Space]
        [SerializeField] TextMeshProUGUI dailyDealTimerText;
        [SerializeField] TextMeshProUGUI dailyDealTimerShadowText;

        private List<DailyDealSave.Deal> dailyDealProducts = new List<DailyDealSave.Deal>();
        private List<int> dailyDealProductIDs = new List<int>();
        private double dailyDealsRefreshTime;

        private DailyDealButton[] dailyDealButtons;

        private Pool dailyDealPool;

        private void OnEnable()
        {
            Character.OnItemEquiped += OnItemEquiped;
        }

        private void OnDisable()
        {
            Character.OnItemEquiped -= OnItemEquiped;
        }

        private void Awake()
        {
            dailyDealPool = new Pool(new PoolSettings("", dailyDealPrefab, 2, true, dailyDealsContainer));

            dailyDealButtons = new DailyDealButton[DAILY_DEALS_AMOUNT];

            GameObject tempDailyDealObject;
            for (int i = 0; i < DAILY_DEALS_AMOUNT; i++)
            {
                tempDailyDealObject = dailyDealPool.GetPooledObject();
                tempDailyDealObject.transform.localScale = Vector3.one;
                tempDailyDealObject.transform.localRotation = Quaternion.identity;
                tempDailyDealObject.transform.localPosition = Vector3.zero;

                DailyDealButton dailyDealButton = tempDailyDealObject.GetComponent<DailyDealButton>();

                tempDailyDealObject.SetActive(true);

                dailyDealButtons[i] = dailyDealButton;
            }

            // Load data from save
            Load();

            StartCoroutine(TimerCoroutine());
        }

        private IEnumerator TimerCoroutine()
        {
            WaitForSeconds waitForSeconds = new WaitForSeconds(60);

            TimeSpan tempTimeSpan;
            double dailyDealsDiffTime;

            while (true)
            {
                dailyDealsDiffTime = dailyDealsRefreshTime - TimeUtils.GetCurrentUnixTimestamp();

                do
                {
                    tempTimeSpan = TimeSpan.FromSeconds(dailyDealsDiffTime + 60);

                    dailyDealTimerText.text = DAILY_DEAL_TIME_TEXT + (tempTimeSpan.Hours > 0 ? " " + tempTimeSpan.Hours + "h" : "") + (tempTimeSpan.Minutes > 0 ? " " + tempTimeSpan.Minutes + "m" : "");
                    dailyDealTimerShadowText.text = dailyDealTimerText.text;

                    yield return waitForSeconds;

                    dailyDealsDiffTime -= 60;
                }
                while (dailyDealsDiffTime > 0);

                dailyDealsRefreshTime = dailyDealsRefreshTime + DAY_HALF_SECONDS;

                RefreshProducts();

                yield return null;
            }
        }

        private void RefreshProducts()
        {
            dailyDealDatabase.GetUniqueIDs(DAILY_DEALS_AMOUNT, ref dailyDealProductIDs);

            dailyDealProducts = new List<DailyDealSave.Deal>();

            int averageItemLevel = (Character.GetAverageItemLevel() + Account.Level) / 2;

            Item tempItem;
            for (int i = 0; i < DAILY_DEALS_AMOUNT; i++)
            {
                tempItem = dailyDealDatabase.GetItemID(dailyDealProductIDs[i]);
                if(tempItem != null)
                {
                    Debug.Log("Item ID: " + dailyDealProductIDs[i] + "; Allowed Rarities: " + tempItem.AllowedItemRarity);

                    DailyDealSave.Deal deal = new DailyDealSave.Deal(dailyDealProductIDs[i]);
                    deal.SetItemRarity(ItemSettings.GetDailyDealRandomRarity(tempItem));
                    deal.SetItemLevel(Random.Range(averageItemLevel, averageItemLevel + 5)); // TODO: Change 5 value with variable

                    dailyDealProducts.Add(deal);
                }
                else
                {
                    Debug.LogError(string.Format("[Daily Deal]: Wrong item ID for product {0}", dailyDealProductIDs[i]));
                }
            }

            Save();
        }

        private void InitDeals()
        {
            for (int i = 0; i < DAILY_DEALS_AMOUNT; i++)
            {
                dailyDealButtons[i].Init(dailyDealDatabase.Products[dailyDealProductIDs[i]], dailyDealProducts[i]);
            }
        }

        private void OnItemEquiped(EquipableItem equipableItemType, ItemHolder itemHolder, ItemHolder previousItemHolder)
        {
            for (int i = 0; i < DAILY_DEALS_AMOUNT; i++)
            {
                dailyDealButtons[i].ReinitStats();
            }
        }

        public void RefreshProductsButton()
        {
            AdsManager.ShowRewardBasedVideo((bool reward) =>
            {
                if (reward)
                {
                    Tween.NextFrame(delegate
                    {
                        RefreshProducts();

                        InitDeals();
                    });
                }
            });
        }

        private void ResetRefreshTime()
        {
            // Reset time
            double dayTime = TimeUtils.GetCurrentDayUnixTimestamp();
            dailyDealsRefreshTime = dayTime + (TimeUtils.GetCurrentUnixTimestamp() - dayTime > DAY_HALF_SECONDS ? DAY_HALF_SECONDS * 2 : DAY_HALF_SECONDS);
        }

        #region Save/Load
        private void Load()
        {
            DailyDealSave.SaveData saveData = DailyDealSave.Load();

            // Load data from save
            dailyDealsRefreshTime = saveData.RefreshTime;

            dailyDealProducts = new List<DailyDealSave.Deal>();
            if (saveData.Deals != null)
            {
                dailyDealProducts.AddRange(saveData.Deals);
                dailyDealProductIDs = saveData.Deals.Select(x => x.ItemID).ToList();

                ResetRefreshTime();

                InitDeals();
            }
            else
            {
                RefreshProducts();

                InitDeals();
            }
        }

        private void Save()
        {
            DailyDealSave.Save(new DailyDealSave.SaveData(dailyDealProducts.ToArray(), dailyDealsRefreshTime));
        }
        #endregion
    }
}
