using System;
using System.Collections.Generic;
using Managers.EntityManagers;
using Utilities;

namespace Classes.Equipments
{
    public class Stormsurge : Equipment
    {
        private float damageCount => 180 + 0.5f * HeroManager.hero.abilityPower;
        private Action<Entity, Entity, float> equipmentEffect;
        private Dictionary<Entity, float> damageSum = new();
        
        public Stormsurge() : base("Stormsurge")
        {
            equipmentEffect = (attacker, target, skillDamageCount) =>
            {
                if (!damageSum.ContainsKey(target))
                {
                    damageSum.Add(target, 0);
                }
                damageSum[target] += skillDamageCount;
                Async.SetAsync(2.5f, null, null, () =>
                {
                    if (target.isAlive)
                    {
                        damageSum[target] -= skillDamageCount;
                    }
                    else
                    {
                        damageSum.Remove(target);
                    }
                });

                if (_passiveSkillActive)
                {
                    foreach (var kv in damageSum)
                    {
                        if (kv.Value >= kv.Key.maxHealthPoint.Value * 0.25f)
                        {
                            _passiveSkillCDTimer = 0;
                            Async.SetAsync(2, null, () =>
                                {
                                    // 若目标已死亡则恢复技能可用
                                    if (!target.isAlive && !_passiveSkillActive)
                                    {
                                        _passiveSkillCDTimer = _passiveSkillCD;
                                    }
                                },
                                () =>
                            {
                                if (kv.Key.isAlive)
                                {
                                    kv.Key.TakeDamage(kv.Key.CalculateAPDamage(attacker, damageCount),
                                        DamageType.AP,
                                        attacker);
                                }
                            });
                        }
                    }
                }
            };
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
            description = string.Format(_passiveSkillName + "\n" + _passiveSkillDescription, damageCount);
            return true;
        }
    }
}