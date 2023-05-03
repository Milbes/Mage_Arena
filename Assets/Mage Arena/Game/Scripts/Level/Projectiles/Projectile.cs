#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Projectile", fileName = "Projectile")]
public class Projectile : ScriptableObject
{
    [SerializeField] float speed;
    [SerializeField] float distance;
    [SerializeField] float damage;
    [SerializeField] bool canPassObstacles;

    [Space]
    [SerializeField] bool isAOE;
    [SerializeField] float radius;

    [Space]
    [SerializeField] GameObject prefabRefference;
    [SerializeField] GameObject particleSpawnProjectile;

    [Space]
    [SerializeField] AudioClip spawnSound;
    [SerializeField] AudioClip hitAudio;
    [SerializeField] AudioClip criticalHitAudio;
    [SerializeField] AudioClip obstacleHitSound;

    public float Speed => speed;
    public float Distance => distance;
    public float Damage => damage;
    public bool CanPassObstacles => canPassObstacles;

    public bool IsAOE => isAOE;
    public float Radius => radius;

    public GameObject PrefabRefference => prefabRefference;
    public GameObject ParticleSpawnProjectile => particleSpawnProjectile;

    public AudioClip SpawnSound => spawnSound;
    public AudioClip ObstacleHitSound => obstacleHitSound;
    public AudioClip HitAudio => hitAudio;
    public AudioClip CriticalHitAudio => criticalHitAudio;
}
