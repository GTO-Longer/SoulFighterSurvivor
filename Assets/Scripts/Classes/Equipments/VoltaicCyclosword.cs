using System;
using Classes.Buffs;
using Managers.EntityManagers;
using Utilities;

namespace Classes.Equipments
{
    public class VoltaicCyclosword : Equipment
    {
        private Action<Entity, Entity, float, float> equipmentEffect;
        
        public VoltaicCyclosword() : base("VoltaicCyclosword")
        {
            equipmentEffect = (attacker, target, _, ratio) =>
            {
                target.TakeDamage(target.CalculateADDamage(attacker, 100 * ratio), DamageType.AD, attacker);
                
                var speedReduce = new SpeedReduce(target, owner, 0.75f, 0.99f);
                target.GainBuff(speedReduce);
            };
        }

        public override void OnEquipmentGet(Entity entity)
        {
            base.OnEquipmentGet(entity);
            var empowered = new Empowered(owner, owner);
            owner.GainBuff(empowered);
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