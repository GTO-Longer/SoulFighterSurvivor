using System;
using Classes.Entities;

namespace Classes.Hexes
{
    public class DualBlade : Hex
    {
        public DualBlade() : base("DualBlade")
        {
            
        }

        public override void OnHexGet(Entity entity)
        {
            base.OnHexGet(entity);
        }

        public override void OnHexRemove()
        {
            base.OnHexRemove();
        }

        public override bool GetHexDetail(out string detail)
        {
            detail = string.Format(hexDetail);
            return true;
        }
    }
}