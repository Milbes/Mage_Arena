using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{

    public class FireOrbsAbility : Ability, IOnTickAbility
    {
        public FireOrbsAbilityData data;

        private List<FireOrbProjectileInfo> fireOrbsInfo;
        //private Transform abilityParticle;
        //private List<ParticleSystem> orbsParticles;

        private float angle;

        public override void ActivateAbility(IGameplayEntity owner)
        {
            base.ActivateAbility(owner);

            fireOrbsInfo = new List<FireOrbProjectileInfo>();

            angle = 0;

            /*abilityParticle = Object.Instantiate(data.orbsParticle).transform;

            orbsParticles = new List<ParticleSystem>();
            for (int i = 0; i < abilityParticle.childCount - 1; i++)
            {
                orbsParticles.Add(abilityParticle.GetChild(i).GetComponent<ParticleSystem>());
                orbsParticles[i].gameObject.SetActive(false);
            }*/

            AddOrb();
        }

        public override void StackIncreased()
        {
            base.StackIncreased();

            AddOrb();
        }

        public void AddOrb() {
            ProjectileBehavior projectile = Object.Instantiate(data.orbProjectile).GetComponent<ProjectileBehavior>();

            projectile.Init(new ProjectileInfo
            {
                canPassObstacles = true,
                damages = new List<Damage> { 
                    new Damage { 
                        amount = owner.GetBaseDamageAmount() * data.orbDamage,
                        isCrit = false,
                        type = DamageType.Fire
                }},
                owner = owner,
                spawnPosition = owner.GetTransform().position,
                direction = Vector3.forward,
                targetsPlayer = false
            });

            projectile.IsOnManualControl = true;

            projectile.transform.localScale = Vector3.one;

            var info = new FireOrbProjectileInfo
            {
                initialAngle = 0,
                realAngle = 0,
                distance = 0,
                orb = projectile
            };

            fireOrbsInfo.Add(info);

            if(stackAmount == 1)
            {
                fireOrbsInfo[1].realAngle = 180;
                fireOrbsInfo[1].initialAngle = 180;
            } else if(stackAmount == 2)
            {
                fireOrbsInfo[1].initialAngle = 120;

                fireOrbsInfo[2].realAngle = 240;
                fireOrbsInfo[2].initialAngle = 240;
            }

            /*for(int i = 0; i <= stackAmount; i++)
            {
            
            }*/

            //orbsParticles[stackAmount].transform.position = fireOrbsInfo[stackAmount].orb.transform.position;
            //orbsParticles[stackAmount].gameObject.SetActive(true);
            //orbsParticles[stackAmount].Play();

        }

        public void Tick()
        {
            angle += data.speed * Time.deltaTime;

            if (angle > 360) angle -= 360f;
            if (angle < -360) angle += 360f;

            for(int i = 0; i < fireOrbsInfo.Count; i++)
            {
                var info = fireOrbsInfo[i];

                info.distance = Mathf.Lerp(info.distance, data.radius, 0.05f);
                info.realAngle = Mathf.Lerp(info.realAngle, info.initialAngle, 0.05f);

                var offset = Quaternion.Euler(0, info.realAngle + angle, 0) * Vector3.forward * info.distance;

                info.orb.transform.position = owner.GetTransform().position + offset;

                info.orb.transform.rotation = Quaternion.FromToRotation(Vector3.forward, offset.normalized);

                //orbsParticles[i].transform.position = info.orb.transform.position;
            }

            //abilityParticle.position = owner.GetTransform().position;
        }

        public override void DisableAbility()
        {
            base.DisableAbility();

            for(int i = 0; i < fireOrbsInfo.Count; i++)
            {
                Object.Destroy(fireOrbsInfo[i].orb.gameObject);
            }

            fireOrbsInfo.Clear();
        }

        private class FireOrbProjectileInfo
        {
            public float initialAngle;
            public float realAngle;
            public float distance;
            public ProjectileBehavior orb;
        }
    }

}