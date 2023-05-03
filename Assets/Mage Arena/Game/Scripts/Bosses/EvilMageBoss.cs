#pragma warning disable 649, 414

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watermelon;


namespace Watermelon
{

    public class EvilMageBoss : EnemyBehavior
    {

        public static readonly string EXPLOSION_PARICLE_POOL_NAME = "Evil Mage Explosion Particle";
        public static readonly string TELEPORTATION_PARTICLE_POOL_NAME = "Evil Mage Teleportation Particle";
        public static readonly string CLONE_SECRET_MESSAGE = "Evil Mage Clone";

        private static readonly int ANIMATION_SPEED_PARAMETER = Animator.StringToHash("Animation Speed");

        [Space]
        [SerializeField] List<Transform> simpleAttackSpawners;

        [Space]
        [SerializeField] ParticleSystem deathParticle;

        [Space]
        [SerializeField] Enemy cloneToSummon;
        [SerializeField] Enemy batToSummon;

        [Space]
        [SerializeField] Projectile explosionProjectile;
        [SerializeField] GameObject explosionProjectileObject;
        //[SerializeField] ParticleSystem explisionParticle;

        private EvilMageData mageData;
        private State state;
        private State previousState;

        private float idleStartTime;
        private float lastBatCheckTime;

        private bool isPlaying = false;
        private bool stopMoving = false;

        private List<TweenCase> tweenCases;

        private List<EvilMageExplosionProjectileBehavior> explosions;

        private Transform stunParticleTransform;

        private float animationSpeed;
        private float AnimationSpeed
        {
            get => animationSpeed;
            set
            {
                animationSpeed = value;
                enemyAnimator.SetFloat(ANIMATION_SPEED_PARAMETER, value);
            }
        }

        private static Pool explosionPool;
        private static Pool teleportationPool;

        private int explosionAttackCounter = 0;

        private bool isClone = false;
        private bool isWaitingClonesDeath = false;

        private bool shouldLookAtPlayer = false;

        Vector3 movementPosition;

        private void Awake()
        {
            explosions = new List<EvilMageExplosionProjectileBehavior>();
        }

        public override void StartAI()
        {
            tweenCases = new List<TweenCase>();

            mageData = enemyData as EvilMageData;

            //InitIdleState();

            if (!PoolManager.ContainsPool(EXPLOSION_PARICLE_POOL_NAME))
            {
                explosionPool = PoolManager.AddPool(new PoolSettings
                {
                    autoSizeIncrement = true,
                    name = EXPLOSION_PARICLE_POOL_NAME,
                    singlePoolPrefab = explosionProjectileObject,
                    size = 5,
                    type = Pool.PoolType.Single,
                    objectsContainer = null
                });
            }

            if (!PoolManager.ContainsPool(TELEPORTATION_PARTICLE_POOL_NAME))
            {
                teleportationPool = PoolManager.AddPool(new PoolSettings
                {
                    autoSizeIncrement = true,
                    name = TELEPORTATION_PARTICLE_POOL_NAME,
                    singlePoolPrefab = mageData.TeleportationParticleObject,
                    size = 5,
                    type = Pool.PoolType.Single,
                    objectsContainer = null
                });
            }

            Tween.NextFrame(() => {
                isPlaying = true;

                isClone = specialMessage == CLONE_SECRET_MESSAGE;

                shouldSpawnCoins = !isClone;

                if (isClone)
                {
                    InitEvilMageClone();
                } else
                {
                    previousState = State.Clones;

                    InitIdleState();
                }

            });

            AnimationSpeed = 1;
        }

        private void InitEvilMageClone()
        {
            CanGetHitOffset = false;

            MaxHealth = mageData.ClonesHealth;
            Health = MaxHealth;

            GameObject teleportationParticle = teleportationPool.GetPooledObject();

            teleportationParticle.transform.position = transform.position.SetY(1.5f);
            teleportationParticle.transform.localScale = Vector3.one;

            transform.localScale = Vector3.zero;

            TweenCase scaleTween = null;
            scaleTween = transform.DOScale(1, 0.5f).OnComplete(() => {
                tweenCases.Remove(scaleTween);

                InitIdleState();
            });

            tweenCases.Add(scaleTween);
        }

