#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watermelon;

public class LevelObjectSpawner : MonoBehaviour
{
    private static LevelObjectSpawner levelObjectSpawner;

    public List<EnemyBehavior> enemies;
    public List<ObstacleBehavior> obstacles;

    private LevelPoolHandler poolHandler;

    public static List<EnemyBehavior> Enemies => levelObjectSpawner.enemies;

    private void Awake()
    {
        levelObjectSpawner = this;

        enemies = new List<EnemyBehavior>();
        obstacles = new List<ObstacleBehavior>();

        poolHandler = GetComponent<LevelPoolHandler>();
    }

    public List<Vector2> GetObstaclesPlaces()
    {
        List<Vector2> places = new List<Vector2>();

        Level.Room room = GameController.CurrentRoom;

        for (int i = 0; i < room.ObstaclesCount; i++)
        {
            Level.ObstacleData obstacleData = room.GetObstacleData(i);

            places.Add(obstacleData.Position);
        }

        return places;
    }

    public void SpawnEnemies()
    {
        Level.Room room = GameController.CurrentRoom;

        for(int i = 0; i < room.EnemiesCount; i++)
        {
            Level.EnemyData enemyData = room.GetEnemyData(i);

            Pool pool = poolHandler.GetPoolByEnemy(enemyData.Enemy);

            EnemyBehavior enemy = pool.GetPooledObject().GetComponent<EnemyBehavior>();

            enemy.Init(enemyData.Position, enemyData.Enemy);

            enemies.Add(enemy);
        }
    }

    public void SpawnAdditionalEnemy(Enemy enemy, Transform spawner, string message = "")
    {
        Pool pool = poolHandler.GetPoolByEnemy(enemy);

        EnemyBehavior enemyBehaviour = pool.GetPooledObject().GetComponent<EnemyBehavior>();

        enemyBehaviour.Init(enemy, spawner, message);

        enemies.Add(enemyBehaviour);
    }

    public void SpawnAdditionalEnemy(Enemy enemy, Vector3 position, string message = "")
    {
        Pool pool = poolHandler.GetPoolByEnemy(enemy);

        EnemyBehavior enemyBehaviour = pool.GetPooledObject().GetComponent<EnemyBehavior>();

        enemyBehaviour.Init(enemy, position, (PlayerController.Position - position).SetY(0).normalized, message);

        enemies.Add(enemyBehaviour);
    }

    public void SpawnAdditionalEnemy(Enemy enemy, Vector3 position)
    {
        Pool pool = poolHandler.GetPoolByEnemy(enemy);

        EnemyBehavior enemyBehaviour = pool.GetPooledObject().GetComponent<EnemyBehavior>();

        enemyBehaviour.Init(new Vector2Int((int)position.x, (int)position.z), enemy);

        enemies.Add(enemyBehaviour);
    }

    public void SpawnObstacles()
    {
        Level.Room room = GameController.CurrentRoom;

        for (int i = 0; i < room.ObstaclesCount; i++)
        {
            Level.ObstacleData obstacleData = room.GetObstacleData(i);

            Pool pool = poolHandler.GetPoolByObstacle(obstacleData.Obstacle);

            ObstacleBehavior obstacle = pool.GetPooledObject().GetComponent<ObstacleBehavior>();

            obstacle.Init(obstacleData.Position);

            obstacles.Add(obstacle);
        }

        WaterObstacleBehaviour.SetUpWater();
    }

    public void DiactivateEnemies()
    {
        for(int i = 0; i < enemies.Count; i++)
        {
            enemies[i].Diactivate();
        }

        enemies.Clear();
    }
}
