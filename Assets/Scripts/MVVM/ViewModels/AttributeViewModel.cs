using System;
using System.Collections.Generic;
using Classes.Entities;
using DataManagement;
using Managers.EntityManagers;
using TMPro;
using UnityEngine;
using Utilities;

namespace MVVM.ViewModels
{
    public class AttributeViewModel : MonoBehaviour
    {
        public Action UnBindEvent;
        public delegate string AttributeDescriptionDelegate();
        public Dictionary<AttributeType, List<AttributeDescriptionDelegate>> attributeDescriptionSettings;
        public Dictionary<AttributeType, List<Property<float>>> attributeDependenciesSettings;
        public static AttributeViewModel Instance;
        private TMP_Text attributeName;
        private TMP_Text attributeDescription;
        private TMP_Text attributeAmount;
        private GameObject background;
        private Hero hero;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            hero = HeroManager.hero;
            attributeName = transform.Find("Background/AttributeNameContainer/AttributeName").GetComponent<TMP_Text>();
            attributeDescription = transform.Find("Background/AttributeDescription").GetComponent<TMP_Text>();
            attributeAmount = transform.Find("Background/AttributeAmount").GetComponent<TMP_Text>();
            background = transform.Find("Background").gameObject;
            background.SetActive(false);

            attributeDescriptionSettings = new Dictionary<AttributeType, List<AttributeDescriptionDelegate>>
            {
                {
                    AttributeType.AttackDamage, new List<AttributeDescriptionDelegate>
                    {
                        () => "攻击力", () => "你的攻击造成的<color=#FF6600>物理伤害</color>的数额\n\n还会提升你用某些技能造成的伤害数额",
                        () => $"攻击力：<color=#0066FF>{hero.attackDamage.Value:F0}</color>（<color=#0066FF>{hero.baseAttackDamage:F0}</color>基础+<color=#00FF00>{hero.attackDamage.Value - hero.baseAttackDamage:F0}</color>加成）"
                    }
                },
                {
                    AttributeType.AbilityPower, new List<AttributeDescriptionDelegate>
                    {
                        () => "法术强度", () => "提升你用大部分技能造成的伤害数额",
                        () => $"法术强度：<color=#0066FF>{hero.abilityPower.Value:F0}</color>（<color=#00FF00>{hero.abilityPower.Value:F0}</color>加成）"
                    }
                },
                {
                    AttributeType.AttackDefense, new List<AttributeDescriptionDelegate>
                    {
                        () => "护甲", () => "减少你承受的<color=#FF6600>物理伤害</color>的数额",
                        () => $"护甲：<color=#0066FF>{hero.attackDefense.Value:F0}</color>（<color=#0066FF>{hero.baseAttackDefense:F0}</color>基础+<color=#00FF00>{hero.attackDefense.Value - hero.baseAttackDefense:F0}</color>加成）\n你所受的物理伤害减少{hero.actualAttackDamageReduction:P0}"
                    }
                },
                {
                    AttributeType.MagicDefense, new List<AttributeDescriptionDelegate>
                    {
                        () => "魔法抗性", () => "减少你所受的<color=#0066FF>魔法伤害</color>的数额",
                        () => $"护甲：<color=#0066FF>{hero.magicDefense.Value:F0}</color>（<color=#0066FF>{hero.baseMagicDefense:F0}</color>基础+<color=#00FF00>{hero.magicDefense.Value - hero.baseMagicDefense:F0}</color>加成）\n你所受的魔法伤害减少{hero.actualMagicDamageReduction:P0}"
                    }
                },
                {
                    AttributeType.AttackSpeed, new List<AttributeDescriptionDelegate>
                    {
                        () => "攻击速度", () => "提升你进行攻击的速率\n\n收益率决定利用攻击速度加成的效率",
                        () => $"当前攻击速度：<color=#00FF00>{hero.attackSpeed.Value:P1}</color>\n每秒攻击次数：<color=#0066FF>{1f / hero.actualAttackInterval:0.##}</color>\n收益率<color=#00FF00>{hero._attackSpeedYield:0.###}</color>"
                    }
                },
                {
                    AttributeType.AbilityHaste, new List<AttributeDescriptionDelegate>
                    {
                        () => "技能急速", () => "允许你更为频繁的释放技能",
                        () => $"当前技能急速：<color=#00FF00>{hero.abilityHaste.Value:F0}</color>\n相当于使你的技能冷却时间缩短<color=#00FF00>{hero.actualAbilityCooldown:P0}</color>"
                    }
                },
                {
                    AttributeType.CriticalRate, new List<AttributeDescriptionDelegate>
                    {
                        () => "暴击几率", () => "提供一定几率来使攻击造成的伤害提升75%",
                        () => $"暴击几率：<color=#0066FF>{hero.criticalRate.Value:P0}</color>"
                    }
                },
                {
                    AttributeType.MovementSpeed, new List<AttributeDescriptionDelegate>
                    {
                        () => "移动速度", () => "你移动时的速度",
                        () => $"移动速度：<color=#0066FF>{hero.movementSpeed.Value:F0}</color>（<color=#0066FF>{hero.baseMovementSpeed:F0}</color>基础+<color=#00FF00>{hero.movementSpeed.Value - hero.baseMovementSpeed:F0}</color>加成）码/秒"
                    }
                },
                {
                    AttributeType.AttackPenetration, new List<AttributeDescriptionDelegate>
                    {
                        () => "物理穿透", () => "你能够无视敌方物理防御的数额和比例",
                        () => $"固定穿透：<color=#0066FF>{hero.attackPenetration.Value:F0}</color>\n百分比穿透：<color=#00FF00>{hero.percentageAttackPenetration.Value:P0}</color>"
                    }
                },
                {
                    AttributeType.MagicPenetration, new List<AttributeDescriptionDelegate>
                    {
                        () => "法术穿透", () => "你能够无视敌方法术防御的数额和比例",
                        () => $"固定穿透：<color=#0066FF>{hero.magicPenetration.Value:F0}</color>\n百分比穿透：<color=#00FF00>{hero.percentageMagicPenetration.Value:P0}</color>"
                    }
                },
                {
                    AttributeType.LifeSteel, new List<AttributeDescriptionDelegate>
                    {
                        () => "生命偷取", () => "在攻击时以一定比例偷取敌方的生命值",
                        () => $"生命偷取：<color=#0066FF>{hero.lifeSteal.Value:P0}</color>"
                    }
                },
                {
                    AttributeType.OmniVamp, new List<AttributeDescriptionDelegate>
                    {
                        () => "全能吸血", () => "在技能命中时以一定比例吸取敌方的生命值",
                        () => $"全能吸血：<color=#0066FF>{hero.omnivamp.Value:P0}</color>"
                    }
                },
                {
                    AttributeType.AttackRange, new List<AttributeDescriptionDelegate>
                    {
                        () => "攻击距离", () => "你攻击时的距离",
                        () => $"攻击距离：<color=#0066FF>{hero.attackRange.Value:F0}</color>码"
                    }
                },
                {
                    AttributeType.CriticalDamage, new List<AttributeDescriptionDelegate>
                    {
                        () => "暴击伤害", () => "你暴击时造成的额外伤害的比例",
                        () => $"暴击时造成<color=#0066FF>{1 + hero.criticalDamage.Value:0.##}</color>倍伤害"
                    }
                }
            };

