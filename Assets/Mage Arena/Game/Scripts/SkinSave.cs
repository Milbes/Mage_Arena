using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkinSave
{
    [SerializeField] int dataId = 0;

    public int playerLevel = 0;

    public int healthLevel = 0;
    public int movementSpeedLevel = 0;
    public int attackSpeedLevel = 0;
    public int damageLevel = 0;
    public int critChanceLevel = 0;
    public int healthRegenLevel = 0;

    public int DataId => dataId;

    public SkinSave(int skinDataId)
    {
        dataId = skinDataId;
    }
}
