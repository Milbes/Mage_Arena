#pragma warning disable 649, 414

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Watermelon;

namespace Watermelon
{

    public class PlayerController : MonoBehaviour, IGameplayEntity
    {
        public static PlayerController playerController;

        public static UltimateJoystick Joystick { get; private set; }

        private static WeaponBehavior Weapon { get; set; }

        [SerializeField] Transform mageParent;

        [Space]
        [SerializeField] HealthCanvas healthCanvas;

        [Space]
        [SerializeField] Transform weaponHolder;

        [SerializeField] PlayerMageBehavior mageBehavior;
        private static PlayerMageBehavior MageBehavior => playerController.mageBehavior;


        private static HealthCanvas HealthCanvas => playerController.healthCanvas;
        
        [SerializeField] Transform shieldTransform;
        private static Transform ShieldTransform => playerController.shieldTransform;

        [Header("Sounds")]
        [SerializeField] AudioClip healSound;
        [SerializeField] AudioClip resurectSound;
        [SerializeField] AudioClip getHitSound;
        [SerializeField] AudioClip deathSound;
        [SerializeField] AudioClip dodgeSound;

        private static AudioClip HealSound => playerController.healSound;
        private static AudioClip ResurectSound => playerController.resurectSound;
        private static AudioClip GetHitSound => playerController.getHitSound;
        private static AudioClip DeathSound => playerController.deathSound;
        private static AudioClip DodgeSound => playerController.dodgeSound;

        private Vector3 input;

        private Rigidbody rb;

        private static bool isPlaying = false;

        public static Vector3 Position => playerController.transform.position;

        public static Transform Transform => playerController.transform;

        private static bool hasUsedSecondLife = false;
        private static bool hasSecondLife = false;
        private static float secondLifeMagnitude = 0;

        public static float MaxHP { get; private set; }
        public static float HP { get; private set; }
        public static float Damage { get; private set; }
        public static float CritChance { get; private set; }
        public static float HealthRegen { get; private set; }

        public static float initialHealingBoost = 1;
        public static float healingBoost = 1;

        public event OnEntityDiedDelegate onEntityDied;
        public event OnEntityHitDelegate onEntityHitEvent;
        public event OnEntityAttackDelegate onEntityAttackEvent;

        private static Character.Stats stats;

        private static IEnumerator ShieldCoroutine()
        {
            while (true)
            {
                yield return null;

                ShieldTransform.eulerAngles += Vector3.up * 180 * Time.deltaTime;
            }
        }

        public void IncreaseAttackSpeed(float magnitude)
        {
            mageBehavior.IncreaseAttackSpeed(magnitude);
        }

        public void IncreaseHealing(float magnitude)
        {
            healingBoost = initialHealingBoost * magnitude;
        }

        public bool IsActiveSelf()
        {
            return gameObject.activeSelf;
        }

        private void Awake()
        {
            playerController = this;

            rb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            Joystick = UltimateJoystick.GetUltimateJoystick("Joystick");
        }

        public static void InitLevel()
        {
            ItemHolder weaponItemHolder = Character.GetEquipedItem(EquipableItem.Weapon);
            if (weaponItemHolder != null)
            {
                WeaponItem weaponItem = weaponItemHolder.Item as WeaponItem;

                Weapon = Instantiate(weaponItem.WeaponPrefab).GetComponent<WeaponBehavior>();

                Weapon.Init(weaponItem);
            }
            else
            {
                // Load default staff
                WeaponItem weaponItem = ItemSettings.GetDefaultWeapon();

                Weapon = Instantiate(weaponItem.WeaponPrefab).GetComponent<WeaponBehavior>();

                Weapon.Init(weaponItem);
            }

            Weapon.transform.SetParent(playerController.weaponHolder);
            Weapon.transform.localScale = Vector3.one * 100;
            Weapon.transform.localPosition = Vector3.zero;
            Weapon.transform.localEulerAngles = Vector3.zero;

            MageBehavior.InitLevel();

            MaxHP = Character.GetStats().health;

            HP = MaxHP;

            isImune = false;
        }

