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
    public class ProfaneHydra : Equipment
    {
        private Action<Entity, Entity, float, float> PassiveEquipmentEffect;
        private float passiveDamageCount => 0.4f * HeroManager.hero.attackDamage;
        private float activeDamageCount => 0.8f * HeroManager.hero.attackDamage;
        
        public ProfaneHydra() : base("ProfaneHydra")
        {
            PassiveEquipmentEffect += (_, target, _, _) =>
            {
                if (target.isAlive)
                {
                    var damageArea = BulletFactory.Instance.CreateBullet(owner, 0.3f, 1);
                    damageArea.OnBulletAwake += (self) =>
                    {
                        self.gameObject.SetActive(true);
                        self.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                        self.effect = EffectManager.Instance.CreateEffect("ProfaneHydra", self.gameObject);

                        self.OnBulletUpdate += (_) =>
                        {
                            damageArea.gameObject.transform.position = target.gameObject.transform.position;

                            var targets = ToolFunctions.IsOverlappingOtherTagAll(self.gameObject, 200);
                            if (targets != null)
                            {
                                foreach (var entity in targets)
                                {
                                    if (Equals(entity, target)) continue;

                                    self.bulletEntityDamageCDTimer.TryAdd(entity, self.bulletDamageCD);

                                    if (self.bulletEntityDamageCDTimer[entity] > self.bulletDamageCD)
                                    {
                                        self.bulletEntityDamageCDTimer[entity] = 0;

                                        var damageValue = entity.CalculateADDamage(self.owner, passiveDamageCount);
                                        entity.TakeDamage(damageValue, DamageType.AD, owner);
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
                }
            };
            
            ActiveSkillEffective += () =>
            {
                if (!_activeSkillActive) return;
                
                var damageArea = BulletFactory.Instance.CreateBullet(owner, 0.3f, 1);
                damageArea.OnBulletAwake += (self) =>
                {
                    self.gameObject.SetActive(true);
                    self.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                    self.effect = EffectManager.Instance.CreateEffect("ProfaneHydra", self.gameObject);
                    self.effect.effect.transform.localScale += self.effect.effect.transform.localScale / 4f;

                    self.OnBulletUpdate += (_) =>
                    {
                        damageArea.gameObject.transform.position = owner.gameObject.transform.position;
                        
                        var targets = ToolFunctions.IsOverlappingOtherTagAll(self.gameObject, 250);
                        if (targets != null)
                        {
                            foreach (var entity in targets)
                            {
                                self.bulletEntityDamageCDTimer.TryAdd(entity, self.bulletDamageCD);

                                if (self.bulletEntityDamageCDTimer[entity] > self.bulletDamageCD)
                                {
                                    self.bulletEntityDamageCDTimer[entity] = 0;

                                    var damageValue = entity.CalculateADDamage(self.owner, activeDamageCount + 0.12f * (entity.maxHealthPoint - entity.healthPoint));
                                    entity.TakeDamage(damageValue, DamageType.AD, owner);
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
                _activeSkillCDTimer = 0;
            };
        }

        public override void OnEquipmentGet(Entity entity)
        {
            base.OnEquipmentGet(entity);
            owner.AttackEffect += PassiveEquipmentEffect;
            owner.EntityUpdateEvent += equipmentTimerUpdate;
        }

        public override void OnEquipmentRemove()
        {
            owner.AttackEffect -= PassiveEquipmentEffect;
            owner.EntityUpdateEvent -= equipmentTimerUpdate;
            base.OnEquipmentRemove();
        }

        public override bool GetPassiveSkillDescription(out string description)
        {
            description = string.Format(_passiveSkillName + "\n" + _passiveSkillDescription, passiveDamageCount);
            return true;
        }

        public override bool GetActiveSkillDescription(out string description)
        {
            description = string.Format(_activeSkillName + "\n" + _activeSkillDescription, activeDamageCount);
            return true;
        }
    }
}