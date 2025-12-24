using System;
using System.Collections.Generic;
using Classes.Buffs;
using Factories;
using Managers;
using Managers.EntityManagers;
using UnityEngine;
using Utilities;

namespace Classes.Equipments
{
    public class Rabadon_sAgony : Equipment
    {
        private Action<Entity, Entity, float, Skill> EquipmentEffect;
        private float damageCount => 15 + 0.05f * HeroManager.hero.abilityPower;
        
        public Rabadon_sAgony() : base("Rabadon_sAgony")
        {
            EquipmentEffect += (_, target, skillDamageCount, skill) =>
            {
                if (skill.skillType == SkillType.RSkill)
                {
                    var damage = target.CalculateAPDamage(owner, skillDamageCount * 0.1f);
                    target.TakeDamage(damage, DamageType.AP, owner);

                    if (!_passiveSkillActive)
                    {
                        return;
                    }

                    var damageArea = BulletFactory.Instance.CreateBullet(owner, 3.5f, 1);
                    damageArea.OnBulletAwake += (self) =>
                    {
                        self.gameObject.SetActive(true);
                        self.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                        damageArea.gameObject.transform.position = target.gameObject.transform.position;
                        self.effect = EffectManager.Instance.CreateEffect("Rabadon_sAgony", self.gameObject);

                        self.OnBulletUpdate += (_) =>
                        {
                            var targets = ToolFunctions.IsOverlappingOtherTagAll(self.gameObject, 200);
                            if (targets != null)
                            {
                                foreach (var entity in targets)
                                {
                                    self.bulletEntityDamageCDTimer.TryAdd(entity, self.bulletDamageCD);

                                    if (self.bulletEntityDamageCDTimer[entity] > self.bulletDamageCD)
                                    {
                                        self.bulletEntityDamageCDTimer[entity] = 0;
                                        
                                        var magicDefenseReduce = new MagicDefenseReduce(entity, owner, 1f, 25);
                                        entity.GetBuff(magicDefenseReduce);

                                        var damageValue = entity.CalculateAPDamage(self.owner, damageCount);
                                        entity.TakeDamage(damageValue, DamageType.AP, owner);
                                    }
                                }
                            }

                            // 子弹的销毁逻辑
                            if (self.bulletContinuousTimer >= self.bulletContinuousTime)
                            {
                                self.bulletEntityDamageCDTimer.Clear();
                                EffectManager.Instance.DestroyEffect(self.effect);
                                self.Destroy();
                            }
                        };
                    };

                    damageArea.Awake();
                    _passiveSkillCDTimer = 0;
                }
            };
        }

        public override void OnEquipmentGet(Entity entity)
        {
            base.OnEquipmentGet(entity);
            owner.AbilityEffect += EquipmentEffect;
            owner.EntityUpdateEvent += equipmentTimerUpdate;
        }

        public override void OnEquipmentRemove()
        {
            owner.AbilityEffect -= EquipmentEffect;
            owner.EntityUpdateEvent -= equipmentTimerUpdate;
            base.OnEquipmentRemove();
        }

        public override bool GetPassiveSkillDescription(out string description)
        {
            description = string.Format(_passiveSkillName + "\n" + _passiveSkillDescription, damageCount);
            return true;
        }
    }
}