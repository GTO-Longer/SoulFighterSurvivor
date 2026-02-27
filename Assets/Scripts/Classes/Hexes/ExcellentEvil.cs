using System;
using Classes.Entities;

namespace Classes.Hexes
{
    public class ExcellentEvil : Hex
    {
        private Action<Entity, Entity, float, Skill> excellentEvil;
        private int sum = 0;
        
        public ExcellentEvil() : base("ExcellentEvil")
        {
            excellentEvil = (_, _, _, _) =>
            {
                owner._abilityPowerBonus.Value += 1;
                sum += 1;
            };
        }

        public override void OnHexGet(Entity entity)
        {
            base.OnHexGet(entity);
            owner.AbilityEffect += excellentEvil;
        }

        public override void OnHexRemove()
        {
            owner.AbilityEffect -= excellentEvil;
            base.OnHexRemove();
        }

        public override bool GetHexDetail(out string detail)
        {
            detail = string.Format(hexDetail, sum);
            return true;
        }
    }
}