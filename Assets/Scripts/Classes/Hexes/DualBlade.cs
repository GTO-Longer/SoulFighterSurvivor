using System;
using Classes.Entities;
using Factories;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

namespace Classes.Hexes
{
    public class DualBlade : Hex
    {
        private Action<Entity, Entity> HexEffect;
        private const float attackBulletSpeed = 2500f;
        
        public DualBlade() : base("DualBlade")
        {
            HexEffect = (_, target) =>
            {
                var bullet = BulletFactory.Instance.CreateBullet(owner);
                bullet.OnBulletAwake += (self) =>
                {
                    self.gameObject.transform.position = owner.gameObject.transform.position;
                    self.gameObject.SetActive(true);
        
                    // 设置目标
                    self.target = target;

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
                        self.gameObject.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);

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
                    // 造成40%的攻击伤害和攻击特效
                    // 计算攻击伤害
                    var damageCount = target.CalculateADDamage(owner, owner.attackDamage) * 0.4f;
                    target.TakeDamage(damageCount, DamageType.AD, owner, Random.Range(0f, 1f) < owner.criticalRate.Value);

                    // 造成攻击特效
                    if (target.isAlive)
                    {
                        owner.AttackEffectActivate(target, damageCount, 0.4f);
                    }
                };
                
                bullet.Awake();
            };
        }

        public override void OnHexGet(Entity entity)
        {
            base.OnHexGet(entity);
            owner.OnAttackHit += HexEffect;
            owner._percentageAttackSpeedBonus.Value += 0.25f;
        }

        public override void OnHexRemove()
        {
            owner.OnAttackHit -= HexEffect;
            owner._percentageAttackSpeedBonus.Value -= 0.25f;
            base.OnHexRemove();
        }

        public override bool GetHexDetail(out string detail)
        {
            detail = string.Format(hexDetail);
            return true;
        }
    }
}