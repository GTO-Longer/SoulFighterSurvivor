using System;
using Utilities;

namespace Classes.Equipments
{
    public class Hullbreaker : Equipment
    {
        private Action<Skill> equipmentEffect;
        
        public Hullbreaker() : base("Hullbreaker")
        {
            equipmentEffect = (skill) =>
            {
                if (_passiveSkillActive && skill.skillType == SkillType.RSkill)
                {
                    var hullbreaker = new Buffs.Hullbreaker(owner, owner);
                    hullbreaker.buffIcon = equipmentIcon;
                    owner.GainBuff(hullbreaker);
                    
                    _passiveSkillCDTimer = 0;
                }
            };
        }
        
        public override void OnEquipmentGet(Entity entity)
        {
            base.OnEquipmentGet(entity);
            owner.OnSkillUsed += equipmentEffect;
            owner.EntityUpdateEvent += equipmentTimerUpdate;
        }
        
        public override void OnEquipmentRemove()
        {
            owner.OnSkillUsed -= equipmentEffect;
            owner.EntityUpdateEvent -= equipmentTimerUpdate;
            base.OnEquipmentRemove();
        }
        
        public override bool GetPassiveSkillDescription(out string description)
        {
            description = string.Format(_passiveSkillName + "\n" + _passiveSkillDescription);
            return true;
        }
    }
}