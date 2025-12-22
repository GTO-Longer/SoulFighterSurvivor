using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace Classes.Buffs
{
    public class AttributeAnvilBonus : Buff
    {
        public static AttributeAnvilBonus Instance;

        private Dictionary<EquipmentAttributeType, float> attributes = new()
        {
            { EquipmentAttributeType.attackDamage, 0 },
            { EquipmentAttributeType.abilityPower, 0 },
            { EquipmentAttributeType.attackPenetration, 0 },
            { EquipmentAttributeType.magicPenetration, 0 },
            { EquipmentAttributeType.attackSpeed, 0 },
            { EquipmentAttributeType.criticalRate, 0 },
            { EquipmentAttributeType.abilityHaste, 0 },
            { EquipmentAttributeType.maxHealthPoint, 0 },
            { EquipmentAttributeType.attackDefense, 0 },
            { EquipmentAttributeType.magicDefense, 0 },
            { EquipmentAttributeType.percentageMovementSpeed, 0 },
            { EquipmentAttributeType.omnivamp, 0 },
            { EquipmentAttributeType.percentageMaxHealthPoint, 0 },
            { EquipmentAttributeType.percentageAttackPenetration, 0 },
            { EquipmentAttributeType.percentageMagicPenetration, 0 },
            { EquipmentAttributeType.criticalDamage, 0 },
            { EquipmentAttributeType.fortune, 0 },
            { EquipmentAttributeType.percentageScaleBonus, 0 },
        };

        private string content
        {
            get
            {
                var str = "";
                foreach (var kv in attributes)
                {
                    switch (kv.Key)
                    {
                        case EquipmentAttributeType.None:Debug.LogError("未找到对应属性！");break;
                        case EquipmentAttributeType.maxHealthPoint: 
                            str += $"+<sprite=\"Attributes\" name=\"MaxHealthPointIcon\">{kv.Value:0.#}最大生命值\n";
                            break;
                        case EquipmentAttributeType.percentageMaxHealthPoint: 
                            str += $"+<sprite=\"Attributes\" name=\"MaxHealthPointIcon\">{kv.Value:P1}最大生命值\n";
                            break;
                        case EquipmentAttributeType.attackSpeed:
                            str += $"+<sprite=\"Attributes\" name=\"AttackSpeedIcon\">{kv.Value:P1}攻击速度\n";
                            break;
                        case EquipmentAttributeType.attackDamage:
                            str += $"+<sprite=\"Attributes\" name=\"AttackDamageIcon\">{kv.Value:0.#}攻击力\n";
                            break;
                        case EquipmentAttributeType.abilityPower:
                            str += $"+<sprite=\"Attributes\" name=\"AbilityPowerIcon\">{kv.Value:0.#}法术强度\n";
                            break;
                        case EquipmentAttributeType.abilityHaste:
                            str += $"+<sprite=\"Attributes\" name=\"AbilityHasteIcon\">{kv.Value:0.#}技能急速\n";
                            break;
                        case EquipmentAttributeType.attackDefense:
                            str += $"+<sprite=\"Attributes\" name=\"AttackDefenseIcon\">{kv.Value:0.#}物理防御\n";
                            break;
                        case EquipmentAttributeType.magicDefense:
                            str += $"+<sprite=\"Attributes\" name=\"MagicDefenseIcon\">{kv.Value:0.#}法术防御\n";
                            break;
                        case EquipmentAttributeType.attackPenetration:
                            str += $"+<sprite=\"Attributes\" name=\"AttackPenetrationIcon\">{kv.Value:0.#}物理穿透\n";
                            break;
                        case EquipmentAttributeType.percentageAttackPenetration:
                            str += $"+<sprite=\"Attributes\" name=\"AttackPenetrationIcon\">{kv.Value:P1}物理穿透\n";
                            break;
                        case EquipmentAttributeType.magicPenetration:
                            str += $"+<sprite=\"Attributes\" name=\"MagicPenetrationIcon\">{kv.Value:0.#}法术穿透\n";
                            break;
                        case EquipmentAttributeType.percentageMagicPenetration:
                            str += $"+<sprite=\"Attributes\" name=\"MagicPenetrationIcon\">{kv.Value:P1}法术穿透\n";
                            break;
                        case EquipmentAttributeType.criticalRate:
                            str += $"+<sprite=\"Attributes\" name=\"CriticalRateIcon\">{kv.Value:P1}暴击率\n";
                            break;
                        case EquipmentAttributeType.criticalDamage:
                            str += $"+<sprite=\"Attributes\" name=\"CriticalDamageIcon\">{kv.Value:P1}暴击伤害\n";
                            break;
                        case EquipmentAttributeType.omnivamp:
                            str += $"+<sprite=\"Attributes\" name=\"OmniVampIcon\">{kv.Value:P1}全能吸血\n";
                            break;
                        case EquipmentAttributeType.percentageMovementSpeed:
                            str += $"+<sprite=\"Attributes\" name=\"MovementSpeedIcon\">{kv.Value:P1}移动速度\n";
                            break;
                        case EquipmentAttributeType.fortune:
                            str += $"+  <sprite=\"Coin\" index=0>{kv.Value:P0}击杀敌人金币获取量\n";
                            break;
                        case EquipmentAttributeType.percentageScaleBonus:
                            str += (kv.Value > 0 ? "+" : "") + $"{kv.Value:P0}体型\n";
                            break;
                        default:
                            Debug.LogWarning($"{kv.Key}缺少属性");
                            break;
                    }
                }
                return str;
            }
        }

        public AttributeAnvilBonus(Entity ownerEntity, Entity sourceEntity) : base(ownerEntity, sourceEntity, "属性增幅", "", 9999, -1)
        {
            OnBuffGet = () =>
            {
                owner.EntityUpdateEvent += (_) =>
                {
                    buffDescription = content;
                };
            };
            
            OnBuffRunOut = () => 
            {
                
            };
        }

        public void AddAttribute(EquipmentAttributeType attributeType, float value)
        {
            attributes[attributeType] += value;
            
            // 获取对应属性
            switch (attributeType)
            {
                case EquipmentAttributeType.None:Debug.LogError("未找到对应属性！");
                    break;
                case EquipmentAttributeType.maxHealthPoint:
                    var healthCache1 = owner.maxHealthPoint.Value;
                    owner._maxHealthPointBonus.Value += value;
                    owner.healthPoint.Value += owner.maxHealthPoint.Value - healthCache1;
                    break;
                case EquipmentAttributeType.percentageMaxHealthPoint:
                    var healthCache2 = owner.maxHealthPoint.Value;
                    owner._percentageMaxHealthPointBonus.Value += value;
                    owner.healthPoint.Value += owner.maxHealthPoint.Value - healthCache2;
                    break;
                case EquipmentAttributeType.attackSpeed:
                    owner._attackSpeedBonus.Value += value;
                    break;
                case EquipmentAttributeType.attackDamage:
                    owner._attackDamageBonus.Value += value;
                    break;
                case EquipmentAttributeType.abilityPower:
                    owner._abilityPowerBonus.Value += value;
                    break;
                case EquipmentAttributeType.abilityHaste:
                    owner._abilityHasteBonus.Value += value;
                    break;
                case EquipmentAttributeType.attackDefense:
                    owner._attackDefenseBonus.Value += value;
                    break;
                case EquipmentAttributeType.magicDefense:
                    owner._magicDefenseBonus.Value += value;
                    break;
                case EquipmentAttributeType.attackPenetration:
                    owner._attackPenetrationBonus.Value += value;
                    break;
                case EquipmentAttributeType.magicPenetration:
                    owner._magicPenetrationBonus.Value += value;
                    break;
                case EquipmentAttributeType.percentageAttackPenetration:
                    owner._percentageAttackPenetrationBonus.Value += value;
                    break;
                case EquipmentAttributeType.percentageMagicPenetration:
                    owner._percentageMagicPenetrationBonus.Value += value;
                    break;
                case EquipmentAttributeType.criticalRate:
                    owner._criticalRateBonus.Value += value;
                    break;
                case EquipmentAttributeType.criticalDamage:
                    owner._criticalDamageBonus.Value += value;
                    break;
                case EquipmentAttributeType.omnivamp:
                    owner.omnivamp.Value += value;
                    break;
                case EquipmentAttributeType.percentageMovementSpeed:
                    owner._percentageMovementSpeedBonus.Value += value;
                    break;
                case EquipmentAttributeType.percentageScaleBonus:
                    owner._percentageScaleBonus.Value += value;
                    break;
                case EquipmentAttributeType.fortune:
                    owner.fortune += value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}