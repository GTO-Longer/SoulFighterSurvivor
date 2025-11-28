using System;
using System.Collections.Generic;
using Classes;
using Classes.Entities;
using DataManagement;
using EntityManagers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace MVVM.ViewModels
{
    public class AttributeViewModel : MonoBehaviour
    {
        public static Action UnBindEvent;
        public static Property<AttributeType> chosenAttribute = new();
        public static Dictionary<AttributeType, List<string>> attributeDescriptionSettings;
        public static Dictionary<AttributeType, List<Property<float>>> attributeDependenciesSettings;
        private Hero hero;

        private void Start()
        {
            hero = HeroManager.hero;
            
            var attributeName = transform.Find("Background/AttributeNameContainer/AttributeName").GetComponent<TMP_Text>();
            var attributeDescription = transform.Find("Background/AttributeDescription").GetComponent<TMP_Text>();
            var attributeAmount = transform.Find("Background/AttributeAmount").GetComponent<TMP_Text>();
            var background = transform.Find("Background").gameObject;
            
            background.SetActive(false);

            attributeDescriptionSettings = new Dictionary<AttributeType, List<string>>
            {
                {
                    AttributeType.AttackDamage, new List<string>
                    {
                        "攻击力", "你的攻击造成的<color=#FF6600>物理伤害</color>的数额\n\n还会提升你用某些技能造成的伤害数额",
                        $"攻击力：<color=#0066FF>{hero.attackDamage.Value:F0}</color>（<color=#0066FF>{hero.baseAttackDamage:F0}</color>基础+<color=#00FF00>{hero.attackDamage.Value - hero.baseAttackDamage:F0}</color>加成）"
                    }
                },
                {
                    AttributeType.AbilityPower, new List<string>
                    {
                        "法术强度", "提升你用大部分技能造成的伤害数额",
                        $"法术强度：<color=#0066FF>{hero.abilityPower.Value:F0}</color>（<color=#00FF00>{hero.abilityPower.Value:F0}</color>加成）"
                    }
                },
                {
                    AttributeType.AttackDefense, new List<string>
                    {
                        "护甲", "减少你承受的<color=#FF6600>物理伤害</color>的数额",
                        $"护甲：<color=#0066FF>{hero.attackDefense.Value:F0}</color>（<color=#0066FF>{hero.baseAttackDefense:F0}</color>基础+<color=#00FF00>{hero.attackDefense.Value - hero.baseAttackDefense:F0}</color>加成）\n你所受的物理伤害减少{hero.actualAttackDamageReduction:P0}"
                    }
                },
                {
                    AttributeType.MagicDefense, new List<string>
                    {
                        "魔法抗性", "减少你所受的<color=#0066FF>魔法伤害</color>的数额",
                        $"护甲：<color=#0066FF>{hero.magicDefense.Value:F0}</color>（<color=#0066FF>{hero.baseMagicDefense:F0}</color>基础+<color=#00FF00>{hero.magicDefense.Value - hero.baseMagicDefense:F0}</color>加成）\n你所受的魔法伤害减少{hero.actualMagicDamageReduction:P0}"
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
                }
            };

            // 绑定属性介绍面板
            UnBindEvent = Binder.BindAttribute(background, attributeName, attributeDescription, attributeAmount, chosenAttribute);
        }

        // 物体销毁时触发注销对应事件
        private void OnDestroy()
        {
            UnBindEvent?.Invoke();
            chosenAttribute = null;
        }
    }
}
