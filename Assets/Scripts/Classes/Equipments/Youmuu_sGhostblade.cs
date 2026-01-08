using System;
using UnityEngine;
using Utilities;

namespace Classes.Equipments
{
    public class Youmuu_sGhostblade : Equipment
    {
        public Youmuu_sGhostblade() : base("Youmuu_sGhostblade")
        {
            ActiveSkillEffective = () =>
            {
                if (!_activeSkillActive) return;
                
                _activeSkillCDTimer = 0;
                var ghostBlade = new Buffs.GhostBlade(owner, owner);
                ghostBlade.buffIcon = equipmentIcon;
                owner.GainBuff(ghostBlade);
            };
        }

        public override void OnEquipmentGet(Entity entity)
        {
            base.OnEquipmentGet(entity);
            owner._movementSpeedBonus.Value += 60;
            owner.EntityUpdateEvent += equipmentTimerUpdate;
        }

        public override void OnEquipmentRemove()
        {
            owner._movementSpeedBonus.Value -= 60;
            owner.EntityUpdateEvent -= equipmentTimerUpdate;
            base.OnEquipmentRemove();
        }

        public override bool GetPassiveSkillDescription(out string description)
        {
            description = string.Format(_passiveSkillName + "\n" + _passiveSkillDescription);
            return true;
        }

        public override bool GetActiveSkillDescription(out string description)
        {
            description = string.Format(_activeSkillName + "\n" + _activeSkillDescription);
            return true;
        }
    }
}