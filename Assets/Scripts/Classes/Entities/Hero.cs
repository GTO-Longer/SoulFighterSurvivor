using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Classes.Skills;
using Components.UI;
using DataManagement;
using DG.Tweening;
using Factories;
using Managers;
using MVVM.ViewModels;
using Systems;
using UnityEngine;
using UnityEngine.AI;
using Utilities;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;

namespace Classes.Entities
{
    public sealed class Hero : Entity
    {
        private string heroName;
        private Transform _attackRangeIndicator;
        
        /// <summary>
        /// 玩家锁定的实体
        /// </summary>
        public Property<Entity> target = new Property<Entity>();
        
        public Property<bool> isMoving = new Property<bool>();
        public Property<bool> showAttributes = new Property<bool>();
        public Property<int> coins = new Property<int>();

        private float cursedBladeTimer;
        private float gainCoinTimer;
        private bool AkeyState;
        public bool canFlash;
        public bool isCuredBladeEffective => cursedBladeTimer > 0f;
        public bool hasBoughtEquipment;
        
        /// <summary>
        /// 是否启用自动攻击模式
        /// </summary>
        private bool _autoAttack;
        /// <summary>
        /// 鼠标位置
        /// </summary>
        public Vector2 _mousePosition;
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
        /// 角色海克斯列表
        /// </summary>
        public List<Hex> hexList = new();
        /// <summary>
        /// 角色装备列表
        /// </summary>
        public List<Property<Equipment>> equipmentList = new();
        public List<Equipment> tempEquipmentList = new();
        /// <summary>
        /// 是否可以移动
        /// </summary>
        public bool canMove;
        /// <summary>
        /// 是否可以使用技能
        /// </summary>
        public bool canUseSkill;
        
        /// <summary>
        /// 创建游戏角色并初始化
        /// </summary>
        public Hero(GameObject obj, Team team, string name) : base(obj, team)
        {
            #region 读取英雄数据和技能配置

            var config = ResourceReader.ReadHeroConfig(name);
            if (config != null)
            {
                heroName = config.heroName;
                
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
            
                #region 读取英雄技能配置
                
                var assembly = Assembly.GetExecutingAssembly();
                var passiveSkillType = assembly.GetType("Classes.Skills." + config._passiveSkill);
                var QSkillType = assembly.GetType("Classes.Skills." + config._QSkill);
                var WSkillType = assembly.GetType("Classes.Skills." + config._WSkill);
                var ESkillType = assembly.GetType("Classes.Skills." + config._ESkill);
                var RSkillType = assembly.GetType("Classes.Skills." + config._RSkill);

                if (passiveSkillType != null)
                {
                    skillList.Add((Skill)Activator.CreateInstance(passiveSkillType));
                }

                if (QSkillType != null)
                {
                    skillList.Add((Skill)Activator.CreateInstance(QSkillType));
                }

                if (WSkillType != null)
                {
                    skillList.Add((Skill)Activator.CreateInstance(WSkillType));
                }

                if (ESkillType != null)
                {
                    skillList.Add((Skill)Activator.CreateInstance(ESkillType));
                }

                if (RSkillType != null)
                {
                    skillList.Add((Skill)Activator.CreateInstance(RSkillType));
                }

                // 绑定技能升级按钮
                for (var index = (int)SkillType.QSkill; index <= (int)SkillType.RSkill; index++)
                {
                    var skill = skillList[index];
                    MVVM.Binder.BindButton(skill.upgradeButton, () => SkillUpgrade(skill));
                }
            
                // 配置召唤师技能
                var flash = new Flash
                {
                    skillType = SkillType.DSkill
                };
                skillList.Add(flash);
            
                var ghostPoro = new GhostPoro
                {
                    skillType = SkillType.FSkill
                };
                skillList.Add(ghostPoro);

                foreach (var skill in skillList)
                {
                    skill.owner = this;
                    
                    // 激活被动技能
                    skill.PassiveAbilityEffective?.Invoke();
                }

                #endregion
            }

            #endregion
            
            // 配置攻击距离指示器
            _attackRangeIndicator = _gameObject.transform.Find("AttackRangeIndicator");
            _attackRangeIndicator.localScale = new Vector2(attackRange / scale + 1, attackRange / scale + 1);

            attackRange.PropertyChanged += (_, _) =>
            {
                _attackRangeIndicator.localScale = new Vector2(attackRange / scale + 1, attackRange / scale + 1);
            };
            _attackRangeIndicator.GetComponent<SpriteRenderer>().enabled = false;
            
            // 配置角色体型
            _gameObject.transform.localScale = new Vector2(scale * 2, scale * 2);
            scale.PropertyChanged += (_, _) =>
            {
                _gameObject.transform.localScale = new Vector2(scale * 2, scale * 2);
                _attackRangeIndicator.localScale = new Vector2(attackRange / scale + 1, attackRange / scale + 1);
            };
            
            // 配置初始装备表
            equipmentList.Add(new Property<Equipment>());
            equipmentList.Add(new Property<Equipment>());
            equipmentList.Add(new Property<Equipment>());
            equipmentList.Add(new Property<Equipment>());
            equipmentList.Add(new Property<Equipment>());
            equipmentList.Add(new Property<Equipment>());
            
            // 创建状态条
            StateBarFactory.Instance.Spawn(this);
            
            // 其他变量初始化
            _autoAttack = false;
            canMove = true;
            canUseSkill = true;
            _attackTimer = 0;
            _regenerateTimer = 0;
            _attackWindUpTimer = 0;
            gainCoinTimer = 0;
            _skillPoint = 0;
            level.Value = 0;
            experience.Value = 0;
            cursedBladeTimer = 0;
            magicPoint.Value = maxMagicPoint.Value;
            healthPoint.Value = maxHealthPoint.Value;
            canFlash = true;
            coins.Value = 1500;
            LevelUp(3);

            // 定义基础攻击命中事件
            AttackEffect += (self, _, adDamage, _) =>
            {
                self.TakeHeal(adDamage * lifeSteal);
                self.TakeHeal(adDamage * omnivamp);
            };
            
            OnAttackHit += (self, targetEntity, isCrit) =>
            {
                // 计算平A伤害
                var damageCount = targetEntity.CalculateADDamage(self, self.attackDamage);
                targetEntity.TakeDamage(damageCount, DamageType.AD, this, isCrit);

                // 造成攻击特效
                if (targetEntity.isAlive)
                {
                    AttackEffectActivate(targetEntity, damageCount, attackEffectRatio);
                }

                // 清空咒刃计时器
                cursedBladeTimer = 0;
            };
            
            // 定义基础技能命中事件
            AbilityEffect += (self, _, skillDamage, _) =>
            {
                self.TakeHeal(skillDamage * omnivamp);
            };

            
            // 定义基础Update事件
            EntityUpdateEvent += (_) =>
            {
                // 持续获取金币
                if (gainCoinTimer > 0.5f)
                {
                    gainCoinTimer = 0;
                    coins.Value += 3;
                }
                else
                {
                    gainCoinTimer += Time.deltaTime;
                }
            };
            
            // 定义基础使用技能事件
            OnSkillUsed += (_) =>
            {
                cursedBladeTimer = 5;
            };
        }

