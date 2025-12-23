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
                var empowered = owner.buffList.Find(buff => buff.buffName == "盈能");
                
                if (empowered != null)
                {
                    if (empowered.buffCount >= 100)
                    {
                        target.TakeDamage(target.CalculateAPDamage(attacker, 100 * ratio), DamageType.AD, attacker);
                        
                        var speedReduce = new SpeedReduce(target, owner, 0.75f, 0.99f);
                        target.GetBuff(speedReduce);
                        
                        empowered.buffCount.Value = 0;
                    }
                }
            };
        }

        public override void OnEquipmentGet(Entity entity)
        {
            base.OnEquipmentGet(entity);
            var empowered = new Empowered(owner, owner);
            owner.GetBuff(empowered);
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