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
        /// 持有者实体
        /// </summary>
        public Entity owner;
        /// <summary>
        /// 获取装备事件
        /// </summary>
        private event Action EquipmentGet;

        /// <summary>
        /// 装备属性字典
        /// </summary>
        public Dictionary<AttributeType, float> equipmentAttributes;
        
        public string equipmentName;
        public string _usageDescription;
        public string _equipmentType;
        public int _cost;

        // 技能主动效果
        public string _passiveSkillDescription;
        public string _passiveSkillName;
        private event Action PassiveSkillEffect;

        // 装备主动效果
        public string _activeSkillDescription;
        public string _activeSkillName;
        private event Action ActiveSkillEffective;
        
        public EquipmentUniqueEffect _uniqueEffect;

        public Equipment(string name, Entity entity)
        {
            owner = entity;
            var config = ResourceReader.ReadEquipmentConfig(name);
            equipmentAttributes = ConvertAttributeList(config._attributeList);
            equipmentName = config.equipmentName;
            _usageDescription = config._usageDescription;
            _equipmentType = config._equipmentType;
            _cost = config._cost;
            _passiveSkillDescription = config._passiveSkillDescription;
            _passiveSkillName = config._passiveSkillName;
            _activeSkillDescription = config._activeSkillDescription;
            _activeSkillName = config._activeSkillName;
            _uniqueEffect = config._uniqueEffect;
        }

        private Dictionary<AttributeType, float> ConvertAttributeList(Dictionary<string, float> rawDict)
        {
            var result = new Dictionary<AttributeType, float>();

            if (rawDict == null)
                return result;

            foreach (var kv in rawDict)
            {
                if (Enum.TryParse(kv.Key, true, out AttributeType attr))
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

        public void RegisterOnEquipmentGet(Action callback)
        {
            EquipmentGet += callback;
        }

        public void RegisterOnActiveSkillEffective(Action callback)
        {
            ActiveSkillEffective += callback;
        }

        public void RegisterOnPassiveSkillEffect(Action callback)
        {
            PassiveSkillEffect += callback;
        }

        /// <summary>
        /// 装备被获取时调用
        /// </summary>
        public void OnEquipmentGet()
        {
            EquipmentGet?.Invoke();
        }

        /// <summary>
        /// 主动技能生效时调用
        /// </summary>
        public void OnActiveSkillEffective()
        {
            ActiveSkillEffective?.Invoke();
        }
    }
}
