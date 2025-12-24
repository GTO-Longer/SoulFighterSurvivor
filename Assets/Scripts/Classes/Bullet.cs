using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities;

namespace Classes
{
    public class Bullet
    {
        public GameObject gameObject;
        public Entity owner;
        public Entity target;
        public int bulletStateID;
        public Effect effect;
        
        private Team _team;
        public float bulletContinuousTime;
        public float bulletContinuousTimer;
        public float bulletDamageCD;
        public Dictionary<Entity, float> bulletEntityDamageCDTimer;
        
        public event Action<Bullet> OnBulletAwake;
        public event Action<Bullet> OnBulletUpdate;
        public event Action<Bullet> OnBulletDestroy;
        public event Action<Bullet> OnBulletHit;

        internal Bullet(Entity owner, GameObject bulletPrefab, float bulletContinuousTime = 0, float bulletDamageCD = 0)
        {
            this.owner = owner;
            gameObject = GameObject.Instantiate(bulletPrefab, bulletPrefab.transform.parent);
            gameObject.tag = owner.gameObject.tag;
            _team = owner.team;
            
            this.bulletContinuousTime = bulletContinuousTime;
            this.bulletDamageCD = bulletDamageCD;
            bulletEntityDamageCDTimer = new Dictionary<Entity, float>();

            OnBulletUpdate += (self) =>
            {
                if (self.bulletContinuousTimer < self.bulletContinuousTime)
                {
                    self.bulletContinuousTimer += Time.deltaTime;
                }

                foreach (var Key in bulletEntityDamageCDTimer.Keys.ToList())
                {
                    if (Key.isAlive)
                    {
                        bulletEntityDamageCDTimer[Key] += Time.deltaTime;
                    }
                }
            };
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
