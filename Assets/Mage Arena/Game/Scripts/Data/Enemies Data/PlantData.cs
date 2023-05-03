#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Enemy/Plant", fileName = "Plant Data")]
public class PlantData : EnemyData
{
    [Space]
    [SerializeField] float runningMaxDistance;
    [SerializeField] float idleDuration;
    [SerializeField] float pathRecalculationDuration;

    public float PathRecalculationDuratuion => pathRecalculationDuration;

    [Space]
    [SerializeField] float attackDistance;
    [SerializeField] float attackDelay;


    public float AttackDistance => attackDistance;
    public float AttackDelay => attackDelay;

    public float RunningMaxDistance => runningMaxDistance;

    public float IdleDuration => idleDuration;
}
