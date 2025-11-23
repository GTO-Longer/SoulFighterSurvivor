using System;
using UnityEngine;
using Utilities;

namespace Classes
{
    public class Bullet
    {
        public GameObject gameObject;
        public Entity owner;
        private Team _team;
        private float _bulletContinuousTime;
        private float _bulletContinuousTimer;
        
        public event Action<Bullet> OnBulletAwake;
        public event Action<Bullet> OnBulletUpdate;
        public event Action<Bullet> OnBulletDestroy;
        // public event Action<Bullet> OnBulletHit;

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
