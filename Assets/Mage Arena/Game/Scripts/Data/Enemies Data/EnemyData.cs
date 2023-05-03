#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyData : ScriptableObject
{
    [Header("Core Settings")]
    [SerializeField] float health;
    [SerializeField] float bodyDamage;
    [SerializeField] float speed;
    [SerializeField] int coinsAmount;

    [Header("Audio")]
    [SerializeField] AudioClip shootAudio;
    [SerializeField] AudioClip deathAudio;

    public float Health => health;
    public float BodyDamage => GameController.CurrentRoom.DamageMultiplier * bodyDamage;
    public float Speed => speed;
    

    public AudioClip ShootAudio => shootAudio;
    public AudioClip DeathAudio => deathAudio;
}
