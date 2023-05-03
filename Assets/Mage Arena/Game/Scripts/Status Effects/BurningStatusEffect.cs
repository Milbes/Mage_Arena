using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Burning Status Effect", menuName = "Content/Status Effects/Burning")]
    public class BurningStatusEffect : StatusEffect
    {

        public float tickDuration;

        public DamageType damageType;

        private List<BurningEffectInfo> currentEffects;

        private bool isClockRunning = false;

        private void Awake()
        {
            type = StatusEffectType.Burning;
        }

        public override void Activate(MonoBehaviour coroutineHolder)
        {
            isClockRunning = false;

            currentEffects = new List<BurningEffectInfo>();

            this.coroutineHolder = coroutineHolder;
        }

        public override void Dispose()
        {
            isClockRunning = false;

            currentEffects.Clear();
        }

        public override void Register(StatusEffectInfo info)
        {
            Register(info as BurningEffectInfo);
        }

        public void Register(BurningEffectInfo info)
        {
            IGameplayEntity entity = info.entity;

            bool exists = false;

            for(int i = 0; i < currentEffects.Count; i++)
            {
                BurningEffectInfo effect = currentEffects[i];
                if(effect.entity == entity)
                {

                    if (effect.damagePerTick < info.damagePerTick) effect.damagePerTick = info.damagePerTick;
                    if (effect.ticksLeft < info.ticksLeft) effect.ticksLeft = info.ticksLeft;

                    exists = true;
                }
            }

            if (!exists)
            {
                currentEffects.Add(info);
            }

            if (!isClockRunning)
            {
                coroutineHolder.StartCoroutine(Tick());
            }
        }

        private IEnumerator Tick()
        {
            var wait = new WaitForSeconds(tickDuration);

            isClockRunning = true;

            while (!currentEffects.IsNullOrEmpty())
            {
                yield return wait;

                for (int i = 0; i < currentEffects.Count; i++)
                {
                    BurningEffectInfo info = currentEffects[i];

                    if (!info.entity.IsActiveSelf())
                    {
                        currentEffects.RemoveAt(i);
                        i--;

                        continue;
                    }

                    info.entity.GetHit(new Damage { amount = info.damagePerTick, type = damageType, isCrit = false });

                    info.ticksLeft--;

                    if(info.ticksLeft != 0)
                    {
                        currentEffects[i] = info;
                    } else
                    {
                        currentEffects.RemoveAt(i);
                        i--;
                    }
                }
            }

            isClockRunning = false;
        }
    }

    public class BurningEffectInfo: StatusEffectInfo
    {
        public BurningEffectInfo()
        {
            type = StatusEffectType.Burning;
        }

        public float damagePerTick;
        public int ticksLeft;
    }
}