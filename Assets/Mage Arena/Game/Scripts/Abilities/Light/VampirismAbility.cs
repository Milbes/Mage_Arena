using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{

    public class VampirismAbility : Ability, IOnEnemyDiedAbility
    {
        public VampirismAbilityData data;
        

        public void OnEnemyDied(IGameplayEntity enemy)
        {
            owner.Heal(data.vampirismMultiplier[stackAmount]);

            GameObject particle = Object.Instantiate(data.healParticle);

            particle.transform.localScale = Vector3.one * 2f;

            PlayerController.playerController.StartCoroutine(ParticleFollow(particle.transform, owner.GetTransform(), 0.5f));
        }

        private IEnumerator ParticleFollow(Transform particle, Transform targer, float duration)
        {
            float time = 0;

            while (time < duration)
            {
                particle.position = targer.position;

                yield return null;
                time += Time.deltaTime;
            }

            Object.Destroy(particle.gameObject);
        }
    }

}