        /// <summary>
        /// 英雄移动逻辑
        /// </summary>
        public override void Move()
        {
            // 获取鼠标位置
            _mousePosition = CameraSystem._mainCamera.ScreenToWorldPoint(Input.mousePosition);
            
            if (PanelUIRoot.Instance.isPanelOpen)
            {
                return;
            }
            
            if (!canMove)
            {
                _agent.SetStop(true);
                return;
            }
            
            // 当玩家按下A键
            if (Input.GetKeyDown(KeyCode.A))
            {
                AkeyState = true;
            }
            else if (Input.anyKeyDown && !Input.GetMouseButton(0))
            {
                AkeyState = false;
            }
            
            // 当玩家点击左键
            if (Input.GetMouseButton(0) && AkeyState)
            {
                target.Value = null;
                _agent.SetStop(false);
                _agent.SetDestination(_mousePosition);
                _autoAttack = true;
                AkeyState = false;
                
                return;
            }
            
            // 当玩家按下S键
            if (Input.GetKey(KeyCode.S))
            {
                target.Value = null;
                _autoAttack = false;
                
                //停止移动
                _agent.SetStop(true);
                _agent.goal = (Vector2)_gameObject.transform.position;
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
                            HUDUIRoot.Instance.targetAttributes.checkTarget.Value = target.Value;
                            break;
                        }
                    }
                }
                
