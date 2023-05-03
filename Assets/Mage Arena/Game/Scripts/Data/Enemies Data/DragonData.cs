#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Enemy/Dragon", fileName = "Dragon Data")]
public class DragonData : EnemyData
{
    [Header("Time Durations")]
    [SerializeField] float idleDuration;
    [SerializeField] float idleAfterRunningDuration;

    [Header("Projectiles Settings")]
    [SerializeField] Projectile projectile;

    [Header("Player Distances")]
    [SerializeField] float playerTooCloseDistance;
    [SerializeField] float runAwayFromPlayerDistance;

    [Space]
    [SerializeField] float projectileDamage;
    [SerializeField] float startFiringDelay;

    public float IdleDuration => idleDuration;
    public float IdleAfterRunningDuration => idleAfterRunningDuration;

    public float PlayerTooCloseDistance => playerTooCloseDistance;
    public float RunAwayFromPlayerDistance => runAwayFromPlayerDistance;


    public Projectile Projectile => projectile;
    public float ProjectileDamage => GameController.CurrentRoom.DamageMultiplier * projectileDamage;
    public float StartFiringDelay => startFiringDelay;
}
