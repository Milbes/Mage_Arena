using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watermelon;

public class DarkBatEnemy : EnemyBehavior
{
    [Space]
    [SerializeField] ParticleSystem deathParticle;

    [Space]
    [SerializeField] List<Transform> projectileSpawners;

    private DarkBatData darkBatData;
    private State state;

    private float idleStartTime;
    private bool isPlaying = false;

    private List<TweenCase> tweenCases;

    public void Start()
    {
        tweenCases = new List<TweenCase>();
    }

    public override void StartAI()
    {
        darkBatData = enemyData as DarkBatData;

        InitIdleState();

        shouldSpawnCoins = SpecialMessage == null || SpecialMessage == "";

        Tween.DelayedCall(0.5f, () => {
            isPlaying = true;
        });
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
        if (Time.time - idleStartTime < darkBatData.IdleDuration)
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
                    InitTeleportState();
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
        TweenCase delayTween3 = null;

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
                    damages = new List<Damage> { new Damage { amount = darkBatData.ProjectileDamage, isCrit = false, type = DamageType.Shadow } },
                    statusEffects = null,
                    direction = projectileSpawners[0].forward,
                    spawnPosition = projectileSpawners[0].position,
                    owner = this,
                    projectile = darkBatData.Projectile,
                    targetsPlayer = true,
                });
            }
            
            tweenCases.Remove(delayTween1);
        });

        delayTween2 = Tween.DelayedCall(0.6f, () => {
            
            if(!IsDying)
            {
                if (enemyData.ShootAudio != null)
                {
                    GameAudioController.PlaySound(enemyData.ShootAudio);
                }

                ProjectilesController.ShootProjectile(new ProjectileInfo
                {
                    canPassObstacles = true,
                    damages = new List<Damage> { new Damage { amount = darkBatData.ProjectileDamage, isCrit = false, type = DamageType.Shadow } },
                    statusEffects = null,
                    direction = projectileSpawners[0].forward,
                    spawnPosition = projectileSpawners[0].position,
                    owner = this,
                    projectile = darkBatData.Projectile,
                    targetsPlayer = true,
                });
            }

            tweenCases.Remove(delayTween3);
        });

        delayTween3 = Tween.DelayedCall(1f, () => {

            InitIdleState();

            tweenCases.Remove(delayTween3);
        });

        tweenCases.Add(delayTween1);
        tweenCases.Add(delayTween2);
        tweenCases.Add(delayTween3);
    }
    #endregion

    #region Teleportation State
    private void InitTeleportState()
    {
        CanGetHitOffset = false;

        state = State.Teleport;

        Vector3 movementPosition;

        do
        {
            movementPosition = new Vector3(Random.Range(0.5f, GameController.CurrentRoom.Size.x - 0.5f), 0, Random.Range(0.5f, GameController.CurrentRoom.Size.y - 0.5f));

        } while (Physics.CheckBox(movementPosition, Vector3.one * 0.5f, new Quaternion(), 2 ^ GameController.OBSTACLE_LAYER) || Vector3.Distance(PlayerController.Position, movementPosition) < 4f);

        enemyAnimator.ResetTrigger(IDLE_TRIGGER);
        enemyAnimator.ResetTrigger(ATTACK_TRIGGER);
        enemyAnimator.SetTrigger(START_RUNNING_TRIGGER);

        GameObject teleportationParticle = Instantiate(darkBatData.TeleportationParticle);

        teleportationParticle.transform.position = transform.position.SetY(1.5f);
        teleportationParticle.transform.localScale = Vector3.one;

        TweenCase scaleTween = null;
        scaleTween = transform.DOScale(0, 1f).OnComplete(() => {
            tweenCases.Remove(scaleTween);

            scaleTween = transform.DOScale(1, 1f).OnComplete(() => {
                tweenCases.Remove(scaleTween);
            });

            tweenCases.Add(scaleTween);
        });

        tweenCases.Add(scaleTween);

        TweenCase delayTween = null;
        delayTween = Tween.DelayedCall(1f, () => { 

            Destroy(teleportationParticle);

            transform.position = movementPosition;

            teleportationParticle = Instantiate(darkBatData.TeleportationParticle);

            teleportationParticle.transform.position = transform.position.SetY(1.5f);
            teleportationParticle.transform.localScale = Vector3.one;

            tweenCases.Remove(delayTween);

            delayTween = Tween.DelayedCall(1f, () => {
                Destroy(teleportationParticle);

                InitIdleState();

                tweenCases.Remove(delayTween);
            });

            tweenCases.Add(delayTween);
        });

        tweenCases.Add(delayTween);
    }
    #endregion

    #region Iteraptions

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

        if(transform.localScale != Vector3.one)
        {
            tweenCases.Add(transform.DOScale(1, 0.2f));
        }
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
        base.Die();

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
        Idle, Teleport, FirstAttack, Dying, Stunned
    }
}
