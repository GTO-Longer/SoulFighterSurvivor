using System;
using Managers.EntityManagers;
using UnityEngine;
using Utilities;

namespace Classes.Equipments
{
    public class Nashors_Tooth : Equipment
    {
        private float damageCount;

        private Action<Entity, Entity, float> equipmentEffect;
        
        public Nashors_Tooth() : base("Nashors_Tooth")
        {
            damageCount = 15 + 0.2f * HeroManager.hero.abilityPower;
            
            equipmentEffect = (attacker, target, _) =>
            {
                target.TakeDamage(target.CalculateAPDamage(attacker, damageCount), DamageType.AP, attacker);
            };
        }

        public override void OnEquipmentGet(Entity entity)
        {
            base.OnEquipmentGet(entity);

            owner.AttackEffect += equipmentEffect;
        }

        public override void OnEquipmentRemove()
        {
            base.OnEquipmentRemove();
            
            owner.AttackEffect -= equipmentEffect;
        }

        public override bool GetPassiveSkillDescription(out string description)
        {
            description = string.Format(_passiveSkillName + "\n" + _passiveSkillDescription, damageCount);
            return true;
        }
    }
}