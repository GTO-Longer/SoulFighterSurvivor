using System;
using Classes.Entities;

namespace Classes.Hexes
{
    public class WisdomOfTime : Hex
    {
        public WisdomOfTime() : base("WisdomOfTime")
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