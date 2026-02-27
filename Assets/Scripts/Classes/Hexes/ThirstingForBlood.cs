using System;
using Classes.Entities;

namespace Classes.Hexes
{
    public class ThirstingForBlood : Hex
    {
        public ThirstingForBlood() : base("ThirstingForBlood")
        {

        }

        public override void OnHexGet(Entity entity)
        {
            base.OnHexGet(entity);
            owner.omnivamp.Value += 0.15f;
        }

        public override void OnHexRemove()
        {
            owner.omnivamp.Value -= 0.15f;
            base.OnHexRemove();
        }

        public override bool GetHexDetail(out string detail)
        {
            detail = string.Format(hexDetail);
            return true;
        }
    }
}