#pragma warning disable 649, 414

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watermelon;

public class ProjectilesController : MonoBehaviour
{

    [SerializeField] List<Projectile> projectileData;


    private static Dictionary<Projectile, Pool> projectilePools;

    private static List<ProjectileMovementInfo> aliveProjectiles;

    private static List<DragonProjectileInfo> aliveDragonProjectiles;
    private static List<GolemProjectile> golemProjectiles;

    private void Awake()
    {
        projectilePools = new Dictionary<Projectile, Pool>();
        aliveProjectiles = new List<ProjectileMovementInfo>();

        aliveDragonProjectiles = new List<DragonProjectileInfo>();
        golemProjectiles = new List<GolemProjectile>();

        InitPools();
    }

    public static bool CreateProjectilePool(Projectile projectile)
    {
        if (projectilePools.ContainsKey(projectile)) return false;

        projectilePools.Add(projectile, PoolManager.AddPool(new PoolSettings
        {
            autoSizeIncrement = true,
            objectsContainer = null,
            size = 10,
            type = Pool.PoolType.Single,
            singlePoolPrefab = projectile.PrefabRefference,
            name = "Projectile_" + projectilePools.Count
        }));

        return true;
    }

    public static void ChangeVelocity(ProjectileBehavior projectile, Vector3 newDirection)
    {
        for (int i = 0; i < aliveProjectiles.Count; i++)
        {
            ProjectileMovementInfo info = aliveProjectiles[i];

            if(info.projectile == projectile)
            {
                info.velocity = newDirection.normalized * info.velocity.magnitude;

                break;
            }
        }
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < aliveProjectiles.Count; i++)
        {
            ProjectileMovementInfo info = aliveProjectiles[i];

            ProjectileBehavior projectile = info.projectile;

            if (projectile != null && (!projectile.IsActive || !projectile.UpdatePosition(info)))
            {
                aliveProjectiles.RemoveAt(i);
                i--;
            }
        }

        for (int i = 0; i < aliveDragonProjectiles.Count; i++)
        {
            DragonProjectileInfo info = aliveDragonProjectiles[i];

            DragonProjectileBehaviour projectile = info.projectile;

            if (projectile.gameObject == null || !projectile.IsActive || !projectile.UpdatePosition(info))
            {
                aliveDragonProjectiles.RemoveAt(i);
                i--;
            }
        }
    }

    
    public static void RemoveProjectile(ProjectileBehavior projectile)
    {
        for (int i = 0; i < aliveProjectiles.Count; i++)
        {
            ProjectileMovementInfo info = aliveProjectiles[i];

            if(info.projectile == projectile)
            {
                aliveProjectiles.RemoveAt(i);
                i--;
            }
        }

        for (int i = 0; i < aliveDragonProjectiles.Count; i++)
        {
            DragonProjectileInfo info = aliveDragonProjectiles[i];

            if (info.projectile == projectile)
            {
                aliveDragonProjectiles.RemoveAt(i);
                i--;
            }
        }
    }

    private void InitPools()
    {
        for(int i = 0; i < projectileData.Count; i++)
        {
            Projectile data = projectileData[i];

            PoolSettings poolSettings = new PoolSettings
            {
                autoSizeIncrement = true,
                objectsContainer = null,
                size = 10,
                type = Pool.PoolType.Single,
                singlePoolPrefab = data.PrefabRefference,
                name = "Projectile_" + i
            };

            Pool pool = PoolManager.AddPool(poolSettings);

            projectilePools.Add(data, pool);
        }
    }

    public static void DestroyPools()
    {
        foreach (Pool projectilePool in projectilePools.Values)
        {
            projectilePool.ReturnToPoolEverything();

            PoolManager.DestroyPool(projectilePool);
        }

    }

    public static MeteorProjectileBehaviour GetMeteorProjectile(Projectile projectile)
    {
        Pool pool = projectilePools[projectile];

        if (pool == null) return null;

        GameObject projectileObject = pool.GetPooledObject();

        return projectileObject.GetComponent<MeteorProjectileBehaviour>();
    }

    public static void ShootGolemProjectile(ProjectileInfo projectileInfo)
    {
        Pool pool = projectilePools[projectileInfo.projectile];

        if (pool == null) return;

        GameObject projectileObject = pool.GetPooledObject();

        GolemProjectile projectile = projectileObject.GetComponent<GolemProjectile>();

        projectile.transform.position = projectileInfo.spawnPosition;
        projectile.transform.forward = projectileInfo.direction;

        //projectile.da= damage;

        projectile.Attack(projectileInfo);

        golemProjectiles.Add(projectile);
    }


    public static void ShootProjectile(ProjectileInfo projectileInfo)
    {
        Pool pool = projectilePools[projectileInfo.projectile];

        if (pool == null) return;

        GameObject projectileObject = pool.GetPooledObject();

        ProjectileBehavior projectile = projectileObject.GetComponent<ProjectileBehavior>();
        projectile.Init(projectileInfo);

        if (projectile is DragonProjectileBehaviour)
        {
            DragonProjectileInfo info = new DragonProjectileInfo();
            info.data = projectileInfo.projectile;
            info.projectile = projectile as DragonProjectileBehaviour;
            info.velocity = (projectileInfo.direction * projectileInfo.projectile.Speed).SetY(0);

            info.initialPosition = projectileInfo.spawnPosition;

            info.explosionPoint = PlayerController.Position;

            aliveDragonProjectiles.Add(info);
        } else
        {
            ProjectileMovementInfo info = new ProjectileMovementInfo();
            info.data = projectileInfo.projectile;
            info.projectile = projectile;
            info.velocity = (projectileInfo.direction * projectileInfo.projectile.Speed).SetY(0);

            aliveProjectiles.Add(info);
        }
    }


    public class ProjectileMovementInfo
    {
        public Vector3 velocity;
        public ProjectileBehavior projectile;
        public Projectile data;

        public float traversedDistance = 0;
    }

    public class DragonProjectileInfo : ProjectileMovementInfo
    {
        public Vector3 initialPosition;
        public Vector3 explosionPoint;

        public new DragonProjectileBehaviour projectile;
    }

    public static void DisposeProjectiles()
    {
        foreach(Projectile projectile in projectilePools.Keys)
        {
            projectilePools[projectile].ReturnToPoolEverything();
        }

        foreach(GolemProjectile projectile in golemProjectiles)
        {
            projectile.Disable();
        }

        foreach(DragonProjectileInfo projectile in aliveDragonProjectiles)
        {
            projectile.projectile.Disable();
        }
    }

}
