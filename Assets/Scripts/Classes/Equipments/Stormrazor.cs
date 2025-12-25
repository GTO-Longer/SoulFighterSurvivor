using System;
using Classes.Buffs;
using Managers.EntityManagers;
using Utilities;

namespace Classes.Equipments
{
    public class Stormrazor : Equipment
    {
        private Action<Entity, Entity, float, float> equipmentEffect;
        
        public Stormrazor() : base("Stormrazor")
        {
            equipmentEffect = (attacker, target, _, ratio) =>
            {
                target.TakeDamage(target.CalculateAPDamage(attacker, 100 * ratio), DamageType.AP, attacker);
                
                var speedBonus = new PercentageSpeedBonus(owner, owner, 1.5f, 0.45f);
                owner.GetBuff(speedBonus);
            };
        }

        public override void OnEquipmentGet(Entity entity)
        {
            base.OnEquipmentGet(entity);
            var empowered = new Empowered(owner, owner);
            owner.GetBuff(empowered);
            owner.EmpoweredEffect += equipmentEffect;
        }

        public override void OnEquipmentRemove()
        {
            owner.EmpoweredEffect -= equipmentEffect;
            base.OnEquipmentRemove();
        }

        public override bool GetPassiveSkillDescription(out string description)
        {
            description = string.Format(_passiveSkillName + "\n" + _passiveSkillDescription);
            return true;
        }
    }
}