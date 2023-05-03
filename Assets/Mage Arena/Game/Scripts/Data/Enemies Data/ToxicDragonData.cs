using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Enemy/Toxic Dragon", fileName = "Toxic Dragon Data")]
public class ToxicDragonData : EnemyData
{
    [SerializeField] float projectileDamage;
    [SerializeField] float fireRange;

    public float ProjectileDamage => GameController.CurrentRoom.DamageMultiplier * projectileDamage;
    public float FireRange => fireRange;
}
