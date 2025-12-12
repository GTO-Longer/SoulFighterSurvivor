using System;
using Classes.Entities;

namespace Classes.Hexes
{
    public class FortuneAnvil : Hex
    {
        public FortuneAnvil() : base("FortuneAnvil")
        {
            
        }

        public override void OnHexGet(Entity entity)
        {
            base.OnHexGet(entity);

            if (owner is Hero hero)
            {
                hero.coins.Value += 1000;
            }
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