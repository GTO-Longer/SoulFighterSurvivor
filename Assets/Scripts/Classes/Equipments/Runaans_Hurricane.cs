using System;
using Factories;
using Managers.EntityManagers;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

namespace Classes.Equipments
{
    public class Runaans_Hurricane : Equipment
    {
        private Action<Entity, Entity, bool> equipmentEffect;
        private float damageCount => 0.7f * HeroManager.hero.attackDamage + 0.42f * HeroManager.hero.abilityPower;
        private const float attackBulletSpeed = 1500;
        
        public Runaans_Hurricane() : base("Runaans_Hurricane")
        {
            equipmentEffect = (_, target, isCrit) =>
            {
                var targets = ToolFunctions.IsOverlappingWithTagAll(target.gameObject, target.gameObject.tag, 400);
                
                if (targets is { Length: > 0 })
                {
                    for (var index = 1; index < Mathf.Min(3, targets.Length); index++)
                    {
                        var bullet = BulletFactory.Instance.CreateBullet(owner);
                        var targetIndex = index;
                        
                        bullet.OnBulletAwake += (self) =>
                        {
                            self.gameObject.transform.position = owner.gameObject.transform.position;
                            self.gameObject.SetActive(true);

                            // 设置目标
                            self.target = targets[targetIndex];

                            // 每帧追踪目标
                            self.OnBulletUpdate += (_) =>
                            {
                                // 锁定目标死亡则清除子弹
                                if (self.target == null || !self.target.isAlive)
                                {
                                    self.Destroy();
                                    return;
                                }

                                var currentPosition = self.gameObject.transform.position;
                                var targetPosition = self.target.gameObject.transform.position;

                                var direction = (targetPosition - currentPosition).normalized;
                                var nextPosition = currentPosition + direction * (attackBulletSpeed * Time.deltaTime);

                                self.gameObject.transform.position = nextPosition;
                                self.gameObject.transform.rotation =
                                    Quaternion.LookRotation(Vector3.forward, direction);

                                // 子弹的销毁逻辑
                                const float destroyDistance = 30f;
                                if (Vector3.Distance(self.gameObject.transform.position, self.target.gameObject.transform.position) <= destroyDistance)
                                {
                                    self.BulletHit();
                                    self.Destroy();
                                }
                            };
                        };

                        bullet.OnBulletHit += (self) =>
                        {
                            // 计算伤害
                            var damage = targets[targetIndex].CalculateADDamage(owner, damageCount);
                            targets[targetIndex].TakeDamage(damage, DamageType.AD, owner, isCrit);

                            // 造成攻击特效
                            if (targets[targetIndex].isAlive)
                            {
                                owner.AttackEffectActivate(targets[targetIndex], damageCount);
                            }
                        };

                        bullet.Awake();
                    }
                }
            };
        }

        public override void OnEquipmentGet(Entity entity)
        {
            base.OnEquipmentGet(entity);
            owner.OnAttackHit += equipmentEffect;
        }

        public override void OnEquipmentRemove()
        {
            owner.OnAttackHit -= equipmentEffect;
            base.OnEquipmentRemove();
        }

        public override bool GetPassiveSkillDescription(out string description)
        {
            description = string.Format(_passiveSkillName + "\n" + _passiveSkillDescription, damageCount);
            return true;
        }
    }
}