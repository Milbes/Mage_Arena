using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watermelon;

public class IceGolemBoss : EnemyBehavior
{

    private static readonly int ANIMATION_SPEED_PARAMETER = Animator.StringToHash("Animation Speed");

    [Space]
    [SerializeField] ParticleSystem deathParticle;

    [Header("Idle State")]
    [SerializeField, Range(1, 10)] float idleDuration;

    [Header("Movement State")]
    [SerializeField] float speed;

    [Header("First Attack")]
    [SerializeField] List<Transform> projectileSpawners;

    private IceGolemBossData golemData;
    private State state;

    private int previosAttack = -1;
    private int secondAttackAmount;

    private float idleStartTime;
    private float lastSecondAttackTime;

    private bool isPlaying = false;
    private bool stopMoving = false;

    private Vector3 movementPosition;

    private List<TweenCase> tweenCases;

    private Transform stunParticleTransform;

    private float animationSpeed;
    private float AnimationSpeed {
        get => animationSpeed;
        set {
            animationSpeed = value;
            enemyAnimator.SetFloat(ANIMATION_SPEED_PARAMETER, value);
        }
    }

    public void Start()
    {
        tweenCases = new List<TweenCase>();
    }

    public override void StartAI()
    {
        golemData = enemyData as IceGolemBossData;

        InitIdleState();

        Tween.DelayedCall(0.5f, () => {
            isPlaying = true;
        });

        AnimationSpeed = 1;
    }

    private void Update()
    {
        if (!isPlaying) return;

        switch (state)
        {
            case State.Idle:
                IdleStateUpdate();
                break;

            case State.SecondAttack:
                SecondAttackUpdate();
                break;

            case State.Moving:
                MovementStateUpdate();
                break;
        }

        if (stunParticleTransform != null) stunParticleTransform.position = transform.position + Vector3.up * 0.05f;
    }


    #region Idle State

    private void InitIdleState()
    {
        state = State.Idle;

        ResetTriggers();
        enemyAnimator.SetTrigger(IDLE_TRIGGER);
        idleStartTime = Time.time;

        CanGetHitOffset = true;
    }

    private void IdleStateUpdate()
    {
        if (Time.time - idleStartTime < golemData.IdleDuration / AnimationSpeed)
        {
            transform.forward = Vector3.Lerp(transform.forward, (PlayerController.Position - transform.position).SetY(0).normalized, 0.15f).normalized;

            return;
        }
        else
        {
            int random;

            do
            {
                random = Random.Range(0, 3);
            }
            while (random == previosAttack);

            previosAttack = random;

            switch (random)
            {
                case 0:
                    InitMovementState(true);
                    break;
                case 1:
                    InitFirstArrackState();
                    break;
                case 2:
                    InitSecondAttackState();
                    break;
            }
        }
    }

    #endregion

    #region Movement State

    private void  InitMovementState(bool canGetHit)
    {
        CanGetHitOffset = false;

        state = State.Moving;

        enemyAnimator.ResetTrigger(IDLE_TRIGGER);
        enemyAnimator.ResetTrigger(ATTACK2_TRIGGER);
        enemyAnimator.SetTrigger(START_RUNNING_TRIGGER);

        stopMoving = true;

        TweenCase delayCase = null;

        delayCase = Tween.DelayedCall(2f / AnimationSpeed, () =>
        {
            movementPosition = PlayerController.Position;
            movementPosition.y = 0;

            stopMoving = false;

            tweenCases.Remove(delayCase);
        });

        tweenCases.Add(delayCase);
    }

    private void MovementStateUpdate()
    {

        if (stopMoving) return;

        Vector3 distance = movementPosition - transform.position;

        transform.forward = Vector3.Lerp(transform.forward, (distance).normalized, 0.15f);

        if (distance.magnitude > golemData.Speed * Time.deltaTime * AnimationSpeed)
        {
            transform.position += distance.normalized * golemData.Speed * Time.deltaTime * AnimationSpeed;
        }
        else
        {
            transform.position = movementPosition;

            InitIdleState();
        }
    }

    #endregion

    #region First Attack State

