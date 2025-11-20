using Entities.Hero;
using UnityEngine;
using UnityEngine.AI;

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
            _gameObject = gameObject;
            
            // 配置敌人寻路组件
            _agent = _gameObject.GetComponent<NavMeshAgent>();
            _agent.updateUpAxis = false;
            _agent.updateRotation = false;
            _agent.speed = actualMovementSpeed;
            
            // 配置敌人体型
            _gameObject.transform.localScale = new Vector2(actualScale * 2, actualScale * 2);
        }
        
        // 敌人进行移动
        public override void Move()
        {
            _agent.SetDestination(HeroManager.hero.gameObject.transform.position);
            _agent.stoppingDistance = actualAttackRange + HeroManager.hero.actualScale;
        }
    }
}