using System;
using Classes.Entities;

namespace Classes.Hexes
{
    public class MajorKiller : Hex
    {
        public MajorKiller() : base("MajorKiller")
        {

        }

        public override void OnHexGet(Entity entity)
        {
            base.OnHexGet(entity);
            owner.fortune += 0.2f;
        }

        public override void OnHexRemove()
        {
            owner.fortune -= 0.2f;
            base.OnHexRemove();
        }

        public override bool GetHexDetail(out string detail)
        {
            detail = string.Format(hexDetail);
            return true;
        }
    }
}