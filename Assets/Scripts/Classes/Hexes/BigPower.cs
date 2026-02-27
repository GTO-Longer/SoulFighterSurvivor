using System;
using Classes.Entities;

namespace Classes.Hexes
{
    public class BigPower : Hex
    {
        public BigPower() : base("BigPower")
        {

        }

        public override void OnHexGet(Entity entity)
        {
            base.OnHexGet(entity);
            owner._percentageAttackDamageBonus.Value += 0.2f;
        }

        public override void OnHexRemove()
        {
            owner._percentageAttackDamageBonus.Value -= 0.2f;
            base.OnHexRemove();
        }

        public override bool GetHexDetail(out string detail)
        {
            detail = string.Format(hexDetail);
            return true;
        }
    }
}