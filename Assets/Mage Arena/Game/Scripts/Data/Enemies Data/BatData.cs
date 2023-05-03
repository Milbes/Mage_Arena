#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Enemy/Bat/Default", fileName = "Bat Data")]
public class BatData : EnemyData
{
    

    [Header("Time Durations")]
    [SerializeField] float idleDuration;
    [SerializeField] float idleAfterRunningDuration;

    [Header("Player Distances")]
    [SerializeField] float playerTooCloseDistance;
    [SerializeField] float runAwayFromPlayerDistance;

    [Header("Projectiles Settings")]
    [SerializeField] Projectile projectile;

    [Space]
    [SerializeField] float projectileDamage;
    [SerializeField] float startFiringDelay;
    [SerializeField] float delayBetweenShots;

    

    public float IdleDuration => idleDuration;
    public float IdleAfterRunningDuration => idleAfterRunningDuration;


    public float PlayerTooCloseDistance => playerTooCloseDistance;
    public float RunAwayFromPlayerDistance => runAwayFromPlayerDistance;


    public Projectile Projectile => projectile;
    public float ProjectileDamage => GameController.CurrentRoom.DamageMultiplier * projectileDamage;
    public float StartFiringDelay =>  startFiringDelay;
    public float DelayBetweenShots => delayBetweenShots;

}
