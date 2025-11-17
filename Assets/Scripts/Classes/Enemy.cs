using Classes;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Utilities;
using Hero;

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
            
            //配置敌人初始属性（测试中）
            movementSpeed.Value = 100;
            scale.Value = 100;
            attackRange.Value = 175;
            attackDamage.Value = 999;
            
            // 配置敌人寻路组件
            _agent = _gameObject.GetComponent<NavMeshAgent>();
            _agent.updateUpAxis = false;
            _agent.updateRotation = false;
            _agent.speed = _actualMovementSpeed;
            
            // 配置敌人体型
            _gameObject.transform.localScale = new Vector2(_actualScale, _actualScale);
        }
        
        // 敌人进行移动
        public override void Move()
        {
            _agent.SetDestination(HeroManager.hero.gameObject.transform.position);
            _agent.stoppingDistance = _actualAttackRange - _actualScale / 2f;
        }
    }
}