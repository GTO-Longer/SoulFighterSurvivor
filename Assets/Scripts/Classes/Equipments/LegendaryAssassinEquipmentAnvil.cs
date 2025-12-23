using Managers;
using Managers.EntityManagers;
using Systems;
using UnityEngine;
using Utilities;

namespace Classes.Equipments
{
    public class LegendaryAssassinEquipmentAnvil : Equipment
    {
        
        public LegendaryAssassinEquipmentAnvil() : base("LegendaryAssassinEquipmentAnvil")
        {
            ActiveSkillEffective += () =>
            {
                var choices = new Choice[3];

                var targetEquipments = EquipmentManager.Instance.equipmentList.FindAll(equipment =>
                    equipment.usageTypes.Contains(UsageType.Assassin) &&
                    equipment.owner == null &&
                    equipment._equipmentType == EquipmentType.Legend &&
                    equipment.canPurchase &&
                    HeroManager.hero.equipmentList.FindAll(equip => equip.Value != null && equipment._uniqueEffect != EquipmentUniqueEffect.None && equipment._uniqueEffect == equip.Value._uniqueEffect).Count == 0);
                
                if (ToolFunctions.GetRandomUniqueItems(targetEquipments, 3, out var result))
                {
                    var index = 0;
                    foreach (var equipment in result)
                    {
                        choices[index].rawColor = true;
                        choices[index].choiceContent = "";
                        choices[index].choiceTitle = equipment.equipmentName;
                        choices[index].choiceIcon = equipment.equipmentIcon;
                        choices[index].choiceIconBorder = null;
                        choices[index].choiceQuality = Quality.Gold;
                        
                        foreach (var kv in equipment.equipmentAttributes)
                        {
                            // 获取装备属性对应的属性描述
                            switch (kv.Key)
                            {
                                case EquipmentAttributeType.None:
                                    Debug.LogError("未找到对应属性！");
                                    break;
                                case EquipmentAttributeType.maxHealthPoint:
                                    choices[index].choiceContent += $"<color={Colors.Health}>+{kv.Value:F0}最大生命值</color> ";
                                    break;
                                case EquipmentAttributeType.percentageMaxHealthPoint:
                                    choices[index].choiceContent += $"<color={Colors.Health}>+{kv.Value:P0}最大生命值</color> ";
                                    break;
                                case EquipmentAttributeType.maxMagicPoint:
                                    choices[index].choiceContent += $"<color=#1DA1FF>+{kv.Value:F0}最大法力值</color> ";
                                    break;
                                case EquipmentAttributeType.percentageMaxMagicPoint:
                                    choices[index].choiceContent += $"<color=#1DA1FF>+{kv.Value:P0}最大法力值</color> ";
                                    break;
                                case EquipmentAttributeType.attackSpeed:
                                    choices[index].choiceContent += $"<color={Colors.AttackSpeed}>+{kv.Value:P0}攻击速度</color> ";
                                    break;
                                case EquipmentAttributeType.percentageAttackSpeed:
                                    choices[index].choiceContent += $"<color={Colors.AttackSpeed}>+{kv.Value:P0}总攻击速度加成</color> ";
                                    break;
                                case EquipmentAttributeType.attackDamage:
                                    choices[index].choiceContent += $"<color=#FF6161>+{kv.Value:F0}攻击力</color> ";
                                    break;
                                case EquipmentAttributeType.percentageAttackDamage:
                                    choices[index].choiceContent += $"<color=#FF6161>+{kv.Value:P0}攻击力</color> ";
                                    break;
                                case EquipmentAttributeType.abilityPower:
                                    choices[index].choiceContent += $"<color={Colors.AbilityPower}>+{kv.Value:F0}法术强度</color> ";
                                    break;
                                case EquipmentAttributeType.percentageAbilityPower:
                                    choices[index].choiceContent += $"<color={Colors.AbilityPower}>+{kv.Value:P0}法术强度</color> ";
                                    break;
                                case EquipmentAttributeType.abilityHaste:
                                    choices[index].choiceContent += $"<color=#FFE761>+{kv.Value:F0}技能急速</color> ";
                                    break;
                                case EquipmentAttributeType.percentageAbilityHaste:
                                    choices[index].choiceContent += $"<color=#FFE761>+{kv.Value:P0}技能急速</color> ";
                                    break;
                                case EquipmentAttributeType.attackDefense:
                                    choices[index].choiceContent += $"<color={Colors.AttackDefense}>+{kv.Value:F0}物理防御</color> ";
                                    break;
                                case EquipmentAttributeType.percentageAttackDefense:
                                    choices[index].choiceContent += $"<color={Colors.AttackDefense}>+{kv.Value:P0}物理防御</color> ";
                                    break;
                                case EquipmentAttributeType.magicDefense:
                                    choices[index].choiceContent += $"<color={Colors.MagicDefense}>+{kv.Value:F0}法术防御</color> ";
                                    break;
                                case EquipmentAttributeType.percentageMagicDefense:
                                    choices[index].choiceContent += $"<color={Colors.MagicDefense}>+{kv.Value:P0}法术防御</color> ";
                                    break;
                                case EquipmentAttributeType.attackPenetration:
                                    choices[index].choiceContent += $"<color={Colors.AttackPenetration}>+{kv.Value:F0}物理穿透</color> ";
                                    break;
                                case EquipmentAttributeType.percentageAttackPenetration:
                                    choices[index].choiceContent += $"<color={Colors.AttackPenetration}>+{kv.Value:P0}物理穿透</color> ";
                                    break;
                                case EquipmentAttributeType.magicPenetration:
                                    choices[index].choiceContent += $"<color={Colors.MagicPenetration}>+{kv.Value:F0}法术穿透</color> ";
                                    break;
                                case EquipmentAttributeType.percentageMagicPenetration:
                                    choices[index].choiceContent += $"<color={Colors.MagicPenetration}>+{kv.Value:P0}法术穿透</color> ";
                                    break;
                                case EquipmentAttributeType.criticalRate:
                                    choices[index].choiceContent += $"<color=#FFB74D>+{kv.Value:P0}暴击率</color> ";
                                    break;
                                case EquipmentAttributeType.criticalDamage:
                                    choices[index].choiceContent += $"<color=#FFA347>+{kv.Value:P0}暴击伤害</color> ";
                                    break;
                                case EquipmentAttributeType.movementSpeed:
                                    choices[index].choiceContent += $"<color={Colors.MoveSpeed}>+{kv.Value:F0}移动速度</color> ";
                                    break;
                                case EquipmentAttributeType.percentageMovementSpeed:
                                    choices[index].choiceContent += $"<color={Colors.MoveSpeed}>+{kv.Value:P0}移动速度</color> ";
                                    break;
                                case EquipmentAttributeType.percentageHealthRegeneration:
                                    choices[index].choiceContent += $"<color={Colors.HealthGeneration}>+{kv.Value:P0}生命值回复</color> ";
                                    break;
                                case EquipmentAttributeType.percentageMagicRegeneration:
                                    choices[index].choiceContent += $"<color={Colors.MagicRegeneration}>+{kv.Value:P0}法力值回复</color> ";
                                    break;
                                case EquipmentAttributeType.omnivamp:
                                    choices[index].choiceContent += $"<color=#6E1BA8>+{kv.Value:P0}全能吸血</color> ";
                                    break;
                                case EquipmentAttributeType.lifeSteal:
                                    choices[index].choiceContent += $"<color={Colors.LifeSteal}>+{kv.Value:P0}生命偷取</color> ";
                                    break;
                            }
                        }

                        if (equipment.GetPassiveSkillDescription(out var passiveContent))
                        {
                            choices[index].choiceContent += "\n";
                            choices[index].choiceContent += passiveContent;
                        }

                        if (equipment.GetActiveSkillDescription(out var activeContent))
                        {
                            choices[index].choiceContent += "\n";
                            choices[index].choiceContent += activeContent;
                        }

                        choices[index].OnSelected += () =>
                        {
                            foreach (var property in HeroManager.hero.equipmentList)
                            {
                                if (property.Value == null)
                                {
                                    HeroManager.hero.hasBoughtEquipment = true;
                                    property.Value = equipment;
                                    equipment.OnEquipmentGet(HeroManager.hero);
                                    
                                    return;
                                }
                            }

                            HeroManager.hero.tempEquipmentList.Add(equipment);
                        };

                        index++;
                    }
                
                    ChoiceSystem.Instance.MakeChoice(choices);
                }
                else
                {
                    Debug.LogWarning("剩余可选装备不足");
                }
            };
        }

        public override bool GetPassiveSkillDescription(out string description)
        {
            description = string.Format(_passiveSkillName + "\n" + _passiveSkillDescription);
            return true;
        }
    }
}