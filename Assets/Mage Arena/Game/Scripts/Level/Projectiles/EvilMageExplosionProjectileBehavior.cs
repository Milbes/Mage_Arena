using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{

    public class EvilMageExplosionProjectileBehavior : ProjectileBehavior
    {

        [SerializeField] Collider trigger;
        [Space]
        [SerializeField] GameObject warningRadius;
        [Space]
        [SerializeField] ParticleSystem explosionInitParticle;
        [SerializeField] ParticleSystem explosionParticle;

        public new void Init(ProjectileInfo projectileInfo, bool setPosition = true)
        {
            base.Init(projectileInfo, setPosition);

            trigger.enabled = false;

            warningRadius.SetActive(true);

            explosionInitParticle.gameObject.SetActive(true);
            explosionInitParticle.Play();

            IsOnManualControl = true;
        }

        public void Explode()
        {
            explosionInitParticle.gameObject.SetActive(false);
            explosionParticle.gameObject.SetActive(true);
            explosionParticle.Play();

            trigger.enabled = true;

            warningRadius.SetActive(false);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
            explosionParticle.gameObject.SetActive(false);

            trigger.enabled = false;
        }


    }

}