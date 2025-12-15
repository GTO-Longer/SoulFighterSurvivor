using System;
using Classes.Entities;
using Managers.EntityManagers;

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
            if (!hexGetEventTriggered)
            {
                HeroManager.hero.coins.Value += 1000;
                hexGetEventTriggered = true;
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