        private void Update()
        {
            if (!isPlaying) return;

            switch (state)
            {
                case State.Idle:
                    IdleStateUpdate();
                    break;

                case State.Clones:
                    ClonesStateUpdate();
                    break;

                case State.Moving:
                    MovementStateUpdate();
                    break;
            }

            if (shouldLookAtPlayer) transform.forward = (PlayerController.Position - transform.position).SetY(0).normalized;

            if (stunParticleTransform != null) stunParticleTransform.position = transform.position + Vector3.up * 0.05f;

            lastBatCheckTime += Time.deltaTime;

            if(lastBatCheckTime >= 2f)
            {
                lastBatCheckTime = 0;

                bool batAlive = false;

                for(int i = 0; i < LevelObjectSpawner.Enemies.Count; i++)
                {
                    if(LevelObjectSpawner.Enemies[i].SpecialMessage == "Bat Minion")
                    {
                        batAlive = true;

                        break;
                    }
                }

                if (!batAlive)
                {
                    Vector3 batPosition;
                    do
                    {
                        batPosition = new Vector3(Random.Range(0.5f, GameController.CurrentRoom.Size.x - 0.5f), 0, Random.Range(0.5f, GameController.CurrentRoom.Size.y - 0.5f));
                    } while (Physics.CheckBox(batPosition, Vector3.one * 0.5f, new Quaternion(), 2 ^ GameController.OBSTACLE_LAYER));

                    LevelController.SpawnAdditionalEnemy(batToSummon, batPosition, "Bat Minion");
                }
            }
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
            if (Time.time - idleStartTime < mageData.IdleDuration / AnimationSpeed)
            {
                transform.forward = Vector3.Lerp(transform.forward, (PlayerController.Position - transform.position).SetY(0).normalized, 0.15f).normalized;

                return;
            }
            else
            {
                int random = Random.Range(0, 2);

                if (isClone)
                {
                    InitMovementState(true);
                } else
                {
                    if(previousState == State.Clones)
                    {
                        if (random == 0) { 
                            
                            InitMovementState(false);
                            
                        } else
                        {
                            previousState = State.ExplosionAttack;

                            InitExplosionAttackState();
                        }
                    } else
                    {
                        if (random == 0)
                        {
                            InitMovementState(false);
                        }
                        else
                        {
                            previousState = State.Clones;

                            InitClonesState();
                        }
                    }
                }
            }
        }

        #endregion

        #region Movement State

        private void InitMovementState(bool canGetHit)
        {
            CanGetHitOffset = false;

            state = State.Moving;

            ResetTriggers();
            enemyAnimator.SetTrigger(START_RUNNING_TRIGGER);

            stopMoving = false;

            do
            {
                movementPosition = new Vector3(Random.Range(transform.position.x - 5f, transform.position.x + 5f), 0, Random.Range(transform.position.z - 5f, transform.position.z + 5f));

                if (movementPosition.x < 0.5f) movementPosition.x = 0.5f;
                if (movementPosition.x > GameController.CurrentRoom.Size.x - 0.5f) movementPosition.x = GameController.CurrentRoom.Size.x - 0.5f;

                if (movementPosition.z < 0.5f) movementPosition.z = 0.5f;
                if (movementPosition.z > GameController.CurrentRoom.Size.y - 0.5f) movementPosition.z = GameController.CurrentRoom.Size.y - 0.5f;

            } while (Physics.CheckBox(movementPosition, Vector3.one * 0.5f, new Quaternion(), 2 ^ GameController.OBSTACLE_LAYER));


        }

        private void MovementStateUpdate()
        {

            if (stopMoving) return;

            Vector3 distance = movementPosition - transform.position;

            transform.forward = Vector3.Lerp(transform.forward, (distance).normalized, 0.15f);

            if (distance.magnitude > mageData.Speed * Time.deltaTime * AnimationSpeed)
            {
                transform.position += distance.normalized * mageData.Speed * Time.deltaTime * AnimationSpeed;
            }
            else
            {
                transform.position = movementPosition;

                InitIdleState();
            }
        }

        #endregion

        #region Explosion Projectile State

