using System;
using System.Collections.Generic;
using DataManagement;
using Factories;
using RVO;
using UnityEngine;
using Utilities;

namespace Classes
{
    /// <summary>
    /// 实体类
    /// </summary>
    public class Entity
    {
        /// <summary>
        /// 对应的游戏物体
        /// </summary>
        protected GameObject _gameObject;
        /// <summary>
        /// 实体所属队伍
        /// </summary>
        protected Team _team;
        /// <summary>
        /// 对应的寻路Agent
        /// </summary>
        protected RVOAgent _agent;
        public RVOAgent agent => _agent;
        public bool isAlive;
        private const int maxLevel = 18;
        public int _skillPoint;
        protected Vector2 forcedDirection;
        protected float damageBoost;
        public List<Buff> buffList;
        
        #region 最终属性

        /// <summary>
        /// 人物等级
        /// </summary>
        public Property<float> level;
        /// <summary>
        /// 经验值
        /// </summary>
        public Property<float> experience;
        /// <summary>
        /// 最大经验值
        /// </summary>
        public Property<float> maxExperience;
        /// <summary>
        /// 最大生命值
        /// </summary>
        public Property<float> maxHealthPoint;
        /// <summary>
        /// 当前生命值
        /// </summary>
        public Property<float> healthPoint;
        /// <summary>
        /// 当前法力值
        /// </summary>
        public Property<float> magicPoint;
        /// <summary>
        /// 最大法力值
        /// </summary>
        public Property<float> maxMagicPoint;
        /// <summary>
        /// 攻击速度
        /// </summary>
        public Property<float> attackSpeed;
        /// <summary>
        /// 攻击力
        /// </summary>
        public Property<float> attackDamage;
        /// <summary>
        /// 法术强度
        /// </summary>
        public Property<float> abilityPower;
        /// <summary>
        /// 技能急速
        /// </summary>
        public Property<float> abilityHaste;
        /// <summary>
        /// 装备技能急速
        /// </summary>
        public Property<float> equipmentAbilityHaste;
        /// <summary>
        /// 召唤师技能急速
        /// </summary>
        public Property<float> summonerAbilityHaste;
        /// <summary>
        /// 法术防御
        /// </summary>
        public Property<float> magicDefense;
        /// <summary>
        /// 物理防御
        /// </summary>
        public Property<float> attackDefense;
        /// <summary>
        /// 物理穿透
        /// </summary>
        public Property<float> attackPenetration;
        /// <summary>
        /// 法术穿透
        /// </summary>
        public Property<float> magicPenetration;
        /// <summary>
        /// 百分比物理穿透
        /// </summary>
        public Property<float> percentageAttackPenetration;
        /// <summary>
        /// 百分比法术穿透
        /// </summary>
        public Property<float> percentageMagicPenetration;
        /// <summary>
        /// 暴击率
        /// </summary>
        public Property<float> criticalRate;
        /// <summary>
        /// 暴击伤害
        /// </summary>
        public Property<float> criticalDamage;
        /// <summary>
        /// 移动速度
        /// </summary>
        public Property<float> movementSpeed;
        /// <summary>
        /// 射程
        /// </summary>
        public Property<float> attackRange;
        /// <summary>
        /// 体型
        /// </summary>
        public Property<float> scale;
        /// <summary>
        /// 生命偷取
        /// </summary>
        public Property<float> lifeSteal;
        /// <summary>
        /// 适应之力
        /// </summary>
        protected Property<float> adaptiveForce;
        /// <summary>
        /// 生命回复
        /// </summary>
        public Property<float> healthRegeneration;
        /// <summary>
        /// 法力回复
        /// </summary>
        public Property<float> magicRegeneration;
        /// <summary>
        /// 全能吸血
        /// </summary>
        public Property<float> omnivamp;
        
        #endregion
        
        #region 实际属性
        
