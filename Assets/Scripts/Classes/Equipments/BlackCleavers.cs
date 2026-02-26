using System;
using Managers.EntityManagers;
using UnityEngine;
using Utilities;

namespace Classes.Equipments
{
    public class BlackCleavers : Equipment
    {

        private Action<Entity, Entity, float> equipmentEffect;
        
        public BlackCleavers() : base("BlackCleavers")
        {
            equipmentEffect = (attacker, target, _) =>
            {
                var blackCleavers = new Buffs.BlackCleavers(target, attacker);
                blackCleavers.buffIcon = equipmentIcon;
                target.GainBuff(blackCleavers);
            };
        }

        public override void OnEquipmentGet(Entity entity)
        {
            base.OnEquipmentGet(entity);
            owner.OnDamage += equipmentEffect;
        }

        public override void OnEquipmentRemove()
        {
            owner.OnDamage -= equipmentEffect;
            base.OnEquipmentRemove();
        }

        public override bool GetPassiveSkillDescription(out string description)
        {
            description = string.Format(_passiveSkillName + "\n" + _passiveSkillDescription);
            return true;
        }
    }
}