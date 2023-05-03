using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Enemy/Boss/Ice Golem", fileName = "Ice Golem Data")]
public class IceGolemBossData : EnemyData
{

    [Header("Projectiles")]
    [SerializeField] Projectile spikeProjectile;
    [SerializeField] float spikeProjectileDamage;

    [Space]
    [SerializeField] Projectile groundProjectile;
    [SerializeField] float groundProjectileDamage;

    [Header("Idle Data")]
    [SerializeField] float idleDuration;

    public float IdleDuration => idleDuration;

    [Header("Audio")]
    [SerializeField] AudioClip projectileArrackAudio;
    [SerializeField] AudioClip stonesAttackAudio;

    public AudioClip ProjectileAttackAudio => projectileArrackAudio;
    public AudioClip StonesAttackAudio => stonesAttackAudio;

    public Projectile SpikeProjectile => spikeProjectile;
    public Projectile GroundProjectile => groundProjectile;

    public float SpikeProjectileDamage => GameController.CurrentRoom.DamageMultiplier * spikeProjectileDamage;
    public float GroundProjectileDamage => GameController.CurrentRoom.DamageMultiplier * groundProjectileDamage;
}
