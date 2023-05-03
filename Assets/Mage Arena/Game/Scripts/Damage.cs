using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Watermelon
{
    [System.Serializable]
    public class Damage {
        public static readonly DamageType[] DamageTypes = (DamageType[])Enum.GetValues(typeof(DamageType));

        public DamageType type;
        public float amount;
        public bool isCrit;

        public Damage Copy()
        {
            return new Damage { 
                type = type,
                amount = amount,
                isCrit = isCrit
            };
        }
    }

    /*
    public class Damage
    {
        

        Dictionary<DamageType, float> damageInfo = new Dictionary<DamageType, float>();

        public bool isCrit;

        public float DamageSum {
            get {
                float sum = 0;

                for(int i = 0; i < DamageTypes.Length; i++)
                {
                    sum += this[DamageTypes[i]];
                }

                return sum;
            }
        }

        public float this[DamageType type]
        {
            get 
            {
                if (damageInfo.ContainsKey(type)) return damageInfo[type];

                return 0;
            }

            set
            {
                if (damageInfo.ContainsKey(type))
                {
                    damageInfo[type] += value; 
                }
                else
                {
                    damageInfo.Add(type, value);
                }
            }
        }

        public static Damage operator +(Damage a, Damage b)
        {
            //Damage damage = new Damage();

            for(int i = 0; i <= DamageTypes.Length; i++)
            {
                DamageType type = DamageTypes[i];

                //damage[type] = a[type] + b[type];

                a[type] =+ b[type];
            }

            a.isCrit = a.isCrit || b.isCrit;

            //damage.isCrit = a.isCrit || b.isCrit;

            return a;
        }

        public bool IsEmpty()
        {
            for(int i = 0; i < DamageTypes.Length; i++)
            {
                if (this[DamageTypes[i]] != 0) return false;
            }

            return true;
        }

        public void Clear()
        {
            damageInfo.Clear();
        }
    }*/


    public enum DamageType
    {
        Base = 0, 
        Fire = 1, 
        Storm = 2, 
        Shadow = 3, 
        Ice = 4
    }

}