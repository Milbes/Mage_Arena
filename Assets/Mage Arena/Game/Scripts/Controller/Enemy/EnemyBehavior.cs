#pragma warning disable 649

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using Watermelon;

public abstract class EnemyBehavior : MonoBehaviour, IGameplayEntity
{

    private static readonly int DAMAGE_MULTIPLIER_ID = Shader.PropertyToID("_DamageMultiplier");

    protected readonly int START_RUNNING_TRIGGER = Animator.StringToHash("Start Running");
    protected readonly int IDLE_TRIGGER = Animator.StringToHash("Idle");
    protected readonly int ATTACK_TRIGGER = Animator.StringToHash("Attack");
    protected readonly int ATTACK2_TRIGGER = Animator.StringToHash("Attack 2");
    protected readonly int DIE_TRIGGER = Animator.StringToHash("Die");
    protected readonly int STUN_TRIGGER = Animator.StringToHash("Stun");
    protected readonly int GET_HIT_TRIGGER = Animator.StringToHash("Get Hit");

    protected readonly int MAGIC_1_TRIGGER = Animator.StringToHash("Magic 1");
    protected readonly int MAGIC_2_TRIGGER = Animator.StringToHash("Magic 2");

    public event OnEntityDiedDelegate onEntityDied;
    public event OnEntityHitDelegate onEntityHitEvent;
    public event OnEntityAttackDelegate onEntityAttackEvent;

    [System.NonSerialized] public bool shouldSpawnCoins = true;

    protected string specialMessage;
    public string SpecialMessage => specialMessage;

    protected bool canAttack = true;

    [SerializeField] protected Animator enemyAnimator;
    [SerializeField] protected NavMeshAgent navMeshAgent;

    [SerializeField] GameObject targetCircle;

    [Space]
    [SerializeField] protected EnemyData enemyData;

    [Space]
    [SerializeField] protected HealthCanvas healthCanvas;

    [Space]
    [SerializeField] protected SkinnedMeshRenderer meshRenderer;
    [SerializeField] protected Transform scaleTransform;

    [Space]
    [SerializeField] protected Collider hitCollider;

    [Space]
    [SerializeField] protected ParticleSystem hitParticle;
    

    private MaterialPropertyBlock propertyBlock;

    public List<Damage> receivedDamage;

    public bool IsBoss => enemy.IsBoss;

    private bool isAlive;
    public bool IsAlive => isAlive;

    public float BodyDamage => enemyData.BodyDamage;
    public float Speed => enemyData.Speed;

    public bool IsDying { get; protected set; }

    public bool CanGetHitOffset { get; protected set; }

    public Vector3 Position { get => transform.position; protected set => transform.position = value; }

    protected Vector3 Forward { get => transform.forward; set => transform.forward = value; }

    public float Health { get; protected set; }
    public float MaxHealth { get; protected set; }

    public bool JustGotHit { get; protected set; }

    [HideInInspector] public Enemy enemy;

    public abstract void StartAI();
    private float lastHitTime;

    public void ResetTriggers()
    {
        

        enemyAnimator.ResetTrigger(START_RUNNING_TRIGGER);
        enemyAnimator.ResetTrigger(IDLE_TRIGGER);
        enemyAnimator.ResetTrigger(ATTACK_TRIGGER);
        enemyAnimator.ResetTrigger(DIE_TRIGGER);
        enemyAnimator.ResetTrigger(GET_HIT_TRIGGER);
        enemyAnimator.ResetTrigger(MAGIC_1_TRIGGER);
        enemyAnimator.ResetTrigger(MAGIC_2_TRIGGER);
    }

    public bool IsActiveSelf()
    {
        return gameObject != null && gameObject.activeSelf;
    }

    public void Diactivate()
    {
        StopAllCoroutines();

        UnselectAsTarget();

        Position = Vector3.zero;
        Forward = Vector3.forward;

        if(navMeshAgent != null) navMeshAgent.enabled = false;

    }

    public virtual void Stun() {}
    public virtual void Unstun(){}

    public virtual void SlowDown(Transform particleTransform, float magnitude) { }
    public virtual void ResetSpeed() { }

    public void IncreaseAttackSpeed(float magnitude)
    {
        // TODO: implement increase attack speed of an enemy
    }

    public void IncreaseHealing(float magnitude)
    {
        // TODO: implement increase healing boost of an enemy
    }

