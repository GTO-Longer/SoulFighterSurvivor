using Entities.Hero;
using UnityEngine;
using UnityEngine.AI;
using Utilities;

namespace Classes
{
    public class Enemy : Entity
    {
        private readonly NavMeshAgent _agent;
        
        /// <summary>
        /// 创建敌人并初始化
        /// </summary>
        public Enemy(GameObject gameObject)
        {

            #region 读取英雄数据配置初始化数据（目前测试采用固定数据）
            
            _baseMaxHealthPoint = 100;
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
            _agent.SetDestination(HeroManager.hero.gameObject.transform.position);
            _agent.stoppingDistance = actualAttackRange + HeroManager.hero.actualScale;
        }
    }
}