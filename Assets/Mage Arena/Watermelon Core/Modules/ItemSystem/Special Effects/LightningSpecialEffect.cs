using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Lighning Effect", menuName = "Inventory/Ligning Effect")]
    public class LightningSpecialEffect : ItemSpecialEffect
    {
        [SerializeField] int damage;

        private Pool lightningPool;

        private List<CoroutineCase> lightCoroutines = new List<CoroutineCase>();

        public override void OnEnemyDies(EnemyBehavior enemy)
        {
            //Debug.Log("Enemy dies");
        }

        public override void OnEnemyHitted(EnemyBehavior enemy, float damage, bool isCritical)
        {
            //Debug.Log("Enemy hitted");

            lightCoroutines.Add(new CoroutineCase(enemy, LightningCoroutine(enemy)));
        }

        public override void OnLevelStarted()
        {
            //Debug.Log("Level started");

            lightningPool = PoolManager.GetPoolByName("Lightning");
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

        private IEnumerator LightningCoroutine(EnemyBehavior enemy)
        {
            EnemyBehavior[] enemyBehaviors = LevelObjectSpawner.Enemies.ToArray();

            List<EnemyBehavior> hittedEnemies = new List<EnemyBehavior>();

            if (lightningPool != null)
            {
                GameObject lightningObject = lightningPool.GetPooledObject();

                enemy.TakeDamage(15, false, false);
                hittedEnemies.Add(enemy);

                lightningObject.transform.position = enemy.transform.position;
                lightningObject.transform.localScale = Vector3.one;
                lightningObject.SetActive(true);

                for (int i = 0; i < 10; i++)
                {
                    float enemyDistance = float.MaxValue;
                    int closestEnemyIndex = -1;
                    for(int e = 0; e < enemyBehaviors.Length; e++)
                    {
                        if(!enemyBehaviors[e].IsDying && enemyBehaviors[e].gameObject.activeSelf && enemyBehaviors[e] != enemy && hittedEnemies.FindIndex(x => x == enemyBehaviors[e]) == -1)
                        {
                            float tempDistance = Vector3.Distance(enemyBehaviors[e].transform.position, enemy.transform.position);
                            if(tempDistance < enemyDistance)
                            {
                                enemyDistance = tempDistance;
                                closestEnemyIndex = e;
                            }
                        }
                    }

                    if(closestEnemyIndex != -1)
                    {
                        enemy = enemyBehaviors[closestEnemyIndex];
                        hittedEnemies.Add(enemy);

                        lightningObject.transform.SetParent(enemyBehaviors[closestEnemyIndex].transform);
                        lightningObject.transform.localScale = Vector3.one;

                        lightningObject.transform.DOLocalMove(Vector3.zero, 0.2f).OnComplete(delegate
                        {
                            enemy.TakeDamage(15, false, false);

                        });

                        yield return new WaitForSeconds(0.2f);
                    }
                    else
                    {
                        yield return new WaitForSeconds(0.2f);

                        break;
                    }
                }

                lightningObject.transform.SetParent(null);
                lightningObject.SetActive(false);
            }

            for (int i = lightCoroutines.Count - 1; i >= 0; i--)
            {
                if (lightCoroutines[i].IsFinished)
                    lightCoroutines.RemoveAt(i);
            }
        }

        public override void OnEffectDisabled()
        {
            //lightningPool.ReturnToPoolEverything();

            //Debug.Log(lightCoroutines.Count);

            //for(int i = 0; i < lightCoroutines.Count; i++)
            //{
            //    lightCoroutines[i].Disable();
            //}

            //lightCoroutines.Clear();

            //Debug.Log(lightCoroutines.Count);
        }

        private class CoroutineCase
        {
            private Coroutine coroutine;
            private IEnumerator enumerator;
            private MonoBehaviour parent;

            private bool isFinished;
            public bool IsFinished => isFinished;

            public CoroutineCase(MonoBehaviour parent, IEnumerator enumerator)
            {
                isFinished = false;

                this.parent = parent;
                this.enumerator = enumerator;

                coroutine = parent.StartCoroutine(TestEnumerator());
            }

            private IEnumerator TestEnumerator()
            {
                while (enumerator.MoveNext())
                {
                    yield return enumerator.Current;
                }

                isFinished = true;
                Debug.Log("Finished");
            }

            public void Disable()
            {
                if(!isFinished)
                {
                    parent.StopCoroutine(coroutine);
                }

                Debug.Log("Disable");
            }
        }
    }
}
