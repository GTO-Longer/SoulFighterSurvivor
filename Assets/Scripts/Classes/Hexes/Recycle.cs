using System;
using Classes.Entities;

namespace Classes.Hexes
{
    public class Recycle : Hex
    {
        public Recycle() : base("Recycle")
        {

        }

        public override void OnHexGet(Entity entity)
        {
            base.OnHexGet(entity);
            owner._abilityHasteBonus.Value += 75;
        }

        public override void OnHexRemove()
        {
            owner._abilityHasteBonus.Value -= 75;
            base.OnHexRemove();
        }

        public override bool GetHexDetail(out string detail)
        {
            detail = string.Format(hexDetail);
            return true;
        }
    }
}