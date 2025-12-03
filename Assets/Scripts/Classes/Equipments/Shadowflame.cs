using System;
using Managers.EntityManagers;
using Utilities;

namespace Classes.Equipments
{
    public class Shadowflame : Equipment
    {
        private Action<Entity, Entity, float> equipmentEffect;
        
        public Shadowflame() : base("Shadowflame")
        {
            equipmentEffect = (attacker, target, damageCount) =>
            {
                target.TakeDamage(damageCount * 0.15f, DamageType.Real, attacker);
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