    public void Init(Vector2Int position, Enemy enemy)
    {
        if (propertyBlock == null) propertyBlock = new MaterialPropertyBlock();

        this.enemy = enemy;

        isAlive = true;
        MaxHealth = enemyData.Health * GameController.CurrentRoom.HealthMultiplier;
        Health = MaxHealth;

        transform.position = new Vector3(position.x, 0, position.y);
        transform.eulerAngles = new Vector3(0, 180, 0);

        healthCanvas.Show();

        healthCanvas.SetFill(1);

        IsDying = false;

        CanGetHitOffset = true;

        StartAI();

        specialMessage = "";

        lastHitTime = 0;

        hitCollider.enabled = true;

        receivedDamage = new List<Damage>();

        //Tween.DelayedCall(1, () => StatusEffectsController.GetStatusEffect<BurningStatusEffect>(StatusEffectType.Burning).RegisterEffect(new BurningEffectInfo { enemy = this, damagePerTick = 100, ticksLeft = 5 }));
    }

    public void Init(Enemy enemy, Transform spawner, string message = "")
    {
        Init(enemy, spawner.position, spawner.forward, message);
    }

    public void Init (Enemy enemy, Vector3 position, Vector3 forward, string message = "")
    {
        if (propertyBlock == null) propertyBlock = new MaterialPropertyBlock();

        this.enemy = enemy;

        isAlive = true;

        MaxHealth = enemyData.Health * GameController.CurrentRoom.HealthMultiplier;
        Health = MaxHealth;

        transform.position = position;

        transform.forward = forward;

        CanGetHitOffset = true;

        healthCanvas.Show();

        healthCanvas.SetFill(1);

        IsDying = false;

        specialMessage = message;

        StartAI();

        hitCollider.enabled = true;

        receivedDamage = new List<Damage>();
    }
    



    public void SelectAsTarget()
    {
        if(targetCircle != null)
            targetCircle.SetActive(true);
    }

    public void UnselectAsTarget()
    {
        targetCircle.SetActive(false);
    }

    public void TakeDamage(float damage, bool isCritical, bool callEvent = true)
    {
        if (!isAlive)
            return;

        if(callEvent)
        {
            // Special effects callback
            SpecialEffectsController.OnEnemyHitted(this, damage, isCritical);
        }

        Health -= damage;

        float fill = Health / MaxHealth;

        if (fill <= 0) {
            healthCanvas.SetFill(0);
            Tween.DelayedCall(1f, () => { 
                healthCanvas.Hide();
            });
        }  else
        {
            healthCanvas.SetFill(fill, true);
        }


        if (Health <= 0)
        {
            isAlive = false;

            UnselectAsTarget();

            IsDying = true;

            Die();

            CameraController.ShakeCamera();

            hitCollider.enabled = false;
        }
    }

    public float GetBaseDamageAmount()
    {
        // TODO: Implement

        return 100;
    }

    public bool HasUsedSecondLife()
    {
        return false;
    }

    public void SetSecondLife(bool isActive, float magnitude)
    {

    }

    public void SetAbilityTyAttack(bool canAttack)
    {
        this.canAttack = canAttack;
    }

    protected virtual void Die()
    {
        onEntityDied?.Invoke(this);

        LevelController.OnEnemyDeth(this);

        AbilitiesController.OnEnemyDied(this);

        if (shouldSpawnCoins)
        {
            if (enemy.IsBoss)
            {
                DropController.SpawnGems(Position);
            }
            else
            {
                DropController.SpawnCoins(Position);
            }

            DropController.CheckDropItem(transform.position);
        }

        Tween.DelayedCall(0.8f, () =>
        {
            if (enemyData.DeathAudio != null)
            {
                GameAudioController.PlaySound(enemyData.DeathAudio);
            }
        }, true);
        

        // Special effects callback
        SpecialEffectsController.OnEnemyDies(this);

        for (int i = 0; i < activeEffectsCount; i++)
        {
            activeEffects[i].OnEnemyDies();
            activeEffects[i].Disable();
        }

        //ClearActiveEffects();

        
    }

    protected virtual void OnHitScale()
    {
        scaleTransform.DOScale(0.75f, 0.1f).SetEasing(Ease.Type.QuadOut).OnComplete(() => scaleTransform.DOScale(1, 0.2f));
    }

    protected virtual void OnHitOffset()
    {

    }

