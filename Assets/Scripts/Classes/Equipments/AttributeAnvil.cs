using System.Collections.Generic;
using Classes.Buffs;
using Managers.EntityManagers;
using Systems;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

namespace Classes.Equipments
{
    public class AttributeAnvil : Equipment
    {
        public Dictionary<string, Dictionary<EquipmentAttributeType, float>> attributeChoices = new()
        {
            {"力量碎片", new () {{EquipmentAttributeType.attackDamage, 25}, {EquipmentAttributeType.abilityPower, 25}}},
            {"攻击力碎片", new () {{EquipmentAttributeType.attackDamage, 35}}},
            {"法术强度碎片", new () {{EquipmentAttributeType.abilityPower, 50}}},
            {"物理穿透碎片", new () {{EquipmentAttributeType.attackPenetration, 20}}},
            {"法术穿透碎片", new () {{EquipmentAttributeType.magicPenetration, 18}}},
            {"攻击速度碎片", new () {{EquipmentAttributeType.attackSpeed, 0.35f}}},
            {"暴击碎片", new () {{EquipmentAttributeType.criticalRate, 0.25f}}},
            {"技能急速碎片", new () {{EquipmentAttributeType.abilityHaste, 35}}},
            {"生命值碎片", new () {{EquipmentAttributeType.maxHealthPoint, 375}}},
            {"坚不可摧碎片", new () {{EquipmentAttributeType.attackDefense, 30}, {EquipmentAttributeType.magicDefense, 30}}},
            {"物理防御碎片", new () {{EquipmentAttributeType.attackDefense, 45}}},
            {"法术防御碎片", new () {{EquipmentAttributeType.magicDefense, 25}}},
            {"迅捷碎片", new () {{EquipmentAttributeType.attackSpeed, 0.25f}, {EquipmentAttributeType.abilityHaste, 20}}},
        };
        
        public Dictionary<string, Dictionary<EquipmentAttributeType, float>> prismaticChoices = new()
        {
            {"移动速度碎片", new () {{EquipmentAttributeType.percentageMovementSpeed, 0.18f}, {EquipmentAttributeType.percentageScaleBonus, -0.1f}}},
            {"生命值与体型碎片", new () {{EquipmentAttributeType.percentageMaxHealthPoint, 0.15f}, {EquipmentAttributeType.percentageScaleBonus, 0.1f}}},
            {"全能吸血碎片", new () {{EquipmentAttributeType.omnivamp, 0.25f}}},
            {"物理穿透碎片", new () {{EquipmentAttributeType.percentageAttackPenetration, 0.175f}}},
            {"法术穿透碎片", new () {{EquipmentAttributeType.percentageMagicPenetration, 0.175f}}},
            {"暴击伤害碎片", new () {{EquipmentAttributeType.criticalDamage, 0.25f}}},
            {"好运碎片", new () {{EquipmentAttributeType.fortune, 0.2f}}},
        };
        
