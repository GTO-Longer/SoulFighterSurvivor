using System;
using System.Collections.Generic;
using System.Reflection;
using DataManagement;
using DG.Tweening;
using Factories;
using MVVM.ViewModels;
using Systems;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Utilities;

namespace Classes.Entities
{
    public class Hero : Entity
    {
        /// <summary>
        /// 寻路组件
        /// </summary>
        private readonly NavMeshAgent _agent;
        
        private Transform _attackRangeIndicator;
        private bool _asyncRotating => DOTween.IsTweening(_gameObject.transform);
        private const float rotateTime = 0.5f;
        private const int maxLevel = 18;
        
        /// <summary>
        /// 玩家锁定的实体
        /// </summary>
        public Property<Entity> target = new Property<Entity>();
        
        public Property<bool> isMoving = new Property<bool>();
        public Property<bool> showAttributes = new Property<bool>();

        public int skillPoint;
        
        /// <summary>
        /// 是否启用自动攻击模式
        /// </summary>
        private bool _autoAttack;
        /// <summary>
        /// 鼠标位置
        /// </summary>
        private Vector2 _mousePosition;
        /// <summary>
        /// 攻击计时器
        /// </summary>
        private float _attackTimer;
        /// <summary>
        /// 攻击前摇计时器
        /// </summary>
        private float _attackWindUpTimer;
        /// <summary>
        /// 生命和法力回复计时器
        /// </summary>
        private float _regenerateTimer;
        /// <summary>
        /// 普攻弹道速度
        /// </summary>
        private const float attackBulletSpeed = 15f;

        public List<Skill> skillList = new();
        /// <summary>
        /// 是否可以打断转身
        /// </summary>
        public bool canCancelTurn;
        /// <summary>
        /// 是否可以打断转身
        /// </summary>
        public bool canUseSkill;
        
        /// <summary>
        /// 创建游戏角色并初始化
        /// </summary>
        public Hero(GameObject gameObject, string name)
        {
            #region 读取英雄数据配置初始化数据

            var config = ResourceReader.ReadHeroConfig(name);
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
                _attackWindUp = config._attackWindUp;
                _attackSpeedYield = config._attackSpeedYield;

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
                    skill.owner= this;
                    skill.SkillEffect();
                    skillList.Add(skill);
                    Debug.Log("Read skill:" + skillList[(int)SkillType.PassiveSkill].skillName +
                              "\nSkillDescription:" + skillList[(int)SkillType.PassiveSkill].GetDescription());
                }

                if (QSkillType != null)
                {
                    var skill = (Skill)Activator.CreateInstance(QSkillType);
                    skill.owner= this;
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

                // 绑定技能升级按钮
                for (var index = (int)SkillType.QSkill; index <= (int)SkillType.RSkill; index++)
                {
                    var skill = skillList[index];
                    MVVM.Binder.BindButton(skill.upgradeButton, () => SkillUpgrade(skill));
                }
                
                #endregion
            }

            #endregion
            
            _gameObject = gameObject;
            _team = Team.Hero;
            canCancelTurn = true;
            canUseSkill = true;
            
            // 配置角色寻路组件
            _agent = _gameObject.GetComponent<NavMeshAgent>();
            _agent.updateUpAxis = false;
            _agent.updateRotation = false;
            
            // 配置角色体型
            _gameObject.transform.localScale = new Vector2(actualScale * 2, actualScale * 2);
            
            // 配置攻击距离指示器
            _attackRangeIndicator = _gameObject.transform.Find("AttackRangeIndicator");
            _attackRangeIndicator.localScale = new Vector2(actualAttackRange / actualScale, actualAttackRange / actualScale);
            _attackRangeIndicator.GetComponent<SpriteRenderer>().enabled = false;
            
            // 创建状态条
            StateBarFactory.Instance.Spawn(this);
            
            // 其他变量初始化
            _autoAttack = false;
            _attackTimer = 0;
            _regenerateTimer = 0;
            _attackWindUpTimer = 0;
            skillPoint = 0;
            level.Value = 0;
            experience.Value = 0;
            magicPoint.Value = maxMagicPoint.Value;
            healthPoint.Value = maxHealthPoint.Value;
            LevelUp();
        }

        private const float rotationSpeed = 10;

