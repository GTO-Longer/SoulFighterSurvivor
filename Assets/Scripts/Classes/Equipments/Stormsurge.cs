using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Managers;
using Managers.EntityManagers;
using UnityEngine;
using Utilities;

namespace Classes.Equipments
{
    public class Stormsurge : Equipment
    {
        private readonly int Explode = Animator.StringToHash("Explode");
        private readonly int Lightning = Animator.StringToHash("Lightning");
        
        private float damageCount => 180 + 0.5f * HeroManager.hero.abilityPower;
        private Action<Entity, Entity, float, Skill> equipmentEffect;
        private Dictionary<Entity, float> damageSum = new();
        private TweenerCore<Vector3, Vector3, VectorOptions> storm;
        private Effect effect;
        
        public Stormsurge() : base("Stormsurge")
        {
            storm = null;
            equipmentEffect = (attacker, target, skillDamageCount, _) =>
            {
                // 技能CD结束
                if (_passiveSkillActive)
                {
                    // 若被伤害目标不在字典内则添加
                    if (!damageSum.ContainsKey(target))
                    {
                        damageSum.Add(target, 0);
                    }

                    // 给每个目标添加各自受到的伤害量
                    damageSum[target] += skillDamageCount;
                    
                    // 2.5秒后清空
                    Async.SetAsync(2.5f, null, null, () =>
                    {
                        damageSum[target] -= skillDamageCount;
                    });

                    // 若没有创建风暴
                    if (storm == null)
                    {
                        // 遍历伤害字典
                        foreach (var kv in damageSum)
                        {
                            // 如果某个目标受到了大于自身25%最大生命值的伤害
                            if (kv.Value >= kv.Key.maxHealthPoint.Value * 0.25f)
                            {
                                // 创建风暴
                                _passiveSkillCDTimer = 0;
                                effect = EffectManager.Instance.CreateEffect("Stormsurge", kv.Key.gameObject);
                                
                                storm = Async.SetAsync(2, null, () =>
                                {
                                    // 若目标已死亡则发生大范围雷暴
                                    if (!target.isAlive)
                                    {
                                        effect.effect.GetComponent<Animator>().SetTrigger(Explode);

                                        effect.effect.transform.DOScale(1200, 0.2f);
                                        Async.SetAsync(0.2f, null, null, () =>
                                        {
                                            var enemies =
                                                ToolFunctions.IsOverlappingWithTagAll(target.gameObject, 350, "Enemy");
                                            foreach (var enemy in enemies)
                                            {
                                                enemy.TakeDamage(enemy.CalculateAPDamage(attacker, damageCount),
                                                    DamageType.AP,
                                                    attacker);
                                            }
                                        });
                                        
                                        KillCore();
                                    }
                                },
                                () =>
                                {
                                    effect.effect.GetComponent<Animator>().SetTrigger(Lightning);
                                    
                                    // 风暴劈下
                                    kv.Key.TakeDamage(kv.Key.CalculateAPDamage(attacker, damageCount),
                                        DamageType.AP,
                                        attacker);
                                    storm = null;
                                });
                            }
                        }
                    }
                }
            };

            equipmentTimerUpdate += (_) =>
            {
                if (storm == null && !_passiveSkillActive)
                {
                    damageSum.Clear();
                }
            };
        }

        private void KillCore()
        {
            storm.Kill();
            storm = null;
        }

        public override void OnEquipmentGet(Entity entity)
        {
            base.OnEquipmentGet(entity);

            owner.EntityUpdateEvent += equipmentTimerUpdate;
            owner.AbilityEffect += equipmentEffect;
        }

        public override void OnEquipmentRemove()
        {
            owner.EntityUpdateEvent -= equipmentTimerUpdate;
            owner.AbilityEffect -= equipmentEffect;
            
            base.OnEquipmentRemove();
        }

        public override bool GetPassiveSkillDescription(out string description)
        {
            description = string.Format(_passiveSkillName + "\n冷却时间:" + _passiveSkillCD + "秒\n" + _passiveSkillDescription, damageCount);
            return true;
        }
    }
}