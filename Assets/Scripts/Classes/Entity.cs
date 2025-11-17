using System.Collections;
using System.Collections.Generic;
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
        
        #region 显示属性
        
        /// <summary>
        /// 人物等级
        /// </summary>
        protected float _level;
        /// <summary>
        /// 经验值
        /// </summary>
        protected float _experience;
        /// <summary>
        /// 最大经验值
        /// </summary>
        protected float _maxExperience;
        /// <summary>
        /// 当前生命值
        /// </summary>
        protected float _healthPoint;
        /// <summary>
        /// 最大生命值
        /// </summary>
        protected float _maxHealthPoint;
        /// <summary>
        /// 当前法力值
        /// </summary>
        protected float _magicPoint;
        /// <summary>
        /// 最大法力值
        /// </summary>
        protected float _maxMagicPoint;
        /// <summary>
        /// 攻击速度
        /// </summary>
        protected float _attackSpeed;
        /// <summary>
        /// 攻击力
        /// </summary>
        protected float _attackDamage;
        /// <summary>
        /// 法术强度
        /// </summary>
        protected float _abilityPower;
        /// <summary>
        /// 技能急速
        /// </summary>
        protected float _abilityHaste;
        /// <summary>
        /// 装备技能急速
        /// </summary>
        protected float _equipmentAbilityHaste;
        /// <summary>
        /// 召唤师技能急速
        /// </summary>
        protected float _summonerAbilityHaste;
        /// <summary>
        /// 法术防御
        /// </summary>
        protected float _abilityDefense;
        /// <summary>
        /// 物理防御
        /// </summary>
        protected float _attackDefense;
        /// <summary>
        /// 物理穿透
        /// </summary>
        protected float _attackPenetration;
        /// <summary>
        /// 法术穿透
        /// </summary>
        protected float _abilityPenetration;
        /// <summary>
        /// 百分比物理穿透
        /// </summary>
        protected float _percentageAttackPenetration;
        /// <summary>
        /// 百分比法术穿透
        /// </summary>
        protected float _percentageAbilityPenetration;
        /// <summary>
        /// 暴击率
        /// </summary>
        protected float _criticalRate;
        /// <summary>
        /// 暴击伤害
        /// </summary>
        protected float _criticalDamage;
        /// <summary>
        /// 移动速度
        /// </summary>
        protected float _movementSpeed;
        /// <summary>
        /// 射程
        /// </summary>
        protected float _attackRange;
        #endregion
        
        #region 实际属性
        
        /// <summary>
        /// 实际移动速度
        /// </summary>
        protected float _actualMovementSpeed => _movementSpeed / 100f;
        /// <summary>
        /// 实际射程
        /// </summary>
        protected float _actualAttackRange => _attackRange / 100f;
        /// <summary>
        /// 实际体型
        /// </summary>
        protected float _actualScale => _scale / 100f;
        /// <summary>
        /// 实际冷却缩减
        /// </summary>
        protected float _actualAbilityCooldown => 100f / (_abilityHaste + 100f);
        #endregion
        
        #region 计算属性
        
        //生命值相关
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
        
        //法力值相关
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
        
        //攻速相关
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
        
        //攻击力相关
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
        
        //法强相关
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
        
        //技能急速相关
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
        
        //物抗相关
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
        
        //法抗相关
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
        
        //物穿相关
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
        
        //法穿相关
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
        
        //暴击相关
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
        
        //移速相关
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
        
        //射程相关
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
        
        //体型相关
        /// <summary>
        /// 基础体型
        /// </summary>
        protected float _baseScale;
        /// <summary>
        /// 百分比体型加成
        /// </summary>
        protected float _percentageScaleBonus;
        /// <summary>
        /// 体型
        /// </summary>
        protected float _scale;
        
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
        /// <summary>
        /// 人物等级
        /// </summary>
        public float level => _level;
        /// <summary>
        /// 经验值
        /// </summary>
        public float experience => _experience;
        /// <summary>
        /// 最大经验值
        /// </summary>
        public float maxExperience => _maxExperience;
        /// <summary>
        /// 当前生命值
        /// </summary>
        public float healthPoint => _healthPoint;
        /// <summary>
        /// 最大生命值
        /// </summary>
        public float maxHealthPoint => _maxHealthPoint;
        /// <summary>
        /// 当前法力值
        /// </summary>
        public float magicPoint => _magicPoint;
        /// <summary>
        /// 最大法力值
        /// </summary>
        public float maxMagicPoint => _maxMagicPoint;
        /// <summary>
        /// 攻击速度
        /// </summary>
        public float attackSpeed => _attackSpeed;
        /// <summary>
        /// 攻击力
        /// </summary>
        public float attackDamage => _attackDamage;
        /// <summary>
        /// 法术强度
        /// </summary>
        public float abilityPower => _abilityPower;
        /// <summary>
        /// 技能急速
        /// </summary>
        public float abilityHaste => _abilityHaste;
        /// <summary>
        /// 法术防御
        /// </summary>
        public float abilityDefense => _abilityDefense;
        /// <summary>
        /// 物理防御
        /// </summary>
        public float attackDefense => _attackDefense;
        /// <summary>
        /// 物理穿透
        /// </summary>
        public float attackPenetration => _attackPenetration;
        /// <summary>
        /// 法术穿透
        /// </summary>
        public float abilityPenetration => _abilityPenetration;
        /// <summary>
        /// 百分比物理穿透
        /// </summary>
        public float percentageAttackPenetration => _percentageAttackPenetration;
        /// <summary>
        /// 百分比法术穿透
        /// </summary>
        public float percentageAbilityPenetration => _percentageAbilityPenetration;
        /// <summary>
        /// 暴击率
        /// </summary>
        public float criticalRate => _criticalRate;
        /// <summary>
        /// 暴击伤害
        /// </summary>
        public float criticalDamage => _criticalDamage;
        /// <summary>
        /// 移动速度
        /// </summary>
        public float movementSpeed => _movementSpeed;
        /// <summary>
        /// 射程
        /// </summary>
        public float attackRange => _attackRange;
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
    }
}