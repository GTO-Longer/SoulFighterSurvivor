using System;
using Utilities;

namespace Classes.Equipments
{
    public class Guillotine : Equipment
    {
        private Action<Entity, Entity, float, Skill> equipmentEffect;
        
        public Guillotine() : base("Guillotine")
        {
            equipmentEffect = (attacker, target, damageCount, skill) =>
            {
                if (skill.skillType == SkillType.RSkill)
                {
                    target.TakeDamage(damageCount * 0.2f, DamageType.Real, attacker);
                }
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