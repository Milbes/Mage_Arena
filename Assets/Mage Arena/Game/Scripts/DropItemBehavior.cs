using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    public class DropItemBehavior : MonoBehaviour
    {
        [SerializeField] SpriteRenderer spriteRenderer;

        public ItemHolder DropItemHolder { get; private set; }

        private Transform target;
        private bool isFollowing;

        private float speed;

        public void Init(ItemHolder itemHolder)
        {
            DropItemHolder = itemHolder;

            spriteRenderer.sprite = itemHolder.Item.Sprite;
            spriteRenderer.size = Vector3.one;

            speed = 50;

            spriteRenderer.transform.forward = -CameraController.MainCamera.transform.forward;
        }

        public void FollowTransform(Transform transform)
        {
            target = transform;

            isFollowing = true;
        }

        public void FixedUpdate()
        {
            if (isFollowing)
            {

                if (Vector3.Distance(target.transform.position, transform.position) < 2)
                {
                    gameObject.SetActive(false);
                    isFollowing = false;
                    target = null;

                    DropController.PickUpItem(DropItemHolder);

                    return;
                }

                transform.position = transform.position + (target.position - transform.position).normalized * speed * Time.deltaTime;

                //constantForce.force = (target.transform.position - transform.position).normalized * 30;
            }
        }
    }
}

