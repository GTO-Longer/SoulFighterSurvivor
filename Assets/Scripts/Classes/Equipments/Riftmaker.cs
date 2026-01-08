using System;
using Managers.EntityManagers;
using UnityEngine;
using Utilities;

namespace Classes.Equipments
{
    public class Riftmaker : Equipment
    {
        private Action<Entity, Entity, float, Skill> equipmentEffect;
        private float addCount => 0.02f * HeroManager.hero.originMaxHealthPoint;
        
        public Riftmaker() : base("Riftmaker")
        {
            equipmentEffect = (attacker, target, _, _) =>
            {
                var voidInfection = new Buffs.VoidInfection(attacker, attacker);
                voidInfection.buffIcon = equipmentIcon;
                attacker.GainBuff(voidInfection);
            };
        }

        public override void OnEquipmentGet(Entity entity)
        {
            base.OnEquipmentGet(entity);
            var efficiency = owner._HPToAP_ConversionEfficiency.Value;
            owner._HPToAP_ConversionEfficiency.Value = new Vector2(efficiency.x, efficiency.y + 0.02f);
            owner.AbilityEffect += equipmentEffect;
        }

        public override void OnEquipmentRemove()
        {
            owner.AbilityEffect -= equipmentEffect;
            var efficiency = owner._HPToAP_ConversionEfficiency.Value;
            owner._HPToAP_ConversionEfficiency.Value = new Vector2(efficiency.x, efficiency.y - 0.02f);
            base.OnEquipmentRemove();
        }

        public override bool GetPassiveSkillDescription(out string description)
        {
            description = string.Format(_passiveSkillName + "\n" + _passiveSkillDescription, addCount);
            return true;
        }
    }
}