using Classes;
using System.Collections;
using System.Collections.Generic;
using MVVM.ViewModels;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Utilities;
using MVVM;
using DataManagement;

namespace Classes
{
    public class Hero : Entity
    {
        private readonly NavMeshAgent _agent;
        private Transform _attackRangeIndicator;
        
        /// <summary>
        /// 玩家锁定的实体
        /// </summary>
        public BindableProperty<Entity> target = new BindableProperty<Entity>();
        public BindableProperty<bool> isMoving = new BindableProperty<bool>();
        public BindableProperty<bool> showAttributes = new BindableProperty<bool>();
        
        /// <summary>
        /// 是否启用自动攻击模式
        /// </summary>
        private bool _autoAttack = false;
        
        /// <summary>
        /// 创建游戏角色并初始化
        /// </summary>
        public Hero(GameObject gameObject)
        {
            _gameObject = gameObject;
            
            // 配置角色初始数值（测试中，后续通过配置表导入）
            level.Value = 1;
            attackRange.Value = 375;
            movementSpeed.Value = 300;
            scale.Value = 100;
            
            // 配置角色寻路组件
            _agent = _gameObject.GetComponent<NavMeshAgent>();
            _agent.updateUpAxis = false;
            _agent.updateRotation = false;
            _agent.speed = _actualMovementSpeed;
            
            // 配置角色体型
            _gameObject.transform.localScale = new Vector2(_actualScale, _actualScale);
            
            // 配置攻击距离指示器
            _attackRangeIndicator = _gameObject.transform.Find("AttackRangeIndicator");
            _attackRangeIndicator.localScale = new Vector2(_actualAttackRange, _actualAttackRange);
            _attackRangeIndicator.GetComponent<SpriteRenderer>().enabled = false;
        }

        /// <summary>
        /// 英雄移动逻辑
        /// </summary>
        public override void Move()
        {
            // 获取鼠标位置
            Vector3 _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            // 当玩家点击左键
            if (Input.GetMouseButtonDown(0))
            {
                // 若提前点击A键，则开启自动攻击
                if (_attackRangeIndicator.GetComponent<SpriteRenderer>().enabled && !Input.GetKey(KeyCode.C))
                {
                    SetTarget(null);
                    _agent.SetDestination(_mousePosition);
                    _agent.stoppingDistance = 0;
                    _autoAttack = true;
                }
            }
            
            // 当玩家点击右键
            if (Input.GetMouseButton(1))
            {
                //开始移动
                _agent.isStopped = false;

                // 检测鼠标点击位置是否有物体
                bool _findTarget = false;
                RaycastHit2D[] _hitBoxes = Physics2D.RaycastAll(_mousePosition, Vector2.zero);
                foreach (RaycastHit2D _hit in _hitBoxes)
                {
                    if (!_hit.collider.IsUnityNull() && !_gameObject.CompareTag(_hit.collider.gameObject.tag))
                    {
                        // 若有目标则设置为新的锁定的目标
                        var newTarget = _hit.collider.gameObject.GetComponent<EntityData>().entity;
                        
                        // 防止反复锁定相同目标
                        if (target.Value == null || !target.Value.Equals(newTarget))
                        {
                            SetTarget(newTarget);
                        }
                        _autoAttack = true;
                        _findTarget = true;
                        break;
                    }
                }

                if (!_findTarget)
                {
                    SetTarget(null);
                }
                
                if (target.Value.IsUnityNull())
                {
                    // 若没有物体则走到对应位置
                    _agent.SetDestination(_mousePosition);
                    _agent.stoppingDistance = 0;

                    // 若按了shift+右键则启动自动攻击模式（走A）
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        _autoAttack = true;
                    }
                    else
                    {
                        _autoAttack = false;
                    }
                }
            }
            
            // 当玩家按下S键
            if (Input.GetKeyDown(KeyCode.S))
            {
                //停止移动
                _agent.isStopped = true;
            }
            
