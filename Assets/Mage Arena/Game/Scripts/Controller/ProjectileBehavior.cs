#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{

    public class ProjectileBehavior : MonoBehaviour
    {

        public bool TargetsPlayer { get; protected set; }

        public bool CanPassObstaceles { get; protected set; }

        public bool IsOnManualControl { get; set; }

        [System.NonSerialized] public int recochetAmount = 0;

        [SerializeField] ParticleSystem projectileParticle;

        [SerializeField] List<TrailRenderer> trails;

        public Vector3 Position { get => transform.position; set => transform.position = value; }
        public Vector3 Forward { get => transform.forward; set => transform.forward = value; }

        public bool IsActive { get => gameObject.activeSelf; set => gameObject.SetActive(value); }

        public ProjectileInfo projectileInfo;

        public void Init(ProjectileInfo projectileInfo, bool setPosition = true)
        {
            TargetsPlayer = projectileInfo.targetsPlayer;

            CanPassObstaceles = projectileInfo.canPassObstacles;

            if (setPosition)
            {
                Position = projectileInfo.spawnPosition;

                Forward = projectileInfo.direction;
            }

            this.projectileInfo = projectileInfo;

            recochetAmount = 0;

            IsOnManualControl = false;

            if(projectileParticle != null)
            {
                projectileParticle.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
                projectileParticle.Play();
            }

            if(trails != null)
            {
                for (int i = 0; i < trails.Count; i++)
                {
                    trails[i].Clear();
                }
            }
            
        }

        public bool UpdatePosition(ProjectilesController.ProjectileMovementInfo info)
        {
            Vector3 path = info.velocity * Time.fixedDeltaTime;

            info.traversedDistance += path.magnitude;

            if (info.traversedDistance > info.data.Distance)
            {
                IsActive = false;

                return false;
            }

            Position += path;

            return true;

        }

        public void OnTriggerEnter(Collider collision)
        {
            int otherLayer = collision.gameObject.layer;

            if (otherLayer == GameController.OBSTACLE_LAYER)
            {
                if (!IsOnManualControl)
                {
                    if (!CanPassObstaceles)
                    {
                        IsActive = false;
                        ProjectilesController.RemoveProjectile(this);

                        if (AbilitiesController.OnProjectileHitObstacle(this, collision))
                        {
                            IsActive = false;
                            ProjectilesController.RemoveProjectile(this);
                        }

                        GameAudioController.PlaySound(projectileInfo.projectile.ObstacleHitSound);
                    }
                }
                
            } else if (otherLayer == GameController.BORDER_LAYER)
            {
                if(!IsOnManualControl){
                    if (AbilitiesController.OnProjectileHitObstacle(this, collision))
                    {
                        IsActive = false;
                        ProjectilesController.RemoveProjectile(this);
                    }

                    GameAudioController.PlaySound(projectileInfo.projectile.ObstacleHitSound);
                }

            }
            else
            {
                if (TargetsPlayer)
                {
                    if (otherLayer != GameController.ENEMY_LAYER && !IsOnManualControl)
                    {
                        IsActive = false;
                        ProjectilesController.RemoveProjectile(this);
                    }
                    if(otherLayer == GameController.PLAYER_LAYER)
                    {
                        PlayerController.playerController.GetHit(projectileInfo);

                        if(projectileInfo.projectile != null)
                        {
                            GameAudioController.PlaySound(projectileInfo.projectile.HitAudio);
                        }
                        
                    }
                }
                else
                {
                    if (otherLayer != GameController.PLAYER_LAYER)
                    {
                        if (!IsOnManualControl)
                        {
                            IsActive = false;
                            ProjectilesController.RemoveProjectile(this);
                        }

                        if (otherLayer == GameController.ENEMY_LAYER)
                        {
                            EnemyBehavior enemy = collision.GetComponent<EnemyBehavior>();

                            enemy.GetHit(projectileInfo);

                            if(!IsOnManualControl) AbilitiesController.ProjectileHitTarget(projectileInfo, enemy);
                            if(projectileInfo.statusEffects != null) StatusEffectsController.RegisterEffect(enemy, projectileInfo);

                            GameAudioController.PlaySound(projectileInfo.projectile.HitAudio);
                        }
                    }
                }
            }
        }
    }

    public class ProjectileInfo
    {
        public Projectile projectile;

        public IGameplayEntity owner;

        public List<Damage> damages;
        public List<StatusEffectInfo> statusEffects;

        public Vector3 spawnPosition;
        public Vector3 direction;

        public bool targetsPlayer;
        public bool canPassObstacles;

        public static ProjectileInfo Clone(ProjectileInfo original)
        {
            var damages = new List<Damage>(original.damages.Count);
            var statusEffects = new List<StatusEffectInfo>();

            for(int i = 0; i < original.damages.Count; i++)
            {
                damages.Add(original.damages[i].Copy());
            }

            statusEffects.AddRange(original.statusEffects);

            return new ProjectileInfo
            {
                projectile = original.projectile,
                owner = original.owner,
                damages = damages,
                statusEffects = statusEffects,
                spawnPosition = original.spawnPosition,
                direction = original.direction,
                targetsPlayer = original.targetsPlayer,
                canPassObstacles = original.canPassObstacles
            };
        }

        public void ReduceDamage(float damagePercent)
        {
            for(int i = 0; i < damages.Count; i++)
            {
                damages[i].amount *= damagePercent;
            }
        }
    }
}