        private void InitExplosionAttackState()
        {
            CanGetHitOffset = false;

            shouldLookAtPlayer = true;

            ResetTriggers();

            explosionAttackCounter++;

            enemyAnimator.SetTrigger(MAGIC_1_TRIGGER);

            state = State.ExplosionAttack;

            SpawnExplosionParticles();

            TweenCase delayCase = null;

            delayCase = Tween.DelayedCall(mageData.DurationBeforeExplosion, () => {

                for(int i = 0; i < explosions.Count; i++)
                {
                    explosions[i].Explode();
                }

                tweenCases.Remove(delayCase);

                delayCase = Tween.DelayedCall(mageData.DurationExplosion, () =>
                {
                    for (int i = 0; i < explosions.Count; i++)
                    {
                        explosions[i].Disable();
                    }

                    explosions.Clear();

                    explosionPool.ReturnToPoolEverything();

                    tweenCases.Remove(delayCase);

                    if (explosionAttackCounter > 2)
                    {
                        explosionAttackCounter = 0;

                        shouldLookAtPlayer = false;

                        InitIdleState();
                    }
                    else
                    {
                        Tween.NextFrame(InitExplosionAttackState);
                    }
                });

                tweenCases.Add(delayCase);
            });

            tweenCases.Add(delayCase);


        }

        private void SpawnExplosionParticles()
        {
            explosions.Clear();

            InitExplosion(PlayerController.Position.SetY(0));

            for(int x = -3; x < 4; x += 6)
            {
                for (int z = -3; z < 4; z += 6)
                {
                    float xPos = PlayerController.Position.x + x;
                    float zPos = PlayerController.Position.z + z;

                    if(xPos >= 2f && xPos <= GameController.CurrentRoom.Size.x - 2f && zPos >= 2f && zPos <= GameController.CurrentRoom.Size.y - 2f)
                    {
                        if (enemyData.ShootAudio != null)
                        {
                            GameAudioController.PlaySound(mageData.MeteorAttackAudio);
                        }

                        InitExplosion(new Vector3(xPos, 0, zPos));
                    }
                }
            }

        }

        private void InitExplosion(Vector3 position)
        {
            var explosion = explosionPool.GetPooledObject().GetComponent<EvilMageExplosionProjectileBehavior>();

            explosion.Init(new ProjectileInfo
            {
                canPassObstacles = true,
                damages = mageData.GetExplosionDamage(),
                statusEffects = null,
                direction = Vector3.forward,
                spawnPosition = position,
                owner = this,
                projectile = explosionProjectile,
                targetsPlayer = true
            });

            explosions.Add(explosion);
        }

        

        #endregion

        #region Clones State

        private void InitClonesState()
        {
            CanGetHitOffset = false;

            isWaitingClonesDeath = false;

            state = State.Clones;

            GameObject teleportationParticle = teleportationPool.GetPooledObject();

            teleportationParticle.transform.position = transform.position.SetY(1.5f);
            teleportationParticle.transform.localScale = Vector3.one;

            TweenCase scaleTween = null;
            scaleTween = transform.DOScale(0, 0.5f).OnComplete(() => {
                tweenCases.Remove(scaleTween);

                LevelObjectSpawner.Enemies.Remove(this);

                SpawnClones();

                isWaitingClonesDeath = true;
            });

            tweenCases.Add(scaleTween);
        }

        private void SpawnClones()
        {
            for(int i = 0; i < 360; i += 72)
            {
                Vector3 position = transform.position + Quaternion.Euler(0, i, 0) * Vector3.forward * 2;

                if (position.x <= 0.5f) position.x = 0.5f;
                if (position.x >= GameController.CurrentRoom.Size.x - 0.5f) position.x = GameController.CurrentRoom.Size.x - 0.5f;
                if (position.z <= 0.5f) position.z = 0.5f;
                if (position.z >= GameController.CurrentRoom.Size.y - 0.5f) position.z = GameController.CurrentRoom.Size.y - 0.5f;

                LevelController.SpawnAdditionalEnemy(cloneToSummon, position, CLONE_SECRET_MESSAGE);
            }
        }

        private void ClonesStateUpdate()
        {
            if (!isWaitingClonesDeath) return;

            bool hasClones = false;

            for(int i = 0; i < LevelObjectSpawner.Enemies.Count; i++)
            {
                if(LevelObjectSpawner.Enemies[i].SpecialMessage == CLONE_SECRET_MESSAGE)
                {
                    hasClones = true;

                    break;
                }
            }

            if (!hasClones)
            {
                isWaitingClonesDeath = false;

                GameObject teleportationParticle = teleportationPool.GetPooledObject();

                teleportationParticle.transform.position = transform.position.SetY(1.5f);
                teleportationParticle.transform.localScale = Vector3.one;

                transform.localScale = Vector3.zero;

                LevelObjectSpawner.Enemies.Add(this);

                TweenCase scaleTween = null;
                scaleTween = transform.DOScale(1, 0.5f).OnComplete(() => {
                    tweenCases.Remove(scaleTween);

                    InitIdleState();
                });

                tweenCases.Add(scaleTween);
            }
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

            for (int i = 0; i < explosions.Count; i++)
            {
                explosions[i].Disable();
            }

            explosions.Clear();

            explosionPool.ReturnToPoolEverything();

        }

