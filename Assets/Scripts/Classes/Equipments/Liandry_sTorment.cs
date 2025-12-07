using System;
using Managers.EntityManagers;
using Utilities;

namespace Classes.Equipments
{
    public class Liandry_sTorment : Equipment
    {
        private Action<Entity, Entity, float, Skill> equipmentEffect;
        public float damageCount => 20 + 0.04f * HeroManager.hero.abilityPower;
        
        public Liandry_sTorment() : base("Liandry_sTorment")
        {
            equipmentEffect = (attacker, target, _, _) =>
            {
                var evilFlame = new Buffs.EvilFlame(target, attacker);
                target.GetBuff(evilFlame);
            };
        }

        public override void OnEquipmentGet(Entity entity)
        {
            base.OnEquipmentGet(entity);
            owner.AbilityEffect += equipmentEffect;
        }

        public override void OnEquipmentRemove()
        {
            owner.AbilityEffect -= equipmentEffect;
            base.OnEquipmentRemove();
        }

        public override bool GetPassiveSkillDescription(out string description)
        {
            description = string.Format(_passiveSkillName + "\n" + _passiveSkillDescription, damageCount);
            return true;
        }
    }
}