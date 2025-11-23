using System;
using UnityEngine;
using Utilities;

namespace Classes
{
    public class Bullet
    {
        public GameObject gameObject;
        private Entity _owner;
        private Team _team;
        private float _bulletContinuousTime;
        private float _bulletContinuousTimer;
        
        public event Action<Bullet> BulletAwake;
        public event Action<Bullet> BulletUpdate;
        public event Action<Bullet> BulletDestroy;

        internal Bullet(Entity owner, GameObject bulletPrefab)
        {
            _owner = owner;
            gameObject = GameObject.Instantiate(bulletPrefab, bulletPrefab.transform.parent);
            _team = owner.team;
        }

        public void Awake()
        {
            BulletAwake?.Invoke(this);
        }

        public void Update()
        {
            BulletUpdate?.Invoke(this);
        }

        public void Destroy()
        {
            BulletDestroy?.Invoke(this);
            
            BulletAwake = null;
            BulletUpdate = null;
            BulletDestroy = null;
            
            GameObject.Destroy(gameObject);
        }
    }
}
