using System;
using System.Collections.Generic;
using System.Linq;
using Factories;
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

        private Action<Bullet> _internalUpdateHandler;
        
        public event Action<Bullet> OnBulletAwake;
        public event Action<Bullet> OnBulletUpdate;
        public event Action<Bullet> OnBulletDestroy;
        public event Action<Bullet> OnBulletHit;

        internal Bullet()
        {
            // 空构造函数，用于对象池创建
            bulletEntityDamageCDTimer = new Dictionary<Entity, float>();

            // 创建内部更新处理器
            _internalUpdateHandler = (self) =>
            {
                if (self.bulletContinuousTimer < self.bulletContinuousTime)
                {
                    self.bulletContinuousTimer += Time.deltaTime;
                }

                foreach (var Key in self.bulletEntityDamageCDTimer.Keys.ToList())
                {
                    if (Key.isAlive)
                    {
                        self.bulletEntityDamageCDTimer[Key] += Time.deltaTime;
                    }
                }
            };
        }

        /// <summary>
        /// 初始化子弹，用于对象池重用
        /// </summary>
        public void Initialize(Entity owner, GameObject bulletPrefab, float bulletContinuousTime = 0, float bulletDamageCD = 0)
        {
            if (owner == null) throw new ArgumentNullException(nameof(owner));
            if (bulletPrefab == null) throw new ArgumentNullException(nameof(bulletPrefab));

            this.owner = owner;
            _team = owner.team;

            this.bulletContinuousTime = bulletContinuousTime;
            this.bulletDamageCD = bulletDamageCD;
            bulletContinuousTimer = 0;
            bulletEntityDamageCDTimer.Clear();
            target = null;
            bulletStateID = 0;
            effect = null;

            // 如果GameObject不存在，则实例化
            if (gameObject == null)
            {
                gameObject = GameObject.Instantiate(bulletPrefab, bulletPrefab.transform.parent);
            }
            else
            {
                // 确保GameObject在正确的父节点下
                if (gameObject.transform.parent != bulletPrefab.transform.parent)
                {
                    gameObject.transform.SetParent(bulletPrefab.transform.parent, false);
                }
            }

            if (owner.gameObject != null)
            {
                gameObject.tag = owner.gameObject.tag;
            }

            OnBulletUpdate += _internalUpdateHandler;

            gameObject.SetActive(true);
        }

        /// <summary>
        /// 清理子弹状态，准备回收到对象池
        /// </summary>
        public void Clear()
        {
            // 清除事件订阅（包括内部处理器）
            OnBulletAwake = null;
            OnBulletUpdate = null;
            OnBulletDestroy = null;
            OnBulletHit = null;

            // 清理字典
            bulletEntityDamageCDTimer.Clear();

            // 重置字段
            owner = null;
            target = null;
            bulletStateID = 0;
            effect = null;
            _team = Team.None; // 假设有Team.None枚举值，如果没有可能需要调整
            bulletContinuousTime = 0;
            bulletContinuousTimer = 0;
            bulletDamageCD = 0;

            // 禁用GameObject，但不销毁，由对象池管理
            if (gameObject != null)
            {
                gameObject.SetActive(false);

                // 重置tag为默认
                gameObject.tag = "Untagged";
            }
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

            // 通过工厂回收子弹（如果工厂存在且使用对象池）
            if (BulletFactory.Instance != null)
            {
                BulletFactory.Instance.ReleaseBullet(this);
            }
            else
            {
                // 回退到直接销毁（非对象池模式）
                if (gameObject != null)
                {
                    GameObject.Destroy(gameObject);
                }
            }
        }
    }
}
