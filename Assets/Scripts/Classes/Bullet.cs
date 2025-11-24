using System;
using UnityEngine;
using Utilities;

namespace Classes
{
    public class Bullet
    {
        public GameObject gameObject;
        public Entity owner;
        public Entity target;
        
        private Team _team;
        private float _bulletContinuousTime;
        private float _bulletContinuousTimer;
        
        public event Action<Bullet> OnBulletAwake;
        public event Action<Bullet> OnBulletUpdate;
        public event Action<Bullet> OnBulletDestroy;
        public event Action<Bullet> OnBulletHit;
        /// <summary>
        /// 攻击特效
        /// </summary>
        public event Action<Bullet> AttackEffect;
        /// <summary>
        /// 技能特效
        /// </summary>
        public event Action<Bullet> AbilityEffect;

        internal Bullet(Entity owner, GameObject bulletPrefab)
        {
            this.owner = owner;
            gameObject = GameObject.Instantiate(bulletPrefab, bulletPrefab.transform.parent);
            _team = owner.team;
        }

        public void Awake()
        {
            OnBulletAwake?.Invoke(this);
        }

        public void Update()
        {
            OnBulletUpdate?.Invoke(this);
        }

        public void BulletHit(Entity entity = null)
        {
            if (entity != null)
            {
                target = entity;
            }

            OnBulletHit?.Invoke(this);
        }

        public void AttackEffectActivate()
        {
            AttackEffect?.Invoke(this);
        }

        public void AbilityEffectActivate()
        {
            AbilityEffect?.Invoke(this);
        }

        public void Destroy()
        {
            OnBulletDestroy?.Invoke(this);
            
            OnBulletAwake = null;
            OnBulletUpdate = null;
            OnBulletDestroy = null;
            
            GameObject.Destroy(gameObject);
        }
    }
}
