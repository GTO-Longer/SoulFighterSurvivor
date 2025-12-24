using System;
using Utilities;

namespace Classes.Equipments
{
    public class Cryptic : Equipment
    {
        private Action<Entity, Entity, float, Skill> equipmentEffect;
        private Action<Entity, Entity, float, float> equipmentEffect2;
        
        public Cryptic() : base("Cryptic")
        {
            equipmentEffect = (attacker, target, damageCount, _) =>
            {
                if (target.maxHealthPoint >= attacker.maxHealthPoint)
                {
                    var ratio = 0.05f + (target.maxHealthPoint - attacker.maxHealthPoint) * 0.00004f;
                    target.TakeDamage(damageCount * ratio, DamageType.Real, attacker);
                }
            };
            
            equipmentEffect2 = (attacker, target, damageCount, _) =>
            {
                if (target.maxHealthPoint >= attacker.maxHealthPoint)
                {
                    var ratio = 0.05f + (target.maxHealthPoint - attacker.maxHealthPoint) * 0.00004f;
                    target.TakeDamage(damageCount * ratio, DamageType.Real, attacker);
                }
            };
        }

        public override void OnEquipmentGet(Entity entity)
        {
            base.OnEquipmentGet(entity);
            owner.AbilityEffect += equipmentEffect;
            owner.AttackEffect += equipmentEffect2;
        }

        public override void OnEquipmentRemove()
        {
            owner.AbilityEffect -= equipmentEffect;
            owner.AttackEffect -= equipmentEffect2;
            base.OnEquipmentRemove();
        }

        public override bool GetPassiveSkillDescription(out string description)
        {
            description = string.Format(_passiveSkillName + "\n" + _passiveSkillDescription);
            return true;
        }
    }
}