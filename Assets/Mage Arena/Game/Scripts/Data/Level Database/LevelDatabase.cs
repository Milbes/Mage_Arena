#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{

    [CreateAssetMenu(menuName = "Level Database/Level Database", fileName = "Level Database")]
    public class LevelDatabase : ScriptableObject
    {
        [SerializeField] List<Level> levels;

        [SerializeField] List<Enemy> enemies;
        [SerializeField] List<Obstacle> obstacles;

        //Editor stuff
        [SerializeField] Texture2D placeholderTexture;
        [SerializeField] Texture2D greenTexture;
        [SerializeField] Texture2D redTexture;
        [SerializeField] Texture2D itemBackgroundTexture;

        public int LevelsCount => levels.Count;

        public int EnemiesCount => enemies.Count;
        public int ObstaclesCount => obstacles.Count;

        public Zone GetZone(int index)
        {
            // DEV
            return new Zone("Magic Forest", levels.ToArray());
        }

        public Level GetLevel(int index)
        {
            if (index < 0 || index >= LevelsCount) return null;

            return levels[index];
        }

        public Enemy GetEnemy(int index)
        {
            if (index < 0 || index >= EnemiesCount) return null;

            return enemies[index];
        }

        public Obstacle GetObstacle(int index)
        {
            if (index < 0 || index >= ObstaclesCount) return null;

            return obstacles[index];
        }

        public int GetNextLevelId(int levelId, bool isOverflow)
        {
            if (!isOverflow)
            {
                return levelId + 1;
            }
            else
            {
                int nextLevel;

                do
                {
                    nextLevel = Random.Range(0, LevelsCount);
                } while (nextLevel == levelId);

                return nextLevel;
            }
        }
    }
}