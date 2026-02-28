using System;
using Classes.Entities;
using Factories;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

namespace Classes.Hexes
{
    public class Zangief : Hex
    {
        private Action<Entity, Entity, float, float> HexEffect;
        private float damageSum = 0;
        
        public Zangief() : base("Zangief")
        {
            HexEffect = (_, target, _, _) =>
            {
                // 计算攻击伤害
                var damageCount = target.CalculateADDamage(owner, owner.maxHealthPoint * 0.035f);
                target.TakeDamage(damageCount, DamageType.AD, owner);
                damageSum += damageCount;
            };
        }

        public override void OnHexGet(Entity entity)
        {
            base.OnHexGet(entity);
            owner.AttackEffect += HexEffect;
        }

        public override void OnHexRemove()
        {
            owner.AttackEffect -= HexEffect;
            base.OnHexRemove();
        }

        public override bool GetHexDetail(out string detail)
        {
            detail = string.Format(hexDetail, damageSum);
            return true;
        }
    }
}