        #endregion

        public enum State
        {
            Idle, Moving, ExplosionAttack, Clones, Dying
        }

        /*
        public override void StartAI()
        {

            mageData = enemyData as EvilMageData;

            Tween.DelayedCall(0.5f, () => {
                StartCoroutine(SimulateAI());
            });
        }

        public IEnumerator SimulateAI()
        {
            while (gameObject.activeSelf)
            {
                yield return RandomMove();
                yield return Idle();
                yield return MeteorAttack();
                yield return Idle();
                yield return SimpleAttack();

            }
        }

        public IEnumerator MeteorAttack()
        {
            CanGetHitOffset = false;

            shouldLookAtPlayer = true;

            enemyAnimator.SetTrigger(MAGIC_1_TRIGGER);

            if (mageData.MeteorAttackAudio != null)
            {
                AudioController.PlaySound(mageData.MeteorAttackAudio, GameController.Sound);
            }

            MeteorProjectileBehaviour projectile = ProjectilesController.GetMeteorProjectile(mageData.MeteorProjectile);

            projectile.transform.position = PlayerController.Position.SetY(0);

            //projectile.Damage[DamageType.Base] = mageData.MeteorProjectile.Damage;

            projectile.Explode();

            yield return new WaitForSeconds(3);
        }

        public IEnumerator SimpleAttack()
        {
            CanGetHitOffset = false;

            shouldLookAtPlayer = true;

            enemyAnimator.SetTrigger(ATTACK_TRIGGER);

            if(mageData.SimpleAttackAudio != null)
            {
                AudioController.PlaySound(mageData.SimpleAttackAudio, GameController.Sound);
            }

            yield return new WaitForSeconds(0.4f);

            //for(int i = 0; i < simpleAttackSpawners.Count; i++)
            //{
            //ProjectilesController.ShootProjectile(simpleProjectile, 100, simpleAttackSpawners[i], true);
            //}

            Vector3 minionPosition;

            do
            {
                minionPosition = new Vector3(Random.Range(0.5f, GameController.CurrentRoom.Size.x - 0.5f), 0, Random.Range(0.5f, GameController.CurrentRoom.Size.y - 0.5f));
            } while (Physics.CheckBox(minionPosition, Vector3.one * 0.5f, new Quaternion(), 2 ^ GameController.OBSTACLE_LAYER));

            LevelController.SpawnAdditionalEnemy(enemyToSummon, minionPosition);

            yield return new WaitForSeconds(1f);
        }

        private IEnumerator Idle()
        {
            CanGetHitOffset = true;

            shouldLookAtPlayer = true;

            enemyAnimator.SetTrigger(IDLE_TRIGGER);

            yield return new WaitForSeconds(idleDuration);

        }

        public IEnumerator RandomMove()
        {
            CanGetHitOffset = false;

            enemyAnimator.ResetTrigger(IDLE_TRIGGER);
            enemyAnimator.SetTrigger(START_RUNNING_TRIGGER);

            Vector3 destinationPosition = new Vector3(Random.Range(GameController.CurrentRoom.Size.x / 2 - 3f, GameController.CurrentRoom.Size.x / 2 + 3f), 0, Random.Range(GameController.CurrentRoom.Size.y / 2 - 5f, GameController.CurrentRoom.Size.y / 2 + 5f));

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
        }


        protected override void Die()
        {
            base.Die();
            enemyAnimator.SetTrigger(DIE_TRIGGER);

            deathParticle.Play();

            Tween.DelayedCall(0.5f, () => {
                transform.DOScale(0, 0.3f).OnComplete(() => {

                    gameObject.SetActive(false);

                    IsDying = false;

                    transform.localScale = Vector3.one;
                });
            });
        }

        private void Update()
        {
            if (shouldLookAtPlayer) Forward = Vector3.Lerp(Forward, (PlayerController.Position - Position).normalized, Time.deltaTime * 5).SetY(0);

            healthCanvas.transform.forward = -CameraController.MainCamera.transform.forward;
        }*/

    }

}