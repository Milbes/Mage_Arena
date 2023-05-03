using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    public class IncreaseMaxHealthAbility : Ability
    {
        public IncreaseMaxHealthAbilityData data;

        public override void ActivateAbility(IGameplayEntity owner)
        {
            base.ActivateAbility(owner);

            ApplyAbility();
        }

        public override void StackIncreased()
        {
            base.StackIncreased();

            ApplyAbility();
        }

        private void ApplyAbility()
        {
            owner.IncreaseMaxHealth(data.healthMultipliers[stackAmount]);

            GameObject particle = Object.Instantiate(data.healParticle);

            PlayerController.playerController.StartCoroutine(ParticleFollow(particle.transform, owner.GetTransform(), 0.5f));
        }

        private IEnumerator ParticleFollow(Transform particle, Transform targer, float duration)
        {
            float time = 0;

            while(time < duration)
            {
                particle.position = targer.position;

                yield return null;
                time += Time.deltaTime;
            }

            Object.Destroy(particle.gameObject);
        }
    }
}