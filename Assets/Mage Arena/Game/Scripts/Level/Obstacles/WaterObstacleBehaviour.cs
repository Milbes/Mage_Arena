#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterObstacleBehaviour : ObstacleBehavior
{

    private static List<WaterObstacleBehaviour> waterObstacles = new List<WaterObstacleBehaviour>();

    [SerializeField] GameObject upperBorder;
    [SerializeField] GameObject downBorder;
    [SerializeField] GameObject leftBorder;
    [SerializeField] GameObject rightBorder;

    [Space]
    [SerializeField] GameObject upperLeftCorner;
    [SerializeField] GameObject upperRightCorner;
    [SerializeField] GameObject downLeftCorner;
    [SerializeField] GameObject downRightCorner;

    public Vector2Int Position { get; private set; }

    public new void Init(Vector2Int position)
    {
        Position = position;

        waterObstacles.Add(this);
    }

    public static void SetUpWater()
    {
        for(int i = 0; i < waterObstacles.Count; i++)
        {
            WaterObstacleBehaviour water = waterObstacles[i];

            bool left = water.HasNeighbour(Vector2Int.left);
            bool right = water.HasNeighbour(Vector2Int.right);
            bool up = water.HasNeighbour(Vector2Int.up);
            bool down = water.HasNeighbour(Vector2Int.down);

            water.upperBorder.SetActive(!up);
            water.downBorder.SetActive(!down);
            water.leftBorder.SetActive(!left);
            water.rightBorder.SetActive(!right);

            bool upLeft = water.HasNeighbour(Vector2Int.up + Vector2Int.left);
            bool upRight = water.HasNeighbour(Vector2Int.up + Vector2Int.right);
            bool downLeft = water.HasNeighbour(Vector2Int.down + Vector2Int.left);
            bool downRight = water.HasNeighbour(Vector2Int.down + Vector2Int.right);

            water.upperLeftCorner.SetActive(up && left && !upLeft);
            water.upperRightCorner.SetActive(up && right && !upRight);
            water.downLeftCorner.SetActive(down && left && !downLeft);
            water.downRightCorner.SetActive(down && right && !downRight);
        }
    }

    private bool HasNeighbour(Vector2Int direction)
    {
        for(int i = 0; i < waterObstacles.Count; i++)
        {
            WaterObstacleBehaviour water = waterObstacles[i];

            if (water == this) continue;

            Vector2Int neighbourPosition = Position + direction;

            if (water.Position == neighbourPosition) return true;
        }

        return false;
    }

    public static void Disable()
    {
        waterObstacles.Clear();
    }
}
