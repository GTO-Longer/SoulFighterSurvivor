using System.Collections.Generic;
using UnityEngine;
using Classes;

namespace Factories
{
    public class BulletFactory : MonoBehaviour
    {
        public int ActiveBulletCount => _activeBullets.Count;
        public static BulletFactory Instance { get; private set; }
        public GameObject bulletPrefab;

        private readonly List<Bullet> _activeBullets = new List<Bullet>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            
                bulletPrefab.SetActive(false);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public Bullet CreateBullet(Entity owner)
        {
            var bullet = new Bullet(owner, bulletPrefab);
            _activeBullets.Add(bullet);
            return bullet;
        }

        private void Update()
        {
            for (int i = _activeBullets.Count - 1; i >= 0; i--)
            {
                var bullet = _activeBullets[i];
                bullet.Update();
            }
        }

        public void ClearAllBullets()
        {
            foreach (var bullet in _activeBullets)
            {
                bullet.Destroy();
            }
            _activeBullets.Clear();
        }
    }
}