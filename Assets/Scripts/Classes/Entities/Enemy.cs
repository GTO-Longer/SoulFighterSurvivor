using Managers.EntityManagers;
using Factories;
using UnityEngine;
using Utilities;

namespace Classes.Entities
{
    public class Enemy : Entity
    {
        private Transform _attackRangeIndicator;
        
        /// <summary>
        /// 攻击计时器
        /// </summary>
        private float _attackTimer;
        /// <summary>
        /// 攻击前摇计时器
        /// </summary>
        private float _attackWindUpTimer;
        /// <summary>
        /// 普攻弹道速度
        /// </summary>
        private const float attackBulletSpeed = 1500f;

        private float gainExpTimer;

        public Entity target;
        
        /// <summary>
        /// 创建敌人并初始化
        /// </summary>
        public Enemy(GameObject obj, Team team) : base(obj, team)
        {
            #region 读取敌人数据配置初始化数据（目前测试采用固定数据）
            
            _baseMaxHealthPoint = 200;
            _baseMaxMagicPoint = 100;
            _baseAttackDamage = 50;
            _baseAttackSpeed = 0.5f;
            _baseAttackDefense = 30;
            _baseMagicDefense = 30;
            _baseHealthRegeneration = 0;
            _baseMagicRegeneration = 0;
            _baseAttackRange = 175;
            _baseMovementSpeed = 200;
            _baseScale = 100;
            _attackWindUp = 0.1f;

            _maxHealthPointGrowth = 50f;
            _maxMagicPointGrowth = 0;
            _attackDamageGrowth = 5f;
            _attackSpeedGrowth = 0.04f;
            _attackDefenseGrowth = 5f;
            _magicDefenseGrowth = 5f;
            _healthRegenerationGrowth = 0;
            _magicRegenerationGrowth = 0;

            #endregion
            
            // 配置敌人体型
            _gameObject.transform.localScale = new Vector2(scale * 2, scale * 2);
            
            // 创建状态条
            StateBarFactory.Instance.Spawn(this);
            
            // 配置攻击距离指示器
            _attackRangeIndicator = _gameObject.transform.Find("AttackRangeIndicator");
            _attackRangeIndicator.localScale = new Vector2(attackRange / scale, attackRange / scale);
            _attackRangeIndicator.GetComponent<SpriteRenderer>().enabled = false;
            
            // 其他变量初始化
            level.Value = Mathf.Max(1, HeroManager.hero.level - 4);
            gainExpTimer = 0;
            magicPoint.Value = maxMagicPoint.Value;
            healthPoint.Value = maxHealthPoint.Value;
        }
        
        // 敌人进行移动
        public override void Move()
        {
            _agent.SetDestination(target.gameObject.transform.position);
            _agent.SetStop(ToolFunctions.IsOverlappingTarget(_attackRangeIndicator.gameObject, target.gameObject));
        }

        public override void Attack()
        {
            if (_attackTimer < 100)
            {
                _attackTimer += Time.deltaTime;
            }

            if (_attackTimer < actualAttackInterval || !_agent.isStopped)
            {
                _attackWindUpTimer = 0;
                return;
            }

            // 计算攻击前摇
            _attackWindUpTimer += Time.deltaTime;
            if (_attackWindUpTimer >= actualAttackInterval * _attackWindUp)
            {
                // 发动攻击
                _attackTimer = 0;
                _attackWindUpTimer = 0;

                var bullet = BulletFactory.Instance.CreateBullet(this);
                bullet.OnBulletAwake += (self) =>
                {
                    self.gameObject.transform.position = _gameObject.transform.position;
                    self.gameObject.SetActive(true);

                    // 设置目标
                    self.target = target;

                    // 每帧追踪目标
                    self.OnBulletUpdate += (_) =>
                    {
                        // 锁定目标死亡则清除子弹
                        if (self.target == null || !self.target.isAlive)
                        {
                            self.Destroy();
                            return;
                        }
                        
                        var currentPosition = self.gameObject.transform.position;
                        var targetPosition = self.target.gameObject.transform.position;

                        var direction = (targetPosition - currentPosition).normalized;
                        var nextPosition = currentPosition + direction * (attackBulletSpeed * Time.deltaTime);

                        self.gameObject.transform.position = nextPosition;
                        self.gameObject.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);

                        // 子弹的销毁逻辑
                        const float destroyDistance = 30f;
                        if (Vector3.Distance(self.gameObject.transform.position,
                                self.target.gameObject.transform.position) <= destroyDistance)
                        {
                            self.BulletHit();
                            self.Destroy();
                        }
                    };
                };

                bullet.OnBulletHit += (self) =>
                {
                    self.target.TakeDamage(self.target.CalculateADDamage(self.owner, self.owner.attackDamage), DamageType.AD, this);
                };

                bullet.Awake();
            }
        }

        /// <summary>
        /// 敌人随时间获得经验值
        /// </summary>
        public void GainExperience()
        {
            gainExpTimer += Time.deltaTime;
            
            if (gainExpTimer > 10)
            {
                GetExperience(500 / (500 + maxExperience.Value) * maxExperience.Value);
                gainExpTimer = 0;
            }
        }

        public override void Die(Entity entity)
        {
            var hero = entity as Hero;

            var totalExp = experience.Value + 5f * (level - 1) * (level - 1) + 80f * (level - 1) + 195;
            hero?.GetExperience(100 + totalExp * 3.4f / (level.Value + 16));
            isAlive = false;
            EnemyFactory.Instance.Despawn(gameObject.GetComponent<EnemyManager>());
        }
    }
}