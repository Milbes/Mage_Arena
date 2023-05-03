using System.Collections;
#pragma warning disable 649

using System.Collections.Generic;
using UnityEngine;
using Watermelon;

public class MeteorProjectileBehaviour : ProjectileBehavior
{
    [SerializeField] Collider explosionTrigger;
    [SerializeField] ParticleSystem explosionParticle;
    [SerializeField] Transform warningCircle;

    public void Explode()
    {
        TargetsPlayer = true;

        explosionTrigger.enabled = false;

        warningCircle.gameObject.SetActive(true);

        warningCircle.localScale = new Vector3(0, 0.05f, 0f);

        warningCircle.DOScale(new Vector3(3, 0.05f, 3), 0.5f);
        Tween.DelayedCall(1.5f, () =>
        {
            explosionTrigger.enabled = true;

            explosionParticle.Play();

            warningCircle.gameObject.SetActive(false);
            Tween.DelayedCall(0.5f, () => {
                explosionTrigger.enabled = false;
                gameObject.SetActive(false);
            });
        });
    }

    
}
