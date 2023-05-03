#pragma warning disable 649, 414

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watermelon;

public class DragonEnemy : EnemyBehavior
{

    [Space]
    [SerializeField] Transform projectileSpawner;

    [Space]
    [SerializeField] ParticleSystem deathParticle;

    DragonData dragonData;

    bool shouldLookAtPlayer = false;
    bool PlayerTooClose => (PlayerController.Position - Position).magnitude <= dragonData.PlayerTooCloseDistance;

    private State state;

    Vector3 movementPoint;
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
        dragonData = enemyData as DragonData;

        InitIdleState();

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

        if(shouldLookAtPlayer) transform.forward = Vector3.Lerp(transform.forward, (PlayerController.Position - transform.position).SetY(0).normalized, 0.15f).normalized;
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
        if (Time.time - idleStartTime < dragonData.IdleDuration)
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
                        InitAttackState();
                    }
                    else
                    {
                        InitIdleState();
                    }

                    break;
            }
        }
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

        shouldLookAtPlayer = true;

        delayCase1 = Tween.DelayedCall(dragonData.StartFiringDelay, () => {

            if (!IsDying)
            {
                if (enemyData.ShootAudio != null)
                {
                    GameAudioController.PlaySound(enemyData.ShootAudio);
                }

                ProjectilesController.ShootProjectile(new ProjectileInfo
                {
                    canPassObstacles = false,
                    damages = new List<Damage>() { new Damage { amount = dragonData.ProjectileDamage, isCrit = false, type = DamageType.Fire } },
                    direction = projectileSpawner.forward,
                    spawnPosition = projectileSpawner.position,
                    owner = this,
                    projectile = dragonData.Projectile,
                    statusEffects = null,
                    targetsPlayer = true
                });
            }
            tweenCases.Remove(delayCase1);
        });

        shouldLookAtPlayer = false;

        delayCase2 = Tween.DelayedCall(1f, () => {
            InitIdleState();

            tweenCases.Remove(delayCase1);
        });

        tweenCases.Add(delayCase1);
        tweenCases.Add(delayCase2);
    }

    #endregion

    #region Movement State

    private void InitMovementState()
    {
        CanGetHitOffset = false;

        state = State.Moving;

        do
        {
            movementPoint = new Vector3(Random.Range(transform.position.x - 5f, transform.position.x + 5f), 0, Random.Range(transform.position.z - 5f, transform.position.z + 5f));

            if (movementPoint.x < 0.5f) movementPoint.x = 0.5f;
            if (movementPoint.x > GameController.CurrentRoom.Size.x - 0.5f) movementPoint.x = GameController.CurrentRoom.Size.x - 0.5f;

            if (movementPoint.z < 0.5f) movementPoint.z = 0.5f;
            if (movementPoint.z > GameController.CurrentRoom.Size.y - 0.5f) movementPoint.z = GameController.CurrentRoom.Size.y - 0.5f;

        } while (Physics.CheckBox(movementPoint, Vector3.one * 0.5f, new Quaternion(), 2 ^ GameController.OBSTACLE_LAYER));


        enemyAnimator.ResetTrigger(IDLE_TRIGGER);
        enemyAnimator.ResetTrigger(ATTACK_TRIGGER);
        enemyAnimator.SetTrigger(START_RUNNING_TRIGGER);

        stopMoving = false;
        
    }

    private void MovementStateUpdate()
    {

        if (stopMoving) return;

        Vector3 distance = movementPoint - transform.position;

        transform.forward = Vector3.Lerp(transform.forward, (distance).normalized, 0.15f);

        if (distance.magnitude > dragonData.Speed * Time.deltaTime)
        {
            transform.position += distance.normalized * dragonData.Speed * Time.deltaTime;
        }
        else
        {
            transform.position = movementPoint;

            InitIdleState();
            
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


    /*
    private void Awake()
    {
        dragonData = (DragonData)enemyData;
    }

    private void Update()
    {
        if (shouldLookAtPlayer) Forward = Vector3.Lerp(Forward, (PlayerController.Position - Position).normalized.SetY(0), Time.deltaTime * 5);
    }

    protected override void Die()
    {
        base.Die();
        enemyAnimator.SetTrigger(DIE_TRIGGER);

        
        Tween.DelayedCall(0.5f, () => {

            deathParticle.Play();

            scaleTransform.transform.DOScale(0, 0.3f).OnComplete(() => { meshRenderer.enabled = false; });

            Tween.DelayedCall(1, () => {

                gameObject.SetActive(false);

                meshRenderer.enabled = true;

                IsDying = false;

                transform.localScale = Vector3.one;

                scaleTransform.transform.localScale = Vector3.one;
            });
        });
    }


    public override void StartAI()
    {

        shouldLookAtPlayer = true;
        Tween.DelayedCall(0.5f, () => {
            StartCoroutine(SimulateAI());
        });
    }

    public IEnumerator SimulateAI()
    {

        while (gameObject.activeSelf)
        {

            yield return Idle();

            yield return IsPlayerClose();

            yield return FireProjectile();

            yield return IsPlayerClose();
        }
    }

    public IEnumerator Idle()
    {

        if (IsDying)
        {
            yield return new WaitForSeconds(1);
            yield break;
        }

        shouldLookAtPlayer = true;

        enemyAnimator.SetTrigger(IDLE_TRIGGER);

        yield return new WaitForSeconds(dragonData.IdleDuration);

    }

    public IEnumerator IsPlayerClose()
    {
        if (IsDying)
        {
            yield return new WaitForSeconds(1);
            yield break;
        }

        if (PlayerTooClose)
        {
            yield return RunAwayFromPlayer();

            enemyAnimator.SetTrigger(IDLE_TRIGGER);

            yield return new WaitForSeconds(dragonData.IdleAfterRunningDuration);
        }
    }

    public IEnumerator RunAwayFromPlayer()
    {
        shouldLookAtPlayer = false;
        if (IsDying)
        {
            yield return new WaitForSeconds(1);
            yield break;
        }

        Vector3 destinationPosition;
        float distanceToPlayer;

        int counter2 = 0;

        do
        {
            counter2++;

            

            int counter1 = 0;

            do
            {
                counter1++;

                

                destinationPosition = new Vector3(Random.Range(0.5f, GameController.CurrentRoom.Size.x - 0.5f), 0, Random.Range(0.5f, GameController.CurrentRoom.Size.y - 0.5f));
                distanceToPlayer = (destinationPosition - PlayerController.Position).magnitude;

                if (counter1 > 1000)
                {

                    Debug.Log("counter1 = " + counter1);

                    break;
                }
            } while (distanceToPlayer <= dragonData.RunAwayFromPlayerDistance);


            if (counter2 > 1000)
            {

                Debug.Log("counter2 = " + counter2);

                break;
            }


        } while (Physics.CheckBox(destinationPosition, Vector3.one * 0.5f, new Quaternion(), 2 ^ GameController.OBSTACLE_LAYER));

        shouldLookAtPlayer = false;

        while (true)
        {
            Vector3 path = destinationPosition - Position;

            Vector3 direction = path.normalized;

            Vector3 framePath = direction * Speed * Time.fixedDeltaTime;

            if (framePath.sqrMagnitude > path.sqrMagnitude)
            {
                Position = destinationPosition;

                break;
            }
            else
            {
                Position += framePath;
                Forward = direction;

                yield return new WaitForFixedUpdate();

            }
        }

        shouldLookAtPlayer = true;

    }

    public IEnumerator FireProjectile()
    {
        shouldLookAtPlayer = true;

        enemyAnimator.SetTrigger(ATTACK_TRIGGER);

        if (enemyData.ShootAudio != null)
        {
            AudioController.PlaySound(enemyData.ShootAudio, GameController.Sound);
        }

        yield return new WaitForSeconds(dragonData.StartFiringDelay);

        //ProjectilesController.ShootProjectile(dragonData.Projectile, dragonData.ProjectileDamage, projectileSpawner, true);
        

        shouldLookAtPlayer = false;
    }

    */

}