            // 当玩家点击A键
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKey(KeyCode.C))
            {
                // 若按下C键则显示属性
                if (Input.GetKey(KeyCode.C))
                {
                    showAttributes.Value = true;
                }

                // 显示攻击范围指示器
                _attackRangeIndicator.GetComponent<SpriteRenderer>().enabled = true;
            }
            else if (Input.anyKeyDown || Input.GetKeyUp(KeyCode.C))
            {
                showAttributes.Value = false;
                _attackRangeIndicator.GetComponent<SpriteRenderer>().enabled = false;
            }
            
            if (_autoAttack)
            {
                if (target.Value.IsUnityNull())
                {
                    // 若没锁定的目标则走到对应位置
                    // 过程中持续索敌
                    // 获取攻击范围指示器的碰撞箱
                    CircleCollider2D collider = _gameObject.transform.Find("AttackRangeIndicator")
                        .GetComponent<CircleCollider2D>();

                    // 将目标设定为最近的敌方目标（tag与自己不同的）
                    SetTarget(IsOverlappingOtherTag(collider, _gameObject.tag)?.entity);
                }
                else
                {
                    // 若有锁定的目标则持续索敌
                    // 走到敌人进入攻击范围为止
                    _agent.SetDestination(target.Value.gameObject.transform.position);
                    _agent.stoppingDistance = _actualAttackRange / 2f + target.Value.actualScale / 2f;
                }
            }
        }

        /// <summary>
        /// 查看目标属性
        /// </summary>
        public void TargetCheck()
        {
            // 获取鼠标位置
            Vector3 _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            // 当玩家点击左键
            if (Input.GetMouseButton(0))
            {
                // 检测鼠标点击位置是否有物体
                RaycastHit2D[] _hitBoxes = Physics2D.RaycastAll(_mousePosition, Vector2.zero);
                
                bool _findTarget = false;
                RaycastHit2D _find = new RaycastHit2D();
                foreach (RaycastHit2D _hit in _hitBoxes)
                {
                    if (!_hit.collider.IsUnityNull() && !_gameObject.CompareTag(_hit.collider.gameObject.tag))
                    {
                        _findTarget = true;
                        _find = _hit;
                        break;
                    }
                }
                
                if (_findTarget)
                {
                    SetTarget(_find.collider.gameObject.GetComponent<EntityData>().entity);
                }
            }
        }

        /// <summary>
        /// 设定英雄目标
        /// </summary>
        private void SetTarget(Entity heroTarget)
        {
            target.Value = heroTarget;
        }
        
        // 静态缓存，避免 GC
        private static readonly Collider2D[] _overlapBuffer = new Collider2D[20];

        /// <summary>
        /// 检测圆形范围内是否有与指定 tag 不同的其他碰撞体，并返回最近的一个 EntityData
        /// </summary>
        private EntityData IsOverlappingOtherTag(CircleCollider2D collider, string excludeTag)
        {
            if (collider == null) return null;

            Vector2 center = collider.bounds.center;
            float radius = collider.radius * collider.transform.lossyScale.x;

            // 使用 NonAlloc 版本避免内存分配
            int count = Physics2D.OverlapCircleNonAlloc(center, radius, _overlapBuffer);

            EntityData nearestEntity = null;
            float nearestDistanceSqr = float.MaxValue;

            for (int i = 0; i < count; i++)
            {
                Collider2D col = _overlapBuffer[i];
                if (col == null) continue;

                GameObject otherGo = col.gameObject;
                if (otherGo == null || otherGo == gameObject) continue; // 排除自身和空对象

                // 跳过相同 Tag 的对象
                if (otherGo.CompareTag(excludeTag)) continue;

                // 获取 EntityData 组件
                EntityData entity = otherGo.GetComponent<EntityData>();
                if (entity == null) continue; // 没有该组件则跳过

                // 计算距离平方（避免开根号）
                float distSqr = (otherGo.transform.position - (Vector3)center).sqrMagnitude;
                if (distSqr < nearestDistanceSqr)
                {
                    nearestDistanceSqr = distSqr;
                    nearestEntity = entity;
                }
            }

            return nearestEntity; // 可能为 null
        }
    }
}