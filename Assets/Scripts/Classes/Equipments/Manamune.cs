using System;
using Managers.EntityManagers;
using UnityEngine;
using Utilities;

namespace Classes.Equipments
{
    public class Manamune : Equipment
    {
        private float baseAttackBonus;
        private float addCount => 0.1f * HeroManager.hero.maxMagicPoint.Value;
        private float damageCount1 => 5 + 0.02f * HeroManager.hero.maxMagicPoint.Value;
        private float damageCount2 => 20 + 0.1f * HeroManager.hero.maxMagicPoint.Value;

        private Action<Entity> equipmentEffect;
        private Action<Entity, Entity, float> equipmentEffect1;
        private Action<Entity, Entity, float, Skill> equipmentEffect2;
        
        public Manamune() : base("Manamune")
        {
            canPurchase = false;
            baseAttackBonus = equipmentAttributes[EquipmentAttributeType.attackDamage];
            equipmentEffect = (hero) =>
            {
                var formerValue = equipmentAttributes[EquipmentAttributeType.attackDamage];
                if (Mathf.Abs(baseAttackBonus + addCount - formerValue) > 0.1f)
                {
                    equipmentAttributes[EquipmentAttributeType.attackDamage] = baseAttackBonus + addCount;
                    hero._attackDamageBonus.Value += equipmentAttributes[EquipmentAttributeType.attackDamage] - formerValue;
                }
            };
            
            equipmentEffect1 = (attacker, target, _) =>
            {
                target.TakeDamage(target.CalculateAPDamage(attacker, damageCount1), DamageType.AD, attacker);
            };
            
            equipmentEffect2 = (attacker, target, _, _) =>
            {
                target.TakeDamage(target.CalculateAPDamage(attacker, damageCount2), DamageType.AD, attacker);
            };
        }

        public override void OnEquipmentGet(Entity entity)
        {
            base.OnEquipmentGet(entity);

            owner.EntityUpdateEvent += equipmentEffect;
            owner.AttackEffect += equipmentEffect1;
            owner.AbilityEffect += equipmentEffect2;
        }

        public override void OnEquipmentRemove()
        {
            owner.EntityUpdateEvent -= equipmentEffect;
            owner.AttackEffect -= equipmentEffect1;
            owner.AbilityEffect -= equipmentEffect2;
            base.OnEquipmentRemove();
        }

        public override bool GetPassiveSkillDescription(out string description)
        {
            description = string.Format(_passiveSkillName + "\n" + _passiveSkillDescription, addCount, damageCount1, damageCount2);
            return true;
        }
    }
}