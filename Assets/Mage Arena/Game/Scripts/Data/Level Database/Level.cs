#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{

    [CreateAssetMenu(menuName = "Level Database/Level", fileName = "Level")]
    public class Level : ScriptableObject
    {
        [SerializeField] List<Room> rooms;
        [SerializeField] GameObject environmentSkin;

        public int RoomsCount => rooms.Count;
        public GameObject EnvironmentSkin => environmentSkin;

        public Room GetRoom(int index)
        {
            if (index < 0 || index >= RoomsCount) return null;

            return rooms[index];
        }

        [System.Serializable]
        public class Room
        {
            [SerializeField] Vector2Int size;

            [SerializeField] List<ObstacleData> obstacles;
            [SerializeField] List<EnemyData> enemies;

            [SerializeField] bool isBossRoom;

            [Space]
            [SerializeField] float coinsAmount;
            [SerializeField] float gemsAmount;
            [SerializeField] float levelProgression;
            [Space]
            [SerializeField] int itemsToDropAmount;
            [SerializeField] ItemRarity dropRarity;

            public float CoinsAmount => coinsAmount;
            public float GemsAmount => gemsAmount;
            public float LevelProgression => levelProgression;

            public int ItemsToDropAmount => itemsToDropAmount;
            public ItemRarity DropRarity => dropRarity;

            [Header("Enemies Multipliers")]
            [SerializeField] float healthMultiplier = 1;
            [SerializeField] float damageMultiplier = 1;


            public float HealthMultiplier => healthMultiplier;
            public float DamageMultiplier => damageMultiplier;

            public Vector2Int Size => size;

            public int ObstaclesCount => obstacles.Count;
            public int EnemiesCount => enemies.Count;

            public bool IsBossRoom => isBossRoom;

            public ObstacleData GetObstacleData(int index)
            {
                if (index < 0 || index >= obstacles.Count) return null;

                return obstacles[index];
            }

            public EnemyData GetEnemyData(int index)
            {
                if (index < 0 || index >= enemies.Count) return null;

                return enemies[index];
            }
        }

        [System.Serializable]
        public class ObstacleData
        {
            [SerializeField] Obstacle obstacle;
            [SerializeField] Vector2Int position;
            [SerializeField] int angle;

            public Obstacle Obstacle => obstacle;
            public Vector2Int Position => position;
            public int Angle => angle;
        }

        [System.Serializable]
        public class EnemyData
        {
            [SerializeField] Enemy enemy;
            [SerializeField] Vector2Int position;
            [SerializeField] int angle;

            public Enemy Enemy => enemy;
            public Vector2Int Position => position;
            public int Angle => angle;
        }
    }

}