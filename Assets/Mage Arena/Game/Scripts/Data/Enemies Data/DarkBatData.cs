using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Enemy/Bat/Dark", fileName = "Dark Bat Data")]
public class DarkBatData : EnemyData
{

    [Header("Idle")]
    [SerializeField] float idleDuration;

    [Header("Teleportation")]
    [SerializeField] GameObject teleportationParticle;

    [Header("Projectiles Settings")]
    [SerializeField] Projectile projectile;

    [Space]
    [SerializeField] float projectileDamage;

    public float IdleDuration => idleDuration;
    public GameObject TeleportationParticle => teleportationParticle;
    public Projectile Projectile => projectile;
    public float ProjectileDamage => GameController.CurrentRoom.DamageMultiplier * projectileDamage;
}
