using System;
using Classes.Entities;

namespace Classes.Hexes
{
    public class Vampires : Hex
    {
        public Vampires() : base("Vampires")
        {

        }

        public override void OnHexGet(Entity entity)
        {
            base.OnHexGet(entity);
            owner.omnivamp.Value += 0.3f;
        }

        public override void OnHexRemove()
        {
            owner.omnivamp.Value -= 0.3f;
            base.OnHexRemove();
        }

        public override bool GetHexDetail(out string detail)
        {
            detail = string.Format(hexDetail);
            return true;
        }
    }
}