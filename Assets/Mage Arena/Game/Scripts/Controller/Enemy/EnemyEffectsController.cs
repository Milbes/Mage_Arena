#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watermelon;

public class EnemyEffectsController : MonoBehaviour
{
    private static EnemyEffectsController effectsController;

    private List<EnemyBehavior> registredEnemies = new List<EnemyBehavior>();
    private int registredEnemiesCount = 0;

    private bool isControllerActive = false;
    private Coroutine tickCoroutine;

    private void Awake()
    {
        effectsController = this;
    }

    private void Start()
    {
        // DEV
        //GameController.StartGame();

        isControllerActive = true;

        tickCoroutine = StartCoroutine(TickCoroutine());
    }

    private void OnDisable()
    {
        //PoolManager.GetPoolByName("Fire").ReturnToPoolEverything();
        //PoolManager.GetPoolByName("Poison").ReturnToPoolEverything();

        isControllerActive = false;

        StopCoroutine(tickCoroutine);
    }

    private IEnumerator TickCoroutine()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(1.0f);

        while (isControllerActive)
        {
            yield return waitForSeconds;

            if(registredEnemiesCount > 0)
            {
                for(int i = 0; i < registredEnemiesCount; i++)
                {
                    registredEnemies[i].DoEffectsTick();

                    if(registredEnemies[i].ActiveEffectsCount == 0)
                    {
                        registredEnemies.RemoveAt(i);
                        registredEnemiesCount--;

                        i--;

                        if (i == -1)
                            break;
                    }
                }
            }
        }
    }

    public static void RegisterEffect(EnemyBehavior enemyBehavior, EnemyEffect enemyEffect)
    {
        int enemyIndex = -1;

        // Find if enemy already registred
        int registeredEnemiesCount = effectsController.registredEnemies.Count;
        for (int i = 0; i < registeredEnemiesCount; i++)
        {
            if (effectsController.registredEnemies[i] == enemyBehavior)
            {
                enemyIndex = i;

                break;
            }
        }

        if (enemyIndex != -1)
        {
            // Add effect to registred enemy
            effectsController.registredEnemies[enemyIndex].AddEffect(enemyEffect);
        }
        else
        {
            // Registred enemy
            effectsController.registredEnemies.Add(enemyBehavior);
            effectsController.registredEnemiesCount++;

            // Add effect
            enemyBehavior.AddEffect(enemyEffect);
        }
    }
}