        /// <summary>
        /// 实际冷却缩减
        /// </summary>
        public float actualAbilityCooldown => 100f / (abilityHaste + 100f);
        /// <summary>
        /// 实际物理减伤率
        /// </summary>
        public float actualAttackDamageReduction => attackDefense / (attackDefense + 100f);
        /// <summary>
        /// 实际魔法减伤率
        /// </summary>
        public float actualMagicDamageReduction => magicDefense / (magicDefense + 100f);
        /// <summary>
        /// 实际攻击间隔
        /// </summary>
        public float actualAttackInterval => attackSpeed == 0 ? 999 : 1f / attackSpeed;
        /// <summary>
        /// 当前生命值百分比
        /// </summary>
        public Property<float> healthPointProportion;
        /// <summary>
        /// 当前法力值百分比
        /// </summary>
        public Property<float> magicPointProportion;
        /// <summary>
        /// 当前经验值百分比
        /// </summary>
        public Property<float> experienceProportion;
        
        #endregion
        
        #region 计算属性
        
        // 生命值相关
        /// <summary>
        /// 最大生命值加成
        /// </summary>
        public Property<float> _maxHealthPointBonus;
        /// <summary>
        /// 百分比最大生命值加成
        /// </summary>
        public Property<float> _percentageMaxHealthPointBonus;
        
        // 法力值相关
        /// <summary>
        /// 法力值加成
        /// </summary>
        public Property<float> _maxMagicPointBonus;
        /// <summary>
        /// 百分比法力值加成
        /// </summary>
        public Property<float> _percentageMaxMagicPointBonus;
        
        // 攻速相关
        /// <summary>
        /// 攻击速度加成
        /// </summary>
        public Property<float> _attackSpeedBonus;
        /// <summary>
        /// 百分比攻击速度加成
        /// </summary>
        public Property<float> _percentageAttackSpeedBonus;
        
        // 攻击力相关
        /// <summary>
        /// 攻击力加成
        /// </summary>
        public Property<float> _attackDamageBonus;
        /// <summary>
        /// 百分比攻击力加成
        /// </summary>
        public Property<float> _percentageAttackDamageBonus;
        
        // 法强相关
        /// <summary>
        /// 法术强度加成
        /// </summary>
        public Property<float> _abilityPowerBonus;
        /// <summary>
        /// 百分比法术强度加成
        /// </summary>
        public Property<float> _percentageAbilityPowerBonus;
        
        // 技能急速相关
        /// <summary>
        /// 技能急速加成
        /// </summary>
        public Property<float> _abilityHasteBonus;
        /// <summary>
        /// 百分比技能急速加成
        /// </summary>
        public Property<float> _percentageAbilityHasteBonus;
        /// <summary>
        /// 装备技能急速加成
        /// </summary>
        public Property<float> _equipmentAbilityHasteBonus;
        /// <summary>
        /// 召唤师技能急速加成
        /// </summary>
        public Property<float> _summonerAbilityHasteBonus;
        
        // 物抗相关
        /// <summary>
        /// 物理防御加成
        /// </summary>
        public Property<float> _attackDefenseBonus;
        /// <summary>
        /// 百分比物理防御加成
        /// </summary>
        public Property<float> _percentageAttackDefenseBonus;
        
        // 法抗相关
        /// <summary>
        /// 法术防御加成
        /// </summary>
        public Property<float> _magicDefenseBonus;
        /// <summary>
        /// 百分比法术防御加成
        /// </summary>
        public Property<float> _percentageMagicDefenseBonus;
        
        // 物穿相关
        /// <summary>
        /// 物理穿透加成
        /// </summary>
        public Property<float> _attackPenetrationBonus;
        /// <summary>
        /// 百分比物理穿透加成
        /// </summary>
        public Property<float> _percentageAttackPenetrationBonus;
        
        // 法穿相关
        /// <summary>
        /// 法术穿透加成
        /// </summary>
        public Property<float> _magicPenetrationBonus;
        /// <summary>
        /// 百分比法术穿透加成
        /// </summary>
        public Property<float> _percentageMagicPenetrationBonus;
        
        // 暴击相关
        /// <summary>
        /// 实际暴击率
        /// </summary>
        public Property<float> _actualCriticalRate;
        /// <summary>
        /// 暴击率加成
        /// </summary>
        public Property<float> _criticalRateBonus;
        /// <summary>
        /// 百分比暴击率加成
        /// </summary>
        public Property<float> _percentageCriticalRateBonus;
        /// <summary>
        /// 基础暴击伤害
        /// </summary>
        public float _baseCriticalDamage;
        /// <summary>
        /// 暴击伤害加成
        /// </summary>
        public Property<float> _criticalDamageBonus;
        
