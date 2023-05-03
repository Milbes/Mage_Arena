#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Enemy/Turtle", fileName = "Turtle Data")]
public class TurtleData : EnemyData
{
    [Header("Behaviour Data")]
    [SerializeField] float idlePhaseDuration;
    [SerializeField] float runningMaxDistance;
    [SerializeField] float pathRecalculationDuration;

    public float PathRecalculationDuratuion => pathRecalculationDuration;

    [Header("Firing Data")]
    [SerializeField] float idleBeforeAttack;
    [SerializeField] float actualFireOffset;
    [SerializeField] float delayBeforeIdle;

    [Space]
    [SerializeField] float shootingDistance;

    [Header("Projectiles Settings")]
    [SerializeField] Projectile projectile;

    [Space]
    [SerializeField] float projectileDamage;


    public float IdlePhaseDuration => idlePhaseDuration;
    public float RunningMaxDistance => runningMaxDistance;

    public float IdleBeforeAttack => idleBeforeAttack;
    public float ActualFireOffset => actualFireOffset;
    public float DelayBeforeIdle => delayBeforeIdle;

    public float ShootingDistance => shootingDistance;

    public Projectile Projectile => projectile;
    public float ProjectileDamage => GameController.CurrentRoom.DamageMultiplier * projectileDamage;
}
