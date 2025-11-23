using System;
using System.Collections.Generic;
using System.Reflection;
using DataManagement;
using DG.Tweening;
using Factories;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Utilities;

namespace Classes
{
    public class Hero : Entity
    {
        private readonly NavMeshAgent _agent;
        private Transform _attackRangeIndicator;
        private bool _asyncRotating => DOTween.IsTweening(_gameObject.transform);
        private const float rotateTime = 0.3f;
        private const float attackBulletSpeed = 15f;
        
        /// <summary>
        /// 玩家锁定的实体
        /// </summary>
        public Property<Entity> target = new Property<Entity>();
        
        public Property<bool> isMoving = new Property<bool>();
        public Property<bool> showAttributes = new Property<bool>();
        
        /// <summary>
        /// 是否启用自动攻击模式
        /// </summary>
        private bool _autoAttack;

        /// <summary>
        /// 攻击计时器
        /// </summary>
        private float _attackTimer;

        public List<Skill> skillList = new();
        
        /// <summary>
        /// 创建游戏角色并初始化
        /// </summary>
        public Hero(GameObject gameObject, string name)
        {
            #region 读取英雄数据配置初始化数据

            var config = ConfigReader.ReadHeroConfig(name);
            if (config != null)
            {
                _baseMaxHealthPoint = config._baseMaxHealthPoint;
                _baseMaxMagicPoint = config._baseMaxMagicPoint;
                _baseAttackDamage = config._baseAttackDamage;
                _baseAttackSpeed = config._baseAttackSpeed;
                _baseAttackDefense = config._baseAttackDefense;
                _baseMagicDefense = config._baseMagicDefense;
                _baseHealthRegeneration = config._baseHealthRegeneration;
                _baseMagicRegeneration = config._baseMagicRegeneration;
                _baseAttackRange = config._baseAttackRange;
                _baseMovementSpeed = config._baseMovementSpeed;
                _baseScale = config._baseScale;

                _maxHealthPointGrowth = config._maxHealthPointGrowth;
                _maxMagicPointGrowth = config._maxMagicPointGrowth;
                _attackDamageGrowth = config._attackDamageGrowth;
                _attackSpeedGrowth = config._attackSpeedGrowth;
                _attackDefenseGrowth = config._attackDefenseGrowth;
                _magicDefenseGrowth = config._magicDefenseGrowth;
                _healthRegenerationGrowth = config._healthRegenerationGrowth;
                _magicRegenerationGrowth = config._magicRegenerationGrowth;
            
                #region 配置英雄技能
                
                var assembly = Assembly.GetExecutingAssembly();
                var passiveSkillType = assembly.GetType("Classes.Skills." + config._passiveSkill);
                var QSkillType = assembly.GetType("Classes.Skills." + config._QSkill);
                var WSkillType = assembly.GetType("Classes.Skills." + config._WSkill);
                var ESkillType = assembly.GetType("Classes.Skills." + config._ESkill);
                var RSkillType = assembly.GetType("Classes.Skills." + config._RSkill);

                if (passiveSkillType != null)
                {
                    var skill = (Skill)Activator.CreateInstance(passiveSkillType);
                    skill.owner = this;
                    skill.SkillEffect();
                    skillList.Add(skill);
                    Debug.Log("Read skill:" + skillList[(int)SkillType.PassiveSkill].skillName +
                              "\nSkillDescription:" + skillList[(int)SkillType.PassiveSkill].GetDescription());
                }

                if (QSkillType != null)
                {
                    var skill = (Skill)Activator.CreateInstance(QSkillType);
                    skill.owner = this;
                    skill.SkillEffect();
                    skillList.Add(skill);
                    Debug.Log("Read skill:" + skillList[(int)SkillType.QSkill].skillName +
                              "\nSkillDescription:" + skillList[(int)SkillType.QSkill].GetDescription());
                }

                if (WSkillType != null)
                {
                    var skill = (Skill)Activator.CreateInstance(WSkillType);
                    skill.owner = this;
                    skill.SkillEffect();
                    skillList.Add(skill);
                    Debug.Log("Read skill:" + skillList[(int)SkillType.WSkill].skillName +
                              "\nSkillDescription:" + skillList[(int)SkillType.WSkill].GetDescription());
                }

                if (ESkillType != null)
                {
                    var skill = (Skill)Activator.CreateInstance(ESkillType);
                    skill.owner = this;
                    skill.SkillEffect();
                    skillList.Add(skill);
                    Debug.Log("Read skill:" + skillList[(int)SkillType.ESkill].skillName +
                              "\nSkillDescription:" + skillList[(int)SkillType.ESkill].GetDescription());
                }

                if (RSkillType != null)
                {
                    var skill = (Skill)Activator.CreateInstance(RSkillType);
                    skill.owner = this;
                    skill.SkillEffect();
                    skillList.Add(skill);
                    Debug.Log("Read skill:" + skillList[(int)SkillType.RSkill].skillName +
                              "\nSkillDescription:" + skillList[(int)SkillType.RSkill].GetDescription());
                }
                
                #endregion
            }

            #endregion
            
            _gameObject = gameObject;
            _team = Team.Hero;
            
            // 配置角色寻路组件
            _agent = _gameObject.GetComponent<NavMeshAgent>();
            _agent.updateUpAxis = false;
            _agent.updateRotation = false;
            _agent.speed = actualMovementSpeed;
            
            // 配置角色体型
            _gameObject.transform.localScale = new Vector2(actualScale * 2, actualScale * 2);
            
            // 配置攻击距离指示器
            _attackRangeIndicator = _gameObject.transform.Find("AttackRangeIndicator");
            _attackRangeIndicator.localScale = new Vector2(actualAttackRange / actualScale, actualAttackRange / actualScale);
            _attackRangeIndicator.GetComponent<SpriteRenderer>().enabled = false;
            
            // 其他变量初始化
            _autoAttack = false;
            _attackTimer = 0;
            level.Value = 1;
            magicPoint.Value = maxMagicPoint.Value;
            healthPoint.Value = maxHealthPoint.Value;
        }

