using System;
using Classes.Entities;
using Managers.EntityManagers;

namespace Classes.Hexes
{
    public class WisdomOfTime : Hex
    {
        private Action<Entity, Entity> HexEffect;
        private int killCount;
        public WisdomOfTime() : base("WisdomOfTime")
        {
            killCount = 0;
            HexEffect = (_, _) =>
            {
                killCount += 1;

                if (killCount >= 50)
                {
                    HeroManager.hero.LevelUp();
                    killCount = 0;
                }
            };
        }
        
        public override void OnHexGet(Entity entity)
        {
            base.OnHexGet(entity);
            HeroManager.hero.maxLevel = 999;
            if (!hexGetEventTriggered)
            {
                HeroManager.hero.LevelUp(HeroManager.hero.hexList.Count == 1 ? 1 : 3);
                hexGetEventTriggered = true;
            }
            owner.OnKillEntity += HexEffect;
        }

        public override void OnHexRemove()
        {
            HeroManager.hero.maxLevel = 18;
            owner.OnKillEntity -= HexEffect;
            base.OnHexRemove();
        }

        public override bool GetHexDetail(out string detail)
        {
            detail = string.Format(hexDetail);
            return true;
        }
    }
}