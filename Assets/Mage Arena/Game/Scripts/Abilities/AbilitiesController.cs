using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    public class AbilitiesController : MonoBehaviour
    {
        private static AbilitiesController instance;

        public static readonly OrbType[] orbTypes = (OrbType[])Enum.GetValues(typeof(OrbType));
        private static readonly int DISSAPEARE_TRIGGER = Animator.StringToHash("Disappeare");

        [SerializeField] AbilitiesDatabase database;
        private static AbilitiesDatabase Database => instance.database;

        [SerializeField] AudioClip spawnOrbSound;
        private static AudioClip SpawnOrbSound => instance.spawnOrbSound;

        private static List<AbilityInfo> activatedAbilities;

        private static List<IOnTickAbility> activeOnTickAbilities;
        private static List<IOnEnemyDiedAbility> activeOnEnemyDiedAbility;
        private static List<IProjectileAmountAbility> activeProjectileAmountAbilities;
        private static List<IProjectileEffectAbility> activeProjectileEffectAbilities;
        private static List<IProjectileHitBorderAbility> activeProjectileHitBorderAbility;
        private static List<IOnTargetHitByProjectileAbility> activeOnTargetHitByProjectileAbilities;

        private static Animator orbAnimator;

        private void Awake()
        {
            instance = this;

            activatedAbilities = new List<AbilityInfo>();

            activeOnTickAbilities = new List<IOnTickAbility>();
            activeOnEnemyDiedAbility = new List<IOnEnemyDiedAbility>();
            activeProjectileAmountAbilities = new List<IProjectileAmountAbility>();
            activeProjectileEffectAbilities = new List<IProjectileEffectAbility>();
            activeProjectileHitBorderAbility = new List<IProjectileHitBorderAbility>();
            activeOnTargetHitByProjectileAbilities = new List<IOnTargetHitByProjectileAbility>();

            Tween.DelayedCall(0.2f, () => {
                Database.abilities.ForEach((data) => {
                    if (data.activateOnPlay)
                    {
                        for(int i = 0; i < data.devTier; i++)
                        {
                            ActivateAbility(data, PlayerController.playerController);
                        }
                    }
                });
            });
        }

        private void Update()
        {
            for (int i = 0; i < activeOnTickAbilities.Count; i++)
            {
                IOnTickAbility ability = activeOnTickAbilities[i];

                ability.Tick();
            }
        }

        public static void OnRoomFinished()
        {
            for(int i = 0; i < activatedAbilities.Count; i++)
            {
                activatedAbilities[i].ability.OnRoomFinished();
            }
        }

        public static void OnRoomEntered()
        {
            for (int i = 0; i < activatedAbilities.Count; i++)
            {
                activatedAbilities[i].ability.OnRoomEntered();
            }
        }

        public static OrbType GetNextAbilityOrbType()
        {
            if (activatedAbilities.IsNullOrEmpty()) return orbTypes.GetRandomItem();

            var lastOrb = activatedAbilities[activatedAbilities.Count - 1].data.orbType;

            while (true) {
                var orb = orbTypes.GetRandomItem();

                if (orb != lastOrb) return orb;
            }
        }

        private static GameObject GetOrbPrefab(OrbType type)
        {
            for(int i = 0; i < Database.orbs.Count; i++)
            {
                OrbData orb = Database.orbs[i];

                if (orb.type == type) return orb.orbPrefab;
            }

            return null;
        }

        public static void SpawnOrbObject(OrbType type)
        {
            GameObject orb = Instantiate(GetOrbPrefab(type));

            orb.transform.position = new Vector3(GameController.CurrentRoom.Size.x / 2f, 2f, GameController.CurrentRoom.Size.y - 5f);
            orb.transform.rotation = Quaternion.Euler(0, 180, 0);

            orbAnimator = orb.transform.GetChild(0).GetComponent<Animator>();

            GameAudioController.PlaySound(SpawnOrbSound);
        }

        public static void DisappeareOrb()
        {
            if(orbAnimator != null) { 
                orbAnimator.ResetTrigger(DISSAPEARE_TRIGGER);
                orbAnimator.SetTrigger(DISSAPEARE_TRIGGER);

                Tween.DelayedCall(1.5f, () => {
                    Destroy(orbAnimator.transform.parent.gameObject);

                    orbAnimator = null;
                });
                
            }
        }

        public static void ShowAbilitiesSelectionWindow(OrbType orb)
        {
            var orbAbilities = Database.abilities.FindAll((data) => data.orbType == orb);

            for(int i = 0; i < orbAbilities.Count; i++)
            {
                var ability = orbAbilities[i];

                if (GetStackAmount(ability) == ability.maxStackAmount)
                {
                    orbAbilities.Remove(ability);
                    i--;
                }
            }

            while(orbAbilities.Count > 3)
            {
                orbAbilities.Remove(orbAbilities.GetRandomItem());
            }

            AbilitiesCanvasBehavior.Show(orbAbilities);
        }

        public static List<AbilityInfo> GetActiveAbilities()
        {
            return activatedAbilities;
        }

        public static int GetStackAmount(Ability ability)
        {
            AbilityInfo info = activatedAbilities.Find((abilityInfo) => abilityInfo.ability == ability);

            if (info == null) return 0;

            return info.stackAmount;
        }

        public static int GetStackAmount(AbilityData data)
        {
            AbilityInfo info = activatedAbilities.Find((abilityInfo) => abilityInfo.data == data);

            if (info == null) return 0;

            return info.stackAmount;
        }

        public static void ActivateAbility(AbilityData abilityData, IGameplayEntity owner)
        {
            AbilityInfo info = activatedAbilities.Find((abilityInfo) => abilityInfo.data == abilityData && abilityInfo.ability.Owner == owner);

            bool isNew = info == null;

            if (!isNew)
            {
                info.stackAmount++;
                info.ability.StackIncreased();
            } else
            {
                info = new AbilityInfo {
                    ability = abilityData.CreateAbilityInstance(),
                    data = abilityData,
                    stackAmount = 1
                };

                activatedAbilities.Add(info);

                info.ability.ActivateAbility(owner);

                if (info.ability is IOnTickAbility onTickAbility) activeOnTickAbilities.Add(onTickAbility);
                if (info.ability is IOnEnemyDiedAbility onEnemyDiedAbility) activeOnEnemyDiedAbility.Add(onEnemyDiedAbility);
                if (info.ability is IProjectileAmountAbility projectileAmountAbility) activeProjectileAmountAbilities.Add(projectileAmountAbility);
                if (info.ability is IProjectileEffectAbility projectileEffectAbility) activeProjectileEffectAbilities.Add(projectileEffectAbility);
                if (info.ability is IProjectileHitBorderAbility projectileHitBorderAbility) activeProjectileHitBorderAbility.Add(projectileHitBorderAbility);
                if (info.ability is IOnTargetHitByProjectileAbility onTargetHitByProjectileAbility) activeOnTargetHitByProjectileAbilities.Add(onTargetHitByProjectileAbility);
            }
        }

        private static void DisableAbility(Ability ability)
        {
            activatedAbilities.RemoveAll((info) => info.ability == ability);

            if (ability is IOnTickAbility onTickAbility) activeOnTickAbilities.Remove(onTickAbility);
            if (ability is IOnEnemyDiedAbility onEnemyDiedAbility) activeOnEnemyDiedAbility.Remove(onEnemyDiedAbility);
            if (ability is IProjectileAmountAbility projectileAmountAbility) activeProjectileAmountAbilities.Remove(projectileAmountAbility);
            if (ability is IProjectileEffectAbility projectileEffectAbility) activeProjectileEffectAbilities.Remove(projectileEffectAbility);
            if (ability is IProjectileHitBorderAbility projectileHitBorderAbility) activeProjectileHitBorderAbility.Remove(projectileHitBorderAbility);
            if (ability is IOnTargetHitByProjectileAbility onTargetHitByProjectileAbility) activeOnTargetHitByProjectileAbilities.Remove(onTargetHitByProjectileAbility);

            ability.DisableAbility();
        }

        public static void ProcessProjectile(ProjectileInfo projectileInfo)
        {
            List<ProjectileInfo> finalProjectiles = new List<ProjectileInfo>();

            finalProjectiles.Add(projectileInfo);

            // Applying effects to the projectiles

            for(int i = 0; i < activeProjectileEffectAbilities.Count; i++)
            {
                var ability = activeProjectileEffectAbilities[i];

                for (int j = 0; j < finalProjectiles.Count; j++)
                {
                    ability.AddEffectToProjectile(finalProjectiles[j]);
                }
            }

            //Applying all projectile amount abilities

            for (int i = 0; i < activeProjectileAmountAbilities.Count; i++)
            {
                IProjectileAmountAbility ability = activeProjectileAmountAbilities[i];

                //List<ProjectileInfo> additionalProjectiles = new List<ProjectileInfo>(); ;

                /*for (int j = 0; j < finalProjectiles.Count; j++)
                {
                    additionalProjectiles.AddRange();
                }*/

                finalProjectiles.AddRange(ability.ApplyAbilityToProjectile(projectileInfo));

                //additionalProjectiles.Clear();
            }

            // Fiering projectiles

            for (int i = 0; i < finalProjectiles.Count; i++)
            {
                ProjectileInfo info = finalProjectiles[i];

                ProjectilesController.ShootProjectile(info);
            }
        }

        public static void ProjectileHitTarget(ProjectileInfo projectileInfo, IGameplayEntity target)
        {
            for(int i = 0; i < activeOnTargetHitByProjectileAbilities.Count; i++)
            {
                var ability = activeOnTargetHitByProjectileAbilities[i];

                if (projectileInfo.owner == ability.GetOwner())
                {
                    ability.OnTargetHitByProjectile(projectileInfo, target);
                }
            }
        }

        public static bool OnProjectileHitObstacle(ProjectileBehavior projectile, Collider collider)
        {
            bool shouldTerminate = true;

            for(int i = 0; i < activeProjectileHitBorderAbility.Count; i++)
            {
                var ability = activeProjectileHitBorderAbility[i];

                if (projectile.projectileInfo.owner == ability.GetOwner())
                {
                    bool save = ability.OnProjectileHitBorder(projectile, collider);

                    if (shouldTerminate && save) shouldTerminate = false;
                }
            }

            return shouldTerminate;
        }

        public static void OnEnemyDied(IGameplayEntity enemy)
        {
            for(int i = 0; i < activeOnEnemyDiedAbility.Count; i++)
            {
                var ability = activeOnEnemyDiedAbility[i];

                ability.OnEnemyDied(enemy);
            }
        }
    }

    public class AbilityInfo
    {
        public AbilityData data;
        public Ability ability;
        public int stackAmount;
    }

    [System.Serializable]
    public struct OrbData {
        public OrbType type;
        public GameObject orbPrefab;
    }
}