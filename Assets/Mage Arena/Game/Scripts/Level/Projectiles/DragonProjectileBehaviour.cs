#pragma warning disable 649, 414

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watermelon;

public class DragonProjectileBehaviour : ProjectileBehavior
{
    [SerializeField] Collider explosionTrigger;
    [SerializeField] ParticleSystem explosionParticle;

    TweenCase explosionDelay;

    public void Explode()
    {
        explosionTrigger.enabled = true;
        explosionParticle.Play();

        explosionDelay = Tween.DelayedCall(0.5f, () =>
        {

            if(gameObject == null)
            {
                explosionDelay.Kill();
                return;
            }

            explosionTrigger.enabled = false;
            IsActive = false;
        });
    }

    public bool UpdatePosition(ProjectilesController.DragonProjectileInfo info)
    {
        Vector3 path = info.velocity * Time.fixedDeltaTime;

        info.traversedDistance += path.magnitude;

        Position += path;

        float t = (Position.SetY(0) - info.initialPosition.SetY(0)).magnitude / (info.initialPosition.SetY(0) - info.explosionPoint.SetY(0)).magnitude;

        if (t >= 1)
        {
            Explode();

            return false;
        }

        if (t <= 0.5f)
        {
            t *= 2;

            float yCoef = Ease.GetFunction(Ease.Type.SineOut)(t);

            Position = Position.SetY(info.initialPosition.y + 1f * yCoef);
        }
        else
        {
            t = 1 - (t - 0.5f) * 2;

            float yCoef = Ease.GetFunction(Ease.Type.SineOut)(t);

            Position = Position.SetY((info.initialPosition.y + 1f) * yCoef);
        }

        return true;
    }

    public new void OnTriggerEnter(Collider collision)
    {
        int otherLayer = collision.gameObject.layer;

        if (TargetsPlayer)
        {
            if (otherLayer == GameController.PLAYER_LAYER)
            {
                PlayerController.playerController.GetHit(projectileInfo);
            }
        }
    }

    public void Disable()
    {
        if (explosionDelay != null && !explosionDelay.isCompleted) explosionDelay.Kill();
    }
}
