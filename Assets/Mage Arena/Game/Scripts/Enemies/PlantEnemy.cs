#pragma warning disable 649, 414

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Watermelon
{

    public class PlantEnemy : EnemyBehavior
    {

        [SerializeField] GameObject attackCollider;

        [Space]
        [SerializeField] ParticleSystem deathParticle;

        bool shouldLookAtPlayer = true;

        private bool IsPlayerInRange { get => (Position - PlayerController.Position).magnitude <= plantData.AttackDistance; }

        PlantData plantData;
        private State state;

        Vector3[] movementPath;
        int nextPoint = 0;

        bool isPlaying = false;
        bool stopMoving = false;

        private List<TweenCase> tweenCases;

        private float idleStartTime;

        private float pathRecalculationTime = 0;
        private float travercedDistance = 0;

        public void Start()
        {
            tweenCases = new List<TweenCase>();
        }

        public override void StartAI()
        {
            plantData = enemyData as PlantData;

            InitIdleState();

            Tween.NextFrame(() => {
                navMeshAgent.enabled = true;
            });

            Tween.DelayedCall(0.5f, () => {
                isPlaying = true;
            });
        }

        private void Update()
        {
            if (!isPlaying && IsDying) return;

            switch (state)
            {
                case State.Idle:
                    IdleStateUpdate();
                    break;

                case State.Moving:
                    MovementStateUpdate();
                    break;
            }
        }

        #region Idle State

        private void InitIdleState()
        {
            state = State.Idle;

            enemyAnimator.ResetTrigger(ATTACK_TRIGGER);
            enemyAnimator.SetTrigger(IDLE_TRIGGER);
            idleStartTime = Time.time;

            CanGetHitOffset = true;
        }

        private void IdleStateUpdate()
        {
            if (Time.time - idleStartTime < plantData.IdleDuration)
            {
                transform.forward = Vector3.Lerp(transform.forward, (PlayerController.Position - transform.position).SetY(0).normalized, 0.15f).normalized;

                return;
            }
            else
            {
                int random = Random.Range(0, 2);

                switch (random)
                {
                    case 0:
                        InitMovementState();
                        break;
                }
            }
        }

        #endregion

        #region Movement State

        private void InitMovementState()
        {
            CanGetHitOffset = false;

            state = State.Moving;

            nextPoint = 0;

            pathRecalculationTime = 0;
            travercedDistance = 0;

            movementPath = GetPath(PlayerController.Position);

            if (movementPath.Length == 0)
            {
                InitMovementState();
            }
            else
            {
                enemyAnimator.ResetTrigger(IDLE_TRIGGER);
                enemyAnimator.ResetTrigger(ATTACK_TRIGGER);
                enemyAnimator.SetTrigger(START_RUNNING_TRIGGER);

                stopMoving = false;
            }
        }

        private void MovementStateUpdate()
        {

            if (stopMoving) return;

            if (travercedDistance >= plantData.RunningMaxDistance)
            {
                if (IsPlayerInRange)
                {
                    InitAttackState();
                }
                else
                {
                    InitIdleState();
                }

                return;
            }

            if (IsPlayerInRange)
            {
                InitAttackState();
                return;
            }

            if (pathRecalculationTime >= plantData.PathRecalculationDuratuion)
            {
                RecalculatePath();
            }

            pathRecalculationTime += Time.deltaTime;

            Vector3 movementPosition = movementPath[nextPoint];

            Vector3 distance = movementPosition - transform.position;

            transform.forward = Vector3.Lerp(transform.forward, (distance).normalized, 0.15f);

            if (distance.magnitude > plantData.Speed * Time.deltaTime)
            {
                Vector3 framePath = distance.normalized * plantData.Speed * Time.deltaTime;

                transform.position += framePath;

                travercedDistance += framePath.magnitude;
            }
            else
            {
                Vector3 framePath = movementPosition - transform.position;

                travercedDistance += framePath.magnitude;

                transform.position = movementPosition;

                nextPoint++;

                if (nextPoint == movementPath.Length)
                {
                    InitIdleState();
                }
            }
        }

        private void RecalculatePath()
        {
            movementPath = GetPath(PlayerController.Position);

            if (movementPath.Length == 0)
            {
                movementPath = GetPath(PlayerController.Position);
            }

            nextPoint = 0;
            pathRecalculationTime = 0;
        }

        private Vector3[] GetPath(Vector3 destination)
        {
            NavMeshPath path = new NavMeshPath();

            navMeshAgent.CalculatePath(destination, path);

            return path.corners;
        }

        #endregion

        #region Attack State

        private void InitAttackState()
        {
            CanGetHitOffset = false;

            enemyAnimator.ResetTrigger(IDLE_TRIGGER);
            enemyAnimator.SetTrigger(ATTACK_TRIGGER);
            state = State.FirstAttack;

            TweenCase delayCase1 = null;
            TweenCase delayCase2 = null;

            delayCase1 = Tween.DelayedCall(plantData.AttackDelay, () => {

                if (!IsDying)
                {
                    attackCollider.gameObject.SetActive(true);
                }

                tweenCases.Remove(delayCase1);
            });

            delayCase2 = Tween.DelayedCall(1f, () => {
                InitIdleState();

                attackCollider.gameObject.SetActive(false);

                tweenCases.Remove(delayCase2);
            });

            tweenCases.Add(delayCase1);
            tweenCases.Add(delayCase2);
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
            InitIdleState();
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

            Tween.DelayedCall(0.5f, () => {
                transform.DOScale(0, 0.3f).OnComplete(() => {

                    gameObject.SetActive(false);

                    IsDying = false;

                    transform.localScale = Vector3.one;
                });
            });
        }

        #endregion

        public enum State
        {
            Idle, Moving, FirstAttack, Dying, Stunned
        }
    }

}