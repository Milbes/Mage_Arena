using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Daily Deal Database", menuName = "Content/Daily Deal Database")]
    public class DailyDealDatabase : ScriptableObject
    {
        [SerializeField] DailyDealSettings settings;
        public DailyDealSettings Settings => settings;

        [SerializeField] DailyDealProduct[] products;
        public DailyDealProduct[] Products => products;

        public void Init()
        {
            for (int i = 0; i < products.Length; i++)
            {
                products[i].Init();
            }

            Debug.Log("[Daily Deals]: Initialized!");
        }

        public Item GetItemID(int productIndex)
        {
            if (products.IsInRange(productIndex))
                return products[productIndex].Item;

            return null;
        }

        public void GetUniqueIDs(int amount, ref List<int> usedIDs)
        {
            if (products.Length >= amount + usedIDs.Count)
            {
                int tempProductID = -1;
                List<int> tempUsedIDs = new List<int>();
                for (int i = 0; i < amount; i++)
                {
                    do
                    {
                        tempProductID = Random.Range(0, products.Length);
                    }
                    while (usedIDs.Contains(tempProductID));

                    usedIDs.Add(tempProductID);
                    tempUsedIDs.Add(tempProductID);
                }

                usedIDs = tempUsedIDs;
            }
        }

        public DailyDealProduct[] GetElements(int amount, ref List<int> usedIDs)
        {
            if(products.Length >= amount + usedIDs.Count)
            {
                int tempProductID = -1;
                List<int> tempUsedIDs = new List<int>();
                DailyDealProduct[] tempProducts = new DailyDealProduct[amount];
                for (int i = 0; i < amount; i++)
                {
                    do
                    {
                        tempProductID = Random.Range(0, products.Length);
                    }
                    while (usedIDs.Contains(tempProductID));

                    usedIDs.Add(tempProductID);
                    tempUsedIDs.Add(tempProductID);

                    tempProducts[i] = products[tempProductID];
                }

                usedIDs = tempUsedIDs;

                return tempProducts;
            }

            Debug.LogError("[Daily Deals]: Products amount should be greater than " + (amount + usedIDs.Count));

            return null;
        }
    }
}
