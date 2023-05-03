using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{

    public class FireBombAbility : Ability, IOnOwnerAttackAbility
    {
        private static readonly string FIRE_BOMB_ABILITY_METEOR_POOL_NAME = "Fire Bomb Ability Meteor Pool";
        private static readonly string FIRE_BOMB_ABILITY_EXPLOSION_POOL_NAME = "Fire Bomb Ability Explosion Pool";

        public FireBombAbilityData data;

        private static Pool explosionPool;
        private static Pool meteorPool;

        public override void ActivateAbility(IGameplayEntity owner)
        {
            base.ActivateAbility(owner);

            owner.onEntityAttackEvent += OnOwnerAttack;

            if (!PoolManager.ContainsPool(FIRE_BOMB_ABILITY_METEOR_POOL_NAME))
            {
                meteorPool = PoolManager.AddPool(new PoolSettings
                {
                    name = FIRE_BOMB_ABILITY_METEOR_POOL_NAME,
                    autoSizeIncrement = true,
                    size = 3,
                    type = Pool.PoolType.Single,
                    singlePoolPrefab = data.meteorParticle
                });
            }

            if (!PoolManager.ContainsPool(FIRE_BOMB_ABILITY_EXPLOSION_POOL_NAME))
            {
                explosionPool = PoolManager.AddPool(new PoolSettings
                {
                    name = FIRE_BOMB_ABILITY_EXPLOSION_POOL_NAME,
                    autoSizeIncrement = true,
                    size = 3,
                    type = Pool.PoolType.Single,
                    singlePoolPrefab = data.explosionParticle
                });
            }
        }

        public override void DisableAbility()
        {
            base.DisableAbility();

            owner.onEntityAttackEvent -= OnOwnerAttack;
        }

        public void OnOwnerAttack(OnOwnerAttackInfo info)
        {

            FireBombInfo fireMeteorInfo = data.fireBombInfo[stackAmount];

            if (fireMeteorInfo.chance >= Random.value)
            {
                EnemyBehavior enemy;

                if (LevelObjectSpawner.Enemies.IsNullOrEmpty()) return;

                int counter = 0;
                do
                {
                    enemy = LevelObjectSpawner.Enemies.GetRandomItem();
                    counter++;
                } while ((enemy == null || enemy.IsDying || !enemy.gameObject.activeSelf) && counter < 50);

                //GameObject particle = Object.Instantiate(data.lightningParticle);

                ParticleSystem meteorParticle = meteorPool.GetPooledObject().GetComponent<ParticleSystem>();
                meteorParticle.transform.position = enemy.Position + data.meteorOffset;
                meteorParticle.transform.localScale = Vector3.one * 0.5f;
                meteorParticle.Play();

                meteorParticle.transform.DOMove(enemy.Position, data.meteorDuration).OnComplete(() => {
                    meteorParticle.Stop();
                    meteorParticle.gameObject.SetActive(false);

                    ParticleSystem explosionParticle = explosionPool.GetPooledObject().GetComponent<ParticleSystem>();
                    explosionParticle.transform.position = meteorParticle.transform.position;
                    explosionParticle.transform.localScale = Vector3.one * 0.5f;
                    explosionParticle.Play();

                    GameAudioController.PlaySound(data.explosionSound);

                    for(int i = 0; i < LevelObjectSpawner.Enemies.Count; i++)
                    {
                        if(!enemy.IsDying && enemy.gameObject.activeSelf && Vector3.Distance(explosionParticle.transform.position, enemy.Position) <= data.explosionRadius)
                        {
                            enemy.GetHit(new Damage
                            {
                                amount = info.baseDamage.amount * fireMeteorInfo.damage,
                                type = DamageType.Fire,
                                isCrit = false,
                            });
                        }
                    }

                    Tween.DelayedCall(data.particleDisableDelay, () =>
                    {
                        explosionParticle.Stop();
                        explosionParticle.gameObject.SetActive(false);
                    });

                });   
            }
        }

    }

}