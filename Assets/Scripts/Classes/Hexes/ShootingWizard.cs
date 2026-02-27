using System;
using Classes.Entities;
using Factories;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

namespace Classes.Hexes
{
    public class ShootingWizard : Hex
    {
        private Action<Entity, Entity, bool> HexEffect;
        private float damageSum = 0;
        
        public ShootingWizard() : base("ShootingWizard")
        {
            HexEffect = (_, target, _) =>
            {
                // 计算攻击伤害
                var damageCount = target.CalculateADDamage(owner, owner.abilityPower);
                target.TakeDamage(damageCount, DamageType.AD, owner);
                damageSum += damageCount;
            };
        }

        public override void OnHexGet(Entity entity)
        {
            base.OnHexGet(entity);
            owner.OnAttackHit += HexEffect;
        }

        public override void OnHexRemove()
        {
            owner.OnAttackHit -= HexEffect;
            base.OnHexRemove();
        }

        public override bool GetHexDetail(out string detail)
        {
            detail = string.Format(hexDetail, damageSum);
            return true;
        }
    }
}