        private static void CalculateStats(bool withHealth = true)
        {
            //if (withHealth) MaxHP = Utils.CalculateHealth(SkinData, skinSave.playerLevel + skinSave.healthLevel);
            //MovementSpeed = 6f;//Utils.CalculateMovementSpeed(SkinData, skinSave.playerLevel + skinSave.movementSpeedLevel);
            //AttackSpeed = 1f;//Utils.CalculateAttackSpeed(SkinData, Weapon.AttackSpeed, skinSave.attackSpeedLevel);
            //Damage = Utils.CalculateDamage(SkinData, Weapon.Damage, skinSave.damageLevel);
            //CritChance = Utils.CalculateCriticalChance(SkinData, skinSave.critChanceLevel);
            //HealthRegen = Utils.CalculateHealthRegen(SkinData, skinSave.healthRegenLevel);*/
        }

        public static void InitPlayer(bool resetHP)
        {
            playerController.transform.position = new Vector3(GameController.CurrentRoom.Size.x / 2f, 1, 1f);

            CameraController.Target = playerController;

            isPlaying = true;

            if (resetHP)
            {
                HP = MaxHP;

                HealthCanvas.SetFill(1, false, (int)MaxHP);
            }

            MageBehavior.InitIdleState();

            isImune = false;

            stats = Character.GetStats();
        }


        public static void DisablePlayer()
        {
            isPlaying = false;
        }

        private EnemyBehavior target;
        private static EnemyBehavior Target => playerController.target;

        private void Update()
        {
            if (!isPlaying) return;

            if (isDying) return;

            if (input == Vector3.zero)
            {
                if (Joystick.GetJoystickState())
                {
                    if (MageBehavior.MageState != PlayerMageBehavior.State.Running)
                    {
                        MageBehavior.InitRunState();
                    }
                } else
                {
                    
                    if(target == null || !target.gameObject.activeSelf)
                    {
                        if (MageBehavior.MageState != PlayerMageBehavior.State.Idle)
                        {
                            MageBehavior.InitIdleState();
                        }
                    } else
                    {
                        if (MageBehavior.MageState != PlayerMageBehavior.State.Attack)
                        {
                            MageBehavior.InitAttackState();
                        }

                        mageParent.forward = Vector3.Lerp(mageParent.forward, (target.transform.position - transform.position).normalized.SetY(0), Time.deltaTime * 30);
                    }
                }
            }
            else
            {
                if (MageBehavior.MageState != PlayerMageBehavior.State.Running)
                {
                    MageBehavior.InitRunState();
                }

                rb.MovePosition(rb.position + input.normalized * MageBehavior.MovementSpeed * Time.deltaTime);

                mageParent.forward = Vector3.Lerp(mageParent.forward, input.normalized, Time.deltaTime * 30);
            }

            if (Random.value < 0.032f)
            {
                EnemyBehavior newTarget = LevelController.GetClosestEnemy(transform.position);

                if(newTarget != null)
                {
                    if (target == null)
                    {
                        target = newTarget;
                        target.SelectAsTarget();
                    }
                    else if (target != newTarget)
                    {
                        target.UnselectAsTarget();
                        target = newTarget;
                        target.SelectAsTarget();
                    }
                }

            }
        }

        public static void FireWeapon()
        {
            Weapon.Fire(Target);

            // TODO : Implement player's damage calculation upon fiering a weapon

            OnOwnerAttackInfo info = new OnOwnerAttackInfo
            {
                baseDamage = GetDamage()[0],
            };

            playerController.onEntityAttackEvent?.Invoke(info);
        }

        private void FixedUpdate()
        {
            healthCanvas.transform.forward = -CameraController.MainCamera.transform.forward;

            input = new Vector3(
                Joystick.HorizontalAxis,
                0,
                Joystick.VerticalAxis);
        }


