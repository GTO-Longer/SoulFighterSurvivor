using System;
using DataManagement;
using UnityEngine;

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
        public Property<float> abilityDefense;
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
        public Property<float> abilityPenetration;
        /// <summary>
        /// 百分比物理穿透
        /// </summary>
        public Property<float> percentageAttackPenetration;
        /// <summary>
        /// 百分比法术穿透
        /// </summary>
        public Property<float> percentageAbilityPenetration;
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
        public Property<float> lifeSteel;
        /// <summary>
        /// 全能吸血
        /// </summary>
        public Property<float> omniVamp;
        
        #endregion
        
        #region 实际属性
        
        /// <summary>
        /// 实际移动速度
        /// </summary>
        protected float _actualMovementSpeed => movementSpeed.Value / 100f;
        /// <summary>
        /// 实际射程
        /// </summary>
        protected float _actualAttackRange => attackRange.Value / 100f;
        /// <summary>
        /// 实际体型
        /// </summary>
        protected float _actualScale => scale.Value / 100f;
        /// <summary>
        /// 实际冷却缩减
        /// </summary>
        protected float _actualAbilityCooldown => 100f / (abilityHaste.Value + 100f);
        #endregion
        
        #region 计算属性
        
        // 生命值相关
        /// <summary>
        /// 基础最大生命值
        /// </summary>
        protected Property<float> _baseMaxHealthPoint;
        /// <summary>
        /// 最大生命值加成
        /// </summary>
        protected Property<float> _maxHealthPointBonus;
        /// <summary>
        /// 百分比最大生命值加成
        /// </summary>
        protected Property<float> _percentageMaxHealthPointBonus;
        
        // 法力值相关
        /// <summary>
        /// 基础法力值
        /// </summary>
        protected Property<float> _baseMagicPoint;
        /// <summary>
        /// 法力值加成
        /// </summary>
        protected Property<float> _maxMagicPointBonus;
        /// <summary>
        /// 百分比法力值加成
        /// </summary>
        protected Property<float> _percentageMaxMagicPointBonus;
        
        // 攻速相关
        /// <summary>
        /// 基础攻击速度
        /// </summary>
        protected Property<float> _baseAttackSpeed;
        /// <summary>
        /// 攻击速度加成
        /// </summary>
        protected Property<float> _attackSpeedBonus;
        /// <summary>
        /// 百分比攻击速度加成
        /// </summary>
        protected Property<float> _percentageAttackSpeedBonus;
        /// <summary>
        /// 攻速收益率
        /// </summary>
        protected Property<float> _attackSpeedYield;
        
        // 攻击力相关
        /// <summary>
        /// 基础攻击力
        /// </summary>
        protected Property<float> _baseAttackDamage;
        /// <summary>
        /// 攻击力加成
        /// </summary>
        protected Property<float> _attackDamageBonus;
        /// <summary>
        /// 百分比攻击力加成
        /// </summary>
        protected Property<float> _percentageAttackDamageBonus;
        
        // 法强相关
        /// <summary>
        /// 基础法术强度
        /// </summary>
        protected Property<float> _baseAbilityPower;
        /// <summary>
        /// 法术强度加成
        /// </summary>
        protected Property<float> _abilityPowerBonus;
        /// <summary>
        /// 百分比法术强度加成
        /// </summary>
        protected Property<float> _percentageAbilityPowerBonus;
        
        // 技能急速相关
        /// <summary>
        /// 基础技能急速
        /// </summary>
        protected Property<float> _baseAbilityHaste;
        /// <summary>
        /// 技能急速加成
        /// </summary>
        protected Property<float> _abilityHasteBonus;
        /// <summary>
        /// 百分比技能急速加成
        /// </summary>
        protected Property<float> _percentageAbilityHasteBonus;
        /// <summary>
        /// 基础装备技能急速
        /// </summary>
        protected Property<float> _baseEquipmentAbilityHaste;
        /// <summary>
        /// 装备技能急速加成
        /// </summary>
        protected Property<float> _equipmentAbilityHasteBonus;
        /// <summary>
        /// 基础召唤师技能急速
        /// </summary>
        protected Property<float> _baseSummonerAbilityHaste;
        /// <summary>
        /// 召唤师技能急速加成
        /// </summary>
        protected Property<float> _summonerAbilityHasteBonus;
        
        // 物抗相关
        /// <summary>
        /// 基础物理防御
        /// </summary>
        protected Property<float> _baseAttackDefense;
        /// <summary>
        /// 物理防御加成
        /// </summary>
        protected Property<float> _attackDefenseBonus;
        /// <summary>
        /// 百分比物理防御加成
        /// </summary>
        protected Property<float> _percentageAttackDefenseBonus;
        
        // 法抗相关
        /// <summary>
        /// 基础法术防御
        /// </summary>
        protected Property<float> _baseAbilityDefense;
        /// <summary>
        /// 法术防御加成
        /// </summary>
        protected Property<float> _abilityDefenseBonus;
        /// <summary>
        /// 百分比法术防御加成
        /// </summary>
        protected Property<float> _percentageAbilityDefenseBonus;
        
        // 物穿相关
        /// <summary>
        /// 基础物理穿透
        /// </summary>
        protected Property<float> _baseAttackPenetration;
        /// <summary>
        /// 物理穿透加成
        /// </summary>
        protected Property<float> _attackPenetrationBonus;
        /// <summary>
        /// 百分比物理穿透
        /// </summary>
        protected Property<float> _percentageAttackPenetrationBonus;
        
        // 法穿相关
        /// <summary>
        /// 基础法术穿透
        /// </summary>
        protected Property<float> _baseAbilityPenetration;
        /// <summary>
        /// 法术穿透加成
        /// </summary>
        protected Property<float> _abilityPenetrationBonus;
        /// <summary>
        /// 百分比法术穿透
        /// </summary>
        protected Property<float> _percentageAbilityPenetrationBonus;
        
        // 暴击相关
        /// <summary>
        /// 基础暴击率
        /// </summary>
        protected Property<float> _baseCriticalRate;
        /// <summary>
        /// 实际暴击率
        /// </summary>
        protected Property<float> _actualCriticalRate;
        /// <summary>
        /// 暴击率加成
        /// </summary>
        protected Property<float> _criticalRateBonus;
        /// <summary>
        /// 基础暴击伤害
        /// </summary>
        protected Property<float> _baseCriticalDamage;
        /// <summary>
        /// 暴击伤害加成
        /// </summary>
        protected Property<float> _criticalDamageBonus;
        
        // 移速相关
        /// <summary>
        /// 基础移动速度
        /// </summary>
        protected Property<float> _baseMovementSpeed;
        /// <summary>
        /// 移动速度加成
        /// </summary>
        protected Property<float> _movementSpeedBonus;
        /// <summary>
        /// 百分比移动速度加成
        /// </summary>
        protected Property<float> _percentageMovementSpeedBonus;
        
        // 射程相关
        /// <summary>
        /// 基础射程
        /// </summary>
        protected Property<float> _baseAttackRange;
        /// <summary>
        /// 射程加成
        /// </summary>
        protected Property<float> _attackRangeBonus;
        /// <summary>
        /// 百分比射程加成
        /// </summary>
        protected Property<float> _percentageAttackRangeBonus;
        
        // 体型加成
        /// <summary>
        /// 基础体型
        /// </summary>
        protected Property<float> _baseScale;
        /// <summary>
        /// 百分比体型加成
        /// </summary>
        protected Property<float> _percentageScaleBonus;
        
        // 无加成的其他数据
        /// <summary>
        /// 适应之力
        /// </summary>
        protected Property<float> _adaptiveForce;
        /// <summary>
        /// 生命偷取
        /// </summary>
        protected Property<float> _lifeSteel;
        /// <summary>
        /// 全能吸血
        /// </summary>
        protected Property<float> _omniVamp;
        
        // 转化率相关
        /// <summary>
        /// 溢出暴击率-攻击力加成 转化率
        /// - <c>Value.x</c>: 剩余的溢出暴击率要乘的系数  
        /// - <c>Value.y</c>: 增加的攻击力加成要乘的系数
        /// </summary>
        protected Property<Vector2> _CRToAD_ConversionEfficiency;
        /// <summary>
        /// 攻击力加成-法强加成 转化率
        /// - <c>Value.x</c>: 剩余的攻击力加成要乘的系数  
        /// - <c>Value.y</c>: 增加的法强加成要乘的系数
        /// </summary>
        protected Property<Vector2> _ADToAP_ConversionEfficiency;
        /// <summary>
        /// 法强加成-攻击力加成 转化率
        /// - <c>Value.x</c>: 剩余的法强加成要乘的系数  
        /// - <c>Value.y</c>: 增加的攻击力加成要乘的系数
        /// </summary>
        protected Property<Vector2> _APToAD_ConversionEfficiency;
        /// <summary>
        /// 生命值加成-攻击力加成 转化率
        /// - <c>Value.x</c>: 剩余的生命值加成要乘的系数  
        /// - <c>Value.y</c>: 增加的攻击力加成要乘的系数
        /// </summary>
        protected Property<Vector2> _HPToAD_ConversionEfficiency;
        /// <summary>
        /// 生命值加成-法强加成 转化率
        /// - <c>Value.x</c>: 剩余的生命值加成要乘的系数  
        /// - <c>Value.y</c>: 增加的法强加成要乘的系数
        /// </summary>
        protected Property<Vector2> _HPToAP_ConversionEfficiency;
        /// <summary>
        /// 法力值加成-生命值加成 转化率
        /// - <c>Value.x</c>: 剩余的法力值加成要乘的系数  
        /// - <c>Value.y</c>: 增加的生命值加成要乘的系数
        /// </summary>
        protected Property<Vector2> _MPToHP_ConversionEfficiency;
        /// <summary>
        /// 法力值加成-攻击力加成 转化率
        /// - <c>Value.x</c>: 剩余的法力值加成要乘的系数  
        /// - <c>Value.y</c>: 增加的攻击力加成要乘的系数
        /// </summary>
        protected Property<Vector2> _MPToAD_ConversionEfficiency;
        /// <summary>
        /// 法力值加成-法强加成 转化率
        /// - <c>Value.x</c>: 剩余的法力值加成要乘的系数  
        /// - <c>Value.y</c>: 增加的法强加成要乘的系数
        /// </summary>
        protected Property<Vector2> _MPToAP_ConversionEfficiency;
        /// <summary>
        /// 法强加成-法力值加成 转化率
        /// - <c>Value.x</c>: 剩余的法强加成要乘的系数  
        /// - <c>Value.y</c>: 增加的法力值加成要乘的系数
        /// </summary>
        protected Property<Vector2> _APToMP_ConversionEfficiency;
        /// <summary>
        /// 双抗加成-攻击力加成 转化率
        /// - <c>Value.x</c>: 剩余的双抗加成要乘的系数  
        /// - <c>Value.y</c>: 增加的攻击力加成要乘的系数
        /// </summary>
        protected Property<Vector2> _DEFToAD_ConversionEfficiency;
        #endregion
        
        #region 获取封装属性
        
        /// <summary>
        /// 对应的游戏物体
        /// </summary>
        public GameObject gameObject => _gameObject;
        
        //实际数据
        /// <summary>
        /// 实际移动速度
        /// </summary>
        public float actualMovementSpeed => _actualMovementSpeed;
        /// <summary>
        /// 实际射程
        /// </summary>
        public float actualAttackRange => _actualAttackRange;
        /// <summary>
        /// 实际体型
        /// </summary>
        public float actualScale => _actualScale;
        #endregion
        
        #region 行为函数

        /// <summary>
        /// 受到伤害
        /// </summary>
        public virtual void TakeDamage(){}
        
        /// <summary>
        /// 造成伤害
        /// </summary>
        public virtual void DealDamage(){}
        
        /// <summary>
        /// 移动
        /// </summary>
        public virtual void Move(){}
        
        /// <summary>
        /// 攻击
        /// </summary>
        public virtual void Attack(){}
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

        protected Entity()
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
            abilityDefense = new Property<float>();
            attackDefense = new Property<float>();
            attackPenetration = new Property<float>();
            abilityPenetration = new Property<float>();
            percentageAttackPenetration = new Property<float>();
            percentageAbilityPenetration = new Property<float>();
            criticalRate = new Property<float>();
            criticalDamage = new Property<float>();
            movementSpeed = new Property<float>();
            attackRange = new Property<float>();
            scale = new Property<float>();
            lifeSteel = new Property<float>();
            omniVamp = new Property<float>();

            #endregion
            
            #region 无配置数据初始化
            
            _maxHealthPointBonus = new Property<float>(0);
            _percentageMaxHealthPointBonus = new Property<float>(0);
            _maxMagicPointBonus = new Property<float>(0);
            _percentageMaxMagicPointBonus = new Property<float>(0);
            _attackSpeedBonus = new Property<float>(0);
            _percentageAttackSpeedBonus = new Property<float>(0);
            _attackDamageBonus = new Property<float>(0);
            _percentageAttackDamageBonus = new Property<float>(0);
            _abilityPowerBonus = new Property<float>(0);
            _percentageAbilityPowerBonus = new Property<float>(0);
            _baseAbilityHaste = new Property<float>(0);
            _abilityHasteBonus = new Property<float>(0);
            _percentageAbilityHasteBonus = new Property<float>(0);
            _baseEquipmentAbilityHaste = new Property<float>(0);
            _equipmentAbilityHasteBonus = new Property<float>(0);
            _baseSummonerAbilityHaste = new Property<float>(0);
            _summonerAbilityHasteBonus = new Property<float>(0);
            _attackDefenseBonus = new Property<float>(0);
            _percentageAttackDefenseBonus = new Property<float>(0);
            _abilityDefenseBonus = new Property<float>(0);
            _percentageAbilityDefenseBonus = new Property<float>(0);
            _baseAttackPenetration = new Property<float>(0);
            _attackPenetrationBonus = new Property<float>(0);
            _percentageAttackPenetrationBonus = new Property<float>(0);
            _baseAbilityPenetration = new Property<float>(0);
            _abilityPenetrationBonus = new Property<float>(0);
            _percentageAbilityPenetrationBonus = new Property<float>(0);
            _baseCriticalRate = new Property<float>(0);
            _criticalRateBonus = new Property<float>(0);
            _actualCriticalRate = new Property<float>(0);
            _baseCriticalDamage = new Property<float>(0);
            _criticalDamageBonus = new Property<float>(0);
            _movementSpeedBonus = new Property<float>(0);
            _percentageMovementSpeedBonus = new Property<float>(0);
            _attackRangeBonus = new Property<float>(0);
            _percentageAttackRangeBonus = new Property<float>(0);
            _percentageScaleBonus = new Property<float>(0);
            _adaptiveForce = new Property<float>(0);
            _lifeSteel = new Property<float>(0);
            _omniVamp = new Property<float>(0);
            
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
                level);

            #endregion
        }
    }
}