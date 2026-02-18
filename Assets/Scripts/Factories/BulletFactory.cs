using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Classes;

namespace Factories
{
    public class BulletFactory : MonoBehaviour
    {
        public int ActiveBulletCount => _activeBullets.Count;
        public static BulletFactory Instance { get; private set; }
        public GameObject bulletPrefab;

        private ObjectPool<Bullet> _bulletPool;
        private readonly List<Bullet> _activeBullets = new List<Bullet>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                bulletPrefab.SetActive(false);

                // 初始化对象池
                _bulletPool = new ObjectPool<Bullet>(
                    createFunc: () =>
                    {
                        var bullet = new Bullet();
                        return bullet;
                    },
                    actionOnGet: bullet =>
                    {},
                    actionOnRelease: bullet =>
                    {
                        bullet.gameObject.SetActive(false);
                        bullet.Clear();
                    },
                    actionOnDestroy: bullet =>
                    {
                        // 销毁时，如果GameObject存在则销毁
                        if (bullet.gameObject != null)
                        {
                            GameObject.Destroy(bullet.gameObject);
                        }
                    },
                    collectionCheck: true,
                    defaultCapacity: 20,
                    maxSize: 1000
                );
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public Bullet CreateBullet(Entity owner, float bulletContinuousTime = 0, float bulletDamageCD = 0)
        {
            var bullet = _bulletPool.Get();
            bullet.Initialize(owner, bulletPrefab, bulletContinuousTime, bulletDamageCD);
            _activeBullets.Add(bullet);
            return bullet;
        }

        private void Update()
        {
            // 使用副本避免在迭代过程中集合被修改
            // 子弹数量可能很多，但创建列表的开销小于迭代时修改集合的风险
            var bulletsToUpdate = new List<Bullet>(_activeBullets);
            for (int i = 0; i < bulletsToUpdate.Count; i++)
            {
                var bullet = bulletsToUpdate[i];
                // 检查子弹是否仍在活跃列表中（可能在之前的更新中被移除）
                // 由于使用副本，即使bullet.Update()中调用Destroy导致从_activeBullets移除也没问题
                // 但我们仍需要确保bullet不为空
                if (bullet == null) continue;
                bullet.Update();
            }
        }

        /// <summary>
        /// 回收子弹到对象池
        /// </summary>
        public void ReleaseBullet(Bullet bullet)
        {
            if (bullet == null) return;

            // 从活跃列表中移除（如果存在）
            _activeBullets.Remove(bullet);

            // 回收到对象池
            // collectionCheck: true 会防止重复释放
            _bulletPool.Release(bullet);
        }

        public void ClearAllBullets()
        {
            // 使用副本避免迭代时修改集合
            var bulletsToClear = new List<Bullet>(_activeBullets);
            foreach (var bullet in bulletsToClear)
            {
                ReleaseBullet(bullet);
            }
            // ReleaseBullet会从_activeBullets中移除子弹，所以不需要额外Clear
            // 但为确保安全，如果还有剩余子弹（理论上不应该），直接清除
            if (_activeBullets.Count > 0)
            {
                _activeBullets.Clear();
            }
        }
    }
}