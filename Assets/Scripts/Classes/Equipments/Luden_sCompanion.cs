using System;
using System.Collections.Generic;
using Classes.Entities;
using Factories;
using Managers.EntityManagers;
using UnityEngine;
using Utilities;

namespace Classes.Equipments
{
    public class Luden_sCompanion : Equipment
    {
        private float damageCount => 80 + 0.05f * HeroManager.hero.abilityPower + 0.06f * (HeroManager.hero.attackDamage - HeroManager.hero.baseAttackDamage);
        private Action<Entity, Entity, float, Skill> equipmentEffect;
        private int chargeCount;
        private float baseCD;
        public Luden_sCompanion() : base("Luden_sCompanion")
        {
            chargeCount = 6;
            baseCD = _passiveSkillCD;
            equipmentEffect = (_, target, _, _) =>
            {
                if (chargeCount <= 0) return;
                if (target == null || !target.isAlive) return;

                var hero = owner as Hero;

                var bulletNum = chargeCount;
                chargeCount = 0;

                const float radius = 150f;
                const float bulletSpeed = 2000f;
                const float delayTime = 0.3f;

                for (var index = 0; index < bulletNum; index++)
                {
                    var bullet = BulletFactory.Instance.CreateBullet(hero);
                    var bulletIndex = index;
                    bullet.OnBulletAwake += (self) =>
                    {
                        self.target = target;
                        self.bulletStateID = 1;
                        self.gameObject.SetActive(true);

                        // 计算环绕角度
                        float angle;
                        if (bulletNum == 1)
                            angle = 0;
                        else
                            angle = (bulletIndex * 2f * Mathf.PI) / bulletNum;

                        // 设置初始位置
                        var ownerPos = hero.gameObject.transform.position;
                        var offset = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f);
                        self.gameObject.transform.position = ownerPos + offset;
                        
                        // 设置初始朝向
                        var targetPos = self.target.gameObject.transform.position;
                        var dir = (targetPos - ownerPos).normalized;
                        self.gameObject.transform.rotation = Quaternion.LookRotation(Vector3.forward, dir);

                        var timer = 0f;

                        self.OnBulletUpdate += (_) =>
                        {
                            // 目标死亡子弹立即消失
                            if (self.target == null || !self.target.isAlive)
                            {
                                self.Destroy();
                                return;
                            }

                            timer += Time.deltaTime;

                            // 停顿
                            if (self.bulletStateID == 1)
                            {
                                if (timer >= delayTime)
                                {
                                    self.bulletStateID = 2;
                                }
                            }
                            // 追踪目标
                            else if (self.bulletStateID == 2)
                            {
                                var curPos = self.gameObject.transform.position;
                                var targetPos = self.target.gameObject.transform.position;

                                var dir = (targetPos - curPos).normalized;
                                self.gameObject.transform.position = curPos + dir * (bulletSpeed * Time.deltaTime);
                                self.gameObject.transform.rotation = Quaternion.LookRotation(Vector3.forward, dir);

                                // 命中检测
                                if (Vector3.Distance(self.gameObject.transform.position, targetPos) <= self.target.scale)
                                {
                                    self.BulletHit();
                                    self.Destroy();
                                }
                            }
                        };
                    };

                    bullet.OnBulletHit += (self) =>
                    {
                        if (self.target == null || !self.target.isAlive) return;

                        var finalDamage = (bulletIndex == 0)
                            ? self.target.CalculateAPDamage(self.owner, damageCount)
                            : self.target.CalculateAPDamage(self.owner, damageCount * 0.35f);

                        self.target.TakeDamage(finalDamage, DamageType.AP, owner);
                    };

                    bullet.Awake();
                }
            };

            equipmentTimerUpdate += (_) =>
            {
                if (_passiveSkillActive)
                {
                    if (chargeCount < 6)
                    {
                        chargeCount += 1;
                    }

                    _passiveSkillCDTimer = 0;
                }
                
                if (chargeCount >= 6)
                {
                    _passiveSkillCD = 0;
                }
                else
                {
                    _passiveSkillCD = baseCD;
                }
            };
        }

        public override void OnEquipmentGet(Entity entity)
        {
            base.OnEquipmentGet(entity);
            owner.AbilityEffect += equipmentEffect;
            owner.EntityUpdateEvent += equipmentTimerUpdate;
        }

        public override void OnEquipmentRemove()
        {
            owner.AbilityEffect -= equipmentEffect;
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