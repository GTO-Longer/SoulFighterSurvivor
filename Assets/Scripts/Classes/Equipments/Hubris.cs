using System;
using UnityEngine;
using Utilities;

namespace Classes.Equipments
{
    public class Hubris : Equipment
    {
        private Action<Entity, Entity> equipmentEffect;
        
        public Hubris() : base("Hubris")
        {
            maxChargeCount.Value = 9999;
            chargeCount.Value = 0;
            
            equipmentEffect = (_, _) =>
            {
                chargeCount.Value += 1;
                var prominence = new Buffs.Prominence(owner, owner, chargeCount.Value);
                prominence.buffIcon = equipmentIcon;
                owner.GetBuff(prominence);
            };
        }

        public override void OnEquipmentGet(Entity entity)
        {
            base.OnEquipmentGet(entity);
            owner.OnKillEntity += equipmentEffect;
        }

        public override void OnEquipmentRemove()
        {
            owner.OnKillEntity -= equipmentEffect;
            base.OnEquipmentRemove();
        }

        public override bool GetPassiveSkillDescription(out string description)
        {
            description = string.Format(_passiveSkillName + "\n" + _passiveSkillDescription);
            return true;
        }
    }
}