        // 移速相关
        /// <summary>
        /// 移动速度加成
        /// </summary>
        public Property<float> _movementSpeedBonus;
        /// <summary>
        /// 百分比移动速度加成
        /// </summary>
        public Property<float> _percentageMovementSpeedBonus;
        
        // 射程相关
        /// <summary>
        /// 射程加成
        /// </summary>
        public Property<float> _attackRangeBonus;
        /// <summary>
        /// 百分比射程加成
        /// </summary>
        public Property<float> _percentageAttackRangeBonus;
        
        // 体型相关
        /// <summary>
        /// 百分比体型加成
        /// </summary>
        public Property<float> _percentageScaleBonus;
        
        // 自然回复相关
        /// <summary>
        /// 百分比生命回复加成
        /// </summary>
        public Property<float> _percentageHealthRegenerationBonus;
        /// <summary>
        /// 百分比法力值回复加成
        /// </summary>
        public Property<float> _percentageMagicRegenerationBonus;
        
        #endregion

        #region 转化率

        /// <summary>
        /// 溢出暴击率-攻击力加成 转化率
        /// - <c>Value.x</c>: 剩余的溢出暴击率要乘的系数  
        /// - <c>Value.y</c>: 增加的攻击力加成要乘的系数
        /// </summary>
        public Property<Vector2> _CRToAD_ConversionEfficiency;
        /// <summary>
        /// 攻击力加成-法强加成 转化率
        /// - <c>Value.x</c>: 剩余的攻击力加成要乘的系数  
        /// - <c>Value.y</c>: 增加的法强加成要乘的系数
        /// </summary>
        public Property<Vector2> _ADToAP_ConversionEfficiency;
        /// <summary>
        /// 法强加成-攻击力加成 转化率
        /// - <c>Value.x</c>: 剩余的法强加成要乘的系数  
        /// - <c>Value.y</c>: 增加的攻击力加成要乘的系数
        /// </summary>
        public Property<Vector2> _APToAD_ConversionEfficiency;
        /// <summary>
        /// 生命值加成-攻击力加成 转化率
        /// - <c>Value.x</c>: 剩余的生命值加成要乘的系数  
        /// - <c>Value.y</c>: 增加的攻击力加成要乘的系数
        /// </summary>
        public Property<Vector2> _HPToAD_ConversionEfficiency;
        /// <summary>
        /// 生命值加成-法强加成 转化率
        /// - <c>Value.x</c>: 剩余的生命值加成要乘的系数  
        /// - <c>Value.y</c>: 增加的法强加成要乘的系数
        /// </summary>
        public Property<Vector2> _HPToAP_ConversionEfficiency;
        /// <summary>
        /// 法力值加成-生命值加成 转化率
        /// - <c>Value.x</c>: 剩余的法力值加成要乘的系数  
        /// - <c>Value.y</c>: 增加的生命值加成要乘的系数
        /// </summary>
        public Property<Vector2> _MPToHP_ConversionEfficiency;
        /// <summary>
        /// 法力值加成-攻击力加成 转化率
        /// - <c>Value.x</c>: 剩余的法力值加成要乘的系数  
        /// - <c>Value.y</c>: 增加的攻击力加成要乘的系数
        /// </summary>
        public Property<Vector2> _MPToAD_ConversionEfficiency;
        /// <summary>
        /// 法力值加成-法强加成 转化率
        /// - <c>Value.x</c>: 剩余的法力值加成要乘的系数  
        /// - <c>Value.y</c>: 增加的法强加成要乘的系数
        /// </summary>
        public Property<Vector2> _MPToAP_ConversionEfficiency;
        /// <summary>
        /// 法强加成-法力值加成 转化率
        /// - <c>Value.x</c>: 剩余的法强加成要乘的系数  
        /// - <c>Value.y</c>: 增加的法力值加成要乘的系数
        /// </summary>
        public Property<Vector2> _APToMP_ConversionEfficiency;
        /// <summary>
        /// 双抗加成-攻击力加成 转化率
        /// - <c>Value.x</c>: 剩余的双抗加成要乘的系数  
        /// - <c>Value.y</c>: 增加的攻击力加成要乘的系数
        /// </summary>
        public Property<Vector2> _DEFToAD_ConversionEfficiency;

