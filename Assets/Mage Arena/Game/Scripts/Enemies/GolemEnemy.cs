#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watermelon;
using UnityEngine.AI;

public class GolemEnemy : EnemyBehavior
{

    bool shouldLookAtPlayer = false;

    [Space]
    [SerializeField] GameObject redStripe;

    [Space]
    [SerializeField] ParticleSystem deathParticle;

    public float lastState = 0;

    Vector3[] movementPath;
    int nextPoint = 0;

    private List<TweenCase> tweenCases;

    private GolemData golemData;
    private State state;

    private float idleStartTime;

    private bool isPlaying = false;
    bool stopMoving = false;

    private bool IsPlayerInRange { get => (Position - PlayerController.Position).magnitude <= golemData.ShootingDistance; }

    public void Start()
    {
        tweenCases = new List<TweenCase>();
    }

    public override void StartAI()
    {
        golemData = enemyData as GolemData;

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
        if (Time.time - idleStartTime < golemData.IdleDuration)
        {
            transform.forward = Vector3.Lerp(transform.forward, (PlayerController.Position - transform.position).SetY(0).normalized, 0.15f).normalized;

            return;
        }
        else
        {
            int random = Random.Range(0, 2);

            if(random == lastState)
            {
                random = Random.Range(0, 2);
            }

            lastState = random;

            switch (random)
            {
                case 0:
                    InitMovementState();
                    break;
                case 1:
                    InitFirstArrackState();
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
            enemyAnimator.ResetTrigger(ATTACK_TRIGGER);
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

        if (distance.magnitude > golemData.MovementSpeed * Time.deltaTime)
        {
            transform.position += distance.normalized * golemData.MovementSpeed * Time.deltaTime;
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

    #region First Attack State

    private void InitFirstArrackState()
    {
        CanGetHitOffset = false;

        state = State.FirstAttack;

        enemyAnimator.ResetTrigger(IDLE_TRIGGER);
        enemyAnimator.SetTrigger(ATTACK_TRIGGER);

        TweenCase delayCase = null;

        delayCase = Tween.DelayedCall(0.5f, () => {

            if (!IsDying)
            {
                if (enemyData.ShootAudio != null)
                {
                    GameAudioController.PlaySound(enemyData.ShootAudio);
                }

                ProjectilesController.ShootGolemProjectile(new ProjectileInfo
                {
                    canPassObstacles = true,
                    targetsPlayer = true,
                    statusEffects = null,
                    damages = new List<Damage>() { new Damage { amount = golemData.ProjectileDamage, isCrit = false, type = DamageType.Base } },
                    direction = transform.forward,
                    spawnPosition = transform.position,
                    owner = this,
                    projectile = golemData.Projectile
                });
            }
            
            tweenCases.Remove(delayCase);

            delayCase = Tween.DelayedCall(0.5f, () => {
                InitIdleState();
            });
        });

        tweenCases.Add(delayCase);
    }

    #endregion

    #region Interaptions

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
