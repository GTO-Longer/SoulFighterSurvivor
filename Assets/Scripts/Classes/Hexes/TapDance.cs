using System;
using Classes.Entities;
using UnityEngine;

namespace Classes.Hexes
{
    public class TapDance : Hex
    {
        private float addAmount => 0.001f * owner.movementSpeed;
        private Action<Entity, Entity, float, float> HexEffect;
        
        public TapDance() : base("TapDance")
        {
            HexEffect = (_, _, _, _) =>
            {
                var tapDance = new Buffs.TapDance(owner, owner);
                owner.GainBuff(tapDance);
            };
        }

        public override void OnHexGet(Entity entity)
        {
            base.OnHexGet(entity);
            owner._MSToAS_ConversionEfficiency.Value += new Vector2(0, 0.001f);
            owner.AttackEffect += HexEffect;
        }

        public override void OnHexRemove()
        {
            owner._MSToAS_ConversionEfficiency.Value -= new Vector2(0, 0.001f);
            owner.AttackEffect -= HexEffect;
            base.OnHexRemove();
        }

        public override bool GetHexDetail(out string detail)
        {
            detail = string.Format(hexDetail, addAmount);
            return true;
        }
    }
}