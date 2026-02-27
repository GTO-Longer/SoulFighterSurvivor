using System;
using Classes.Entities;
using Factories;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

namespace Classes.Hexes
{
    public class LightningStrike : Hex
    {
        private Action<Entity, Entity, bool> HexEffect;
        private float damageSum = 0;
        private float damageCount => 20 + owner.abilityPower * 0.05f;
        
        public LightningStrike() : base("LightningStrike")
        {
            HexEffect = (_, target, isCrit) =>
            {
                if (owner.attackSpeed >= 4)
                {
                    // 计算攻击伤害
                    var damage = target.CalculateAPDamage(owner, damageCount);
                    target.TakeDamage(damage, DamageType.AP, owner, isCrit);
                    damageSum += damage;
                }
            };
        }

        public override void OnHexGet(Entity entity)
        {
            base.OnHexGet(entity);
            owner.OnAttackHit += HexEffect;
            owner._percentageAttackSpeedBonus.Value += 0.2f;
        }

        public override void OnHexRemove()
        {
            owner.OnAttackHit -= HexEffect;
            owner._percentageAttackSpeedBonus.Value -= 0.2f;
            base.OnHexRemove();
        }

        public override bool GetHexDetail(out string detail)
        {
            detail = string.Format(hexDetail, damageCount, damageSum);
            return true;
        }
    }
}