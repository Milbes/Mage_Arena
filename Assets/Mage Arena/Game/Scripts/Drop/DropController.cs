using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watermelon;

public class DropController : MonoBehaviour
{
    private static readonly string COIN_POOL_NAME = "Coin";
    private static readonly string GEM_POOL_NAME = "Gem";
    private static readonly string DROP_ITEM_POOL_NAME = "Drop Item";

    private static DropController instance;

    private static Pool coinPool;
    private static Pool gemPool;
    private static Pool dropItemPool;

    private static List<DropCurrency> currencies;
    private static List<DropItemBehavior> dropItems;

    public static List<ItemHolder> pickedUpItems;
    
    [SerializeField] AudioClip currenciesSpawnAudio;

    private static int lastRoomId = -1;
    private static int itemsDroppedAmount = -1;

    private void Awake()
    {
        instance = this;

        coinPool = PoolManager.GetPoolByName(COIN_POOL_NAME);
        gemPool = PoolManager.GetPoolByName(GEM_POOL_NAME);
        dropItemPool = PoolManager.GetPoolByName(DROP_ITEM_POOL_NAME);


        if (currencies == null)
        {
            currencies = new List<DropCurrency>();
        } else
        {
            currencies.Clear();
        }

        if(dropItems == null)
        {
            dropItems = new List<DropItemBehavior>();
        } else
        {
            dropItems.Clear();
        }

        pickedUpItems = new List<ItemHolder>();

        lastRoomId = -1;
        itemsDroppedAmount = -1;
    }

    public static void CheckDropItem(Vector3 dropPosition)
    {
        if(lastRoomId != GameController.CurrentRoomId)
        {
            itemsDroppedAmount = 0;

            lastRoomId = GameController.CurrentRoomId;
        }

        if(itemsDroppedAmount < GameController.CurrentRoom.ItemsToDropAmount)
        {
            itemsDroppedAmount++;

            Item[] allowedItems = ItemDatabase.GetItems();

            ItemHolder itemHolder = allowedItems.GetRandomItem().GetDefaultHolder();
            itemHolder.SetItemRarity(GameController.CurrentRoom.DropRarity);
            itemHolder.SetItemLevel(Account.Level);

            if(Random.value > 0.5f)
            {
                DropItem(itemHolder, dropPosition);
            }
        }
    }

    public static void PickUpItem(ItemHolder holder)
    {
        pickedUpItems.Add(holder);
    }

    public static void SpawnCoins(Vector3 position)
    {
        coinPool = PoolManager.GetPoolByName(COIN_POOL_NAME);

        for (int i = 0; i < 5; i++)
        {
            DropCurrency coin = coinPool.GetPooledObject().GetComponent<DropCurrency>();

            coin.transform.position = position + Vector3.up * 0.2f;

            coin.Init();

            currencies.Add(coin);
        }

        AudioController.PlaySound(instance.currenciesSpawnAudio, GameController.Sound * 2);
    }

    public static void SpawnGems(Vector3 position)
    {
        gemPool = PoolManager.GetPoolByName(GEM_POOL_NAME);

        for (int i = 0; i < 5; i++)
        {
            DropCurrency gem = gemPool.GetPooledObject().GetComponent<DropCurrency>();

            gem.transform.position = position + Vector3.up * 0.2f;

            gem.Init();

            currencies.Add(gem);
        }

        AudioController.PlaySound(instance.currenciesSpawnAudio, GameController.Sound * 2);
    }

    public static void DropItem(ItemHolder item, Vector3 position)
    {
        DropItemBehavior dropItem = dropItemPool.GetPooledObject().GetComponent<DropItemBehavior>();

        dropItem.Init(item);
        dropItem.transform.position = position;

        dropItems.Add(dropItem);
    }

    public static void SpawnSingleGem(Vector3 position)
    {
        DropCurrency gem = gemPool.GetPooledObject().GetComponent<DropCurrency>();

        gem.transform.position = position + Vector3.up * 0.2f;

        gem.Init();

        currencies.Add(gem);
    }

    public static void CollectCoins()
    {
        for(int i = 0; i < currencies.Count; i++)
        {
            currencies[i].FollowTransform(PlayerController.Transform);
        }

        for(int i = 0; i < dropItems.Count; i++)
        {
            dropItems[i].FollowTransform(PlayerController.Transform);
        }
    }

    public static void RemoveCurrency(DropCurrency currency)
    {
        currencies.Remove(currency);
    }

    public static void RemoveCoinsToPool()
    {
        dropItemPool.ReturnToPoolEverything();
        coinPool.ReturnToPoolEverything();
        gemPool.ReturnToPoolEverything();
    }

}