        #endregion

        #region 基础属性
        
        /// <summary>
        /// 基础最大生命值
        /// </summary>
        protected float _baseMaxHealthPoint;
        /// <summary>
        /// 基础法力值
        /// </summary>
        protected float _baseMaxMagicPoint;
        /// <summary>
        /// 基础攻击速度
        /// </summary>
        protected float _baseAttackSpeed;
        /// <summary>
        /// 基础攻击力
        /// </summary>
        protected float _baseAttackDamage;
        /// <summary>
        /// 基础物理防御
        /// </summary>
        protected float _baseAttackDefense;
        /// <summary>
        /// 基础法术防御
        /// </summary>
        protected float _baseMagicDefense;
        /// <summary>
        /// 基础生命回复加成
        /// </summary>
        public float _baseHealthRegeneration;
        /// <summary>
        /// 基础法力值回复加成
        /// </summary>
        public float _baseMagicRegeneration;
        /// <summary>
        /// 基础射程
        /// </summary>
        protected float _baseAttackRange;
        /// <summary>
        /// 基础移动速度
        /// </summary>
        protected float _baseMovementSpeed;
        /// <summary>
        /// 基础体型
        /// </summary>
        protected float _baseScale;
        /// <summary>
        /// 攻击蓄力占比
        /// </summary>
        protected float _attackWindUp;
        /// <summary>
        /// 攻速收益率
        /// </summary>
        public float _attackSpeedYield;

        #endregion
        
        #region 成长属性
        
        /// <summary>
        /// 最大生命值成长值
        /// </summary>
        protected float _maxHealthPointGrowth;
        /// <summary>
        /// 生命回复成长值
        /// </summary>
        protected float _healthRegenerationGrowth;
        /// <summary>
        /// 攻击力成长值
        /// </summary>
        protected float _attackDamageGrowth;
        /// <summary>
        /// 攻击速度成长值
        /// </summary>
        protected float _attackSpeedGrowth;
        /// <summary>
        /// 最大法力值成长值
        /// </summary>
        protected float _maxMagicPointGrowth;
        /// <summary>
        /// 法力回复成长值
        /// </summary>
        protected float _magicRegenerationGrowth;
        /// <summary>
        /// 物理抗性成长值
        /// </summary>
        protected float _attackDefenseGrowth;
        /// <summary>
        /// 法术抗性成长值
        /// </summary>
        protected float _magicDefenseGrowth;
        
        #endregion
        
        #region 获取封装属性
        
        /// <summary>
        /// 对应的游戏物体
        /// </summary>
        public GameObject gameObject => _gameObject;
        /// <summary>
        /// 实体所属队伍
        /// </summary>
        public Team team => _team;

        /// <summary>
        /// 基础攻击力
        /// </summary>
        public float baseAttackDamage => _baseAttackDamage + _attackDamageGrowth * level;
        /// <summary>
        /// 基础护甲
        /// </summary>
        public float baseAttackDefense => _baseAttackDefense + _attackDefenseGrowth * level;
        /// <summary>
        /// 基础魔法抗性
        /// </summary>
        public float baseMagicDefense => _baseMagicDefense + _magicDefenseGrowth * level;
        /// <summary>
        /// 基础移动速度
        /// </summary>
        public float baseMovementSpeed => _baseMovementSpeed + _movementSpeedBonus;
        
        #endregion

        #region 实体事件

        /// <summary>
        /// 受伤事件
        /// </summary>
        public event Action<Entity, Entity, float> OnHurt;
        public void Hurt(Entity attacker, float damageCount)
        {
            OnHurt?.Invoke(this, attacker, damageCount);
        } 
        
        /// <summary>
        /// 击杀单位
        /// </summary>
        public event Action<Entity, Entity> OnKillEntity;
        public void KillEntity(Entity target)
        {
            OnKillEntity?.Invoke(this, target);
        }
        
