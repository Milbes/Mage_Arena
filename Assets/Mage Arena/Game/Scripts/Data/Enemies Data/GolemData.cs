#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Enemy/Golem", fileName = "Golem Data")]
public class GolemData : EnemyData
{
    [Header("Behaviour Data")]
    [SerializeField] float idlePhaseDuration;
    [SerializeField] float runningMaxDistance;

    [Header("Firing Data")]
    [SerializeField] float idleBeforeAttack;
    [SerializeField] float actualFireOffset;
    [SerializeField] float delayBeforeIdle;

    [Header("Movement Data")]
    [SerializeField] float movementSpeed;
    public float MovementSpeed => movementSpeed;

    [SerializeField] float idleDuration;
    public float IdleDuration => idleDuration;

    [Space]
    [SerializeField] float shootingDistance;

    [SerializeField] Projectile projectile;
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
