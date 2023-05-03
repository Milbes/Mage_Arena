#pragma warning disable 649, 414

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Watermelon;

namespace Watermelon
{

    public class SlimeEnemy : EnemyBehavior
    {

        [SerializeField] Transform slime;
        [Space]
        [SerializeField] Transform firstChildSpawner;
        [SerializeField] Transform secondChildSpawner;

        [Space]
        [SerializeField] ParticleSystem deathParticle;

        SlimeData slimeData;
        private State state;

        bool isPlaying = false;
        bool stopMoving = false;

        private List<TweenCase> tweenCases;

        Vector3 direction;
        Vector3 targetPosition;
        Vector3 hitNormal;

        public override void StartAI()
        {

            tweenCases = new List<TweenCase>();

            slimeData = enemyData as SlimeData;

            if (specialMessage == "child")
            {
                slime.localScale = Vector3.one * slimeData.ChildScale;
            }
            else
            {
                slime.localScale = Vector3.one * slimeData.ParentScale;
            }

            direction = new Vector3(Random.Range(0, 2) * 2 - 1, 0, Random.Range(0, 2) * 2 - 1).normalized;

            Tween.NextFrame(() => {
                navMeshAgent.enabled = true;

                Tween.NextFrame(() => {

                    isPlaying = true;

                    InitMovementState();
                });
            });

            
        }

        private void Update()
        {
            if (!isPlaying || IsDying) return;

            switch (state)
            {
                case State.Moving:
                    MovementStateUpdate();
                    break;
            }
        }

        #region Movement State

        private void InitMovementState()
        {
            CanGetHitOffset = false;

            state = State.Moving;

            enemyAnimator.SetTrigger(START_RUNNING_TRIGGER);

            stopMoving = false;

            CalculatePath();
        }



        private void CalculatePath()
        {
            navMeshAgent.Raycast(transform.position + direction * 100, out NavMeshHit hit);

            targetPosition = hit.position;

            hitNormal = hit.normal;
        }

        private void MovementStateUpdate()
        {

            if (stopMoving) return;

            Vector3 disvanceLeft = targetPosition - transform.position;

            Vector3 frameDistance = disvanceLeft.normalized * slimeData.Speed * Time.deltaTime;

            if(disvanceLeft.magnitude >= slimeData.Speed * Time.deltaTime)
            {
                transform.position += frameDistance;

                transform.forward = frameDistance.normalized;
            } else
            {
                transform.position = targetPosition;

                transform.forward = frameDistance.normalized;

                direction = Vector3.Reflect(direction, hitNormal);

                CalculatePath();
            }
        }

        #endregion

        #region Interaprions

        public override void Stun()
        {
            if (state == State.Dying) return;

            ResetTriggers();

            enemyAnimator.SetTrigger(STUN_TRIGGER);

            for (int i = 0; i < tweenCases.Count; i++)
            {
                if (!tweenCases[i].isCompleted) tweenCases[i].Kill();
            }

            tweenCases.Clear();

            state = State.Stunned;

            CanGetHitOffset = false;
        }

        public override void Unstun()
        {
            InitMovementState();
        }

        protected override void OnHitScale()
        {
            TweenCase bounceCase = null;
            bounceCase = scaleTransform.DOScale(0.9f, 0.1f).SetEasing(Ease.Type.QuadOut).OnComplete(() => {

                tweenCases.Remove(bounceCase);

                bounceCase = scaleTransform.DOScale(1, 0.2f).OnComplete(() => {
                    tweenCases.Remove(bounceCase);
                });

                tweenCases.Add(bounceCase);
            });

            tweenCases.Add(bounceCase);
        }

        protected override void OnHitOffset()
        {
            /*
            CanGetHitOffset = false;

            if (state == State.Idle)
            {
                idleStartTime += 0.5f;

                TweenCase delayCase = null;

                delayCase = Tween.DelayedCall(1.5f, () => {
                    if (state == State.Idle)
                        CanGetHitOffset = true;
                    tweenCases.Remove(delayCase);
                });

                tweenCases.Add(delayCase);
            }
            */
        }

        protected override void Die()
        {

            state = State.Dying;

            base.Die();

            enemyAnimator.SetTrigger(DIE_TRIGGER);

            for (int i = 0; i < tweenCases.Count; i++)
            {
                if (!tweenCases[i].isCompleted) tweenCases[i].Kill();
            }

            tweenCases.Clear();


            deathParticle.Play();

            Tween.DelayedCall(0.4f, () =>
            {
                if (specialMessage != "child")
                {
                    LevelController.SpawnAdditionalEnemy(enemy, firstChildSpawner, "child");
                    LevelController.SpawnAdditionalEnemy(enemy, secondChildSpawner, "child");
                }
            });

            Tween.DelayedCall(0.5f, () => {

                scaleTransform.transform.DOScale(0, 0.3f).OnComplete(() => {
                    meshRenderer.enabled = false;

                    specialMessage = "";

                });


                Tween.DelayedCall(1, () => {

                    gameObject.SetActive(false);

                    meshRenderer.enabled = true;

                    IsDying = false;

                    transform.localScale = Vector3.one;

                    scaleTransform.transform.localScale = Vector3.one;
                });
            });
        }

        #endregion

        public enum State
        {
            Moving, Dying, Stunned
        }
    }
}