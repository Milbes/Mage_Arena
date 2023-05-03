using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Poison Effect", menuName = "Inventory/Poison Effect")]
    public class PoisonSpecialEffect : ItemSpecialEffect
    {
        [SerializeField] int time;
        [SerializeField] int poisonDamage;

        public override void OnEnemyDies(EnemyBehavior enemy)
        {

        }

        public override void OnEnemyHitted(EnemyBehavior enemy, float damage, bool isCritical)
        {
            //Debug.Log("Enemy hitted");

            EnemyEffectsController.RegisterEffect(enemy, new PoisonEnemyEffect(EnemyEffectType.Poison, time, poisonDamage));
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
