#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watermelon;

public class LevelController : MonoBehaviour
{
    private static LevelController instance;

    [SerializeField] Transform finishZone;
    [SerializeField] Transform greenOrbTrigger;

    public static int ProgressionLevel { get; private set; }

    public static ItemHolder TestDropItem 
    {
        get
        {
            Item[] allowedItems = ItemDatabase.GetItems();

            ItemHolder itemHolder = allowedItems.GetRandomItem().GetDefaultHolder();
            ItemRarity itemRarity = ItemSettings.GetRandomRarity();
            itemHolder.SetItemRarity(itemRarity);
            itemHolder.SetItemLevel(Account.Level);

            return itemHolder;
        }
    }

    private static Transform GreenOrbTrigger => instance.greenOrbTrigger;


    private static LevelPoolHandler poolHandler;
    private static LevelObjectSpawner objectSpawner;

    private static EnvironmentBehavior CurrentEnviroment;

    static bool isInFinishState = false;

    public static int deathCountdown = -1;

    private static int currentLevelXP;

    private void Awake()
    {
        instance = this;

        poolHandler = GetComponent<LevelPoolHandler>();
        objectSpawner = GetComponent<LevelObjectSpawner>();
    }

    private void Start()
    {
        poolHandler.InitPools();
    }

    public static void InitLevel()
    {
        if(CurrentEnviroment != null) Destroy(CurrentEnviroment.gameObject);
        CurrentEnviroment = Instantiate(GameController.CurrentLevel.EnvironmentSkin).GetComponent<EnvironmentBehavior>();

        PlayerController.InitLevel();

        // Special effects callback
        SpecialEffectsController.OnLevelStarted();

        currentLevelXP = 0;

        ProgressionLevel = 0;

        Tween.NextFrame(() => GamePanelBehavior.SetProgression(0, false));
    }

    public static void OnEnemyDeth(EnemyBehavior enemy)
    {
        currentLevelXP += (Account.Level * 7 + 45);

        objectSpawner.enemies.Remove(enemy);
    }

    public static void ApplyExpirience()
    {
        Account.AddExperience(currentLevelXP);
        currentLevelXP = 0;
    }

    public static List<Vector2> GetObstaclesPlaces()
    {
        return objectSpawner.GetObstaclesPlaces();
    }

    public static void InitRoom()
    {
        float width = GameController.CurrentRoom.Size.x;
        float depth = GameController.CurrentRoom.Size.y;

        instance.finishZone.position = new Vector3(width / 2f, 0, depth - 1f + 1.5f);
        instance.finishZone.gameObject.SetActive(false);

        CurrentEnviroment.Init();
        CurrentEnviroment.StopExitParticle();

        if (GameController.HealthRoom)
        {
            GreenOrbTrigger.gameObject.SetActive(true);

            GreenOrbTrigger.position = new Vector3(width / 2f, 0, depth - 4);

            SetFinishState();
        } 
        else
        {
            GreenOrbTrigger.gameObject.SetActive(false);

            objectSpawner.SpawnEnemies();
            objectSpawner.SpawnObstacles();
        }

        isInFinishState = false;

        if (GameController.CurrentRoom.IsBossRoom)
        {
            GamePanelBehavior.ShowBossLevelText();
        }

        GamePanelBehavior.NewRoom();

        // Special effects callback
        SpecialEffectsController.OnRoomStarted();

        AbilitiesController.OnRoomEntered();
    }

    public static void DiscardLevel()
    {
        objectSpawner.DiactivateEnemies();
        WaterObstacleBehaviour.Disable();

        poolHandler.ReturnEnemiesToPool();
        poolHandler.ReturnObstaclesToPool();

        CurrentEnviroment.ReturnPropToPool();
        DropController.RemoveCoinsToPool();

        ProjectilesController.DisposeProjectiles();
    }

    public static void DestroyPools()
    {
        poolHandler.DestroyPools();
        ProjectilesController.DestroyPools();
    }

