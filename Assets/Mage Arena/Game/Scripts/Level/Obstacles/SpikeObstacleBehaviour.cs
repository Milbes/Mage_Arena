#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{

    public class SpikeObstacleBehaviour : ObstacleBehavior
    {

        [SerializeField] float damage;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == GameController.PLAYER_LAYER)
            {
                PlayerController.TakeDamage(damage);
            }
        }
    }

}