            attributeDependenciesSettings = new Dictionary<AttributeType, List<Property<float>>>
            {
                {
                    AttributeType.AttackDamage, new List<Property<float>>
                    {hero.attackDamage, hero.level}
                },
                {
                    AttributeType.AbilityPower, new List<Property<float>>
                    {hero.abilityPower}
                },
                {
                    AttributeType.AttackDefense, new List<Property<float>>
                    {hero.attackDefense, hero.level}
                },
                {
                    AttributeType.MagicDefense, new List<Property<float>>
                    {hero.magicDefense, hero.level}
                },
                {
                    AttributeType.AttackSpeed, new List<Property<float>>
                        {hero.attackSpeed, hero.level}
                },
                {
                    AttributeType.AbilityHaste, new List<Property<float>>
                        {hero.abilityHaste}
                },
                {
                    AttributeType.CriticalRate, new List<Property<float>>
                        {hero.criticalRate}
                },
                {
                    AttributeType.MovementSpeed, new List<Property<float>>
                        {hero.movementSpeed}
                },
                {
                    AttributeType.AttackPenetration, new List<Property<float>>
                        {hero.attackPenetration, hero.percentageAttackPenetration}
                },
                {
                    AttributeType.MagicPenetration, new List<Property<float>>
                        {hero.magicPenetration, hero.percentageMagicPenetration}
                },
                {
                    AttributeType.LifeSteel, new List<Property<float>>
                        {hero.lifeSteal}
                },
                {
                    AttributeType.OmniVamp, new List<Property<float>>
                        {hero.omnivamp}
                },
                {
                    AttributeType.AttackRange, new List<Property<float>>
                        {hero.attackRange}
                },
                {
                    AttributeType.CriticalDamage, new List<Property<float>>
                        {hero.criticalDamage}
                }
            };
        }

        /// <summary>
        /// 绑定属性面板
        /// </summary>
        /// <param name="chosenAttribute"></param>
        public void BindAttributePanel(AttributeType chosenAttribute)
        {
            // 绑定属性介绍面板
            UnBindEvent = Binder.BindAttribute(background, attributeName, attributeDescription, attributeAmount, chosenAttribute);
            background.SetActive(true);
        }

        /// <summary>
        /// 属性面板解绑
        /// </summary>
        public void UnBindAttribute()
        {
            background.SetActive(false);
            UnBindEvent?.Invoke();
        }

        // 物体销毁时触发注销对应事件
        private void OnDestroy()
        {
            UnBindAttribute();
        }
    }
}
