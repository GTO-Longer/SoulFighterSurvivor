using System;
using System.Collections.Generic;
using UnityEngine;
using DataManagement;
using Utilities;

namespace Classes
{
    public class Equipment
    {
        /// <summary>
        /// 装备属性字典
        /// </summary>
        public Dictionary<EquipmentAttributeType, float> equipmentAttributes;

        public Entity owner;

        public string id;
        public string equipmentName;
        public string _usageDescription;
        public EquipmentType _equipmentType;
        public int _cost;
        public Sprite equipmentIcon;

        // 技能主动效果
        protected string _passiveSkillDescription;
        protected string _passiveSkillName;
        public float _passiveSkillCD;
        public float _passiveSkillCDTimer;
        public Property<float> _passiveSkillCDProportion;
        public Property<string> _passiveSkillCDDif;
        protected bool _passiveSkillActive => _passiveSkillCDTimer >= _passiveSkillCD;
        public bool havePassiveSkillCD => _passiveSkillCD > 0; 

        // 装备主动效果
        protected string _activeSkillDescription;
        protected string _activeSkillName;
        public float _activeSkillCD;
        public float _activeSkillCDTimer;
        public Property<float> _activeSkillCDProportion;
        
        public Property<string> _activeSkillCDDif;
        protected Action ActiveSkillEffective;
        protected bool _activeSkillActive => _activeSkillCDTimer >= _activeSkillCD;
        public bool haveActiveSkillCD => _activeSkillCD > 0; 
        
        public EquipmentUniqueEffect _uniqueEffect;
        protected Action<Entity> equipmentTimerUpdate;

        public Equipment(string name)
        {
            var config = ResourceReader.ReadEquipmentConfig(name);
            id = config.id;
            equipmentAttributes = ConvertAttributeList(config._attributeList);
            equipmentName = config.equipmentName;
            _usageDescription = config._usageDescription;
            Enum.TryParse(config._equipmentType, true, out EquipmentType equipmentType);
            _equipmentType = equipmentType;
            _cost = config._cost;
            _passiveSkillDescription = config._passiveSkillDescription;
            _passiveSkillName = config._passiveSkillName;
            _activeSkillDescription = config._activeSkillDescription;
            _activeSkillName = config._activeSkillName;
            _uniqueEffect = config._uniqueEffect;
            _activeSkillCD = config._activeSkillCD;
            _passiveSkillCD = config._passiveSkillCD;
            _passiveSkillCDTimer = _passiveSkillCD;
            _activeSkillCDTimer = _activeSkillCD;
            
            _passiveSkillCDProportion = new Property<float>();
            _activeSkillCDProportion = new Property<float>();
            _passiveSkillCDDif =  new Property<string>();
            _activeSkillCDDif =  new Property<string>();
            
            equipmentIcon = ResourceReader.ReadIcon(id);
            
            equipmentTimerUpdate = (_) =>
            {
                if (!_passiveSkillActive)
                {
                    _passiveSkillCDTimer += Time.deltaTime;
                    _passiveSkillCDProportion.Value = 1 - (_passiveSkillCD == 0 ? 1 :_passiveSkillCDTimer / _passiveSkillCD);
                    var dif = _passiveSkillCD - _passiveSkillCDTimer;
                    _passiveSkillCDDif.Value = dif <= 0.01f ? "" : (dif >= 1 ? $"{dif:F0}" : $"{dif:F1}");
                }
                if (!_activeSkillActive)
                {
                    _activeSkillCDTimer += Time.deltaTime;
                    _activeSkillCDProportion.Value = 1 - (_activeSkillCD == 0 ? 1 :_activeSkillCDTimer / _activeSkillCD);
                    var dif = _activeSkillCD - _activeSkillCDTimer;
                    _activeSkillCDDif.Value = dif <= 0.01f ? "" : (dif >= 1 ? $"{dif:F0}" : $"{dif:F1}");
                }
            };
        }

        private Dictionary<EquipmentAttributeType, float> ConvertAttributeList(Dictionary<string, float> rawDict)
        {
            var result = new Dictionary<EquipmentAttributeType, float>();

            if (rawDict == null)
                return result;

            foreach (var kv in rawDict)
            {
                if (Enum.TryParse(kv.Key, true, out EquipmentAttributeType attr))
                {
                    result[attr] = kv.Value;
                }
                else
                {
                    Debug.LogWarning($"[Equipment] 未识别的属性类型：{kv.Key}");
                }
            }

            return result;
        }

