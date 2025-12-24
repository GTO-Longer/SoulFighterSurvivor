using System;
using Classes.Entities;
using Managers.EntityManagers;

namespace Classes.Hexes
{
    public class HellfireConduit : Hex
    {
        private float damageCount => 6 + 0.012f * HeroManager.hero.abilityPower + 0.028f * HeroManager.hero.attackDamage;
        private Action<Entity, Entity, float, Skill> HexEffect;
        public static bool HellfireConduitEffective;
        
        public HellfireConduit() : base("HellfireConduit")
        {
            HexEffect = (attacker, target, _, _) =>
            {
                var hellFire = new Buffs.HellFire(target, attacker);
                hellFire.buffIcon = hexIcon;
                target.GetBuff(hellFire);
            };
        }

        public override void OnHexGet(Entity entity)
        {
            base.OnHexGet(entity);
            owner.AbilityEffect += HexEffect;
            HellfireConduitEffective = true;
        }

        public override void OnHexRemove()
        {
            HellfireConduitEffective = false;
            owner.AbilityEffect -= HexEffect;
            base.OnHexRemove();
        }

        public override bool GetHexDetail(out string detail)
        {
            detail = string.Format(hexDetail, damageCount);
            return true;
        }
    }
}