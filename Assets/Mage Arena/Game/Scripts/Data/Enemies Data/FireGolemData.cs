using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Enemy/Fire Golem", fileName = "Fire Golem Data")]
public class FireGolemData : EnemyData
{

    [Space]
    [SerializeField] Projectile projectile;
    [SerializeField] float projectileDamage;

    public Projectile Projectile => projectile;
    public float ProjectileDamage => GameController.CurrentRoom.DamageMultiplier * projectileDamage;
}
