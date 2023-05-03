using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watermelon;
using UnityEngine.AI;

public class FireGolemEnemy : EnemyBehavior
{
    [Space]
    [SerializeField] ParticleSystem deathParticle;

    [Header("Idle State")]
    [SerializeField, Range(1, 10)] float idleDuration;

    [Header("Movement State")]
    [SerializeField] float speed;

    [Header("First Attack")]
    [SerializeField] List<Transform> projectileSpawners;

    private FireGolemData golemData;
    private State state;

    Vector3[] movementPath;
    int nextPoint = 0;

    bool isPlaying = false;
    bool stopMoving = false;

    private float idleStartTime;

    private List<TweenCase> tweenCases;

    public void Start()
    {
        tweenCases = new List<TweenCase>();
    }

    public override void StartAI()
    {
        golemData = enemyData as FireGolemData;

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
        if (Time.time - idleStartTime < idleDuration)
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
                case 1:
                    if (canAttack)
                    {
                        InitFirstArrackState();
                    } else
                    {
                        InitIdleState();
                    }
                    
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

        Vector3 movementPosition;

        do
        {
            movementPosition = new Vector3(Random.Range(transform.position.x - 5f, transform.position.x + 5f), 0, Random.Range(transform.position.z - 5f, transform.position.z + 5f));

            if (movementPosition.x < 0.5f) movementPosition.x = 0.5f;
            if (movementPosition.x > GameController.CurrentRoom.Size.x - 0.5f) movementPosition.x = GameController.CurrentRoom.Size.x - 0.5f;

            if (movementPosition.z < 0.5f) movementPosition.z = 0.5f;
            if (movementPosition.z > GameController.CurrentRoom.Size.y - 0.5f) movementPosition.z = GameController.CurrentRoom.Size.y - 0.5f;

        } while (Physics.CheckBox(movementPosition, Vector3.one * 0.5f, new Quaternion(), 2 ^ GameController.OBSTACLE_LAYER));

        nextPoint = 0;

        movementPath = GetPath(movementPosition);

        if (movementPath.Length == 0)
        {
            InitMovementState();
        }
        else
        {
            enemyAnimator.ResetTrigger(IDLE_TRIGGER);
            enemyAnimator.ResetTrigger(ATTACK2_TRIGGER);
            enemyAnimator.SetTrigger(START_RUNNING_TRIGGER);

            stopMoving = false;
        }
    }

    private void MovementStateUpdate()
    {

        if (stopMoving) return;

        Vector3 movementPosition = movementPath[nextPoint];

        Vector3 distance = movementPosition - transform.position;

        transform.forward = Vector3.Lerp(transform.forward, (distance).normalized, 0.15f);

        if (distance.magnitude > speed * Time.deltaTime)
        {
            transform.position += distance.normalized * speed * Time.deltaTime;
        }
        else
        {
            transform.position = movementPosition;

            nextPoint++;

            if (nextPoint == movementPath.Length)
            {
                InitIdleState();
            }
        }
    }

    private Vector3[] GetPath(Vector3 destination)
    {
        NavMeshPath path = new NavMeshPath();

        navMeshAgent.CalculatePath(destination, path);

        return path.corners;
    }

    #endregion

    #region Attack State

    private void InitFirstArrackState()
    {
        CanGetHitOffset = false;

        enemyAnimator.ResetTrigger(IDLE_TRIGGER);
        enemyAnimator.SetTrigger(ATTACK_TRIGGER);
        state = State.FirstAttack;

        TweenCase delayCase1 = null;
        TweenCase delayCase2 = null;

        delayCase1 = Tween.DelayedCall(0.5f, () => {

            if (!IsDying)
            {
                if (enemyData.ShootAudio != null)
                {
                    GameAudioController.PlaySound(enemyData.ShootAudio);
                }

                for (int i = 0; i < projectileSpawners.Count; i++)
                {
                    //ProjectilesController.ShootProjectile(golemData.Projectile, golemData.ProjectileDamage, projectileSpawners[i], true);

                    ProjectilesController.ShootProjectile(new ProjectileInfo
                    {
                        canPassObstacles = false,
                        damages = new List<Damage>() { new Damage { amount = golemData.ProjectileDamage, isCrit = false, type = DamageType.Fire } },
                        direction = projectileSpawners[i].forward,
                        spawnPosition = projectileSpawners[i].position,
                        owner = this,
                        projectile = golemData.Projectile,
                        statusEffects = null,
                        targetsPlayer = true
                    });
                }
            }

            tweenCases.Remove(delayCase1);
        });

        delayCase2 = Tween.DelayedCall(1f, () => {
            InitIdleState();

            tweenCases.Remove(delayCase1);
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