    public void GetHit(ProjectileInfo projectileInfo)
    {
        if(projectileInfo.damages != null) receivedDamage.AddRange(projectileInfo.damages);

    }

    public void GetHit(Damage damage)
    {
        receivedDamage.Add(damage);
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public void IncreaseMaxHealth(float multiplier)
    {
        MaxHealth *= multiplier;
    }

    public void Heal(float percent)
    {
        Health += Health * percent;

        if (Health > MaxHealth) Health = MaxHealth;
    }

    private void LateUpdate()
    {
        if (receivedDamage.IsNullOrEmpty()) return;

        float combinedDamage = 0;

        for(int i = 0; i < Damage.DamageTypes.Length; i++)
        {
            DamageType type = Damage.DamageTypes[i];

            float damageOfType = 0;

            receivedDamage.FindAll((damage) => damage.type == type).ForEach((damage) => damageOfType += damage.amount);

            if (damageOfType > 0) {

                // TODO: Check for imunities

                // TODO: Feed damage to a Health Canvas

                healthCanvas.ShowText(HealthCanvas.GetTextType(type), damageOfType.ToString());

                // TODO: Replace this simplified version later
                combinedDamage += damageOfType;
            } 
        }

        // TODO: Sound on get hit

        /*
        if (GameController.Sound != 0)
        {
            AudioController.PlaySound(projectile.projectileInfo.projectile.CriticalHitAudio, 1.2f);
        }

        if (GameController.Sound != 0)
        {
            AudioController.PlaySound(projectile.projectileInfo.projectile.HitAudio);
        } */

        TakeDamage(combinedDamage, false, false);

        hitParticle.Play();

        receivedDamage.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {

        // TODD: Rewrite on get hit behavior

        if(other.gameObject.layer == GameController.PROJECTILE_LAYER && !IsDying)
        {
            
            ProjectileBehavior projectile = other.GetComponent<ProjectileBehavior>();
            if(projectile == null) projectile = other.transform.parent.GetComponent<ProjectileBehavior>();

            if (!projectile.TargetsPlayer)
            {
                float time = Time.time;

                TweenCase shineCase1 = null;
                shineCase1 = this.DOAction((start, end, t) =>
                {
                    propertyBlock.SetFloat(DAMAGE_MULTIPLIER_ID, start + (end - start) * t);

                    meshRenderer.SetPropertyBlock(propertyBlock);

                    if (IsDying) shineCase1.Kill();


                }, 0, 0.4f, 0.1f).OnComplete(() => {

                    TweenCase shineCase2 = null;

                    shineCase2 = this.DOAction((start, end, t) =>
                    {
                        if (IsDying)
                        {
                            shineCase2.Kill();

                            return;
                        }

                        propertyBlock.SetFloat(DAMAGE_MULTIPLIER_ID, start + (end - start) * t);

                        meshRenderer.SetPropertyBlock(propertyBlock);

                    }, 0.4f, 0, 0.2f);
                });

                OnHitScale();

                if (time - lastHitTime > 0.7f && CanGetHitOffset /*&& damage < Health*/)
                {

                    lastHitTime = time;

                    Vector3 offset = Position + (Position - PlayerController.Position).normalized * 0.5f;

                    offset.y = 0;

                    if (NavMesh.Raycast(Position, offset, out NavMeshHit hit, NavMesh.AllAreas))
                    {
                        transform.DOMove(hit.position, 0.2f).SetEasing(Ease.Type.ExpoOut);
                        //Position = hit.position;
                    }
                    else
                    {
                        transform.DOMove(offset, 0.2f).SetEasing(Ease.Type.ExpoOut);
                    }

                    JustGotHit = true;

                    OnHitOffset();
                }
            }
        }
    }

    private List<EnemyEffect> activeEffects = new List<EnemyEffect>();
    public List<EnemyEffect> ActiveEffects => activeEffects;

    private int activeEffectsCount;
    public int ActiveEffectsCount => activeEffectsCount;

    public void AddEffect(EnemyEffect enemyEffect)
    {
        // Attach enemy behavior to effect
        enemyEffect.Init(this);

        for(int i = 0; i < activeEffectsCount; i++)
        {
            if(enemyEffect.EnemyEffectType == activeEffects[i].EnemyEffectType)
            {
                // Call override callback
                activeEffects[i].OnEffectOverrided();

                activeEffects[i].Init(enemyEffect.Time);

                return;
            }
        }

        // Call start callback
        enemyEffect.OnEffectStart();

        activeEffects.Add(enemyEffect);
        activeEffectsCount++;
    }

    public void RemoveEffect(EnemyEffect enemyEffect)
    {
        int activeEffectsCount = activeEffects.Count;
        for (int i = 0; i < activeEffectsCount; i++)
        {
            if (enemyEffect.EnemyEffectType == activeEffects[i].EnemyEffectType)
            {
                activeEffects[i].OnEffectRemoved();

                activeEffects.RemoveAt(i);
                activeEffectsCount--;

                return;
            }
        }
    }

    public void DoEffectsTick()
    {
        for (int i = 0; i < activeEffectsCount; i++)
        {
            if(activeEffects[i].IsActive)
            {
                // Do custom effect tick
                activeEffects[i].OnTick();

                // Do effect default tick
                activeEffects[i].Tick();
            }
            else
            {
                activeEffects[i].OnEffectRemoved();

                activeEffects.RemoveAt(i);
                activeEffectsCount--;

                i--;
            }
        }
    }
}

public enum EnemyEffectType
{
    Unknown = 0,
    Poison = 1,
    Fire = 2,
    Ice = 3
}

public abstract class EnemyEffect
{
    protected EnemyEffectType enemyEffectType;
    public EnemyEffectType EnemyEffectType => enemyEffectType;

    protected EnemyBehavior enemyBehavior;
    public EnemyBehavior EnemyBehavior => enemyBehavior;

    protected int time;
    public int Time => time;

    private bool isActive = false;
    public bool IsActive => isActive;

    public EnemyEffect(EnemyEffectType enemyEffectType, int time)
    {
        this.enemyEffectType = enemyEffectType;
        this.time = time;
    }

    public void Init(EnemyBehavior enemyBehavior)
    {
        this.enemyBehavior = enemyBehavior;

        isActive = true;
    }

    public void Init(int time)
    {
        this.time = time;

        isActive = true;
    }

    public void Tick()
    {
        if (!isActive)
            return;

        time -= 1;

        if (time <= 0)
            Disable();
    }

    public void Disable()
    {
        isActive = false;
    }

    public abstract void OnTick();

    public virtual void OnEnemyDies() { }

    public virtual void OnEffectStart() { }
    public virtual void OnEffectOverrided() { }
    public virtual void OnEffectRemoved() { }
}

public sealed class FireEnemyEffect : EnemyEffect
{
    private int damage;

    private GameObject effectGameObject;

    public FireEnemyEffect(EnemyEffectType enemyEffectType, int time, int damage) : base(enemyEffectType, time)
    {
        this.damage = damage;
    }

    public override void OnEffectStart()
    {
        effectGameObject = PoolManager.GetPoolByName("Fire").GetPooledObject();
        effectGameObject.transform.SetParent(enemyBehavior.transform);
        effectGameObject.transform.localPosition = Vector3.zero;
        effectGameObject.transform.localRotation = Quaternion.identity;
        effectGameObject.transform.localScale = Vector3.one * 5;
        effectGameObject.SetActive(true);

        effectGameObject.GetComponent<ParticleSystem>().Play();
    }

    public override void OnEffectRemoved()
    {
        effectGameObject.transform.SetParent(null);
        effectGameObject.SetActive(false);
    }

    public override void OnTick()
    {
        enemyBehavior.TakeDamage(damage, false, false);
    }
}

public sealed class PoisonEnemyEffect : EnemyEffect
{
    private int damage;

    private GameObject effectGameObject;

    public PoisonEnemyEffect(EnemyEffectType enemyEffectType, int time, int damage) : base(enemyEffectType, time)
    {
        this.damage = damage;
    }

    public override void OnEffectStart()
    {
        effectGameObject = PoolManager.GetPoolByName("Poison").GetPooledObject();
        effectGameObject.transform.SetParent(enemyBehavior.transform);
        effectGameObject.transform.localPosition = Vector3.zero;
        effectGameObject.transform.localRotation = Quaternion.identity;
        effectGameObject.transform.localScale = Vector3.one;
        effectGameObject.SetActive(true);

        effectGameObject.GetComponent<ParticleSystem>().Play();
    }

    public override void OnEffectRemoved()
    {
        effectGameObject.transform.SetParent(null);
        effectGameObject.SetActive(false);
    }

    public override void OnTick()
    {
        enemyBehavior.TakeDamage(damage, false, false);
    }
}