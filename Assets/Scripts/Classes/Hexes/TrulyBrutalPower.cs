using System;
using Classes.Entities;

namespace Classes.Hexes
{
    public class TrulyBrutalPower : Hex
    {
        public TrulyBrutalPower() : base("TrulyBrutalPower")
        {

        }

        public override void OnHexGet(Entity entity)
        {
            base.OnHexGet(entity);
            owner._attackDamageBonus.Value += 40;
            owner._abilityHasteBonus.Value += 20;
            owner._attackPenetrationBonus.Value += 15;
        }

        public override void OnHexRemove()
        {
            owner._attackDamageBonus.Value -= 40;
            owner._abilityHasteBonus.Value -= 20;
            owner._attackPenetrationBonus.Value -= 15;
            base.OnHexRemove();
        }

        public override bool GetHexDetail(out string detail)
        {
            detail = string.Format(hexDetail);
            return true;
        }
    }
}