using System;
using Classes.Entities;

namespace Classes.Hexes
{
    public class SoulSuck : Hex
    {
        public SoulSuck() : base("SoulSuck")
        {

        }

        public override void OnHexGet(Entity entity)
        {
            base.OnHexGet(entity);
            owner._criticalRateBonus.Value += 0.25f;
            owner.lifeSteal.Value += 0.12f;
        }

        public override void OnHexRemove()
        {
            owner._criticalRateBonus.Value -= 0.25f;
            owner.lifeSteal.Value -= 0.12f;
            base.OnHexRemove();
        }

        public override bool GetHexDetail(out string detail)
        {
            detail = string.Format(hexDetail);
            return true;
        }
    }
}