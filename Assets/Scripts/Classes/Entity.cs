using System;
using UnityEngine;
using MVVM;

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
        
        #region 显示属性
        
        /// <summary>
        /// 人物等级
        /// </summary>
        public BindableProperty<float> level = new BindableProperty<float>();
        /// <summary>
        /// 经验值
        /// </summary>
        public BindableProperty<float> experience = new BindableProperty<float>();
        /// <summary>
        /// 最大经验值
        /// </summary>
        public BindableProperty<float> maxExperience = new BindableProperty<float>();
        /// <summary>
        /// 当前生命值
        /// </summary>
        public BindableProperty<float> healthPoint = new BindableProperty<float>();
        /// <summary>
        /// 最大生命值
        /// </summary>
        public BindableProperty<float> maxHealthPoint = new BindableProperty<float>();
        /// <summary>
        /// 当前法力值
        /// </summary>
        public BindableProperty<float> magicPoint = new BindableProperty<float>();
        /// <summary>
        /// 最大法力值
        /// </summary>
        public BindableProperty<float> maxMagicPoint = new BindableProperty<float>();
        /// <summary>
        /// 攻击速度
        /// </summary>
        public BindableProperty<float> attackSpeed = new BindableProperty<float>();
        /// <summary>
        /// 攻击力
        /// </summary>
        public BindableProperty<float> attackDamage = new BindableProperty<float>();
        /// <summary>
        /// 法术强度
        /// </summary>
        public BindableProperty<float> abilityPower = new BindableProperty<float>();
        /// <summary>
        /// 技能急速
        /// </summary>
        public BindableProperty<float> abilityHaste = new BindableProperty<float>();
        /// <summary>
        /// 装备技能急速
        /// </summary>
        public BindableProperty<float> equipmentAbilityHaste = new BindableProperty<float>();
        /// <summary>
        /// 召唤师技能急速
        /// </summary>
        public BindableProperty<float> summonerAbilityHaste = new BindableProperty<float>();
        /// <summary>
        /// 法术防御
        /// </summary>
        public BindableProperty<float> abilityDefense = new BindableProperty<float>();
        /// <summary>
        /// 物理防御
        /// </summary>
        public BindableProperty<float> attackDefense = new BindableProperty<float>();
        /// <summary>
        /// 物理穿透
        /// </summary>
        public BindableProperty<float> attackPenetration = new BindableProperty<float>();
        /// <summary>
        /// 法术穿透
        /// </summary>
        public BindableProperty<float> abilityPenetration = new BindableProperty<float>();
        /// <summary>
        /// 百分比物理穿透
        /// </summary>
        public BindableProperty<float> percentageAttackPenetration = new BindableProperty<float>();
        /// <summary>
        /// 百分比法术穿透
        /// </summary>
        public BindableProperty<float> percentageAbilityPenetration = new BindableProperty<float>();
        /// <summary>
        /// 暴击率
        /// </summary>
        public BindableProperty<float> criticalRate = new BindableProperty<float>();
        /// <summary>
        /// 暴击伤害
        /// </summary>
        public BindableProperty<float> criticalDamage = new BindableProperty<float>();
        /// <summary>
        /// 移动速度
        /// </summary>
        public BindableProperty<float> movementSpeed = new BindableProperty<float>();
        /// <summary>
        /// 射程
        /// </summary>
        public BindableProperty<float> attackRange = new BindableProperty<float>();
        /// <summary>
        /// 体型
        /// </summary>
        public BindableProperty<float> scale = new BindableProperty<float>();
        /// <summary>
        /// 生命偷取
        /// </summary>
        public BindableProperty<float> lifeSteel = new BindableProperty<float>();
        /// <summary>
        /// 全能吸血
        /// </summary>
        public BindableProperty<float> omniVamp = new BindableProperty<float>();
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
        protected float _baseMaxHealthPoint;
        /// <summary>
        /// 最大生命值加成
        /// </summary>
        protected float _maxHealthPointBonus;
        /// <summary>
        /// 百分比最大生命值加成
        /// </summary>
        protected float _percentageMaxHealthPointBonus;
        
        // 法力值相关
        /// <summary>
        /// 基础法力值
        /// </summary>
        protected float _baseMagicPoint;
        /// <summary>
        /// 法力值加成
        /// </summary>
        protected float _magicPointBonus;
        /// <summary>
        /// 百分比法力值加成
        /// </summary>
        protected float _percentageMagicPointBonus;
        
        // 攻速相关
        /// <summary>
        /// 基础攻击速度
        /// </summary>
        protected float _baseAttackSpeed;
        /// <summary>
        /// 攻击速度加成
        /// </summary>
        protected float _attackSpeedBonus;
        /// <summary>
        /// 百分比攻击速度加成
        /// </summary>
        protected float _percentageAttackSpeedBonus;
        /// <summary>
        /// 攻速收益率
        /// </summary>
        protected float _attackSpeedYield;
        
        // 攻击力相关
        /// <summary>
        /// 基础攻击力
        /// </summary>
        protected float _baseAttackDamage;
        /// <summary>
        /// 攻击力加成
        /// </summary>
        protected float _attackDamageBonus;
        /// <summary>
        /// 百分比攻击力加成
        /// </summary>
        protected float _percentageAttackDamageBonus;
        
        // 法强相关
        /// <summary>
        /// 基础法术强度
        /// </summary>
        protected float _baseAbilityPower;
        /// <summary>
        /// 法术强度加成
        /// </summary>
        protected float _abilityPowerBonus;
        /// <summary>
        /// 百分比法术强度加成
        /// </summary>
        protected float _percentageAbilityPowerBonus;
        
        // 技能急速相关
        /// <summary>
        /// 基础技能急速
        /// </summary>
        protected float _baseAbilityHaste;
        /// <summary>
        /// 技能急速加成
        /// </summary>
        protected float _abilityHasteBonus;
        /// <summary>
        /// 百分比技能急速加成
        /// </summary>
        protected float _percentageAbilityHasteBonus;
        /// <summary>
        /// 基础装备技能急速
        /// </summary>
        protected float _baseEquipmentAbilityHaste;
        /// <summary>
        /// 装备技能急速加成
        /// </summary>
        protected float _equipmentAbilityHasteBonus;
        /// <summary>
        /// 百分比装备技能急速加成
        /// </summary>
        protected float _percentageEquipmentAbilityHasteBonus;
        /// <summary>
        /// 基础召唤师技能急速
        /// </summary>
        protected float _baseSummonerAbilityHaste;
        /// <summary>
        /// 召唤师技能急速加成
        /// </summary>
        protected float _summonerAbilityHasteBonus;
        /// <summary>
        /// 百分比召唤师技能急速加成
        /// </summary>
        protected float _percentageSummonerAbilityHasteBonus;
        
        // 物抗相关
        /// <summary>
        /// 基础物理防御
        /// </summary>
        protected float _baseAttackDefense;
        /// <summary>
        /// 物理防御加成
        /// </summary>
        protected float _attackDefenseBonus;
        /// <summary>
        /// 百分比物理防御加成
        /// </summary>
        protected float _percentageAttackDefenseBonus;
        
        // 法抗相关
        /// <summary>
        /// 基础法术防御
        /// </summary>
        protected float _baseAbilityDefense;
        /// <summary>
        /// 法术防御加成
        /// </summary>
        protected float _abilityDefenseBonus;
        /// <summary>
        /// 百分比法术防御加成
        /// </summary>
        protected float _percentageAbilityDefenseBonus;
        
        // 物穿相关
        /// <summary>
        /// 基础物理穿透
        /// </summary>
        protected float _baseAttackPenetration;
        /// <summary>
        /// 物理穿透加成
        /// </summary>
        protected float _attackPenetrationBonus;
        /// <summary>
        /// 百分比物理穿透
        /// </summary>
        protected float _percentageAttackPenetrationBonus;
        
        // 法穿相关
        /// <summary>
        /// 基础法术穿透
        /// </summary>
        protected float _baseAbilityPenetration;
        /// <summary>
        /// 法术穿透加成
        /// </summary>
        protected float _abilityPenetrationBonus;
        /// <summary>
        /// 百分比法术穿透
        /// </summary>
        protected float _percentageAbilityPenetrationBonus;
        
        // 暴击相关
        /// <summary>
        /// 基础暴击率
        /// </summary>
        protected float _baseCriticalRate;
        /// <summary>
        /// 实际暴击率
        /// </summary>
        protected float _actualCriticalRate;
        /// <summary>
        /// 暴击率加成
        /// </summary>
        protected float _criticalRateBonus;
        /// <summary>
        /// 基础暴击伤害
        /// </summary>
        protected float _baseCriticalDamage;
        /// <summary>
        /// 暴击伤害加成
        /// </summary>
        protected float _criticalDamageBonus;
        
        // 移速相关
        /// <summary>
        /// 基础移动速度
        /// </summary>
        protected float _baseMovementSpeed;
        /// <summary>
        /// 移动速度加成
        /// </summary>
        protected float _movementSpeedBonus;
        /// <summary>
        /// 百分比移动速度加成
        /// </summary>
        protected float _percentageMovementSpeedBonus;
        
        // 射程相关
        /// <summary>
        /// 基础射程
        /// </summary>
        protected float _baseAttackRange;
        /// <summary>
        /// 射程加成
        /// </summary>
        protected float _attackRangeBonus;
        /// <summary>
        /// 百分比射程加成
        /// </summary>
        protected float _percentageAttackRangeBonus;
        
        // 体型相关
        /// <summary>
        /// 基础体型
        /// </summary>
        protected float _baseScale;
        /// <summary>
        /// 百分比体型加成
        /// </summary>
        protected float _percentageScaleBonus;
        
        //TODO:添加适应之力、生命偷取和全能吸血
        
        //转化率相关
        /// <summary>
        /// 暴击率-攻击力转化率
        /// </summary>
        protected float _criticalRateToAttackConversionEfficiency;
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
    }
}