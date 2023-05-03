#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Enemy/Slime", fileName = "Slime Data")]
public class SlimeData : EnemyData
{
    [Header("Size")]
    [SerializeField] float parentScale;
    [SerializeField] float childScale;

    [Space]
    [SerializeField] float runningMaxDistance;
    [SerializeField] float idleDuration;


    public float ParentScale => parentScale;
    public float ChildScale => childScale;

    public float RunningMaxDistance => runningMaxDistance;

    public float IdleDuration => idleDuration;

}