    private void InitFirstArrackState()
    {
        CanGetHitOffset = false;

        enemyAnimator.ResetTrigger(IDLE_TRIGGER);
        enemyAnimator.ResetTrigger(ATTACK2_TRIGGER);
        enemyAnimator.SetTrigger(ATTACK_TRIGGER);
        state = State.FirstAttack;

        TweenCase delayCase1 = null;
        TweenCase delayCase2 = null;

        delayCase1 = Tween.DelayedCall(0.5f / AnimationSpeed, () => {

            if (!IsDying)
            {

                GameAudioController.PlaySound(golemData.ProjectileAttackAudio);

                for (int i = 0; i < projectileSpawners.Count; i++)
                {
                    ProjectilesController.ShootProjectile(new ProjectileInfo
                    {
                        targetsPlayer = true,
                        owner = this,
                        canPassObstacles = false,
                        damages = new List<Damage>() { new Damage { amount = golemData.SpikeProjectileDamage, isCrit = false, type = DamageType.Base } },
                        statusEffects = null,
                        direction = projectileSpawners[i].forward,
                        spawnPosition = projectileSpawners[i].position,
                        projectile = golemData.SpikeProjectile
                    });
                }
            }

            tweenCases.Remove(delayCase1);
        });

        delayCase2 = Tween.DelayedCall(1f / AnimationSpeed, () => {
            InitIdleState();

            tweenCases.Remove(delayCase1);
        });

        tweenCases.Add(delayCase1);
        tweenCases.Add(delayCase2);
    }

    #endregion

    #region Second Attack State

    private void InitSecondAttackState()
    {
        CanGetHitOffset = false;

        state = State.SecondAttack;

        secondAttackAmount = 0;

        SecondAttack();
    }

    private void SecondAttackUpdate()
    {

        float timeDifference = Time.time - lastSecondAttackTime;

        if (timeDifference < 1 / AnimationSpeed)
        {
            return;
        } else if (timeDifference < 1.5f / AnimationSpeed)
        {
            transform.forward = Vector3.Lerp(transform.forward, (PlayerController.Position - transform.position).SetY(0).normalized, 0.15f).normalized;
        } else
        {
            if(secondAttackAmount >= 5)
            {
                InitIdleState();
            } else
            {
                SecondAttack();
            }
        }
    }

    private void SecondAttack()
    {

        enemyAnimator.ResetTrigger(IDLE_TRIGGER);
        enemyAnimator.SetTrigger(ATTACK2_TRIGGER);

        lastSecondAttackTime = Time.time;
        secondAttackAmount++;

        TweenCase delayCase = null;

        delayCase = Tween.DelayedCall(0.5f / AnimationSpeed, () => {

            CameraController.ShakeCamera();

            if (!IsDying)
            {
                GameAudioController.PlaySound(golemData.StonesAttackAudio);

                ProjectilesController.ShootGolemProjectile(new ProjectileInfo
                {
                    canPassObstacles = true,
                    targetsPlayer = true,
                    statusEffects = null,
                    damages = new List<Damage>() { new Damage { amount = golemData.GroundProjectileDamage, isCrit = false, type = DamageType.Base } },
                    direction = transform.forward,
                    spawnPosition = transform.position,
                    owner = this,
                    projectile = golemData.GroundProjectile
                });
            }

            tweenCases.Remove(delayCase);
        });

        tweenCases.Add(delayCase);
    }

    #endregion

    #region Interaptions

    public override void SlowDown(Transform particleTransform, float magnitude)
    {
        stunParticleTransform = particleTransform;

        AnimationSpeed = magnitude;
    }

    public override void ResetSpeed()
    {
        stunParticleTransform = null;

        AnimationSpeed = 1;
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
            idleStartTime += 1f / AnimationSpeed;

            TweenCase delayCase = null;

            delayCase = Tween.DelayedCall(2.5f / AnimationSpeed, () => {
                if (state == State.Idle)
                    CanGetHitOffset = true;
                tweenCases.Remove(delayCase);
            });

            tweenCases.Add(delayCase);
        }
    }

    protected override void Die()
    {
        base.Die();

        state = State.Dying;

        enemyAnimator.SetTrigger(DIE_TRIGGER);

        for(int i = 0; i < tweenCases.Count; i++)
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
        Idle, Moving, FirstAttack, SecondAttack, Dying
    }
}
