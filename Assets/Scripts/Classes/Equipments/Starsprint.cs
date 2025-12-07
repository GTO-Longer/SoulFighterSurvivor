using System;
using Utilities;

namespace Classes.Equipments
{
    public class Starsprint : Equipment
    {
        private Action<Entity, Entity, float, Skill> equipmentEffect;
        
        public Starsprint() : base("Starsprint")
        {
            equipmentEffect = (attacker, _, _, _) =>
            {
                var spellDance = new Buffs.SpellDance(attacker, attacker);
                attacker.GetBuff(spellDance);
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
            description = string.Format(_passiveSkillName + "\n" + _passiveSkillDescription);
            return true;
        }
    }
}