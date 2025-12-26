using System;
using Managers.EntityManagers;
using Utilities;

namespace Classes.Equipments
{
    public class BladeOfTheRuinedKing : Equipment
    {
        private Action<Entity, Entity, float, float> equipmentEffect;
        
        public BladeOfTheRuinedKing() : base("BladeOfTheRuinedKing")
        {
            equipmentEffect = (attacker, target, _, ratio) =>
            {
                target.TakeDamage(target.CalculateADDamage(attacker, target.healthPoint * 0.08f * ratio), DamageType.AD, attacker);
            };
        }

        public override void OnEquipmentGet(Entity entity)
        {
            base.OnEquipmentGet(entity);

            owner.AttackEffect += equipmentEffect;
        }

        public override void OnEquipmentRemove()
        {
            owner.AttackEffect -= equipmentEffect;
            base.OnEquipmentRemove();
        }

        public override bool GetPassiveSkillDescription(out string description)
        {
            description = string.Format(_passiveSkillName + "\n" + _passiveSkillDescription);
            return true;
        }
    }
}