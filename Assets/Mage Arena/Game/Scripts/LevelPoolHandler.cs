#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watermelon;

public class LevelPoolHandler : MonoBehaviour
{

    private Dictionary<Obstacle, Pool> obstaclePools;
    private Dictionary<Enemy, Pool> enemyPools;

    public void InitPools()
    {
        obstaclePools = new Dictionary<Obstacle, Pool>();
        enemyPools = new Dictionary<Enemy, Pool>();

        InitObstaclePools();
        InitEnemyPools();
    }

    private void InitObstaclePools()
    {
        for (int i = 0; i < GameController.LevelDatabase.ObstaclesCount; i++)
        {
            Obstacle obstacle = GameController.LevelDatabase.GetObstacle(i);

            PoolSettings poolSettings = new PoolSettings
            {
                autoSizeIncrement = true,
                objectsContainer = null,
                size = 5,
                type = Pool.PoolType.Single,
                singlePoolPrefab = obstacle.Prefab,
                name = "Obstacle_" + i
            };

            Pool obstaclePool = PoolManager.AddPool(poolSettings);
            obstaclePools.Add(obstacle, obstaclePool);
        }
    }

    private void InitEnemyPools()
    {
        for (int i = 0; i < GameController.LevelDatabase.EnemiesCount; i++)
        {
            Enemy movable = GameController.LevelDatabase.GetEnemy(i);

            PoolSettings poolSettings = new PoolSettings
            {
                autoSizeIncrement = true,
                objectsContainer = null,
                size = 5,
                type = Pool.PoolType.Single,
                singlePoolPrefab = movable.Prefab,
                name = "Enemy_" + i
            };

            Pool movablePool = PoolManager.AddPool(poolSettings);

            enemyPools.Add(movable, movablePool);
        }
    } 

    public Pool GetPoolByEnemy(Enemy enemy)
    {
        return enemyPools[enemy];
    }

    public Pool GetPoolByObstacle(Obstacle obstacle)
    {
        return obstaclePools[obstacle];
    }

    public void ReturnObstaclesToPool()
    {
        foreach (Pool obstaclePool in obstaclePools.Values)
        {
            obstaclePool.ReturnToPoolEverything();
        }
    }

    public void ReturnEnemiesToPool()
    {
        foreach (Pool movablePool in enemyPools.Values)
        {
            movablePool.ReturnToPoolEverything();
        }
    }

    public void DestroyPools()
    {
        foreach (Pool movablePool in enemyPools.Values)
        {
            PoolManager.DestroyPool(movablePool);
        }

        foreach (Pool obstaclePool in obstaclePools.Values)
        {
            PoolManager.DestroyPool(obstaclePool);
        }
    }
}