        public static void Heal()
        {
            float prevHp = HP;

            HP += MaxHP / 2;

            if (HP > MaxHP) HP = MaxHP;

            HealthCanvas.SetFill(HP / MaxHP, false, (int)HP);

            HealthCanvas.TakeHeal((int)(HP - prevHp));

            GameAudioController.PlaySound(HealSound);
        }

        public static void TakeDamage(float damage)
        {
            // Special effects callback
            //damage = SpecialEffectsController.OnPlayerHitted(damage);

            HP -= damage;

            //HealthCanvas.TakeDamage((int)damage);
            HealthCanvas.SetFill(HP < 0 ? 0 : HP / MaxHP, false, (int)HP);

            if (HP <= 0)
            {
                MageBehavior.InitDyingState();

                isDying = true;

                GameAudioController.PlaySound(DeathSound);

                Tween.DelayedCall(1, () => {

                    if (hasSecondLife)
                    {
                        RessurectSecondLife();
                    } else
                    {
                        if (!GameController.HasResurected)
                        {
                            GamePanelBehavior.ShowResurectPopup();
                        }
                        else
                        {
                            Finish();
                        }
                    }
                });
            }
            else
            {
                //isImune = true;
                //Tween.DelayedCall(0.5f, () => isImune = false);
            }
        }

        private static void RessurectSecondLife()
        {
            hasSecondLife = false;
            hasUsedSecondLife = true;

            MageBehavior.InitResurrectingState();

            HP = MaxHP * secondLifeMagnitude;

            HealthCanvas.SetFill(HP / MaxHP, false, (int)HP);

            GameAudioController.PlaySound(ResurectSound);

            Tween.DelayedCall(1.2f, () => {
                isDying = false;
            });
        }

        public static void Resurrect()
        {
            MageBehavior.InitResurrectingState();

            GameController.HasResurected = true;

            HP = MaxHP;

            HealthCanvas.SetFill(1, false, (int)MaxHP);

            GameAudioController.PlaySound(ResurectSound);

            Tween.DelayedCall(1.2f, () => {
                isDying = false;
            });
        }

        public static void Finish()
        {
            isDying = false;
            isPlaying = false;

            GameController.FinishRoom(false);
        }

        private static List<Damage> takenDamage = new List<Damage>();

        public void GetHit(Damage damage)
        {
            takenDamage.Add(damage);
        }

        public void GetHit(ProjectileInfo projectileInfo)
        {
            if (isDying || !isPlaying) return;

            if (!isImune)
            {
                OnOwnerGetHitInfo info = new OnOwnerGetHitInfo
                {
                    projectileInfo = projectileInfo,
                    projectileDirection = projectileInfo.direction,
                    hitPosition = transform.position
                };

                onEntityHitEvent?.Invoke(info);

                if (!info.missed)
                {
                    takenDamage.AddRange(projectileInfo.damages);

                    GameAudioController.PlaySound(getHitSound);
                } else
                {
                    HealthCanvas.ShowText(HealthTextType.Air, "Miss", true);

                    GameAudioController.PlaySound(dodgeSound);
                }

                isImune = true;
                Tween.DelayedCall(0.3f, () =>
                {
                    isImune = false;
                });
            }
        }

        private void LateUpdate()
        {
            if (takenDamage.IsNullOrEmpty()) return;

            if (isDying || !isPlaying) return;

            float combinedDamage = 0;

            for (int i = 0; i < takenDamage.Count; i++)
            {
                Damage damage = takenDamage[i];

                switch (damage.type)
                {
                    case DamageType.Fire:

                        damage.amount *= (100 - stats.fireResistance.Value) / 100f;
                        break;

                    case DamageType.Ice:

                        damage.amount *= (100 - stats.iceResistance.Value) / 100f;
                        break;

                    case DamageType.Shadow:

                        damage.amount *= (100 - stats.shadowResistance.Value) / 100f;
                        break;

                    case DamageType.Storm:

                        damage.amount *= (100 - stats.stormResistance.Value) / 100f;
                        break;
                }

                combinedDamage += damage.amount;

                healthCanvas.ShowText(HealthCanvas.GetTextType(damage.type), damage.amount.ToString(), damage.isCrit);
            }

            if (combinedDamage < 0) combinedDamage = 0;

            TakeDamage(combinedDamage);

            takenDamage.Clear();
        }