                _agent.SetDestination(_mousePosition);
            }
            
            if (_autoAttack)
            {
                if (target.Value == null)
                {
                    // 自动攻击过程中持续索敌
                    if (ToolFunctions.IsOverlappingOtherTag(_attackRangeIndicator.gameObject, _gameObject.tag, out var entity))
                    {
                        target.Value = entity;
                        HUDUIRoot.Instance.targetAttributes.checkTarget.Value = entity;
                        _agent.SetStop(true);
                    }
                }
                
                if (target.Value != null)
                {
                    // 走到敌人进入攻击范围为止
                    _agent.SetDestination(target.Value.gameObject.transform.position);
                    _agent.SetStop (Vector2.Distance(gameObject.transform.position, target.Value.gameObject.transform.position) <= target.Value.scale + scale + attackRange);
                }
            }
        }

        public void ShowAttribute()
        {
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

            if (target.Value == null || !_agent.isStopped || !canMove)
            {
                _attackWindUpTimer = 0;
                return;
            }

            if (Vector2.Distance(target.Value.gameObject.transform.position, gameObject.transform.position) > attackRange.Value + scale + target.Value.scale)
            {
                return;
            }

            // 英雄转向目标
            var rotateDirection = new Vector2(
                target.Value.gameObject.transform.position.x - _gameObject.transform.position.x,
                target.Value.gameObject.transform.position.y - _gameObject.transform.position.y
            );
            RotateTo(ref rotateDirection);

            if (_attackTimer < actualAttackInterval)
            {
                _attackWindUpTimer = 0;
                return;
            }

            // 计算攻击前摇
            _attackWindUpTimer += Time.deltaTime;
            if (_attackWindUpTimer >= actualAttackInterval * _attackWindUp)
            {
                // 计算是否暴击
                var isCrit = Random.Range(0f, 1f) < criticalRate.Value;
                
                // 播放音效
                if (isCrit)
                {
                    AudioManager.Instance.Play($"Hero/{heroName}/CritAttack_OnCast", "CritAttack_OnCast");
                    AudioManager.Instance.Play($"Hero/{heroName}/CritAttack_Voice", "CritAttack_Voice");
                }
                else
                {
                    AudioManager.Instance.Play($"Hero/{heroName}/Attack_OnCast", "Attack_OnCast");
                    AudioManager.Instance.Play($"Hero/{heroName}/Attack_Voice", "Attack_Voice");
                }
                
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
                    self.owner.AttackHit(self.target, isCrit);

                    if (isCrit)
                    {
                        AudioManager.Instance.Play($"Hero/{heroName}/Attack_OnHit", "Attack_OnHit");
                    }
                    else
                    {
                        AudioManager.Instance.Play($"Hero/{heroName}/CritAttack_OnHit", "CritAttack_OnHit");
                    }
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
                if (healthPoint.Value < maxHealthPoint.Value)
                {
                    healthPoint.Value += healthRegeneration.Value;
                }

                if (magicPoint.Value < maxMagicPoint.Value)
                {
                    magicPoint.Value += magicRegeneration.Value;
                }

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
                    skill.SkillEnterCoolDown();
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
            var checkTarget = HUDUIRoot.Instance.targetAttributes.checkTarget;
            if (PanelUIRoot.Instance.isPanelOpen)
            {
                return;
            }
            
            // 当玩家点击左键
            if (Input.GetMouseButton(0))
            {
                HUDUIRoot.Instance.targetAttributes.checkTarget.Value = null;
                if (ToolFunctions.IsObjectAtMousePoint(out var results, gameObject.tag))
                {
                    foreach (var result in results.Where(result => result.GetComponent<EntityData>()))
                    {
                        HUDUIRoot.Instance.targetAttributes.checkTarget.Value = result.GetComponent<EntityData>().entity;
                        break;
                    }
                }
            }

            if (checkTarget.Value == null)
            {
                HUDUIRoot.Instance.targetAttributes.checkTarget.Value = target.Value;
            }
            else if (!checkTarget.Value.isAlive)
            {
                HUDUIRoot.Instance.targetAttributes.checkTarget.Value = null;
            }
        }

        /// <summary>
        /// 冲刺
        /// </summary>
        /// <param name="destinationDistance">位移距离</param>
        /// <param name="dashDuration">位移时长</param>
        /// <param name="direction">位移方向</param>
        /// <param name="onComplete">位移完成回调</param>
        /// <param name="onUpdate">位移Update回调</param>
        /// <param name="skillUsable">是否可以使用技能</param>
        /// <param name="flashUsable">是否可以闪现</param>
        public void Dash(float destinationDistance, float dashDuration, Vector2 direction, TweenCallback onComplete = null, TweenCallback onUpdate = null, bool skillUsable = false, bool flashUsable = false)
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
            currentDash = gameObject.transform.DOMove(targetPosition, dashDuration)
            .OnUpdate(() =>
            {
                canFlash = flashUsable;
                canUseSkill = skillUsable;
                canMove = false;
                onUpdate?.Invoke();
            })
            .OnComplete(() =>
            {
                canFlash = true;
                canUseSkill = true;
                canMove = true;
                
                // 恢复agent
                _agent.Warp(targetPosition);
                _agent.SetStop(false);

                currentDash = null;
                
                onComplete?.Invoke();
            })
            .OnKill(() =>
            {
                canFlash = true;
                canUseSkill = true;
                canMove = true;
                
                // 恢复agent
                _agent.SetStop(false);

                currentDash = null;
                
                onComplete?.Invoke();
            });
        }

        /// <summary>
        /// 闪烁
        /// </summary>
        public void Flash(Vector2 direction, float distance)
        {
            // 打断当前位移
            currentDash.Kill();
            
            // 计算目标点
            var targetPosition = (Vector2)gameObject.transform.position + direction.normalized * distance;
            NavMesh.SamplePosition(targetPosition, out var hit, 10000, NavMesh.AllAreas);
            agent.Warp(hit.position);
            forcedDirection = direction;
            gameObject.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        }

        /// <summary>
        /// 技能升级
        /// </summary>
        public void SkillUpgrade(Skill skill)
        {
            if (_skillPoint > 0 && skill.skillType is >= SkillType.QSkill and <= SkillType.RSkill)
            {
                skill.SkillUpgrade();

                if (HUDUIRoot.Instance.skillInfo.chosenSkill.Value != null)
                {
                    HUDUIRoot.Instance.skillInfo.chosenSkill.Value = null;
                    HUDUIRoot.Instance.skillInfo.chosenSkill.Value = skill;
                }
            }
        }

        /// <summary>
        /// 获取金币
        /// </summary>
        public void GainCoin(int amount)
        {
            var value = (int)(amount * (1 + fortune));
            coins.Value += value;
            ScreenTextFactory.Instance.Spawn(_gameObject.transform.position, $"+  <sprite=\"Coin\" index=0>{value:D}", 1f,
                250, 75, Color.yellow);
        }

        /// <summary>
        /// 穿上装备
        /// </summary>
        public void PurchaseEquipment(Equipment equipment)
        {
            if (equipment == null) return;
            if (!equipment.canPurchase) return;
            
            var uniqueCheck = equipmentList.Find(equip => equip.Value != null && equip.Value._uniqueEffect.Intersect(equipment._uniqueEffect).Any());
            if (equipment.owner == null && coins.Value >= equipment._cost)
            {
                // 若是锻造器则直接使用，不进入装备槽
                if (equipment._equipmentType == EquipmentType.Anvil)
                {
                    coins.Value -= equipment._cost;
                    equipment.OnActiveSkillEffective();
                    ShopSystem.Instance.CloseShopPanel();
                    return;
                }
                
                // 装备独一性检测
                if ((equipment._uniqueEffect?.Count > 0 && uniqueCheck == null) || equipment._uniqueEffect?.Count <= 0)
                {
                    foreach (var property in equipmentList)
                    {
                        if (property.Value == null)
                        {
                            hasBoughtEquipment = true;
                            coins.Value -= equipment._cost;
                            property.Value = equipment;
                            equipment.OnEquipmentGet(this);
                            return;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 出售装备
        /// </summary>
        public void SellEquipment(Equipment equipment)
        {
            if (equipment == null) return;
            
            foreach (var property in equipmentList)
            {
                if (property.Value == equipment)
                {
                    coins.Value += (int)(property.Value._cost * 0.7f);
                    property.Value.OnEquipmentRemove();
                    property.Value = null;

                    if (tempEquipmentList.Count > 0)
                    {
                        property.Value = tempEquipmentList[0];
                        tempEquipmentList[0].OnEquipmentGet(this);
                        tempEquipmentList.RemoveAt(0);
                        return;
                    }
                    
                    return;
                }
            }
        }
        
        /// <summary>
        /// 使用装备主动技能
        /// </summary>
        public void EquipmentActiveSkillRelease(int equipmentId)
        {
            if (equipmentList[equipmentId].Value != null)
            {
                equipmentList[equipmentId].Value.OnActiveSkillEffective();
            }
        }

        /// <summary>
        /// 获取海克斯
        /// </summary>
        public void GetHex(Hex hex)
        {
            hexList.Add(hex);
            HexListViewModel.Instance.SetHex(hexList.IndexOf(hex), hex);
            hex.OnHexGet(this);
        }

        /// <summary>
        /// 移除海克斯
        /// </summary>
        public void RemoveHex(Hex hex)
        {
            HexListViewModel.Instance.SetHex(hexList.IndexOf(hex), null);
            hexList.Remove(hex);
            hex.OnHexRemove();
        }

        /// <summary>
        /// 移除全部海克斯
        /// </summary>
        public void RemoveAllHex()
        {
            foreach (var hex in hexList)
            {
                hex.OnHexRemove();
            }
            hexList.Clear();
        }

        public override void Die(Entity killer)
        {
            base.Die(killer);
            
            AudioManager.Instance.Play($"Hero/{heroName}/Death", "Death");
            AudioManager.Instance.Play($"Hero/{heroName}/Death_Voice", "Death_Voice");
        }
    }
}