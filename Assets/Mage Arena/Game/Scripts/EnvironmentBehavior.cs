#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Watermelon;

public class EnvironmentBehavior : MonoBehaviour
{

    private static readonly string TILES_POOL_NAME = "Tiles";
    private static readonly string STONES_POOL_NAME = "Stones"; 
    private static readonly string GRASS_POOL_NAME = "Grass Patch";

    private static readonly int TILING_ID = Shader.PropertyToID("_Tiling");

    [Space]
    [SerializeField] Transform leftEnvironment;
    [SerializeField] Transform rightEnvironment;
    [SerializeField] Transform topEnvironment;
    [SerializeField] Transform bottomEnvironment;

    [Space]
    [SerializeField] Transform levelField;

    [Space]
    [SerializeField] NavMeshSurface navMeshSurface;
    [SerializeField] ParticleSystem exitParticle;

    [Space]
    [SerializeField] List<Transform> exitPoints;

    [Space]
    [SerializeField] Material floorMaterial;

    public void StartExitParticle()
    {
        exitParticle.gameObject.SetActive(true);
        exitParticle.Play();
        //Debug.Log(Start)
    }

    public void StopExitParticle()
    {
        exitParticle.Stop();
        exitParticle.gameObject.SetActive(false);
    }

    public List<Transform> ExitPoints => exitPoints;

    Pool tilesPool;
    Pool stonesPool;
    Pool grassPool;

    private void Awake()
    {
        tilesPool = PoolManager.GetPoolByName(TILES_POOL_NAME);
        stonesPool = PoolManager.GetPoolByName(STONES_POOL_NAME);
        grassPool = PoolManager.GetPoolByName(GRASS_POOL_NAME);
    }

    public void Init()
    {
        tilesPool = PoolManager.GetPoolByName(TILES_POOL_NAME);
        stonesPool = PoolManager.GetPoolByName(STONES_POOL_NAME);
        grassPool = PoolManager.GetPoolByName(GRASS_POOL_NAME);

        tilesPool.ReturnToPoolEverything();
        stonesPool.ReturnToPoolEverything();
        grassPool.ReturnToPoolEverything();

        Level.Room room = GameController.CurrentRoom;

        leftEnvironment.position = new Vector3(-5f, -0.5f, 40);
        rightEnvironment.position = new Vector3(16f, -0.5f, 40);

        topEnvironment.position = new Vector3(5.5f, -0.5f, room.Size.y);
        bottomEnvironment.position = new Vector3(5.5f, -0.5f, -5f);

        levelField.position = new Vector3(room.Size.x / 2f, 0, room.Size.y / 2f);
        levelField.localScale = new Vector3(room.Size.x, room.Size.y, 1);

        floorMaterial.SetVector(TILING_ID, new Vector2(1, room.Size.y / 10f));

        navMeshSurface.BuildNavMesh();

        List<Vector2> places = LevelController.GetObstaclesPlaces();

        for(int i = 0; i < 10; i++)
        {
            Vector2 place = new Vector2(Random.Range(1, GameController.CurrentRoom.Size.x), Random.Range(1, GameController.CurrentRoom.Size.y));

            if (places.Contains(place))
            {
                i--;
                continue;
            }
            Transform tile = tilesPool.GetPooledObject().transform;
            tile.position = new Vector3(place.x + 0.5f, -0.05f, place.y + 0.5f);

            tile.localScale = Vector3.one * 0.5f;

            tile.eulerAngles = Vector3.up * 90 * Random.Range(0, 4);

            places.Add(place);
        }

        for (int i = 0; i < 5; i++)
        {
            Vector2 place = new Vector2(Random.Range(1, GameController.CurrentRoom.Size.x), Random.Range(1, GameController.CurrentRoom.Size.y));

            if (places.Contains(place))
            {
                i--;
                continue;
            }
            Transform stone = stonesPool.GetPooledObject().transform;
            stone.position = new Vector3(place.x + 0.5f, 0, place.y + 0.5f);

            stone.localScale = Vector3.one;

            stone.eulerAngles = Vector3.up * 90 * Random.Range(0, 4);

            places.Add(place);
        }

        for (int i = 0; i < 5; i++)
        {
            Vector2 place = new Vector2(Random.Range(1, GameController.CurrentRoom.Size.x), Random.Range(1, GameController.CurrentRoom.Size.y));

            if (places.Contains(place))
            {
                i--;
                continue;
            }
            Transform grass = grassPool.GetPooledObject().transform;
            grass.position = new Vector3(place.x + 0.5f, 0, place.y + 0.5f);

            grass.localScale = Vector3.one;

            grass.eulerAngles = Vector3.up * 90 * Random.Range(0, 4);

            places.Add(place);
        }
    }

    public void ReturnPropToPool()
    {
        tilesPool.ReturnToPoolEverything();
        stonesPool.ReturnToPoolEverything();
        grassPool.ReturnToPoolEverything();
    }

    public void GetPathTowardsPlayer()
    {
        
    }

}
