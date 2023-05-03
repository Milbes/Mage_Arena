using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{

    public class PlayerMageBehavior : MonoBehaviour
    {
        private static readonly int START_RUNNING_TRIGGER = Animator.StringToHash("Start Running");
        private static readonly int FIRE_TRIGGER = Animator.StringToHash("Fire");
        private static readonly int IDLE_TRIGGER = Animator.StringToHash("Idle");
        private static readonly int DIE_TRIGGER = Animator.StringToHash("Die");
        private static readonly int RESURRECT_TRIGGER = Animator.StringToHash("Resurrect");

        private static readonly int ATTACK_SPEED_VARIABLE = Animator.StringToHash("Attack Speed");
        private static readonly int MOVEMENT_SPEED_VARIABLE = Animator.StringToHash("Movement Speed");

        private static readonly string STEP_PARTICLE_POOL_NAME = "StepParticle";

        private static Pool stepParticlePool;

        public State MageState { get; private set; }

        [SerializeField] SkinData skinData;

        [Space]
        [SerializeField] Animator wizardAnimator;

        private SkinSave skinSave;

        public SkinData SkinData => skinData;

        public Animator WizardAnimator => wizardAnimator;

        [SerializeField] List<AudioClip> stepSounds;

        public float MovementSpeed
        {
            get => WizardAnimator.GetFloat(MOVEMENT_SPEED_VARIABLE);
            private set => WizardAnimator.SetFloat(MOVEMENT_SPEED_VARIABLE, value);
        }

        private float initialAttackSpeed;
        private float attackSpeed;
        public float AttackSpeed
        {
            get => attackSpeed;
            private set
            {

                if (value > 0.75f)
                {
                    AttackAnimationSpeed = 1;
                }
                else
                {
                    AttackAnimationSpeed = value / 0.75f;
                }

                WizardAnimator.SetFloat(ATTACK_SPEED_VARIABLE, 1f / AttackAnimationSpeed);
                attackSpeed = value;

            }
        }
        public float AttackAnimationSpeed { get; private set; }

        List<TweenCase> tweenCases;

        private float lastStepTime;

        private void Awake()
        {
            stepParticlePool = PoolManager.GetPoolByName(STEP_PARTICLE_POOL_NAME);

            tweenCases = new List<TweenCase>();
        }

        private void Start()
        {
            Character.Stats stats = Character.GetStats();

            MovementSpeed = stats.movementSpeed;
            initialAttackSpeed = stats.attackSpeed;
            AttackSpeed = stats.attackSpeed;
        }

        public void Update()
        {
            switch (MageState)
            {
                case State.Running:
                
                    float time = Time.time;

                    if (time - lastStepTime > SkinData.StepDelay)
                    {
                        lastStepTime = time;

                        if (SkinData.StepSound != null)
                        {
                            //AudioController.PlaySound(SkinData.StepSound, GameController.Sound * 0.7f, Random.Range(0.9f, 1.2f));
                            GameAudioController.PlaySound(stepSounds.GetRandomItem());
                        }

                        ParticleSystem stepParticle = stepParticlePool.GetPooledObject().GetComponent<ParticleSystem>();
                        stepParticle.Play();
                        stepParticle.transform.position = transform.position.SetY(0);

                        Tween.DelayedCall(0.5f, () => {
                            if(stepParticle != null) stepParticle.gameObject.SetActive(false);
                        });
                    }

                    break;
                
            }
        }

        public void InitLevel()
        {
            skinSave = GameController.GetSkinSave(skinData);

            skinSave.healthLevel = 0;
            skinSave.damageLevel = 0;
            skinSave.movementSpeedLevel = 0;
            skinSave.attackSpeedLevel = 0;
            skinSave.critChanceLevel = 0;
            skinSave.healthRegenLevel = 0;
        }

        private void ResetTriggers()
        {
            wizardAnimator.ResetTrigger(START_RUNNING_TRIGGER);
            wizardAnimator.ResetTrigger(FIRE_TRIGGER);
            wizardAnimator.ResetTrigger(IDLE_TRIGGER);
            wizardAnimator.ResetTrigger(DIE_TRIGGER);
            wizardAnimator.ResetTrigger(RESURRECT_TRIGGER);

            for (int i = 0; i < tweenCases.Count; i++)
            {
                if (!tweenCases[i].isCompleted) tweenCases[i].Kill();
            }

            tweenCases.Clear();
        }


        public void InitIdleState()
        {
            MageState = State.Idle;

            ResetTriggers();

            wizardAnimator.SetTrigger(IDLE_TRIGGER);
        }


        public void InitAttackState()
        {
            MageState = State.Attack;

            ResetTriggers();

            Attack();
        }

        public void Attack()
        {
            wizardAnimator.SetTrigger(FIRE_TRIGGER);

            float delay = AttackAnimationSpeed * 0.75f / 2f;

            TweenCase delayCase = null;
            delayCase = Tween.DelayedCall(delay, () => {

                PlayerController.FireWeapon();

                tweenCases.Remove(delayCase);

                delayCase = Tween.DelayedCall(AttackSpeed - delay, () =>
                {
                    tweenCases.Remove(delayCase);

                    Attack();
                });

                tweenCases.Add(delayCase);
            });

            tweenCases.Add(delayCase);
        }

        public void InitRunState()
        {
            MageState = State.Running;

            ResetTriggers();

            wizardAnimator.SetTrigger(START_RUNNING_TRIGGER);
        }

        public void InitDyingState()
        {
            MageState = State.Dying;

            ResetTriggers();

            wizardAnimator.SetTrigger(DIE_TRIGGER);

            if (skinData.DeathSound != null)
            {
                AudioController.PlaySound(skinData.DeathSound, GameController.Sound);
            }
        }

        public void InitResurrectingState()
        {
            MageState = State.Resurrecting;

            ResetTriggers();

            wizardAnimator.SetTrigger(RESURRECT_TRIGGER);

            if (skinData.ResurrectSound != null)
            {
                AudioController.PlaySound(skinData.ResurrectSound, GameController.Sound);
            }
        }

        public void IncreaseAttackSpeed(float magnitude)
        {
            AttackSpeed = initialAttackSpeed / magnitude;
        }

        public enum State
        {
            Idle, Attack, Running, Dying, Stunned, Resurrecting
        }

    }

}