        public AttributeAnvil() : base("AttributeAnvil")
        {
            ActiveSkillEffective += () =>
            {
                var rng = Random.Range(0, 21);
                var choices = new Choice[3];

                if (rng < 15)
                {
                    // 银色属性，75%概率
                    if (ToolFunctions.GetRandomUniqueItems(attributeChoices, 3, out var result))
                    {
                        var index = 0;
                        var randomValue = (Random.Range(0, 7) * 5 + 50) / 100f;
                        
                        foreach (var kvp in result)
                        {
                            var choice = new Choice(kvp.Key, "", null, null, () => {}, Quality.Silver);
                            
                            foreach (var kv in kvp.Value)
                            {
                                // 将属性加入加成
                                switch (kv.Key)
                                {
                                    case EquipmentAttributeType.None:Debug.LogError("未找到对应属性！");break;
                                    case EquipmentAttributeType.maxHealthPoint: 
                                        choice.choiceContent += $"+<sprite=\"Attributes\" name=\"MaxHealthPointIcon\">{kv.Value * randomValue:0.#}最大生命值（{kv.Value*0.5f:0.#}-{kv.Value*0.8f:0.#}）\n";
                                        break;
                                    case EquipmentAttributeType.attackSpeed:
                                        choice.choiceContent += $"+<sprite=\"Attributes\" name=\"AttackSpeedIcon\">{kv.Value * randomValue * 100:0.#}%攻击速度（{kv.Value*0.5f * 100:0.#}%-{kv.Value*0.8f * 100:0.#}%）\n";
                                        break;
                                    case EquipmentAttributeType.attackDamage:
                                        choice.choiceContent += $"+<sprite=\"Attributes\" name=\"AttackDamageIcon\">{kv.Value * randomValue:0.#}攻击力（{kv.Value*0.5f:0.#}-{kv.Value*0.8f:0.#}）\n";
                                        break;
                                    case EquipmentAttributeType.abilityPower:
                                        choice.choiceContent += $"+<sprite=\"Attributes\" name=\"AbilityPowerIcon\">{kv.Value * randomValue:0.#}法术强度（{kv.Value*0.5f:0.#}-{kv.Value*0.8f:0.#}）\n";
                                        break;
                                    case EquipmentAttributeType.abilityHaste:
                                        choice.choiceContent += $"+<sprite=\"Attributes\" name=\"AbilityHasteIcon\">{kv.Value * randomValue:0.#}技能急速（{kv.Value*0.5f:0.#}-{kv.Value*0.8f:0.#}）\n";
                                        break;
                                    case EquipmentAttributeType.attackDefense:
                                        choice.choiceContent += $"+<sprite=\"Attributes\" name=\"AttackDefenseIcon\">{kv.Value * randomValue:0.#}物理防御（{kv.Value*0.5f:0.#}-{kv.Value*0.8f:0.#}）\n";
                                        break;
                                    case EquipmentAttributeType.magicDefense:
                                        choice.choiceContent += $"+<sprite=\"Attributes\" name=\"MagicDefenseIcon\">{kv.Value * randomValue:0.#}法术防御（{kv.Value*0.5f:0.#}-{kv.Value*0.8f:0.#}）\n";
                                        break;
                                    case EquipmentAttributeType.attackPenetration:
                                        choice.choiceContent += $"+<sprite=\"Attributes\" name=\"AttackPenetrationIcon\">{kv.Value * randomValue:0.#}物理穿透（{kv.Value*0.5f:0.#}-{kv.Value*0.8f:0.#}）\n";
                                        break;
                                    case EquipmentAttributeType.magicPenetration:
                                        choice.choiceContent += $"+<sprite=\"Attributes\" name=\"MagicPenetrationIcon\">{kv.Value * randomValue:0.#}法术穿透（{kv.Value*0.5f:0.#}-{kv.Value*0.8f:0.#}）\n";
                                        break;
                                    case EquipmentAttributeType.criticalRate:
                                        choice.choiceContent += $"+<sprite=\"Attributes\" name=\"CriticalRateIcon\">{kv.Value * randomValue * 100:0.#}%暴击率（{kv.Value*0.5f * 100:0.#}%-{kv.Value*0.8f * 100:0.#}%）\n";
                                        break;
                                    default:
                                        Debug.LogWarning($"{kv.Key}缺少属性");
                                        break;
                                }

                                choice.OnSelected += () =>
                                {
                                    // 添加锻体buff
                                    if (AttributeAnvilBonus.Instance == null)
                                    {
                                        var attributeAnvilBonus = new AttributeAnvilBonus(HeroManager.hero, HeroManager.hero);
                                        attributeAnvilBonus.buffIcon = equipmentIcon;
                                        HeroManager.hero.GainBuff(attributeAnvilBonus);
                                        AttributeAnvilBonus.Instance = attributeAnvilBonus;
                                    }

                                    AttributeAnvilBonus.Instance.AddAttribute(kv.Key, kv.Value * randomValue);
                                };
                            }

                            choice.OnSelected += () => { AttributeAnvilBonus.Instance.buffCount.Value += 1; };
                            choices[index] = choice;
                            index += 1;
                        }
                    }
                }
                else if (rng < 19)
                {
                    // 金色属性，20%概率
                    if (ToolFunctions.GetRandomUniqueItems(attributeChoices, 3, out var result))
                    {
                        var index = 0;
                        
                        foreach (var kvp in result)
                        {
                            var choice = new Choice(kvp.Key, "", null, null, () => {}, Quality.Gold);
                            
                            foreach (var kv in kvp.Value)
                            {
                                switch (kv.Key)
                                {
                                    case EquipmentAttributeType.None:Debug.LogError("未找到对应属性！");break;
                                    case EquipmentAttributeType.maxHealthPoint: 
                                        choice.choiceContent += $"+<sprite=\"Attributes\" name=\"MaxHealthPointIcon\">{kv.Value:0.#}最大生命值\n";
                                        break;
                                    case EquipmentAttributeType.attackSpeed:
                                        choice.choiceContent += $"+<sprite=\"Attributes\" name=\"AttackSpeedIcon\">{kv.Value * 100:0.#}%攻击速度\n";
                                        break;
                                    case EquipmentAttributeType.attackDamage:
                                        choice.choiceContent += $"+<sprite=\"Attributes\" name=\"AttackDamageIcon\">{kv.Value:0.#}攻击力\n";
                                        break;
                                    case EquipmentAttributeType.abilityPower:
                                        choice.choiceContent += $"+<sprite=\"Attributes\" name=\"AbilityPowerIcon\">{kv.Value:0.#}法术强度\n";
                                        break;
                                    case EquipmentAttributeType.abilityHaste:
                                        choice.choiceContent += $"+<sprite=\"Attributes\" name=\"AbilityHasteIcon\">{kv.Value:0.#}技能急速\n";
                                        break;
                                    case EquipmentAttributeType.attackDefense:
                                        choice.choiceContent += $"+<sprite=\"Attributes\" name=\"AttackDefenseIcon\">{kv.Value:0.#}物理防御\n";
                                        break;
                                    case EquipmentAttributeType.magicDefense:
                                        choice.choiceContent += $"+<sprite=\"Attributes\" name=\"MagicDefenseIcon\">{kv.Value:0.#}法术防御\n";
                                        break;
                                    case EquipmentAttributeType.attackPenetration:
                                        choice.choiceContent += $"+<sprite=\"Attributes\" name=\"AttackPenetrationIcon\">{kv.Value:0.#}物理穿透\n";
                                        break;
                                    case EquipmentAttributeType.magicPenetration:
                                        choice.choiceContent += $"+<sprite=\"Attributes\" name=\"MagicPenetrationIcon\">{kv.Value:0.#}法术穿透\n";
                                        break;
                                    case EquipmentAttributeType.criticalRate:
                                        choice.choiceContent += $"+<sprite=\"Attributes\" name=\"CriticalRateIcon\">{kv.Value * 100:0.#}%暴击率\n";
                                        break;
                                    default:
                                        Debug.LogWarning($"{kv.Key}缺少属性");
                                        break;
                                }

                                choice.OnSelected += () =>
                                {
                                    // 添加锻体buff
                                    if (AttributeAnvilBonus.Instance == null)
                                    {
                                        var attributeAnvilBonus = new AttributeAnvilBonus(HeroManager.hero, HeroManager.hero);
                                        attributeAnvilBonus.buffIcon = equipmentIcon;
                                        HeroManager.hero.GainBuff(attributeAnvilBonus);
                                        AttributeAnvilBonus.Instance = attributeAnvilBonus;
                                    }

                                    AttributeAnvilBonus.Instance.AddAttribute(kv.Key, kv.Value);
                                };
                            }
                            
                            choice.OnSelected += () => { AttributeAnvilBonus.Instance.buffCount.Value += 1; };
                            choices[index] = choice;
                            index += 1;
                        }
                    }
                }
                else
                {
                    // 棱彩属性，5%概率
                    if (ToolFunctions.GetRandomUniqueItems(prismaticChoices, 3, out var result))
                    {
                        var index = 0;
                        
                        foreach (var kvp in result)
                        {
                            var choice = new Choice(kvp.Key, "", null, null, () => {}, Quality.Prismatic);
                            
                            foreach (var kv in kvp.Value)
                            {
                                switch (kv.Key)
                                {
                                    case EquipmentAttributeType.None:Debug.LogError("未找到对应属性！");break;
                                    case EquipmentAttributeType.percentageMovementSpeed: 
                                        choice.choiceContent += $"+<sprite=\"Attributes\" name=\"MovementSpeedIcon\">{kv.Value:P0}移动速度\n";
                                        break;
                                    case EquipmentAttributeType.percentageScaleBonus:
                                        choice.choiceContent += (kv.Value > 0 ? "+" : "") + $"{kv.Value:P0}体型\n";
                                        break;
                                    case EquipmentAttributeType.percentageMaxHealthPoint:
                                        choice.choiceContent += $"+<sprite=\"Attributes\" name=\"MaxHealthPointIcon\">{kv.Value:P0}最大生命值\n";
                                        break;
                                    case EquipmentAttributeType.omnivamp:
                                        choice.choiceContent += $"+<sprite=\"Attributes\" name=\"OmniVampIcon\">{kv.Value:P0}全能吸血\n";
                                        break;
                                    case EquipmentAttributeType.percentageAttackPenetration:
                                        choice.choiceContent += $"+<sprite=\"Attributes\" name=\"AttackPenetrationIcon\">{kv.Value:P0}物理穿透\n";
                                        break;
                                    case EquipmentAttributeType.percentageMagicPenetration:
                                        choice.choiceContent += $"+<sprite=\"Attributes\" name=\"MagicPenetrationIcon\">{kv.Value:P0}法术穿透\n";
                                        break;
                                    case EquipmentAttributeType.criticalDamage:
                                        choice.choiceContent += $"+<sprite=\"Attributes\" name=\"CriticalDamageIcon\">{kv.Value:P0}暴击伤害\n";
                                        break;
                                    case EquipmentAttributeType.fortune:
                                        choice.choiceContent += $"+  <sprite=\"Coin\" index=0>{kv.Value:P0}击杀敌人金币获取量\n";
                                        break;
                                    default:
                                        Debug.LogWarning($"{kv.Key}缺少属性");
                                        break;
                                }

                                choice.OnSelected += () =>
                                {
                                    // 添加锻体buff
                                    if (AttributeAnvilBonus.Instance == null)
                                    {
                                        var attributeAnvilBonus = new AttributeAnvilBonus(HeroManager.hero, HeroManager.hero);
                                        attributeAnvilBonus.buffIcon = equipmentIcon;
                                        HeroManager.hero.GainBuff(attributeAnvilBonus);
                                        AttributeAnvilBonus.Instance = attributeAnvilBonus;
                                    }

                                    AttributeAnvilBonus.Instance.AddAttribute(kv.Key, kv.Value);
                                };
                            }
                            
                            choice.OnSelected += () => { AttributeAnvilBonus.Instance.buffCount.Value += 1; };
                            choices[index] = choice;
                            index += 1;
                        }
                    }
                }

                if(AttributeAnvilBonus.Instance == null){
                    ChoiceSystem.Instance.MakeChoice(false, choices);
                    return;
                }
                
                // 增幅碎片
                if (AttributeAnvilBonus.Instance.buffCount.Value > 10 && AttributeAnvilBonus.Instance.attributeBonus <= 1.01f && !HeroManager.hero.hasBoughtEquipment)
                {
                    if (Random.Range(AttributeAnvilBonus.Instance.buffCount.Value / 4, 10) >= 8)
                    {
                        var target = Random.Range(0, 3);
                        var bonus = (Random.Range((AttributeAnvilBonus.Instance.buffCount.Value - 10) / 3, 16) * 4 + 20) / 100f;
                        choices[target].choiceContent = $"+{bonus:P0}效能给所有其他属性碎片（20%-80%）";
                        choices[target].choiceIcon = null;
                        choices[target].choiceTitle = "碎片增幅碎片";
                        choices[target].OnSelected = () =>
                        {
                            AttributeAnvilBonus.Instance.AddBonus(bonus);
                        };
                    }
                }
                
                ChoiceSystem.Instance.MakeChoice(false, choices);
            };
        }

        public override bool GetPassiveSkillDescription(out string description)
        {
            description = string.Format(_passiveSkillName + "\n" + _passiveSkillDescription);
            return true;
        }

        public override bool GetActiveSkillDescription(out string description)
        {
            description = string.Format(_activeSkillName + "\n" + _activeSkillDescription);
            return true;
        }
    }
}