        /// <summary>
        /// 技能命中事件
        /// </summary>
        public event Action<Entity, Entity> OnSkillHit;
        public void SkillHit(Entity target)
        {
            OnSkillHit?.Invoke(this, target);
        }
        
        /// <summary>
        /// 攻击命中事件
        /// </summary>
        public event Action<Entity, Entity> OnAttackHit;
        public void AttackHit(Entity target)
        {
            OnAttackHit?.Invoke(this, target);
        }

        /// <summary>
        /// 持续更新事件
        /// </summary>
        public event Action<Entity> EntityUpdateEvent;
        public void EntityUpdate()
        {
            EntityUpdateEvent?.Invoke(this);
        }
        
        /// <summary>
        /// 攻击特效
        /// </summary>
        public event Action<Entity, Entity, float> AttackEffect;
        public void AttackEffectActivate(Entity target, float damageCount)
        {
            AttackEffect?.Invoke(this, target, damageCount);
        }
        
        /// <summary>
        /// 技能特效
        /// </summary>
        public event Action<Entity, Entity, float, Skill> AbilityEffect;
        public void AbilityEffectActivate(Entity target, float damageCount, Skill skill)
        {
            AbilityEffect?.Invoke(this, target, damageCount, skill);
        }

        #endregion
        
        #region 行为函数

        /// <summary>
        /// 受到伤害
        /// </summary>
        public void TakeDamage(float damageCount, DamageType damageType, Entity damageSource, bool isCritical = false)
        {
            if (isCritical)
            {
                damageCount *= (1 + damageSource.criticalDamage.Value);

                var color = damageType switch
                {
                    DamageType.AD => Color.red,
                    DamageType.Real => Color.white,
                    _ => Color.blue
                };
                
                ScreenTextFactory.Instance.Spawn(_gameObject.transform.position, $"-{damageCount:F0}", 0.5f,
                    200 * Mathf.Max(0.5f, damageCount / (damageCount + 100)), Mathf.Clamp(damageCount / 3f, 30, 100), color);
            }
            else
            {
                var color = damageType switch
                { 
                    DamageType.AD => new Color(1, 0.6f, 0, 1),
                    DamageType.Real => Color.white,
                    DamageType.AP => new Color(0, 0.6f, 1, 1),
                    DamageType.None => Color.black,
                    _ => throw new ArgumentOutOfRangeException(nameof(damageType), damageType, null)
                };
                
                ScreenTextFactory.Instance.Spawn(_gameObject.transform.position, $"-{damageCount:F0}", 0.5f,
                    150 * Mathf.Max(0.5f, damageCount / (damageCount + 100)), Mathf.Clamp(damageCount / 3f, 30, 100),
                    color);
            }
           
            healthPoint.Value -= damageCount * damageBoost;
            if (healthPoint.Value < 1)
            {
                healthPoint.Value = 0;
                damageSource.KillEntity(this);
                Die(damageSource);
            }
            
            Hurt(damageSource, damageCount);
        }

        /// <summary>
        /// 受到治疗
        /// </summary>
        public void TakeHeal(float healCount)
        {
            healthPoint.Value += healCount;
            if (healCount > 1)
            {
                ScreenTextFactory.Instance.Spawn(_gameObject.transform.position, $"+{healCount:F0}", 0.5f,
                    150 * Mathf.Max(0.5f, healCount / (healCount + 100)), Mathf.Clamp(healCount / 2f, 30, 100), Color.green);
            }

            if (healthPoint.Value > maxHealthPoint.Value)
            {
                healthPoint.Value = maxHealthPoint.Value;
            }
        }

        /// <summary>
        /// 受到法力值回复
        /// </summary>
        public void TakeMagicRecover(float recoverCount)
        {
            magicPoint.Value += recoverCount;
            if (magicPoint.Value > maxMagicPoint.Value)
            {
                magicPoint.Value = maxMagicPoint.Value;
            }
        }

        public float CalculateADDamage(Entity damageSource, float damageCount)
        {
            var damageAttackDefense = (attackDefense - damageSource.attackPenetration)
                                      * (1 - damageSource.percentageAttackPenetration);
            return  damageCount * 
                    (1 - damageAttackDefense / (damageAttackDefense + 100f));
        }