        /// <summary>
        /// 英雄移动逻辑
        /// </summary>
        public override void Move()
        {
            if (!_agent.updatePosition) return;

            // 设置速度
            _agent.speed = actualMovementSpeed;
            
            // 获取鼠标位置
            _mousePosition = CameraSystem._mainCamera.ScreenToWorldPoint(Input.mousePosition);
            
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

                if (!ToolFunctions.IsObjectAtMousePoint(out var obj, gameObject.tag))
                {
                    target.Value = null;
                    _autoAttack = false;
                }
                else
                {
                    _autoAttack = true;
                    target.Value = obj[0].GetComponent<EntityData>().entity;
                }
                
                if (target.Value.IsUnityNull())
                {
                    // 若没有物体则走到对应位置
                    _agent.SetDestination(_mousePosition);
                    _agent.stoppingDistance = 0;
                    
                    // 打断当前的转身动作
                    if (canCancelTurn)
                    {
                        DOTween.Kill(gameObject.transform);
                    }

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
                    target.Value = ToolFunctions.IsOverlappingOtherTag(_attackRangeIndicator.gameObject);
                }
                
                if (target.Value != null)
                {
                    // 若有锁定的目标则持续索敌
                    // 走到敌人进入攻击范围为止
                    _agent.SetDestination(target.Value.gameObject.transform.position);
                    _agent.isStopped = Vector2.Distance(gameObject.transform.position, target.Value.gameObject.transform.position)
                                       <= actualScale + actualAttackRange + 0.1f;
                }
            }

            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 0);
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
            {
                _attackWindUpTimer = 0;
                return;
            }

            // 英雄转向目标
            var rotateDirection = new Vector2(
                target.Value.gameObject.transform.position.x - _gameObject.transform.position.x,
                target.Value.gameObject.transform.position.y - _gameObject.transform.position.y
            );

            if (!_asyncRotating)
            {
                Async.SetAsync(rotateTime, gameObject.transform, () => RotateTo(rotateDirection));
            }

            if (_attackTimer < actualAttackInterval)
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
                            self.BulletHit();
                            self.Destroy();
                        }
                    };
                };

                bullet.OnBulletHit += (self) =>
                {
                    // 计算平A伤害
                    self.target.TakeDamage(self.target.CalculateADDamage(self.owner, self.owner.attackDamage), DamageType.AD);
                    
                    // 造成攻击特效
                    self.AttackEffectActivate();
                };
                
                bullet.Awake();
            }
        }

        /// <summary>
        /// 生命和法力值回复
        /// </summary>
        public void Regenerate()
        {
            _regenerateTimer += Time.deltaTime;

            if (_regenerateTimer >= 5)
            {
                TakeHeal(healthRegeneration);
                TakeMagicRecover(magicRegeneration);
                _regenerateTimer = 0;
            }
        }

        /// <summary>
        /// 技能冷却回转
        /// </summary>
        public void SkillCoolDown()
        {
            foreach (var skill in skillList)
            {
                skill.coolDownTimer += Time.deltaTime;
                if (skill.specialTimer < 0)
                {
                    skill.SpecialTimeOut();
                    skill.specialTimer = 0;
                }
                else if (skill.specialTimer > 0)
                {
                    skill.specialTimer -= Time.deltaTime;
                }

                // 设置技能冷却mask和文字
                if (skill.skillCoolDownMask != null && skill.skillCD != null && skill.upgradeButton != null)
                {
                    skill.skillCoolDownMask.fillAmount = skill.skillCoolDownProportion;
                    skill.skillCD.text = skill.actualSkillCoolDown - skill.coolDownTimer >= 1 ? $"{skill.actualSkillCoolDown - skill.coolDownTimer:F0}" : $"{skill.actualSkillCoolDown - skill.coolDownTimer:F1}";
                    skill.skillCD.gameObject.SetActive(skill.skillCoolDownProportion > 0);
                    if (skill.skillType is >= SkillType.QSkill and <= SkillType.RSkill)
                    {
                        skill.upgradeButton.gameObject.SetActive(skillPoint > 0);
                        skill.upgradeButton.interactable = skill.SkillCanUpgrade();
                    }
                }
            }
        }
        
        public void SetRotate()
        {
            if (!_agent.hasPath || _agent.path.corners.Length == 0 || _agent.isStopped || _asyncRotating)
                return;

            var targetPosition = _agent.path.corners.Length > 1 ? _agent.path.corners[1] : _agent.destination;

            RotateTo(targetPosition - _gameObject.transform.position);
        }

        /// <summary>
        /// 查看目标属性
        /// </summary>
        public void TargetCheck()
        {
            // 当玩家点击左键
            if (Input.GetMouseButton(0))
            {
                if (ToolFunctions.IsObjectAtMousePoint(out var results, gameObject.tag))
                {
                    target.Value = results[0].GetComponent<EntityData>().entity;
                }
            }
        }
        
        /// <summary>
        /// 冲刺
        /// </summary>
        /// <param name="destinationDistance">位移距离</param>
        /// <param name="dashDuration">位移时长</param>
        /// <param name="direction">位移方向</param>
        /// <param name="onComplete">位移完成回调</param>
        public void Dash(float destinationDistance, float dashDuration, Vector2 direction, TweenCallback onComplete = null)
        {
            // 标准化方向
            direction = direction.normalized;

            // 计算按照方向的原目标点
            var originPoint = (Vector2)gameObject.transform.position + direction * destinationDistance;

            // 计算实际落点
            var targetPosition = originPoint;

            // 定义搜索步长
            const float step = 0.1f;
            var maxSteps = (int)(destinationDistance / step);
            
            // 判断原目标点是否合法
            // 不合法则开始逐步搜索合法点
            var path = new NavMeshPath();
            if (!(_agent.CalculatePath(originPoint, path) && path.status == NavMeshPathStatus.PathComplete))
            {
                for (var i = 1; i <= maxSteps; i++)
                {
                    // 正向搜索
                    var forward = originPoint + direction * (step * i);
                    if (_agent.CalculatePath(forward, path) && path.status == NavMeshPathStatus.PathComplete)
                    {
                        targetPosition = forward;
                        break;
                    }

                    // 反向搜索
                    var backward = originPoint - direction * (step * i);
                    if (_agent.CalculatePath(backward, path) && path.status == NavMeshPathStatus.PathComplete)
                    {
                        targetPosition = backward;
                        break;
                    }
                }
            }
            
            // 设置面向
            gameObject.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

            // 关闭agent
            _agent.isStopped = true;
            _agent.updatePosition = false;
            
            _agent.nextPosition = gameObject.transform.position;

            // 使用DOTween平滑位移
            gameObject.transform.DOMove(targetPosition, dashDuration)
            .OnUpdate(() =>
            {
                canUseSkill = false;
                canCancelTurn = false;
                _agent.nextPosition = gameObject.transform.position;
            })
            .OnComplete(() =>
            {
                canUseSkill = true;
                canCancelTurn = true;
                
                // 恢复agent
                _agent.Warp(targetPosition);
                _agent.nextPosition = targetPosition;
                
                _agent.isStopped = false;
                _agent.updatePosition = true;
                
                onComplete?.Invoke();
            });
        }

        /// <summary>
        /// 转向鼠标指针
        /// </summary>
        public override void RotateToMousePoint()
        {
            // 计算方向
            var direction = new Vector2(
                _mousePosition.x - _gameObject.transform.position.x,
                _mousePosition.y - _gameObject.transform.position.y
            );
            
            Async.SetAsync(rotateTime,gameObject.transform, () => RotateTo(direction)); 
        }

        /// <summary>
        /// 获取经验
        /// </summary>
        public void GetExperience(float count)
        {
            experience.Value += count;
            if (experience.Value >= maxExperience.Value)
            {
                LevelUp();
                experience.Value -= maxExperience.Value;
            }
        }

        /// <summary>
        /// 玩家升级
        /// </summary>
        public void LevelUp()
        {
            if (level < maxLevel)
            {
                var maxHealthPointCache = maxHealthPoint.Value;
                var maxMagicPointCache = maxMagicPoint.Value;

                level.Value += 1;
                skillPoint += 1;

                healthPoint.Value += maxHealthPoint.Value - maxHealthPointCache;
                magicPoint.Value += maxMagicPoint.Value - maxMagicPointCache;
            }
        }

        /// <summary>
        /// 技能升级
        /// </summary>
        /// <param name="skill"></param>
        public void SkillUpgrade(Skill skill)
        {
            if (skillPoint > 0 && skill.skillType != SkillType.PassiveSkill)
            {
                skill.SkillUpgrade();
                skillPoint -= 1;
                
                SkillViewModel.chosenSkill.Value = SkillViewModel.chosenSkill.Value != null ? skill : null;
            }
        }
        
        #region 私有工具函数
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