using System;
using Classes.Entities;

namespace Classes.Hexes
{
    public class BrutalPower : Hex
    {
        public BrutalPower() : base("BrutalPower")
        {

        }

        public override void OnHexGet(Entity entity)
        {
            base.OnHexGet(entity);
            owner._attackDamageBonus.Value += 25;
            owner._abilityHasteBonus.Value += 10;
            owner._attackPenetrationBonus.Value += 5;
        }

        public override void OnHexRemove()
        {
            owner._attackDamageBonus.Value -= 25;
            owner._abilityHasteBonus.Value -= 10;
            owner._attackPenetrationBonus.Value -= 5;
            base.OnHexRemove();
        }

        public override bool GetHexDetail(out string detail)
        {
            detail = string.Format(hexDetail);
            return true;
        }
    }
}