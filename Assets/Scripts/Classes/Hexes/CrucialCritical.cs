using System;
using Classes.Entities;

namespace Classes.Hexes
{
    public class CrucialCritical : Hex
    {
        public CrucialCritical() : base("CrucialCritical")
        {

        }

        public override void OnHexGet(Entity entity)
        {
            base.OnHexGet(entity);
            owner._criticalRateBonus.Value += 0.5f;
        }

        public override void OnHexRemove()
        {
            owner._criticalRateBonus.Value -= 0.5f;
            base.OnHexRemove();
        }

        public override bool GetHexDetail(out string detail)
        {
            detail = string.Format(hexDetail);
            return true;
        }
    }
}