        private const float rotationSpeed = 10;

        /// <summary>
        /// 英雄移动逻辑
        /// </summary>
        public override void Move()
        {
            // 获取鼠标位置
            var _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            // 当玩家点击左键
            if (Input.GetMouseButtonDown(0))
            {
                // 若提前点击A键，则开启自动攻击
                if (_attackRangeIndicator.GetComponent<SpriteRenderer>().enabled && !Input.GetKey(KeyCode.C))
                {
                    target.Value = null;
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
                        // 若有目标则设置为新的目标
                        var newTarget = _hit.collider.gameObject.GetComponent<EntityData>().entity;
                        
                        // 防止反复锁定相同目标
                        if (target.Value == null || !target.Value.Equals(newTarget))
                        {
                            target.Value = newTarget;
                        }
                        _autoAttack = true;
                        _findTarget = true;
                        break;
                    }
                }

                if (!_findTarget)
                {
                    target.Value = null;
                }
                
                if (target.Value.IsUnityNull())
                {
                    // 若没有物体则走到对应位置
                    _agent.SetDestination(_mousePosition);
                    _agent.stoppingDistance = 0;

                    // 若按了shift+右键则启动自动攻击模式（走A）
                    _autoAttack = Input.GetKey(KeyCode.LeftShift);
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
                    var collider = _gameObject.transform.Find("AttackRangeIndicator").GetComponent<CircleCollider2D>();

                    // 将目标设定为最近的敌方目标（tag与自己不同的）
                    target.Value = IsOverlappingOtherTag(collider, _gameObject.tag)?.entity;
                }
                
                if (target.Value != null)
                {
                    // 若有锁定的目标则持续索敌
                    // 走到敌人进入攻击范围为止
                    _agent.SetDestination(target.Value.gameObject.transform.position);
                    
                    // 获取攻击范围指示器的碰撞箱，判断是否可以攻击到敌人
                    var collider = _gameObject.transform.Find("AttackRangeIndicator").GetComponent<CircleCollider2D>();
                    if (IsOverlappingOtherTag(collider, _gameObject.tag)?.entity.gameObject == target.Value.gameObject)
                    {
                        _agent.isStopped = true;
                    }
                    else
                    {
                        _agent.isStopped = false;
                    }
                }
            }
        }

