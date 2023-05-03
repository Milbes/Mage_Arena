using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    public class StatusEffectsController : MonoBehaviour
    {
        private static StatusEffectsController statusEffectsController;
        
        [SerializeField] List<StatusEffect> statusEffects;
        private static List<StatusEffect> StatusEffects => statusEffectsController.statusEffects;

        private void Awake()
        {
            statusEffectsController = this;
        }

        private void Start()
        {
            statusEffects.ForEach((effect) => effect.Activate(this));
        }

        private void OnDestroy()
        {
            statusEffects.ForEach((effect) => effect.Dispose());
        }

        public static T GetStatusEffect<T>(StatusEffectType type) where T : StatusEffect
        {
            StatusEffect effect = GetStatusEffect(type);

            if (effect != null)
            {
                return (T)effect;
            }

            return null;
        }

        public static StatusEffect GetStatusEffect(StatusEffectType type)
        {
            return StatusEffects.Find((effect) => effect.type == type);
        }

        public static void RegisterEffect(IGameplayEntity entity, ProjectileInfo projectileInfo)
        {
            for(int i = 0; i < projectileInfo.statusEffects.Count; i++)
            {
                StatusEffectInfo info = projectileInfo.statusEffects[i];

                info.entity = entity;

                GetStatusEffect(info.Type).Register(info);
            }
        }

        public static void RegisterEffect(IGameplayEntity entity, StatusEffectInfo info)
        {
            GetStatusEffect(info.Type).Register(info);
        }
    }
}