    public static EnemyBehavior GetClosestEnemy(Vector3 playerPosition)
    {
        

        Dictionary<EnemyBehavior, float> distances = new Dictionary<EnemyBehavior, float>();

        for (int i = 0; i < objectSpawner.enemies.Count; i++)
        {
            EnemyBehavior enemy = objectSpawner.enemies[i];

            if (!enemy.gameObject.activeSelf || enemy.IsDying) continue;

            distances.Add(enemy, (enemy.transform.position - playerPosition).magnitude);
        }

        Dictionary<EnemyBehavior, float> unobstructedEnemies = new Dictionary<EnemyBehavior, float>();

        int layer = (int)Mathf.Pow(2, GameController.OBSTACLE_LAYER);

        Vector3 playerElevatedPos = playerPosition + Vector3.up;
        
        foreach (EnemyBehavior enemy in distances.Keys)
        {
            Vector3 enemyElevatedPos = enemy.Position + Vector3.up;

            if (!Physics.Raycast(playerElevatedPos, (enemyElevatedPos - playerElevatedPos).normalized, distances[enemy], layer))
            {
                unobstructedEnemies.Add(enemy, distances[enemy]);
            }
        }

        Dictionary<EnemyBehavior, float> enemiesToConsider;

        if (unobstructedEnemies.Count == 0)
        {
            enemiesToConsider = distances;
        } else
        {
            enemiesToConsider = unobstructedEnemies;
        }

        EnemyBehavior closestEnemy = null;
        float minDistance = float.PositiveInfinity;

        foreach(EnemyBehavior enemy in enemiesToConsider.Keys)
        {
            if(enemiesToConsider[enemy] < minDistance)
            {
                closestEnemy = enemy;

                minDistance = enemiesToConsider[enemy];
            }
        }

        bool allDead = true;

        for(int i = 0; i < objectSpawner.enemies.Count; i++)
        {
            if (objectSpawner.enemies[i].gameObject.activeSelf)
            {
                allDead = false;
                break;
            }
        }

        if (closestEnemy == null && !isInFinishState && allDead)
        {
            if (deathCountdown == -1) {
                deathCountdown = 0;
                return null;
            } else if(deathCountdown != 0)
            {
                deathCountdown--;
                return null;
            }

            deathCountdown = -1;

            isInFinishState = true;

            if (!GameController.HealthRoom)
            {
                DropController.CollectCoins();

                // TODO: change to a real progression here

                bool levelingUp = GameController.CurrentRoom.LevelProgression - ProgressionLevel > 1f;
                if (levelingUp)
                {
                    ProgressionLevel = Mathf.FloorToInt(GameController.CurrentRoom.LevelProgression);

                    OrbType type = AbilitiesController.GetNextAbilityOrbType();

                    AbilitiesController.SpawnOrbObject(type);

                    Tween.DelayedCall(0.5f, () => {

                        AbilitiesController.ShowAbilitiesSelectionWindow(type);
                    });
                } else
                {
                    SetFinishState();
                }

                GamePanelBehavior.SetProgression(GameController.CurrentRoom.LevelProgression - ProgressionLevel, levelingUp, true);

                Currency.ChangeCoins((int) GameController.CurrentRoom.CoinsAmount);
                Currency.ChangeGems((int) GameController.CurrentRoom.GemsAmount);

            }

            
        } else
        {
            deathCountdown = -1;
        }

        return closestEnemy;
        
    }

    public static void SetFinishState()
    {
        instance.finishZone.gameObject.SetActive(true);
        CurrentEnviroment.StartExitParticle();

        
    }

    public static void SpawnAdditionalEnemy(Enemy enemy, Transform spawner, string message = "")
    {
        objectSpawner.SpawnAdditionalEnemy(enemy, spawner, message);
    }

    public static void SpawnAdditionalEnemy(Enemy enemy, Vector3 position, string message = "")
    {
        objectSpawner.SpawnAdditionalEnemy(enemy, position, message);
    }

    public static void GetPathTowardsPlayer()
    {
        CurrentEnviroment.GetPathTowardsPlayer();
    }

    public static void GreenOrbEntered()
    {
        GreenOrbTrigger.gameObject.SetActive(false);
        GamePanelBehavior.ShowGreenOrbPanel();
    }

}
