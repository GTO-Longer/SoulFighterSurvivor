using System;
using UnityEngine;
using Utilities;

namespace Classes.Equipments
{
    public class EdgeOfNight : Equipment
    {
        private Action<Entity, Entity> equipmentEffect;
        
        public EdgeOfNight() : base("EdgeOfNight")
        {
            equipmentEffect = (_, _) =>
            {
                if (_passiveSkillActive)
                {
                    var speedBonus = new Buffs.SpeedBonus(owner, owner, 1.5f, 200);
                    owner.GainBuff(speedBonus);
                    
                    _passiveSkillCDTimer = 0;
                }
            };
        }

        public override void OnEquipmentGet(Entity entity)
        {
            base.OnEquipmentGet(entity);
            owner._attackPenetrationBonus.Value += 15;
            owner.OnKillEntity += equipmentEffect;
            owner.EntityUpdateEvent += equipmentTimerUpdate;
        }

        public override void OnEquipmentRemove()
        {
            owner.OnKillEntity -= equipmentEffect;
            owner.EntityUpdateEvent -= equipmentTimerUpdate;
            owner._attackPenetrationBonus.Value -= 15;
            base.OnEquipmentRemove();
        }

        public override bool GetPassiveSkillDescription(out string description)
        {
            description = string.Format(_passiveSkillName + "\n" + _passiveSkillDescription);
            return true;
        }
    }
}