using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public static float CalculateHealth(SkinData data, int level)
    {
        float levelHealth = 0;
        for(int i = 0; i < level; i++)
        {
            levelHealth += data.Upgrades[i].health;
        }

        return data.InitialStats.health + levelHealth;
    }

    public static float CalculateMovementSpeed(SkinData data, int level)
    {
        float levelMovementSpeed = 0;
        for (int i = 0; i < level; i++)
        {
            levelMovementSpeed += data.Upgrades[i].movementSpeed;
        }

        return data.InitialStats.movementSpeed + levelMovementSpeed;
    }

    public static float CalculateAttackSpeed(SkinData data, float weaponAttackSpeed, int level)
    {
        float levelAttackSpeed = 0;
        for (int i = 0; i < level; i++)
        {
            levelAttackSpeed += data.Upgrades[i].attackSpeed;
        }

        return weaponAttackSpeed * (data.InitialStats.movementSpeed - levelAttackSpeed);
    }

    public static float CalculateDamage(SkinData data, float weaponDamage, int level)
    {
        float levelDamage = 0;
        for (int i = 0; i < level; i++)
        {
            levelDamage += data.Upgrades[i].damage;
        }

        return weaponDamage * (data.InitialStats.damage + levelDamage);
    }

    public static float CalculateCriticalChance(SkinData data, int level)
    {
        float levelCriticalChance = 0;
        for (int i = 0; i < level; i++)
        {
            levelCriticalChance += data.Upgrades[i].critDamagePercent;
        }

        return data.InitialStats.critDamagePercent + levelCriticalChance;
    }

    public static float CalculateHealthRegen(SkinData data, int level)
    {
        //float levelHealthRegen = 0;
        //for (int i = 0; i < level; i++)
        //{
        //    levelHealthRegen += data.Upgrades[i].HealthRegenStep;
        //}

        //return data.InitialHealthRegen + levelHealthRegen;

        return 0;
    }
}
