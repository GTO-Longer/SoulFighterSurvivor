using System;
using Classes.Buffs;
using Factories;
using Managers;
using Managers.EntityManagers;
using UnityEngine;
using Utilities;

namespace Classes.Equipments
{
    public class Stridebreaker : Equipment
    {
        private Action<Entity, Entity, float, float> PassiveEquipmentEffect;
        private float passiveDamageCount => 0.4f * HeroManager.hero.attackDamage;
        private float activeDamageCount => 1.75f * HeroManager.hero.attackDamage + 1.05f * HeroManager.hero.abilityPower;
        
        public Stridebreaker() : base("Stridebreaker")
        {
            PassiveEquipmentEffect += (_, target, _, _) =>
            {
                if (target.isAlive)
                {
                    var damageArea = BulletFactory.Instance.CreateBullet(owner, 0.3f, 1);
                    damageArea.OnBulletAwake += (self) =>
                    {
                        self.gameObject.transform.position = target.gameObject.transform.position;
                        self.gameObject.SetActive(true);
                        self.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                        self.effect = EffectManager.Instance.CreateEffect("Stridebreaker", self.gameObject);

                        self.OnBulletUpdate += (_) =>
                        {
                            self.gameObject.transform.position = target.gameObject.transform.position;

                            var targets = ToolFunctions.IsOverlappingWithOtherTagAll(self.gameObject, 200);
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
                    self.gameObject.transform.position = owner.gameObject.transform.position;
                    self.gameObject.SetActive(true);
                    self.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                    self.effect = EffectManager.Instance.CreateEffect("Stridebreaker", self.gameObject);
                    self.effect.effect.transform.localScale += self.effect.effect.transform.localScale / 4f;

                    self.OnBulletUpdate += (_) =>
                    {
                        self.gameObject.transform.position = owner.gameObject.transform.position;
                        
                        var targets = ToolFunctions.IsOverlappingWithOtherTagAll(self.gameObject, 250);
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
                                    
                                    var speedReduce = new SpeedReduce(entity, owner, 3f, 0.4f);
                                    entity.GainBuff(speedReduce);
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