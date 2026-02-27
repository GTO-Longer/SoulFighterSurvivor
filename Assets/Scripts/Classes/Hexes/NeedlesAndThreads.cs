using System;
using Classes.Entities;

namespace Classes.Hexes
{
    public class NeedlesAndThreads : Hex
    {
        public NeedlesAndThreads() : base("NeedlesAndThreads")
        {

        }

        public override void OnHexGet(Entity entity)
        {
            base.OnHexGet(entity);
            owner._percentageAttackPenetrationBonus.Value += 0.2f;
            owner._percentageMagicPenetrationBonus.Value += 0.2f;
        }

        public override void OnHexRemove()
        {
            owner._percentageAttackPenetrationBonus.Value -= 0.2f;
            owner._percentageMagicPenetrationBonus.Value -= 0.2f;
            base.OnHexRemove();
        }

        public override bool GetHexDetail(out string detail)
        {
            detail = string.Format(hexDetail);
            return true;
        }
    }
}