        /// <summary>
        /// 装备被获取时调用
        /// </summary>
        public virtual void OnEquipmentGet(Entity entity)
        {
            owner = entity;
            
            // 将装备属性加入加成
            foreach (var kv in equipmentAttributes)
            {
                // 获取装备属性
                switch (kv.Key)
                {
                    case EquipmentAttributeType.None:Debug.LogError("未找到对应属性！");
                        break;
                    case EquipmentAttributeType.maxHealthPoint:
                        var healthCache1 = owner.maxHealthPoint.Value;
                        owner._maxHealthPointBonus.Value += kv.Value;
                        owner.healthPoint.Value += owner.maxHealthPoint.Value - healthCache1;
                        break;
                    case EquipmentAttributeType.percentageMaxHealthPoint:
                        var healthCache2 = owner.maxHealthPoint.Value;
                        owner._percentageMaxHealthPointBonus.Value += kv.Value;
                        owner.healthPoint.Value += owner.maxHealthPoint.Value - healthCache2;
                        break;
                    case EquipmentAttributeType.maxMagicPoint:
                        var magicCache1 = owner.maxMagicPoint.Value;
                        owner._maxMagicPointBonus.Value += kv.Value;
                        owner.magicPoint.Value += owner.maxMagicPoint.Value - magicCache1;
                        break;
                    case EquipmentAttributeType.percentageMaxMagicPoint:
                        var magicCache2 = owner.maxMagicPoint.Value;
                        owner._percentageMaxMagicPointBonus.Value += kv.Value;
                        owner.magicPoint.Value += owner.maxMagicPoint.Value - magicCache2;
                        break;
                    case EquipmentAttributeType.attackSpeed:
                        owner._attackSpeedBonus.Value += kv.Value;
                        break;
                    case EquipmentAttributeType.percentageAttackSpeed:
                        owner._percentageAttackSpeedBonus.Value += kv.Value;
                        break;
                    case EquipmentAttributeType.attackDamage:
                        owner._attackDamageBonus.Value += kv.Value;
                        break;
                    case EquipmentAttributeType.percentageAttackDamage:
                        owner._percentageAttackDamageBonus.Value += kv.Value;
                        break;
                    case EquipmentAttributeType.abilityPower:
                        owner._abilityPowerBonus.Value += kv.Value;
                        break;
                    case EquipmentAttributeType.percentageAbilityPower:
                        owner._percentageAbilityPowerBonus.Value += kv.Value;
                        break;
                    case EquipmentAttributeType.abilityHaste:
                        owner._abilityHasteBonus.Value += kv.Value;
                        break;
                    case EquipmentAttributeType.percentageAbilityHaste:
                        owner._percentageAbilityHasteBonus.Value += kv.Value;
                        break;
                    case EquipmentAttributeType.attackDefense:
                        owner._attackDefenseBonus.Value += kv.Value;
                        break;
                    case EquipmentAttributeType.percentageAttackDefense:
                        owner._percentageAttackDefenseBonus.Value += kv.Value;
                        break;
                    case EquipmentAttributeType.magicDefense:
                        owner._magicDefenseBonus.Value += kv.Value;
                        break;
                    case EquipmentAttributeType.percentageMagicDefense:
                        owner._percentageMagicDefenseBonus.Value += kv.Value;
                        break;
                    case EquipmentAttributeType.attackPenetration:
                        owner._attackPenetrationBonus.Value += kv.Value;
                        break;
                    case EquipmentAttributeType.percentageAttackPenetration:
                        owner._percentageAttackPenetrationBonus.Value += kv.Value;
                        break;
                    case EquipmentAttributeType.magicPenetration:
                        owner._magicPenetrationBonus.Value += kv.Value;
                        break;
                    case EquipmentAttributeType.percentageMagicPenetration:
                        owner._percentageMagicPenetrationBonus.Value += kv.Value;
                        break;
                    case EquipmentAttributeType.criticalRate:
                        owner._criticalRateBonus.Value += kv.Value;
                        break;
                    case EquipmentAttributeType.criticalDamage:
                        owner._criticalDamageBonus.Value += kv.Value;
                        break;
                    case EquipmentAttributeType.movementSpeed:
                        owner._movementSpeedBonus.Value += kv.Value;
                        break;
                    case EquipmentAttributeType.percentageMovementSpeed:
                        owner._percentageMovementSpeedBonus.Value += kv.Value;
                        break;
                    case EquipmentAttributeType.percentageHealthRegeneration:
                        owner._percentageHealthRegenerationBonus.Value += kv.Value;
                        break;
                    case EquipmentAttributeType.percentageMagicRegeneration:
                        owner._percentageMagicRegenerationBonus.Value += kv.Value;
                        break;
                    case EquipmentAttributeType.omnivamp:
                        owner.omnivamp.Value += kv.Value;
                        break;
                    case EquipmentAttributeType.lifeSteal:
                        owner.lifeSteal.Value += kv.Value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// 装备被移除时调用
        /// </summary>
        public virtual void OnEquipmentRemove()
        {
            if (owner == null) return;
            
            // 注销装备被动能力
            foreach (var kv in equipmentAttributes)
            {
                // 移除装备属性
                switch (kv.Key)
                {
                    case EquipmentAttributeType.None:Debug.LogError("未找到对应属性！");
                        break;
                    case EquipmentAttributeType.maxHealthPoint:
                        owner._maxHealthPointBonus.Value -= kv.Value;
                        break;
                    case EquipmentAttributeType.percentageMaxHealthPoint:
                        owner._percentageMaxHealthPointBonus.Value -= kv.Value;
                        break;
                    case EquipmentAttributeType.maxMagicPoint:
                        owner._maxMagicPointBonus.Value -= kv.Value;
                        break;
                    case EquipmentAttributeType.percentageMaxMagicPoint:
                        owner._percentageMaxMagicPointBonus.Value -= kv.Value;
                        break;
                    case EquipmentAttributeType.attackSpeed:
                        owner._attackSpeedBonus.Value -= kv.Value;
                        break;
                    case EquipmentAttributeType.percentageAttackSpeed:
                        owner._percentageAttackSpeedBonus.Value -= kv.Value;
                        break;
                    case EquipmentAttributeType.attackDamage:
                        owner._attackDamageBonus.Value -= kv.Value;
                        break;
                    case EquipmentAttributeType.percentageAttackDamage:
                        owner._percentageAttackDamageBonus.Value -= kv.Value;
                        break;
                    case EquipmentAttributeType.abilityPower:
                        owner._abilityPowerBonus.Value -= kv.Value;
                        break;
                    case EquipmentAttributeType.percentageAbilityPower:
                        owner._percentageAbilityPowerBonus.Value -= kv.Value;
                        break;
                    case EquipmentAttributeType.abilityHaste:
                        owner._abilityHasteBonus.Value -= kv.Value;
                        break;
                    case EquipmentAttributeType.percentageAbilityHaste:
                        owner._percentageAbilityHasteBonus.Value -= kv.Value;
                        break;
                    case EquipmentAttributeType.attackDefense:
                        owner._attackDefenseBonus.Value -= kv.Value;
                        break;
                    case EquipmentAttributeType.percentageAttackDefense:
                        owner._percentageAttackDefenseBonus.Value -= kv.Value;
                        break;
                    case EquipmentAttributeType.magicDefense:
                        owner._magicDefenseBonus.Value -= kv.Value;
                        break;
                    case EquipmentAttributeType.percentageMagicDefense:
                        owner._percentageMagicDefenseBonus.Value -= kv.Value;
                        break;
                    case EquipmentAttributeType.attackPenetration:
                        owner._attackPenetrationBonus.Value -= kv.Value;
                        break;
                    case EquipmentAttributeType.percentageAttackPenetration:
                        owner._percentageAttackPenetrationBonus.Value -= kv.Value;
                        break;
                    case EquipmentAttributeType.magicPenetration:
                        owner._magicPenetrationBonus.Value -= kv.Value;
                        break;
                    case EquipmentAttributeType.percentageMagicPenetration:
                        owner._percentageMagicPenetrationBonus.Value -= kv.Value;
                        break;
                    case EquipmentAttributeType.criticalRate:
                        owner._criticalRateBonus.Value -= kv.Value;
                        break;
                    case EquipmentAttributeType.criticalDamage:
                        owner._criticalDamageBonus.Value -= kv.Value;
                        break;
                    case EquipmentAttributeType.movementSpeed:
                        owner._movementSpeedBonus.Value -= kv.Value;
                        break;
                    case EquipmentAttributeType.percentageMovementSpeed:
                        owner._percentageMovementSpeedBonus.Value -= kv.Value;
                        break;
                    case EquipmentAttributeType.percentageHealthRegeneration:
                        owner._percentageHealthRegenerationBonus.Value -= kv.Value;
                        break;
                    case EquipmentAttributeType.percentageMagicRegeneration:
                        owner._percentageMagicRegenerationBonus.Value -= kv.Value;
                        break;
                    case EquipmentAttributeType.omnivamp:
                        owner.omnivamp.Value -= kv.Value;
                        break;
                    case EquipmentAttributeType.lifeSteal:
                        owner.lifeSteal.Value -= kv.Value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            owner = null;
        }

        /// <summary>
        /// 主动技能生效时调用
        /// </summary>
        public void OnActiveSkillEffective()
        {
            ActiveSkillEffective?.Invoke();
        }

        public virtual bool GetPassiveSkillDescription(out string description)
        {
            description = "";
            return false;
        }

        public virtual bool GetActiveSkillDescription(out string description)
        {
            description = "";
            return false;
        }
    }
}
