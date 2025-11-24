using EntityManagers;
using Factories;
using UnityEngine;
using UnityEngine.AI;
using Utilities;

namespace Classes.Entities
{
    public class Enemy : Entity
    {
        private readonly NavMeshAgent _agent;
        
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
        private const float attackBulletSpeed = 15f;

        private Entity target = HeroManager.hero;
        
        /// <summary>
        /// 创建敌人并初始化
        /// </summary>
        public Enemy(GameObject gameObject)
        {

            #region 读取敌人数据配置初始化数据（目前测试采用固定数据）
            
            _baseMaxHealthPoint = 600;
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

            _maxHealthPointGrowth = 0;
            _maxMagicPointGrowth = 0;
            _attackDamageGrowth = 0;
            _attackSpeedGrowth = 0;
            _attackDefenseGrowth = 0;
            _magicDefenseGrowth = 0;
            _healthRegenerationGrowth = 0;
            _magicRegenerationGrowth = 0;

            #endregion

            _gameObject = gameObject;
            _team = Team.Enemy;
            
            // 配置敌人寻路组件
            _agent = _gameObject.GetComponent<NavMeshAgent>();
            _agent.updateUpAxis = false;
            _agent.updateRotation = false;
            _agent.speed = actualMovementSpeed;
            
            // 配置敌人体型
            _gameObject.transform.localScale = new Vector2(actualScale * 2, actualScale * 2);
            
            // 其他变量初始化
            level.Value = 1;
            magicPoint.Value = maxMagicPoint.Value;
            healthPoint.Value = maxHealthPoint.Value;
        }
        
        // 敌人进行移动
        public override void Move()
        {
            _agent.SetDestination(target.gameObject.transform.position);
            _agent.stoppingDistance = actualAttackRange + target.actualScale;
            
            var distance = Vector3.Distance(target.gameObject.transform.position, gameObject.transform.position);
            _agent.isStopped = distance <= _agent.stoppingDistance + 0.1f;
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
                        var currentPosition = self.gameObject.transform.position;
                        var targetPosition = self.target.gameObject.transform.position;

                        var direction = (targetPosition - currentPosition).normalized;
                        var nextPosition = currentPosition + direction * (attackBulletSpeed * Time.deltaTime);

                        self.gameObject.transform.position = nextPosition;
                        self.gameObject.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);

                        // 子弹的销毁逻辑
                        const float destroyDistance = 0.3f;
                        if (Vector3.Distance(self.gameObject.transform.position,
                                self.target.gameObject.transform.position) <= destroyDistance)
                        {
                            self.BulletHit();
                            self.AttackEffectActivate();
                            self.Destroy();
                        }
                    };
                };

                bullet.OnBulletHit += (self) =>
                {
                    self.target.TakeDamage(self.target.CalculateADDamage(self.owner, self.owner.attackDamage));
                };

                bullet.Awake();
            }
        }
    }
}