        public float CalculateAPDamage(Entity damageSource, float damageCount)
        {
            var damageMagicDefense = (magicDefense - damageSource.magicPenetration)
                                     * (1 - damageSource.percentageMagicPenetration);
            return  damageCount * 
                    (1 - damageMagicDefense / (damageMagicDefense + 100f));
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
                _skillPoint += 1;

                healthPoint.Value += maxHealthPoint.Value - maxHealthPointCache;
                magicPoint.Value += maxMagicPoint.Value - maxMagicPointCache;
            }
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
        /// 移动
        /// </summary>
        public virtual void Move(){}
        
        /// <summary>
        /// 攻击
        /// </summary>
        public virtual void Attack(){}
        
        private const float rotationSpeed = 10;
        
        /// <summary>
        /// 平滑转向指定方向
        /// </summary>
        /// <param name="direction">方向</param>
        public void RotateTo(ref Vector2 direction)
        {
            if (forcedDirection != Vector2.zero)
            {
                direction = forcedDirection;
                forcedDirection = Vector2.zero;
            }
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
        
        /// <summary>
        /// 死亡
        /// </summary>
        public virtual void Die(Entity killer){}
        
        /// <summary>
        /// 获取buff
        /// </summary>
        public void GetBuff(Buff buff)
        {
            var buffInList = buffList.Find(buffInList => buffInList.buffName == buff.buffName);
            if (buffInList == null)
            {
                buffList.Add(buff.GetBuff());
            }
            else if(!buffInList.isUnique)
            {
                buffList.Add(buff.GetBuff());
            }
            else
            {
                // 刷新持续时间
                buffInList.buffDurationTimer = 0;
            }
        }
        
        #endregion

        #region 定义函数

        public override bool Equals(object obj)
        {
            if (obj is not Entity other)
            {
                return false;
            }
            return gameObject == other.gameObject;
        }
        
        public override int GetHashCode()
        {
            return HashCode.Combine(gameObject);
        }

        #endregion

        protected Entity(GameObject gameObject, Team team)
        {
            #region 最终数据初始化
            
            level = new Property<float>();
            experience = new Property<float>();
            maxExperience = new Property<float>();
            healthPoint = new Property<float>();
            magicPoint = new Property<float>();
            maxMagicPoint = new Property<float>();
            attackSpeed = new Property<float>();
            attackDamage = new Property<float>();
            abilityPower = new Property<float>();
            abilityHaste = new Property<float>();
            equipmentAbilityHaste = new Property<float>();
            summonerAbilityHaste = new Property<float>();
            magicDefense = new Property<float>();
            attackDefense = new Property<float>();
            attackPenetration = new Property<float>();
            magicPenetration = new Property<float>();
            criticalRate = new Property<float>();
            criticalDamage = new Property<float>();
            movementSpeed = new Property<float>();
            attackRange = new Property<float>();
            scale = new Property<float>();
            healthPointProportion = new Property<float>();
            magicPointProportion = new Property<float>();
            experienceProportion = new Property<float>();
            
            // 无加成变量
            healthRegeneration = new Property<float>(0, DataType.Int);
            magicRegeneration = new Property<float>(0, DataType.Int);
            adaptiveForce = new Property<float>(0, DataType.Int);
            lifeSteal = new Property<float>(0, DataType.Percentage);
            omnivamp = new Property<float>(0, DataType.Percentage);
            percentageAttackPenetration = new Property<float>(0, DataType.Percentage);
            percentageMagicPenetration = new Property<float>(0, DataType.Percentage);

            #endregion
            
            #region 无配置数据初始化
            
            _maxHealthPointBonus = new Property<float>();
            _percentageMaxHealthPointBonus = new Property<float>();
            _maxMagicPointBonus = new Property<float>();
            _percentageMaxMagicPointBonus = new Property<float>();
            _attackSpeedBonus = new Property<float>();
            _percentageAttackSpeedBonus = new Property<float>();
            _attackDamageBonus = new Property<float>();
            _percentageAttackDamageBonus = new Property<float>();
            _abilityPowerBonus = new Property<float>();
            _percentageAbilityPowerBonus = new Property<float>();
            _abilityHasteBonus = new Property<float>();
            _percentageAbilityHasteBonus = new Property<float>();
            _equipmentAbilityHasteBonus = new Property<float>();
            _summonerAbilityHasteBonus = new Property<float>();
            _attackDefenseBonus = new Property<float>();
            _percentageAttackDefenseBonus = new Property<float>();
            _magicDefenseBonus = new Property<float>();
            _percentageMagicDefenseBonus = new Property<float>();
            _attackPenetrationBonus = new Property<float>();
            _percentageAttackPenetrationBonus = new Property<float>();
            _magicPenetrationBonus = new Property<float>();
            _percentageMagicPenetrationBonus = new Property<float>();
            _criticalRateBonus = new Property<float>();
            _percentageCriticalRateBonus = new Property<float>();
            _actualCriticalRate = new Property<float>();
            _criticalDamageBonus = new Property<float>();
            _movementSpeedBonus = new Property<float>();
            _percentageMovementSpeedBonus = new Property<float>();
            _attackRangeBonus = new Property<float>();
            _percentageAttackRangeBonus = new Property<float>();
            _percentageScaleBonus = new Property<float>();
            _percentageHealthRegenerationBonus = new Property<float>();
            _percentageMagicRegenerationBonus = new Property<float>();
            _baseCriticalDamage = 0.75f;
            
            #endregion

            #region 转化率数据初始化
            
            _CRToAD_ConversionEfficiency = new Property<Vector2>(new Vector2(0f, 50f));
            _ADToAP_ConversionEfficiency = new Property<Vector2>(new Vector2(1f, 0f));
            _APToAD_ConversionEfficiency = new Property<Vector2>(new Vector2(1f, 0f));
            _HPToAD_ConversionEfficiency = new Property<Vector2>(new Vector2(1f, 0f));
            _HPToAP_ConversionEfficiency = new Property<Vector2>(new Vector2(1f, 0f));
            _MPToHP_ConversionEfficiency = new Property<Vector2>(new Vector2(1f, 0f));
            _MPToAD_ConversionEfficiency = new Property<Vector2>(new Vector2(1f, 0f));
            _MPToAP_ConversionEfficiency = new Property<Vector2>(new Vector2(1f, 0f));
            _APToMP_ConversionEfficiency = new Property<Vector2>(new Vector2(1f, 0f));
            _DEFToAD_ConversionEfficiency = new Property<Vector2>(new Vector2(1f, 0f));
            
            #endregion
            
            #region 配置计算属性依赖
            
            maxExperience = new Property<float>(() => 5f * level * level + 80f * level + 195,
                DataType.Int,
                level);
            maxHealthPoint = new Property<float>(() => (_baseMaxHealthPoint + _maxHealthPointGrowth * level + _maxHealthPointBonus) * (1 + _percentageMaxHealthPointBonus),
                DataType.Int,
                level, _maxHealthPointBonus, _percentageMaxHealthPointBonus);
            maxMagicPoint = new Property<float>(() => (_baseMaxMagicPoint + _maxMagicPointGrowth * level + _maxMagicPointBonus) * (1 + _percentageMaxMagicPointBonus),
                DataType.Int,
                level, _maxMagicPointBonus, _percentageMaxMagicPointBonus);
            attackDamage = new Property<float>(() => (_baseAttackDamage + _attackDamageGrowth * level + _attackDamageBonus + maxMagicPoint * _MPToAD_ConversionEfficiency.Value.y) * (1 + _percentageAttackDamageBonus),
                DataType.Int,
                level, _attackDamageBonus, _percentageAttackDamageBonus, maxMagicPoint, _MPToAD_ConversionEfficiency);
            abilityPower = new Property<float>(() => (_abilityPowerBonus + maxMagicPoint * _MPToAP_ConversionEfficiency.Value.y) * (1 + _percentageAbilityPowerBonus),
                DataType.Int,
                _abilityPowerBonus, _percentageAbilityPowerBonus, maxMagicPoint, _MPToAP_ConversionEfficiency);
            attackSpeed = new Property<float>(() => (_baseAttackSpeed + _attackSpeedGrowth * level + _attackSpeedBonus * _attackSpeedYield) * (1 + _percentageAttackSpeedBonus),
                DataType.Float,
                level, _attackSpeedBonus, _percentageAttackSpeedBonus);
            abilityHaste = new Property<float>(() => _abilityHasteBonus * (1 + _percentageAbilityHasteBonus),
                DataType.Int,
                _abilityHasteBonus, _percentageAbilityHasteBonus);
            magicDefense = new Property<float>(() => (_baseMagicDefense + _magicDefenseGrowth * level + _magicDefenseBonus) * (1 + _percentageMagicDefenseBonus),
                DataType.Int,
                level, _magicDefenseBonus, _percentageMagicDefenseBonus);
            attackDefense = new Property<float>(() => (_baseAttackDefense + _attackDefenseGrowth * level + _attackDefenseBonus) * (1 + _percentageAttackDefenseBonus),
                DataType.Int,
                level, _attackDefenseBonus, _percentageAttackDefenseBonus);
            attackPenetration = new Property<float>(() => _attackPenetrationBonus * (1 + _percentageAttackPenetrationBonus),
                DataType.Int,
                _attackPenetrationBonus, _percentageAttackPenetrationBonus);
            magicPenetration = new Property<float>(() => _magicPenetrationBonus * (1 + _percentageMagicPenetrationBonus),
                DataType.Int,
                _magicPenetrationBonus, _percentageMagicPenetrationBonus);
            criticalRate = new Property<float>(() => _criticalRateBonus * (1 + _percentageCriticalRateBonus),
                DataType.Percentage,
                _criticalRateBonus, _percentageCriticalRateBonus);
            movementSpeed = new Property<float>(() => (_baseMovementSpeed + _movementSpeedBonus) * (1 + _percentageMovementSpeedBonus),
                DataType.Int,
                _movementSpeedBonus, _percentageMovementSpeedBonus);
            attackRange = new Property<float>(() => (_baseAttackRange + _attackRangeBonus) * (1 + _percentageAttackRangeBonus),
                DataType.Int,
                _attackRangeBonus, _percentageAttackRangeBonus);
            scale = new Property<float>(() => _baseScale * (1 + _percentageScaleBonus),
                DataType.Int,
                _percentageScaleBonus);
            healthRegeneration = new Property<float>(() => (_baseHealthRegeneration + _healthRegenerationGrowth * level ) * (1 + _percentageHealthRegenerationBonus),
                DataType.Float,
                level, _percentageHealthRegenerationBonus);
            magicRegeneration = new Property<float>(() => (_baseMagicRegeneration + _magicRegenerationGrowth * level ) * (1 + _percentageMagicRegenerationBonus),
                DataType.Float,
                level, _percentageMagicRegenerationBonus);
            healthPointProportion = new Property<float>(() => maxHealthPoint == 0 ? 0 : healthPoint / maxHealthPoint,
                 DataType.Percentage,
                 maxHealthPoint, healthPoint);
            magicPointProportion = new Property<float>(() => maxMagicPoint == 0 ? 0 : magicPoint / maxMagicPoint,
                DataType.Percentage,
                maxMagicPoint, magicPoint);
            experienceProportion = new Property<float>(() => maxExperience == 0 ? 0 : experience / maxExperience,
                DataType.Percentage,
                experience, level);
            criticalDamage = new Property<float>(() => _baseCriticalDamage + _criticalDamageBonus.Value,
                DataType.Percentage,
                _criticalDamageBonus, level);

            #endregion

            #region 持有组件初始化
            
            // 配置游戏物体
            _gameObject = gameObject;
            
            // 配置队伍
            _team = team;
            
            // 配置角色寻路组件
            _agent = _gameObject.GetComponent<RVOAgent>();
            _agent.enabled = true;
            _agent.AgentInitialization(this);

            #endregion

            #region 其他属性初始化
            
            isAlive = true;
            forcedDirection = Vector2.zero;
            damageBoost = 1;
            buffList = new List<Buff>();

            #endregion
        }
    }
}