using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Fire Effect", menuName = "Inventory/Fire Effect")]
    public class FireSpecialEffect : ItemSpecialEffect
    {
        [SerializeField] int time;
        [SerializeField] int fireDamage;

        public override void OnEnemyDies(EnemyBehavior enemy)
        {

        }

        public override void OnEnemyHitted(EnemyBehavior enemy, float damage, bool isCritical)
        {
            //Debug.Log("Enemy hitted");

            EnemyEffectsController.RegisterEffect(enemy, new FireEnemyEffect(EnemyEffectType.Poison, time, fireDamage));
        }

        public override void OnLevelStarted()
        {
            //Debug.Log("Level started");
        }

        public override float OnPlayerHitted(float damage)
        {
            //Debug.Log("Player hitted");

            return damage;
        }

        public override void OnRoomStarted()
        {
            //Debug.Log("Room started");
        }

        public override void OnEffectDisabled()
        {

        }
    }
}
