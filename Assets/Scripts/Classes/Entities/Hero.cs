using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DataManagement;
using DG.Tweening;
using Factories;
using MVVM.ViewModels;
using Systems;
using UnityEngine;
using UnityEngine.AI;
using Utilities;
using Vector2 = UnityEngine.Vector2;

namespace Classes.Entities
{
    public class Hero : Entity
    {
        private Transform _attackRangeIndicator;
        
        /// <summary>
        /// 玩家锁定的实体
        /// </summary>
        public Property<Entity> target = new Property<Entity>();
        public Property<bool> isMoving = new Property<bool>();
        public Property<bool> showAttributes = new Property<bool>();
        public Property<int> coins = new Property<int>();

        private float cursedBladeTimer;
        public bool isCuredBladeEffective => cursedBladeTimer > 0f;
        
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
        private const float attackBulletSpeed = 1500f;

        /// <summary>
        /// 角色技能列表
        /// </summary>
        public List<Skill> skillList = new();
        /// <summary>
        /// 角色装备列表
        /// </summary>
        public List<Equipment> equipmentList = new();
        /// <summary>
        /// 是否可以移动
        /// </summary>
        public bool canMove;
        /// <summary>
        /// 是否可以使用技能
        /// </summary>
        public bool canUseSkill;

        #region 英雄事件
        
        // Q技能释放事件
        public event Action<Hero, Entity> OnQSkillRelease;
        public void QSkillRelease(Entity targetEntity = null)
        {
            cursedBladeTimer = 5;
            OnQSkillRelease?.Invoke(this, targetEntity);
        }
        
        // W技能释放事件
        public event Action<Hero, Entity> OnWSkillRelease;
        public void WSkillRelease(Entity targetEntity = null)
        {
            cursedBladeTimer = 5;
            OnWSkillRelease?.Invoke(this, targetEntity);
        }
        
        // E技能释放事件
        public event Action<Hero, Entity> OnESkillRelease;
        public void ESkillRelease(Entity targetEntity = null)
        {
            cursedBladeTimer = 5;
            OnESkillRelease?.Invoke(this, targetEntity);
        }
        
        // R技能释放事件
        public event Action<Hero, Entity> OnRSkillRelease;
        public void RSkillRelease(Entity targetEntity = null)
        {
            cursedBladeTimer = 5;
            OnRSkillRelease?.Invoke(this, targetEntity);
        }

        #endregion
        
        /// <summary>
        /// 创建游戏角色并初始化
        /// </summary>
        public Hero(GameObject obj, Team team, string name) : base(obj, team)
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
            
            // 配置角色体型
            _gameObject.transform.localScale = new Vector2(scale * 2, scale * 2);
            
            // 配置攻击距离指示器
            _attackRangeIndicator = _gameObject.transform.Find("AttackRangeIndicator");
            _attackRangeIndicator.localScale = new Vector2(attackRange / scale, attackRange / scale);
            _attackRangeIndicator.GetComponent<SpriteRenderer>().enabled = false;
            
            // 创建状态条
            StateBarFactory.Instance.Spawn(this);
            
            // 其他变量初始化
            _autoAttack = false;
            canMove = true;
            canUseSkill = true;
            _attackTimer = 0;
            _regenerateTimer = 0;
            _attackWindUpTimer = 0;
            _skillPoint = 0;
            level.Value = 0;
            experience.Value = 0;
            cursedBladeTimer = 0;
            magicPoint.Value = maxMagicPoint.Value;
            healthPoint.Value = maxHealthPoint.Value;
            coins.Value = 750;
            LevelUp();

            // 定义基础攻击命中事件
            OnAttackHit += (self, targetEntity) =>
            {
                // 计算平A伤害
                var damageCount = targetEntity.CalculateADDamage(self, self.attackDamage);
                targetEntity.TakeDamage(damageCount, DamageType.AD, this);

                // 造成攻击特效
                if (targetEntity.isAlive)
                {
                    AttackEffectActivate(targetEntity, damageCount);
                }

                // 清空咒刃计时器
                cursedBladeTimer = 0;
            };
            
            // 定义基础技能命中事件
            // AbilityEffect += ;
        }