        public void Stun()
        {
            // TODO: Implement player getting stunned
        }

        public void Unstun()
        {
            // TODO: Implement player getting unstunned
        }

        public void SlowDown(Transform particleTransform, float magnitude)
        {
            // TODO: Implement player slowing down
        }

        public void ResetSpeed()
        {
            // TODO: Implement player speed reseting
        }

        public void IncreaseMaxHealth(float multiplier)
        {
            float addition = MaxHP * (multiplier - 1);

            HP += addition;
            MaxHP += addition;

            healthCanvas.TakeHeal((int) addition);
            healthCanvas.SetFill(HP / MaxHP, true, (int)HP);

            GameAudioController.PlaySound(healSound);
        }

        public void Heal(float percent)
        {
            float difference = MaxHP * percent * healingBoost;

            HP += difference;
            if (HP > MaxHP) HP = MaxHP;

            healthCanvas.TakeHeal((int)difference);
            healthCanvas.SetFill(HP / MaxHP, true, (int)HP);

            GameAudioController.PlaySound(healSound);
        }

        public bool HasUsedSecondLife()
        {
            return hasUsedSecondLife;
        }

        public void SetSecondLife(bool isActive, float magnitude)
        {
            hasSecondLife = isActive;
            secondLifeMagnitude = magnitude;
        }

        public void SetAbilityTyAttack(bool canAttack)
        {

        }

        static bool isDying = false;

        public static bool isImune = false;

        private void OnTriggerEnter(Collider other)
        {

            if (other.gameObject.layer == GameController.FINISH_LAYER)
            {
                GameController.FinishRoom(true);
            }
            /*else if (other.gameObject.layer == GameController.PROJECTILE_LAYER && !isDying && !isImune)
            {

                ProjectileBehavior projectile = other.GetComponent<ProjectileBehavior>();
                if (projectile == null) projectile = other.transform.parent.GetComponent<ProjectileBehavior>();
                if (projectile.TargetsPlayer)
                {
                    //float damage = AbilitiesController.OnPlayerHit(projectile.Damage);

                    

                    // TODO: implement player taking damage

                    // TODO: implement player imunity and resistance checks

                    //TakeDamage(info.damage);
                }
            }*/
            else if (other.gameObject.layer == GameController.ENEMY_LAYER && !isDying && !isImune)
            {

                EnemyBehavior enemy = other.GetComponent<EnemyBehavior>();

                if (enemy == null)
                {
                    enemy = other.transform.parent.GetComponent<EnemyBehavior>();
                }

                //TakeDamage(enemy.BodyDamage);

                GetHit(new Damage
                {
                    amount = enemy.BodyDamage,
                    isCrit = false,
                    type = DamageType.Base
                });

            }
            else if (other.gameObject.layer == GameController.GREEN_ORB_LAYER)
            {
                LevelController.GreenOrbEntered();
            }
        }

        public static List<Damage> GetDamage()
        {
            return new List<Damage>(stats.GetDamages());
        }

        public static List<StatusEffectInfo> GetStatusEffects()
        {
            // TODO: implement player's status effects
            
            return new List<StatusEffectInfo>();// { new BurningEffectInfo { damagePerTick = 50, ticksLeft = 5 }, new StunEffectInfo { duration = 5 } };
        }

        public Transform GetTransform()
        {
            return transform;
        }

        public float GetBaseDamageAmount()
        {
            return stats.GetBaseDamage().amount;
        }
    }
}