        /// <summary>
        /// 英雄攻击逻辑
        /// </summary>
        public override void Attack()
        {
            if (_attackTimer < 100)
            {
                _attackTimer += Time.deltaTime;
            }
            
            if (target.Value == null || !_agent.isStopped) 
                return;
            
            // 英雄转向目标
            var rotateDirection = new Vector2(
                target.Value.gameObject.transform.position.x - _gameObject.transform.position.x,
                target.Value.gameObject.transform.position.y - _gameObject.transform.position.y
            );

            if (!_asyncRotating)
            {
                Async.SetAsync(_gameObject.transform, () => RotateTo(rotateDirection), rotateTime);
            }

            if(_attackTimer < actualAttackInterval)
                return;

            // 发动攻击
            _attackTimer = 0;

            var bullet = BulletFactory.Instance.CreateBullet(this);
            bullet.OnBulletAwake += (self) =>
            {
                self.gameObject.transform.position = _gameObject.transform.position;
                self.gameObject.SetActive(true);
    
                // 设置目标
                self.target = target.Value;

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
                    if (Vector3.Distance(self.gameObject.transform.position, self.target.gameObject.transform.position) <= destroyDistance)
                    {
                        self.Destroy();
                    }
                };
            };
            
            bullet.Awake();
        }
        
        public void SetRotate()
        {
            if (!_agent.hasPath || _agent.path.corners.Length == 0 || _agent.isStopped || _asyncRotating)
                return;

            Vector3 targetPosition;
    
            if (_agent.path.corners.Length > 1)
            {
                targetPosition = _agent.path.corners[1];
            }
            else
            {
                targetPosition = _agent.destination;
            }

            // 计算 XY 平面方向
            var direction = new Vector2(
                targetPosition.x - _gameObject.transform.position.x,
                targetPosition.y - _gameObject.transform.position.y
            );

            RotateTo(direction);
        }

        /// <summary>
        /// 查看目标属性
        /// </summary>
        public void TargetCheck()
        {
            // 获取鼠标位置
            var _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            // 当玩家点击左键
            if (Input.GetMouseButton(0))
            {
                // 检测鼠标点击位置是否有物体
                var _hitBoxes = Physics2D.RaycastAll(_mousePosition, Vector2.zero);
                
                var _findTarget = false;
                var _find = new RaycastHit2D();
                foreach (var _hit in _hitBoxes)
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
                    target.Value = _find.collider.gameObject.GetComponent<EntityData>().entity;
                }
            }
        }

        /// <summary>
        /// 转向鼠标指针
        /// </summary>
        public override void RotateToMousePoint()
        {
            // 获取鼠标位置
            var _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            // 计算 XY 平面方向
            var direction = new Vector2(
                _mousePosition.x - _gameObject.transform.position.x,
                _mousePosition.y - _gameObject.transform.position.y
            );
            
            Async.SetAsync(_gameObject.transform, () => RotateTo(direction), rotateTime); 
        }
        
        #region 私有工具函数
        
        // 静态缓存，避免 GC
        private static readonly Collider2D[] _overlapBuffer = new Collider2D[20];

        /// <summary>
        /// 检测圆形范围内是否有与指定tag不同的其他碰撞体，并返回最近的一个EntityData
        /// </summary>
        private EntityData IsOverlappingOtherTag(CircleCollider2D collider, string excludeTag)
        {
            if (collider == null) return null;

            Vector2 center = collider.bounds.center;
            var radius = collider.radius * collider.transform.lossyScale.x;

            // 使用 NonAlloc 版本避免内存分配
            var count = Physics2D.OverlapCircleNonAlloc(center, radius, _overlapBuffer);

            EntityData nearestEntity = null;
            var nearestDistanceSqr = float.MaxValue;

            for (var i = 0; i < count; i++)
            {
                var col = _overlapBuffer[i];
                if (col == null) continue;

                var otherGo = col.gameObject;
                if (otherGo == null || otherGo == gameObject) continue; // 排除自身和空对象

                // 跳过相同 Tag 的对象
                if (otherGo.CompareTag(excludeTag)) continue;

                // 获取 EntityData 组件
                var entity = otherGo.GetComponent<EntityData>();
                if (entity == null) continue; // 没有该组件则跳过

                // 计算距离平方（避免开根号）
                var distSqr = (otherGo.transform.position - (Vector3)center).sqrMagnitude;
                if (distSqr < nearestDistanceSqr)
                {
                    nearestDistanceSqr = distSqr;
                    nearestEntity = entity;
                }
            }

            return nearestEntity; // 可能为 null
        }

        /// <summary>
        /// 平滑转向指定方向
        /// </summary>
        /// <param name="direction">方向</param>
        private void RotateTo(Vector2 direction)
        {
            if (direction.sqrMagnitude > 0.01f)
            {
                var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                var targetRot = Quaternion.Euler(0, 0, angle);

                _gameObject.transform.rotation = Quaternion.Slerp(
                    _gameObject.transform.rotation,
                    targetRot,
                    rotationSpeed * Time.deltaTime
                );
            }
        }
        
        #endregion
    }
}