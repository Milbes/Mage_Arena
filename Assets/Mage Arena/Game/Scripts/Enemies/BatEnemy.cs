#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watermelon;

public class BatEnemy : EnemyBehavior
{
    [Space]
    [SerializeField] List<Transform> projectileSpawners;
    [SerializeField] ParticleSystem deathParticle;
    
    [SerializeField] Transform batParent;

    BatData batData;
    private State state;

    private float idleStartTime;
    private bool flownLastTime = false;
    private bool isPlaying = false;

    private Vector3 movementPosition;

    private List<TweenCase> tweenCases;

    private void Awake()
    {
        batData = (BatData)enemyData;
    }

    public void Start()
    {
        tweenCases = new List<TweenCase>();
    }

    private void Update()
    {
        if (!isPlaying) return;
        if (IsDying) return;

        switch (state)
        {
            case State.Idle:
                IdleStateUpdate();
                break;

            case State.Flying:
                FlyingStateUpdate();
                break;
        }
    }

    public override void StartAI()
    {
        meshRenderer.transform.localScale = Vector3.one;

        isPlaying = true;

        flownLastTime = false;

        InitIdleState();
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
        if (Time.time - idleStartTime < batData.IdleDuration)
        {
            transform.forward = Vector3.Lerp(transform.forward, (PlayerController.Position - transform.position).SetY(0).normalized, 0.15f).normalized;

            return;
        }
        else
        {

            int random;

            if (Vector3.Distance(transform.position, PlayerController.Position) <= 5f)
            {
                random = 0;
            } else if(flownLastTime)
            {
                random = 1;
            } else
            {
                random = Random.Range(0, 2);
            }

            switch (random)
            {
                case 0:
                    flownLastTime = true;

                    InitFlyingState();
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

    #region Attack State

    private void InitFirstArrackState()
    {
        CanGetHitOffset = false;

        state = State.FirstAttack;

        enemyAnimator.SetTrigger(ATTACK_TRIGGER);

        TweenCase delayTween1 = null;
        TweenCase delayTween2 = null;

        delayTween1 = Tween.DelayedCall(0.5f, () => {

            if (!IsDying)
            {
                if (enemyData.ShootAudio != null)
                {
                    GameAudioController.PlaySound(enemyData.ShootAudio);
                }

                ProjectilesController.ShootProjectile(new ProjectileInfo
                {
                    canPassObstacles = true,
                    damages = new List<Damage> { new Damage { amount = batData.ProjectileDamage, isCrit = false, type = DamageType.Shadow } },
                    statusEffects = null,
                    direction = projectileSpawners[0].forward,
                    spawnPosition = projectileSpawners[0].position,
                    owner = this,
                    projectile = batData.Projectile,
                    targetsPlayer = true,
                });
            }
            tweenCases.Remove(delayTween1);
        });

        delayTween2 = Tween.DelayedCall(1f, () => {

            InitIdleState();

            tweenCases.Remove(delayTween2);
        });

        tweenCases.Add(delayTween1);
        tweenCases.Add(delayTween2);
    }

    #endregion

    #region Flying State

    public void InitFlyingState()
    {
        CanGetHitOffset = false;

        state = State.Flying;

        do
        {
            movementPosition = new Vector3(Random.Range(0.5f, GameController.CurrentRoom.Size.x - 0.5f), 0, Random.Range(0.5f, GameController.CurrentRoom.Size.y - 0.5f));

        } while (Vector3.Distance(PlayerController.Position, movementPosition) <= 5f || Physics.CheckBox(movementPosition, Vector3.one * 0.5f, new Quaternion(), 2 ^ GameController.OBSTACLE_LAYER));

        enemyAnimator.ResetTrigger(IDLE_TRIGGER);
        enemyAnimator.ResetTrigger(ATTACK_TRIGGER);
        enemyAnimator.SetTrigger(START_RUNNING_TRIGGER);
    }

    private void FlyingStateUpdate()
    {

        Vector3 distance = movementPosition - transform.position;

        transform.forward = Vector3.Lerp(transform.forward, (distance).normalized, 0.15f);

        if (distance.magnitude > batData.Speed * Time.deltaTime)
        {
            transform.position += distance.normalized * batData.Speed * Time.deltaTime;
        }
        else
        {
            transform.position = movementPosition;

            InitIdleState();
        }
    }

    #endregion

    #region Interaptions
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

    protected override void Die()
    {
        base.Die();

        IsDying = true;

        state = State.Dying;

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
        Idle, Flying, FirstAttack, Dying, Stunned
    }
}