        /// <summary>
        /// 英雄移动逻辑
        /// </summary>
        public override void Move()
        {
            // 获取鼠标位置
            _mousePosition = CameraSystem._mainCamera.ScreenToWorldPoint(Input.mousePosition);

            if (!canMove)
            {
                _agent.SetStop(true);
                return;
            }
            
            // 当玩家点击左键
            if (Input.GetMouseButtonDown(0))
            {
                // 若提前点击A键，则开启自动攻击
                if (_attackRangeIndicator.GetComponent<SpriteRenderer>().enabled && !Input.GetKey(KeyCode.C))
                {
                    target.Value = null;
                    _agent.SetDestination(_mousePosition);
                    _autoAttack = true;
                }
            }
            
            // 当玩家按下S键
            if (Input.GetKey(KeyCode.S))
            {
                target.Value = null;
                _autoAttack = false;
                
                //停止移动
                _agent.SetStop(true);
            }
            
            // 当玩家点击右键
            if (Input.GetMouseButton(1))
            {
                // 检测右键是否点击到敌人
                if (!ToolFunctions.IsObjectAtMousePoint(out var obj, "Enemy", true))
                {
                    _agent.SetStop(false);
                    target.Value = null;
                    _autoAttack = Input.GetKey(KeyCode.LeftShift);
                }
                else
                {
                    _autoAttack = true;

                    foreach (var result in obj)
                    {
                        if (result.GetComponent<EntityData>())
                        {
                            target.Value = result.GetComponent<EntityData>().entity;
                            break;
                        }
                    }
                }
                
                _agent.SetDestination(_mousePosition);
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
                if (target.Value == null)
                {
                    // 自动攻击过程中持续索敌
                    if (ToolFunctions.IsOverlappingOtherTag(_attackRangeIndicator.gameObject, _gameObject.tag, out var entity))
                    {
                        target.Value = entity;
                        _agent.SetStop(true);
                    }
                }
                
                if (target.Value != null)
                {
                    // 走到敌人进入攻击范围为止
                    _agent.SetDestination(target.Value.gameObject.transform.position);
                    _agent.SetStop (Vector2.Distance(gameObject.transform.position, target.Value.gameObject.transform.position)
                                     <= scale + attackRange);
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
            {
                _attackWindUpTimer = 0;
                return;
            }

            // 英雄转向目标
            var rotateDirection = new Vector2(
                target.Value.gameObject.transform.position.x - _gameObject.transform.position.x,
                target.Value.gameObject.transform.position.y - _gameObject.transform.position.y
            );
            RotateTo(rotateDirection);

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
                        if (Vector3.Distance(self.gameObject.transform.position, self.target.gameObject.transform.position) <= destroyDistance)
                        {
                            self.BulletHit();
                            self.Destroy();
                        }
                    };
                };

                bullet.OnBulletHit += (self) =>
                {
                    // 触发普通攻击命中事件
                    self.owner.AttackHit(self.target);
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

                skill.UpdateSkillUI();
            }

            if (cursedBladeTimer > 0)
            {
                cursedBladeTimer -= Time.deltaTime;
            }
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
                    foreach (var result in results.Where(result => result.GetComponent<EntityData>()))
                    {
                        target.Value = result.GetComponent<EntityData>().entity;
                        break;
                    }
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
            const float step = 10f;
            var maxSteps = (int)(destinationDistance / step);
            
            // 判断原目标点是否合法
            // 不合法则开始逐步搜索合法点
            var path = new NavMeshPath();
            if (!(_agent.navAgent.CalculatePath(originPoint, path) && path.status == NavMeshPathStatus.PathComplete))
            {
                for (var i = 1; i <= maxSteps; i++)
                {
                    // 正向搜索
                    var forward = originPoint + direction * (step * i);
                    if (_agent.navAgent.CalculatePath(forward, path) && path.status == NavMeshPathStatus.PathComplete)
                    {
                        targetPosition = forward;
                        break;
                    }
            
                    // 反向搜索
                    var backward = originPoint - direction * (step * i);
                    if (_agent.navAgent.CalculatePath(backward, path) && path.status == NavMeshPathStatus.PathComplete)
                    {
                        targetPosition = backward;
                        break;
                    }
                }
            }
            
            // 设置面向
            gameObject.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

            // 关闭agent
            _agent.SetStop(true);

            // 使用DOTween平滑位移
            gameObject.transform.DOMove(targetPosition, dashDuration)
            .OnUpdate(() =>
            {
                canUseSkill = false;
                canMove = false;
            })
            .OnComplete(() =>
            {
                canUseSkill = true;
                canMove = true;
                
                // 恢复agent
                _agent.Warp(targetPosition);
                _agent.SetStop(false);
                
                onComplete?.Invoke();
            });
        }

        /// <summary>
        /// 技能升级
        /// </summary>
        public void SkillUpgrade(Skill skill)
        {
            if (_skillPoint > 0 && skill.skillType is >= SkillType.QSkill and <= SkillType.RSkill)
            {
                skill.SkillUpgrade();
                _skillPoint -= 1;

                if (SkillViewModel.chosenSkill.Value != null)
                {
                    SkillViewModel.chosenSkill.Value = null;
                    SkillViewModel.chosenSkill.Value = skill;
                }
            }
        }

        /// <summary>
        /// 穿上装备
        /// </summary>
        public void PurchaseEquipment(Equipment equipment)
        {
            if (equipmentList.Count < 6 && equipment.owner == null && coins.Value > equipment._cost)
            {
                coins.Value -= equipment._cost;
                equipmentList.Add(equipment);
                equipment.OnEquipmentGet(this);
            }
        }
    }
}