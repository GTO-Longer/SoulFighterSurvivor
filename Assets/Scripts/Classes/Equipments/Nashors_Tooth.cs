using System;
using Managers.EntityManagers;
using Utilities;

namespace Classes.Equipments
{
    public class Nashors_Tooth : Equipment
    {
        private float damageCount => 15 + 0.2f * HeroManager.hero.abilityPower;

        private Action<Entity, Entity, float, float> equipmentEffect;
        
        public Nashors_Tooth() : base("Nashors_Tooth")
        {
            equipmentEffect = (attacker, target, _, ratio) =>
            {
                target.TakeDamage(target.CalculateAPDamage(attacker, damageCount * ratio), DamageType.AP, attacker);
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
            description = string.Format(_passiveSkillName + "\n" + _passiveSkillDescription, damageCount);
            return true;
        }
    }
}